using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoFinal.Migrations
{
    /// <inheritdoc />
    public partial class projFinal4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Utilizadors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Utilizadors");
        }
    }
}
