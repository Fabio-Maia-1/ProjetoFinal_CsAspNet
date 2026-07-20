using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoFinal.Migrations
{
    /// <inheritdoc />
    public partial class projFinal3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "posterPath",
                table: "Filmes",
                newName: "PosterPath");

            migrationBuilder.RenameColumn(
                name: "disponivel",
                table: "Filmes",
                newName: "Disponivel");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "TiposDeBilhetes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "TiposDeBilhetes");

            migrationBuilder.RenameColumn(
                name: "PosterPath",
                table: "Filmes",
                newName: "posterPath");

            migrationBuilder.RenameColumn(
                name: "Disponivel",
                table: "Filmes",
                newName: "disponivel");
        }
    }
}
