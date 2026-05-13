using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soleil.Migrations
{
    /// <inheritdoc />
    public partial class updateeyescan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResultPercentage",
                table: "EyeScanTests",
                newName: "TdProbability");

            migrationBuilder.AddColumn<double>(
                name: "AsdProbability",
                table: "EyeScanTests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Confidence",
                table: "EyeScanTests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Decision",
                table: "EyeScanTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PointsAnalyzed",
                table: "EyeScanTests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Recommendation",
                table: "EyeScanTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "EyeScanTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AsdProbability",
                table: "EyeScanTests");

            migrationBuilder.DropColumn(
                name: "Confidence",
                table: "EyeScanTests");

            migrationBuilder.DropColumn(
                name: "Decision",
                table: "EyeScanTests");

            migrationBuilder.DropColumn(
                name: "PointsAnalyzed",
                table: "EyeScanTests");

            migrationBuilder.DropColumn(
                name: "Recommendation",
                table: "EyeScanTests");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "EyeScanTests");

            migrationBuilder.RenameColumn(
                name: "TdProbability",
                table: "EyeScanTests",
                newName: "ResultPercentage");
        }
    }
}
