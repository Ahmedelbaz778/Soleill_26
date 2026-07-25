using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soleil.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileImageToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "Doctors");
        }
    }
}
