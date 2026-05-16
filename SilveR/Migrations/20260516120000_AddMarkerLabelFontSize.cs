using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SilveR.Models;

namespace SilveR.Migrations
{
    [DbContext(typeof(SilveRContext))]
    [Migration("20260516120000_AddMarkerLabelFontSize")]
    public partial class AddMarkerLabelFontSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MarkerLabelFontSize",
                table: "UserOptions",
                type: "REAL",
                nullable: false,
                defaultValue: 3.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkerLabelFontSize",
                table: "UserOptions");
        }
    }
}
