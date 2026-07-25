using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soleil.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionNumberToGameSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SessionNumber",
                table: "GameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionNumber",
                table: "GameSessions");
        }
    }
}
