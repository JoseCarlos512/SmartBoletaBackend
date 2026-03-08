using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBoleta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCargaMasiva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CargaMasivas",
                columns: table => new
                {
                    CargaMasivaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioSolicitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Periodo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    TotalArchivos = table.Column<int>(type: "int", nullable: false),
                    ArchivosProcessados = table.Column<int>(type: "int", nullable: false),
                    ArchivosExitosos = table.Column<int>(type: "int", nullable: false),
                    ArchivosFallidos = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    FechaFin = table.Column<DateTime>(type: "DATETIME2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargaMasivas", x => x.CargaMasivaId);
                    table.ForeignKey(
                        name: "FK_CargaMasivas_Tenants",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId");
                    table.ForeignKey(
                        name: "FK_CargaMasivas_Usuarios",
                        column: x => x.UsuarioSolicitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateTable(
                name: "CargaMasivaArchivos",
                columns: table => new
                {
                    CargaMasivaArchivoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CargaMasivaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArchivoNombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ArchivoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    UsuarioIdentificadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BoletaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ErrorMensaje = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TextoOcr = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargaMasivaArchivos", x => x.CargaMasivaArchivoId);
                    table.ForeignKey(
                        name: "FK_CargaMasivaArchivos_CargaMasivas",
                        column: x => x.CargaMasivaId,
                        principalTable: "CargaMasivas",
                        principalColumn: "CargaMasivaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_carga_masiva_archivos_carga_masiva_id",
                table: "CargaMasivaArchivos",
                column: "CargaMasivaId");

            migrationBuilder.CreateIndex(
                name: "ix_carga_masivas_tenant_id",
                table: "CargaMasivas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "ix_carga_masivas_usuario_solicitante_id",
                table: "CargaMasivas",
                column: "UsuarioSolicitanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargaMasivaArchivos");

            migrationBuilder.DropTable(
                name: "CargaMasivas");
        }
    }
}
