# MiBoleta Backend — Evaluación de Requerimientos y Plan de Implementación

## 1) Resumen ejecutivo

Estado actual del backend: **base funcional inicial** con API REST, entidades `Tenant` y `Usuario`, CQRS con MediatR, EF Core y JWT para emisión de tokens. Aún faltan varios módulos críticos para cumplir alcance productivo de MiBoleta (documentos, firma, notificaciones, auditoría, almacenamiento cloud, colas y políticas de seguridad por rol/tenant).

En esta revisión se recomienda construir un **MVP por etapas**, priorizando seguridad multitenant y gestión documental.

---

## 2) Matriz de cumplimiento contra requerimientos

Leyenda: ✅ Cumplido | 🟡 Parcial | ❌ Pendiente

| # | Requerimiento | Estado | Observación |
|---|---|---|---|
| 1 | Arquitectura general (.NET, REST, capas) | 🟡 | Existe estructura por proyectos y controladores REST; falta desacoplar completamente y robustecer seguridad de transporte/configuración. |
| 2 | Multitenant | 🟡 | Entidades tienen `TenantId` (en usuario), pero falta filtro global por tenant y validación estricta por recurso. |
| 3 | Gestión de usuarios | 🟡 | Crear/listar/login disponible; faltan actualizar, activar/desactivar, recuperación de contraseña y sesiones refresh. |
| 4 | Roles y permisos | ❌ | No hay módulo formal de RBAC ni políticas por endpoint. |
| 5 | Dashboard usuario | ❌ | No existe módulo dashboard ni agregados de métricas. |
| 6 | Gestión de documentos | ❌ | No hay entidad/servicios de documentos ni carga masiva. |
| 7 | Almacenamiento de archivos | ❌ | No existe adaptador cloud ni URLs firmadas. |
| 8 | Visualización de documentos | ❌ | Falta endpoint de URL temporal y tracking de visualización. |
| 9 | Firma electrónica | ❌ | Falta modelo de firmas, validaciones y anti-duplicado. |
| 10 | Notificaciones | ❌ | Falta envío de correo, cola y reintentos. |
| 11 | Auditoría y trazabilidad | ❌ | No existe modelo/eventos de auditoría consultables. |
| 12 | Seguridad | 🟡 | JWT básico; faltan políticas de autorización por rol, hardening y cifrado de datos sensibles adicionales. |
| 13 | API backend completa | 🟡 | Auth/usuarios/tenants existen; faltan documentos/firma/dashboard/auditoría. |
| 14 | Base de datos | 🟡 | Modelo relacional básico; faltan tablas núcleo (documentos, firmas, auditoría, notificaciones). |
| 15 | Despliegue y mantenimiento | 🟡 | Falta documentación operativa de despliegue, observabilidad y runbooks. |

---

## 3) Prioridad sugerida (roadmap)

### Fase 1 — Seguridad y base multitenant (inmediato)
1. Implementar autenticación JWT validada en middleware.
2. Implementar `TenantContext` + filtrado por tenant en queries/repositorios.
3. Implementar RBAC mínimo: `AdminEmpresa`, `Empleado`.
4. Agregar endpoint de actualizar/activar/desactivar usuario.

### Fase 2 — Núcleo documental
1. Crear módulo `Documentos` (entidad + CQRS).
2. Integrar almacenamiento cloud (Azure Blob / S3) con URLs temporales.
3. Carga masiva con procesamiento asíncrono (cola/Hangfire).
4. Dashboard básico por usuario.

### Fase 3 — Firma, notificaciones y auditoría
1. Firma electrónica simple + anti doble firma.
2. Notificaciones por email con reintentos y registro histórico.
3. Auditoría transversal (login, carga, visualización, firma).

---

## 4) Qué clases/interfaces crear (propuesta concreta por capa)

## Domain
- `Documento` (Aggregate Root)
- `FirmaDocumento`
- `Notificacion`
- `AuditoriaEvento`
- `DocumentoCategoria` (enum/value object)
- `EstadoDocumento` (enum: Pendiente/Firmado/Rechazado)
- `RolUsuario` (enum: AdminEmpresa/Empleado)

## Application
- `Modules/Documentos/Command/SubirDocumentoCommand`
- `Modules/Documentos/Command/SubirLoteDocumentosCommand`
- `Modules/Documentos/Query/ObtenerDocumentosUsuarioQuery`
- `Modules/Firmas/Command/FirmarDocumentoCommand`
- `Modules/Dashboard/Query/ObtenerResumenDashboardQuery`
- `Modules/Auditoria/Query/ObtenerEventosAuditoriaQuery`
- `Modules/Usuarios/Command/ActualizarUsuarioCommand`
- `Modules/Usuarios/Command/CambiarEstadoUsuarioCommand`
- `Modules/Auths/Command/SolicitarRecuperacionPasswordCommand`
- `Modules/Auths/Command/RestablecerPasswordCommand`

Interfaces clave:
- `IStorageService` (subir/obtener URL temporal/eliminar)
- `INotificationService` (correo/notificación interna)
- `IBackgroundJobClient` o `IQueuePublisher`
- `IAuditService`
- `ITenantProvider`

## Infrastructure
- `Persistence/Configurations/DocumentoConfiguration`
- `Repositories/DocumentoRepository`
- `Storage/AzureBlobStorageService` (o `S3StorageService`)
- `Notifications/SmtpNotificationService`
- `BackgroundJobs/DocumentProcessingJob`
- `Security/TenantQueryFilter` (global query filters)
- `Security/PermissionHandler` + políticas
- `Auditing/AuditInterceptor` (EF SaveChanges interceptor)

## API
- `Controllers/DocumentosController`
- `Controllers/FirmasController`
- `Controllers/DashboardController`
- `Controllers/AuditoriaController`
- `Middleware/TenantResolutionMiddleware`
- `Middleware/RequestCorrelationMiddleware`

---

## 5) Diseño de base de datos recomendado (mínimo)

Tablas nuevas mínimas:
- `documentos`
- `firma_documento`
- `notificaciones`
- `notificacion_envios`
- `auditoria_eventos`

Todos con:
- `tenant_id`
- metadatos de creación/actualización
- índices por tenant + columnas de búsqueda (correo, dni, estado)

---

## 6) Recomendaciones técnicas inmediatas

1. **No exponer secretos** en `appsettings.json`; mover a variables de entorno/secret manager.
2. Agregar **pruebas unitarias** para handlers críticos (`login`, `crear usuario`, `tenant filter`).
3. Definir un estándar de errores (`ErrorCodes`) y respuestas problem details.
4. Activar OpenAPI con seguridad JWT documentada.
5. Registrar logs estructurados con `Serilog` + correlación.

---

## 7) Definition of Done por módulo (extracto)

### Documentos
- Subida individual y masiva.
- Asociación por DNI/código/correo.
- Persistencia de metadatos + URL de almacenamiento.
- Consultas paginadas y filtradas.

### Firma
- Solo usuario propietario o admin de tenant.
- Guardar IP, fecha/hora, usuario.
- Bloquear re-firma.

### Auditoría
- Evento por login, upload, visualización, firma.
- Consulta por tenant, rango de fecha y usuario.

