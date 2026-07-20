using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoFinal.Migrations
{
    /// <inheritdoc />
    public partial class projFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Filmes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duracao = table.Column<int>(type: "int", nullable: false),
                    Genero = table.Column<int>(type: "int", nullable: false),
                    ClassificacaoEtaria = table.Column<int>(type: "int", nullable: true),
                    posterPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    disponivel = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filmes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroFilas = table.Column<int>(type: "int", nullable: false),
                    LugaresPorFila = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDeBilhetes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDeBilhetes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilizadors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoUtilizador = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lugares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fila = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    SalaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lugares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lugares_Salas_SalaId",
                        column: x => x.SalaId,
                        principalTable: "Salas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FilmeId = table.Column<int>(type: "int", nullable: false),
                    SalaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessoes_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessoes_Salas_SalaId",
                        column: x => x.SalaId,
                        principalTable: "Salas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compras_Utilizadors_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Utilizadors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bilhetes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SessaoId = table.Column<int>(type: "int", nullable: false),
                    LugarId = table.Column<int>(type: "int", nullable: false),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    TipoId = table.Column<int>(type: "int", nullable: false),
                    TipoDeBilheteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bilhetes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bilhetes_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bilhetes_Lugares_LugarId",
                        column: x => x.LugarId,
                        principalTable: "Lugares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bilhetes_Sessoes_SessaoId",
                        column: x => x.SessaoId,
                        principalTable: "Sessoes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bilhetes_TiposDeBilhetes_TipoDeBilheteId",
                        column: x => x.TipoDeBilheteId,
                        principalTable: "TiposDeBilhetes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bilhetes_CompraId",
                table: "Bilhetes",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Bilhetes_LugarId",
                table: "Bilhetes",
                column: "LugarId");

            migrationBuilder.CreateIndex(
                name: "IX_Bilhetes_SessaoId",
                table: "Bilhetes",
                column: "SessaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Bilhetes_TipoDeBilheteId",
                table: "Bilhetes",
                column: "TipoDeBilheteId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ClienteId",
                table: "Compras",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Lugares_SalaId",
                table: "Lugares",
                column: "SalaId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_FilmeId",
                table: "Sessoes",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_SalaId",
                table: "Sessoes",
                column: "SalaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bilhetes");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Lugares");

            migrationBuilder.DropTable(
                name: "Sessoes");

            migrationBuilder.DropTable(
                name: "TiposDeBilhetes");

            migrationBuilder.DropTable(
                name: "Utilizadors");

            migrationBuilder.DropTable(
                name: "Filmes");

            migrationBuilder.DropTable(
                name: "Salas");
        }
    }
}
