using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SilveR.Migrations
{
    public partial class InitalCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Datasets",
                columns: table => new
                {
                    DatasetID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DatasetName = table.Column<string>(maxLength: 50, nullable: false),
                    VersionNo = table.Column<int>(nullable: false),
                    TheData = table.Column<string>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datasets", x => x.DatasetID);
                });

            migrationBuilder.CreateTable(
                name: "Scripts",
                columns: table => new
                {
                    ScriptID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScriptDisplayName = table.Column<string>(maxLength: 100, nullable: false),
                    ScriptFileName = table.Column<string>(maxLength: 50, nullable: false),
                    RequiresDataset = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scripts", x => x.ScriptID);
                });

            migrationBuilder.CreateTable(
                name: "UserOptions",
                columns: table => new
                {
                    UserOptionID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LineTypeSolid = table.Column<string>(nullable: false),
                    LineTypeDashed = table.Column<string>(nullable: false),
                    GraphicsFont = table.Column<string>(nullable: false),
                    FontStyle = table.Column<string>(nullable: false),
                    GraphicsTextColour = table.Column<string>(nullable: false),
                    ColourFill = table.Column<string>(nullable: false),
                    BWFill = table.Column<string>(nullable: false),
                    CategoryBarFill = table.Column<string>(nullable: false),
                    ColourLine = table.Column<string>(nullable: false),
                    BWLine = table.Column<string>(nullable: false),
                    LegendTextColour = table.Column<string>(nullable: false),
                    LegendPosition = table.Column<string>(nullable: false),
                    PaletteSet = table.Column<string>(nullable: false),
                    OutputData = table.Column<string>(nullable: false),
                    OutputPlotsAsPdf = table.Column<string>(nullable: false),
                    OutputPlotsInBW = table.Column<string>(nullable: false),
                    GeometryDisplay = table.Column<string>(nullable: false),
                    CovariateRegressionCoefficients = table.Column<string>(nullable: false),
                    AssessCovariateInteractions = table.Column<string>(nullable: false),
                    TitleSize = table.Column<int>(nullable: false),
                    XAxisTitleFontSize = table.Column<int>(nullable: false),
                    YAxisTitleFontSize = table.Column<int>(nullable: false),
                    XLabelsFontSize = table.Column<int>(nullable: false),
                    YLabelsFontSize = table.Column<int>(nullable: false),
                    GraphicsXAngle = table.Column<int>(nullable: false),
                    GraphicsXHorizontalJust = table.Column<decimal>(nullable: false),
                    GraphicsYAngle = table.Column<int>(nullable: false),
                    GraphicsYVerticalJust = table.Column<decimal>(nullable: false),
                    PointSize = table.Column<int>(nullable: false),
                    PointShape = table.Column<int>(nullable: false),
                    LineSize = table.Column<int>(nullable: false),
                    LegendTextSize = table.Column<int>(nullable: false),
                    PdfWidth = table.Column<int>(nullable: false),
                    PdfHeight = table.Column<int>(nullable: false),
                    JpegWidth = table.Column<int>(nullable: false),
                    JpegHeight = table.Column<int>(nullable: false),
                    GraphicsBWLow = table.Column<decimal>(nullable: false),
                    GraphicsBWHigh = table.Column<decimal>(nullable: false),
                    GraphicsWidthJitter = table.Column<decimal>(nullable: false),
                    GraphicsHeightJitter = table.Column<decimal>(nullable: false),
                    ErrorBarWidth = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOptions", x => x.UserOptionID);
                });

            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    AnalysisID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnalysisGuid = table.Column<string>(maxLength: 128, nullable: false),
                    DatasetID = table.Column<int>(nullable: true),
                    DatasetName = table.Column<string>(maxLength: 50, nullable: true),
                    ScriptID = table.Column<int>(nullable: false),
                    Tag = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    RProcessOutput = table.Column<string>(nullable: true),
                    HtmlOutput = table.Column<string>(nullable: true),
                    DateAnalysed = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.AnalysisID);
                    table.ForeignKey(
                        name: "FK_Analyses_Datasets",
                        column: x => x.DatasetID,
                        principalTable: "Datasets",
                        principalColumn: "DatasetID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analyses_Scripts",
                        column: x => x.ScriptID,
                        principalTable: "Scripts",
                        principalColumn: "ScriptID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Arguments",
                columns: table => new
                {
                    ArgumentID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnalysisID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Value = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arguments", x => x.ArgumentID);
                    table.ForeignKey(
                        name: "FK_Arguments_Analyses",
                        column: x => x.AnalysisID,
                        principalTable: "Analyses",
                        principalColumn: "AnalysisID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_DatasetID",
                table: "Analyses",
                column: "DatasetID");

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_ScriptID",
                table: "Analyses",
                column: "ScriptID");

            migrationBuilder.CreateIndex(
                name: "IX_Arguments_AnalysisID",
                table: "Arguments",
                column: "AnalysisID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arguments");

            migrationBuilder.DropTable(
                name: "UserOptions");

            migrationBuilder.DropTable(
                name: "Analyses");

            migrationBuilder.DropTable(
                name: "Datasets");

            migrationBuilder.DropTable(
                name: "Scripts");
        }
    }
}
