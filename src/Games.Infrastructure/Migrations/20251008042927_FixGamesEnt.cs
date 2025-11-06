using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Games.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixGamesEnt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "JOGOS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Quantidade",
                table: "JOGOS",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
