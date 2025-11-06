using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Obracore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "obras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dt_inicio = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    dt_fim_prevista = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    dt_fim = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    dt_criacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    dt_edicao = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status_obra = table.Column<string>(type: "char(1)", maxLength: 1, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_obras", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "perfis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfis", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    senha = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dt_criacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    dt_edicao = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "documentos_obras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    titulo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    caminho = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dt_criacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    obras_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos_obras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_documentos_obras_obras_obras_id",
                        column: x => x.obras_id,
                        principalTable: "obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "etapas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "char(1)", maxLength: 1, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ordem = table.Column<int>(type: "int", nullable: true),
                    dt_inicio = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    dt_fim_prevista = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    dt_fim = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    etapascol = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    obras_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etapas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_etapas_obras_obras_id",
                        column: x => x.obras_id,
                        principalTable: "obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    obras_id = table.Column<int>(type: "int", nullable: false),
                    tipo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    titulo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    caminho_arquivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    enviado_por_id = table.Column<int>(type: "int", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    data_edicao = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_documentos_obras_obras_id",
                        column: x => x.obras_id,
                        principalTable: "obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_documentos_usuarios_enviado_por_id",
                        column: x => x.enviado_por_id,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuario_perfil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PerfilId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario_perfil", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuario_perfil_perfis_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "perfis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuario_perfil_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios_obras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ObraId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_obras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_obras_obras_ObraId",
                        column: x => x.ObraId,
                        principalTable: "obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarios_obras_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "custos_etapas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    dt_registro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    etapas_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custos_etapas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_custos_etapas_etapas_etapas_id",
                        column: x => x.etapas_id,
                        principalTable: "etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "custos_obra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    dt_registro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    etapas_id = table.Column<int>(type: "int", nullable: true),
                    obras_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custos_obra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_custos_obra_etapas_etapas_id",
                        column: x => x.etapas_id,
                        principalTable: "etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_custos_obra_obras_obras_id",
                        column: x => x.obras_id,
                        principalTable: "obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "documentos_etapa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    titulo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    caminho = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dt_criacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    etapas_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos_etapa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_documentos_etapa_etapas_etapas_id",
                        column: x => x.etapas_id,
                        principalTable: "etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "financas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    obras_id = table.Column<int>(type: "int", nullable: false),
                    etapas_id = table.Column<int>(type: "int", nullable: true),
                    descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    tipo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_registro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    data_edicao = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_financas_etapas_etapas_id",
                        column: x => x.etapas_id,
                        principalTable: "etapas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_financas_obras_obras_id",
                        column: x => x.obras_id,
                        principalTable: "obras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_custos_etapas_etapas_id",
                table: "custos_etapas",
                column: "etapas_id");

            migrationBuilder.CreateIndex(
                name: "IX_custos_obra_etapas_id",
                table: "custos_obra",
                column: "etapas_id");

            migrationBuilder.CreateIndex(
                name: "IX_custos_obra_obras_id",
                table: "custos_obra",
                column: "obras_id");

            migrationBuilder.CreateIndex(
                name: "IX_documentos_enviado_por_id",
                table: "documentos",
                column: "enviado_por_id");

            migrationBuilder.CreateIndex(
                name: "IX_documentos_obras_id",
                table: "documentos",
                column: "obras_id");

            migrationBuilder.CreateIndex(
                name: "IX_documentos_etapa_etapas_id",
                table: "documentos_etapa",
                column: "etapas_id");

            migrationBuilder.CreateIndex(
                name: "IX_documentos_obras_obras_id",
                table: "documentos_obras",
                column: "obras_id");

            migrationBuilder.CreateIndex(
                name: "IX_etapas_obras_id",
                table: "etapas",
                column: "obras_id");

            migrationBuilder.CreateIndex(
                name: "IX_financas_etapas_id",
                table: "financas",
                column: "etapas_id");

            migrationBuilder.CreateIndex(
                name: "IX_financas_obras_id",
                table: "financas",
                column: "obras_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_perfil_PerfilId",
                table: "usuario_perfil",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_perfil_UsuarioId",
                table: "usuario_perfil",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_obras_ObraId",
                table: "usuarios_obras",
                column: "ObraId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_obras_UsuarioId",
                table: "usuarios_obras",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "custos_etapas");

            migrationBuilder.DropTable(
                name: "custos_obra");

            migrationBuilder.DropTable(
                name: "documentos");

            migrationBuilder.DropTable(
                name: "documentos_etapa");

            migrationBuilder.DropTable(
                name: "documentos_obras");

            migrationBuilder.DropTable(
                name: "financas");

            migrationBuilder.DropTable(
                name: "usuario_perfil");

            migrationBuilder.DropTable(
                name: "usuarios_obras");

            migrationBuilder.DropTable(
                name: "etapas");

            migrationBuilder.DropTable(
                name: "perfis");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "obras");
        }
    }
}
