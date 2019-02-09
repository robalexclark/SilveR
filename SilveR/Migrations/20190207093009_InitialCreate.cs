using Microsoft.EntityFrameworkCore.Migrations;

namespace SilveR.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayLSMeanslines",
                table: "UserOptions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplaySEMlines",
                table: "UserOptions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScatterLabels",
                table: "UserOptions",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayLSMeanslines",
                table: "UserOptions");

            migrationBuilder.DropColumn(
                name: "DisplaySEMlines",
                table: "UserOptions");

            migrationBuilder.DropColumn(
                name: "ScatterLabels",
                table: "UserOptions");
        }
    }
}
