# MiBoleta — README (español)

> Plataforma MiBoleta — backend en **.NET 9.0.302** y frontend en **Angular 17**.
> Arquitectura: **Clean Architecture** + **DDD**. Patrones y componentes: **Result**, **Mediator**, **CQRS**, **JWT**, **AutoMapper**, **Handlers**, **EF Core Migrations**, **SignalR**, **OCR** para detección de texto en ficheros, y procesos en segundo plano para carga masiva (por ejemplo **Hangfire**). Información y alcance base en la propuesta del proyecto. 

---

## Tabla de contenido

1. Descripción
2. Tech stack
3. Principios arquitectónicos
4. Estructura del repositorio (sugerida)
5. Patrones y decisiones clave
6. Autenticación y autorización (JWT)
7. Persistencia y migraciones
8. Notificaciones en tiempo real (SignalR)
9. OCR — detección de texto en ficheros
10. Carga masiva y procesos en segundo plano (Hangfire/colas)
11. Desarrollo local — pasos rápidos
12. Variables de entorno importantes
13. Scripts útiles
14. Despliegue, CI/CD y recomendaciones
15. Contribuciones y convenciones
16. Licencia

---

## 1) Descripción

MiBoleta es una plataforma para la gestión y distribución de boletas digitales (carga individual y masiva de PDFs), con visualización, firma electrónica simple, notificaciones y auditoría. El backend seguirá una Clean Architecture basada en DDD para separar responsabilidades y facilitar pruebas, mantenimiento y evolución.

---

## 2) Tech stack

* Backend: **.NET 9.0.302**, C#, Entity Framework Core (última versión compatible), MediatR (Mediator), AutoMapper.
* Frontend: **Angular 17**, TypeScript, Angular Material.
* Autenticación: **JWT** (tokens expirables, refresh token opcional).
* Real-time: **SignalR** para notificaciones push.
* OCR: opción local (Tesseract) o servicio en la nube (Azure Cognitive Services Computer Vision / OCR).
* Background / Carga masiva: **Hangfire** para jobs en background o patrones de cola (RabbitMQ / Azure Service Bus) si se requiere escalado.
* Base de datos: SQL Server (o el RDBMS elegido).
* Documentación: **Swagger / OpenAPI**.
* CI/CD: GitHub Actions / Azure DevOps / GitLab CI (según preferencia).

---

## 3) Principios arquitectónicos

* **Clean Architecture + DDD**: capas típicas — `Domain` (entidades, agregados, value objects, reglas de negocio), `Application` (casos de uso, DTOs, interfaces), `Infrastructure` (EF Core, repositorios, servicios externos), `API` (controllers), `WebClient` (Angular).
* **Separation of concerns** y dependencia dirigida hacia el dominio (inversion of control).
* **Testable**: lógica en `Domain`/`Application` con unit tests; `Infrastructure` limitada a adaptadores.

---

## 4) Estructura del repositorio (sugerida)

```
/src
  /Api                        # API .NET (entry point)
  /Application                # Casos de uso, DTOs, interfaces, contratos
  /Domain                     # Entidades, ValueObjects, Reglas
  /Infrastructure             # EF Core, Migrations, Persistence, OCR adapters, Email, SignalR hubs
  /BackgroundJobs             # Hangfire jobs / workers / consumidores de cola
  /Shared                     # Result pattern, exceptions comunes, utilidades
/frontend
  /mi-boleta-angular          # Angular 17 app
/scripts
/tests
  /UnitTests
  /IntegrationTests
/docs
  README.md
  ARCHITECTURE.md
```

---

## 5) Patrones y decisiones clave

* **Result pattern**: métodos del dominio/aplicación devuelven `Result<T>` o `Result` para evitar excepciones como flujo normal y facilitar manejo de errores.
* **Mediator (MediatR)**: todos los comandos/queries pasan por MediatR; handlers en `Application/Handlers`.
* **CQRS**: separar Commands (escritura) y Queries (lectura). Queries pueden usar proyecciones optimizadas o Dapper si se requiere rendimiento.
* **Handlers**: handlers por comando/query, inyectan repositorios / servicios necesarios.
* **AutoMapper**: mapeos DTO <-> Domain/Entity en `Application` con perfiles centralizados.
* **Validación**: FluentValidation (o validator propio) en comandos antes de ejecutar handlers.
* **Transaccionalidad**: Unit of Work en Infrastructure (DbContext), y manejo de transacciones explícitas cuando varias operaciones cruzan agregados.

---

## 6) Autenticación y autorización (JWT)

* Emisión de access token con claims (rol, tenant/empresa, userId).
* Refresh token opcional almacenado en DB (si se requiere renovar sin login).
* Políticas y roles: autorización por políticas (Roles: Admin, Manager, User, etc.).
* Seguridad: secrets gestionados por vault o variables de entorno; no almacenar secretos en repo.

---

## 7) Persistencia y migraciones

* EF Core con Migrations. Mantener migraciones en el proyecto `Infrastructure.Persistence`.
* Estrategia: migraciones en CI/CD o al arrancar en entornos no productivos; no aplicar migraciones automáticas en prod sin control.
* Tests: usar InMemory o Sqlite para pruebas unitarias/integración.

---

## 8) Notificaciones en tiempo real (SignalR)

* Hub(s) para notificaciones de publicación de boletas, estado de procesamiento de OCR, progreso en cargas masivas.
* Backend emite eventos desde handlers o jobs; SignalR envía a los clientes conectados.
* Soporte para grupos por empresa/usuario (multi-tenant).

---

## 9) OCR — detección de texto en ficheros

Opciones y recomendaciones:

* **Opción cloud (recomendada si presupuesto lo permite):** Azure Cognitive Services (OCR / Read API) o Google Vision — alta precisión, soporte PDF multi-page y layout detection.
* **Opción local (sin costos externos):** Tesseract OCR (recomendado para PoC o privacidad), requiere preprocesamiento (deskew, binarization) para mejores resultados.
* Flujo sugerido: al subir archivo → persistir metadatos → encolar job OCR → extraer texto y entidades (DNI, nombres, montos) → vincular boleta al usuario → notificar resultado (SignalR / email).
* Guardar logs/resultado OCR y confidence scores para auditoría.

---

## 10) Carga masiva y procesamiento en segundo plano

* **Hangfire**: scheduler + dashboard para jobs recurrentes y procesamiento en background. Bueno para jobs de larga duración y retry.
* **Colas (cuando se requiere alto throughput):** RabbitMQ / Azure Service Bus / AWS SQS + workers escalables.
* Diseño: uploader en frontend sube archivos en lotes; backend recibe y crea jobs por archivo/registro; workers procesan OCR, validaciones y persistencia.
* Reporte de progreso: SignalR para feedback en la UI.

---

## 11) Desarrollo local — pasos rápidos

1. Clonar repo.
2. Backend:

   * Instalar .NET 9 SDK (versión 9.0.302).
   * `cd src/Api`
   * Configurar `appsettings.Development.json` con connection string y JWT secrets (ver sección variables).
   * `dotnet ef database update --project ../Infrastructure.Persistence` (o usar script).
   * `dotnet run` (levanta API).
3. Frontend:

   * `cd frontend/mi-boleta-angular`
   * Instalar dependencias `npm install`
   * `ng serve` (Angular 17)
4. Abrir `http://localhost:4200` y `https://localhost:5001/swagger/index.html`.

---

## 12) Variables de entorno importantes (ejemplo)

* `ConnectionStrings__DefaultConnection`
* `Jwt__Issuer`, `Jwt__Audience`, `Jwt__Secret`, `Jwt__ExpiresMinutes`
* `Hangfire__StorageConnection` (si se usa)
* `OCR__Provider` (Tesseract | Azure) y claves (`OCR__Azure__Key`, `OCR__Azure__Endpoint`)
* `SignalR__AllowedOrigins`
* `Smtp__Host`, `Smtp__User`, `Smtp__Pass`
* `Storage__BlobConnection` (si se usan blobs para ficheros)

---

## 13) Scripts útiles (ejemplos)

* `scripts/migrate.sh`: aplicar migraciones.
* `scripts/seed-dev.sh`: datos iniciales para desarrollo.
* `frontend/package.json` scripts: `start`, `build`, `test`.
* `dotnet` scripts: `dotnet test`, `dotnet ef migrations add`, etc.

---

## 14) Despliegue y CI/CD — recomendaciones

* Pipeline típicos: build → tests → publicar artefactos → despliegue a staging → migraciones controladas → smoke tests → deploy prod.
* No aplicar migraciones automáticas en prod sin backup y plan rollback.
* Para escalado de background jobs considere separar worker processes y usar colas.
* Secrets: usar KeyVault / Secret Manager / GitHub Secrets (no en repo).

---

## 15) Contribuciones, convenciones y buenas prácticas

* Branching: `main` protegido; ramas feature `feature/xxx`; PRs con revisión.
* Convenciones de código: StyleCop / EditorConfig para C#.
* Tests: mínimo unit tests para dominio/handlers y cobertura básica para queries/commands críticos.
* Documentación: actualizar `ARCHITECTURE.md` y `API` con Swagger.

---

## 16) Plantillas de archivos importantes

* `ARCHITECTURE.md` — diagrama de capas, flujo CQRS, decisión OCR/colas.
* `API/README.md` — endpoints principales, autenticación, muestras de request/response.
* `frontend/README.md` — cómo levantar Angular, variables de entorno.

---

## 17) Notas finales y próximos pasos rápidos

* Priorizar diseño del **Domain** (agregados, invariantes) y definir contratos de los casos de uso.
* Definir proveedor OCR (Tesseract para PoC; Azure/Google para producción con PDF multi-page).
* Decidir estrategia de colas vs Hangfire según volumen esperado.
* Mantener la propuesta técnica y estimación como referencia (alcance y tiempos) dentro de `/docs`. 

