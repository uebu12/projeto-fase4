using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fiap.EcoColeta.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ECO_PONTO_COLETA",
                columns: table => new
                {
                    PT_COLETA_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NOME = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ENDERECO = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BAIRRO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CAPACIDADE_MAXIMA_KG = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    VOLUME_ATUAL_KG = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ATIVO = table.Column<bool>(type: "bit", nullable: false),
                    DATA_CADASTRO = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ECO_PONTO_COLETA", x => x.PT_COLETA_ID);
                });

            migrationBuilder.CreateTable(
                name: "ECO_TIPO_RESIDUO",
                columns: table => new
                {
                    TP_RESIDUO_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NOME = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FATOR_IMPACTO_CO2 = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    RECICLAVEL = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ECO_TIPO_RESIDUO", x => x.TP_RESIDUO_ID);
                });

            migrationBuilder.CreateTable(
                name: "ECO_ALERTA",
                columns: table => new
                {
                    ALERTA_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PontoColetaId = table.Column<int>(type: "int", nullable: false),
                    DATA_ALERTA = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PERCENTUAL_OCUPACAO = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MENSAGEM = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RESOLVIDO = table.Column<bool>(type: "bit", nullable: false),
                    DATA_RESOLUCAO = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ECO_ALERTA", x => x.ALERTA_ID);
                    table.ForeignKey(
                        name: "FK_ECO_ALERTA_ECO_PONTO_COLETA_PontoColetaId",
                        column: x => x.PontoColetaId,
                        principalTable: "ECO_PONTO_COLETA",
                        principalColumn: "PT_COLETA_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ECO_COLETA",
                columns: table => new
                {
                    COLETA_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PontoColetaId = table.Column<int>(type: "int", nullable: false),
                    TipoResiduoId = table.Column<int>(type: "int", nullable: false),
                    QUANTIDADE_KG = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DATA_COLETA = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OBSERVACAO = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ECO_COLETA", x => x.COLETA_ID);
                    table.ForeignKey(
                        name: "FK_ECO_COLETA_ECO_PONTO_COLETA_PontoColetaId",
                        column: x => x.PontoColetaId,
                        principalTable: "ECO_PONTO_COLETA",
                        principalColumn: "PT_COLETA_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ECO_COLETA_ECO_TIPO_RESIDUO_TipoResiduoId",
                        column: x => x.TipoResiduoId,
                        principalTable: "ECO_TIPO_RESIDUO",
                        principalColumn: "TP_RESIDUO_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ECO_TIPO_RESIDUO",
                columns: new[] { "TP_RESIDUO_ID", "FATOR_IMPACTO_CO2", "NOME", "RECICLAVEL" },
                values: new object[,]
                {
                    { 1, 0.9m, "Papel/Papelão", true },
                    { 2, 1.5m, "Plástico", true },
                    { 3, 0.3m, "Vidro", true },
                    { 4, 9.0m, "Metal/Alumínio", true },
                    { 5, 1.2m, "Eletrônico", true },
                    { 6, 0.5m, "Orgânico", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ECO_ALERTA_PontoColetaId",
                table: "ECO_ALERTA",
                column: "PontoColetaId");

            migrationBuilder.CreateIndex(
                name: "IX_ECO_COLETA_DATA_COLETA",
                table: "ECO_COLETA",
                column: "DATA_COLETA");

            migrationBuilder.CreateIndex(
                name: "IX_ECO_COLETA_PontoColetaId",
                table: "ECO_COLETA",
                column: "PontoColetaId");

            migrationBuilder.CreateIndex(
                name: "IX_ECO_COLETA_TipoResiduoId",
                table: "ECO_COLETA",
                column: "TipoResiduoId");

            migrationBuilder.CreateIndex(
                name: "IX_ECO_TIPO_RESIDUO_NOME",
                table: "ECO_TIPO_RESIDUO",
                column: "NOME",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ECO_ALERTA");

            migrationBuilder.DropTable(
                name: "ECO_COLETA");

            migrationBuilder.DropTable(
                name: "ECO_PONTO_COLETA");

            migrationBuilder.DropTable(
                name: "ECO_TIPO_RESIDUO");
        }
    }
}
