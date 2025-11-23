using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace soulBalanceGs.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelasIniciais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_SOUL_BALANCE_USUARIO",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    senha = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_SOUL_BALANCE_USUARIO", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Atividades",
                columns: table => new
                {
                    AtividadeId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TipoAtividade = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Inicio = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Fim = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DuracaoMinutosAtividade = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    FkIdUsuario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atividades", x => x.AtividadeId);
                    table.ForeignKey(
                        name: "FK_Atividades_TB_SOUL_BALANCE_USUARIO_FkIdUsuario",
                        column: x => x.FkIdUsuario,
                        principalTable: "TB_SOUL_BALANCE_USUARIO",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckinManuais",
                columns: table => new
                {
                    chekin_id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    humor = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    energia = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    foco = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    time = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    fk_id_usuario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckinManuais", x => x.chekin_id);
                    table.ForeignKey(
                        name: "FK_CheckinManuais_TB_SOUL_BALANCE_USUARIO_fk_id_usuario",
                        column: x => x.fk_id_usuario,
                        principalTable: "TB_SOUL_BALANCE_USUARIO",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DadosSensores",
                columns: table => new
                {
                    DadosSensorId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TipoDado = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Valor = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Time = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FkIdUsuario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosSensores", x => x.DadosSensorId);
                    table.ForeignKey(
                        name: "FK_DadosSensores_TB_SOUL_BALANCE_USUARIO_FkIdUsuario",
                        column: x => x.FkIdUsuario,
                        principalTable: "TB_SOUL_BALANCE_USUARIO",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_FkIdUsuario",
                table: "Atividades",
                column: "FkIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_CheckinManuais_fk_id_usuario",
                table: "CheckinManuais",
                column: "fk_id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_DadosSensores_FkIdUsuario",
                table: "DadosSensores",
                column: "FkIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_TB_SOUL_BALANCE_USUARIO_email",
                table: "TB_SOUL_BALANCE_USUARIO",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Atividades");

            migrationBuilder.DropTable(
                name: "CheckinManuais");

            migrationBuilder.DropTable(
                name: "DadosSensores");

            migrationBuilder.DropTable(
                name: "TB_SOUL_BALANCE_USUARIO");
        }
    }
}
