using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SilveR.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Datasets",
                columns: table => new
                {
                    DatasetID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DatasetName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    VersionNo = table.Column<int>(type: "INTEGER", nullable: false),
                    TheData = table.Column<string>(type: "TEXT", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datasets", x => x.DatasetID);
                });

            migrationBuilder.CreateTable(
                name: "Scripts",
                columns: table => new
                {
                    ScriptID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScriptDisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ScriptFileName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RequiresDataset = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scripts", x => x.ScriptID);
                });

            migrationBuilder.CreateTable(
                name: "UserOptions",
                columns: table => new
                {
                    UserOptionID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LineTypeSolid = table.Column<string>(type: "TEXT", nullable: false),
                    LineTypeDashed = table.Column<string>(type: "TEXT", nullable: false),
                    GraphicsFont = table.Column<string>(type: "TEXT", nullable: false),
                    FontStyle = table.Column<string>(type: "TEXT", nullable: false),
                    GraphicsTextColour = table.Column<string>(type: "TEXT", nullable: false),
                    ColourFill = table.Column<string>(type: "TEXT", nullable: false),
                    BWFill = table.Column<string>(type: "TEXT", nullable: false),
                    FillTransparency = table.Column<double>(type: "REAL", nullable: false),
                    CategoryBarFill = table.Column<string>(type: "TEXT", nullable: false),
                    ColourLine = table.Column<string>(type: "TEXT", nullable: false),
                    BWLine = table.Column<string>(type: "TEXT", nullable: false),
                    LegendTextColour = table.Column<string>(type: "TEXT", nullable: false),
                    LegendPosition = table.Column<string>(type: "TEXT", nullable: false),
                    PaletteSet = table.Column<string>(type: "TEXT", nullable: false),
                    OutputData = table.Column<bool>(type: "INTEGER", nullable: false),
                    OutputAnalysisOptions = table.Column<bool>(type: "INTEGER", nullable: false),
                    OutputPlotsInBW = table.Column<bool>(type: "INTEGER", nullable: false),
                    GeometryDisplay = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayModelCoefficients = table.Column<bool>(type: "INTEGER", nullable: false),
                    CovariateRegressionCoefficients = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssessCovariateInteractions = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayLSMeansLines = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplaySEMLines = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayPointLabels = table.Column<bool>(type: "INTEGER", nullable: false),
                    TitleSize = table.Column<int>(type: "INTEGER", nullable: false),
                    XAxisTitleFontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    YAxisTitleFontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    XLabelsFontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    YLabelsFontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    GraphicsXAngle = table.Column<int>(type: "INTEGER", nullable: false),
                    GraphicsXHorizontalJust = table.Column<double>(type: "REAL", nullable: false),
                    GraphicsYAngle = table.Column<int>(type: "INTEGER", nullable: false),
                    GraphicsYVerticalJust = table.Column<double>(type: "REAL", nullable: false),
                    PointSize = table.Column<int>(type: "INTEGER", nullable: false),
                    PointShape = table.Column<int>(type: "INTEGER", nullable: false),
                    LineSize = table.Column<int>(type: "INTEGER", nullable: false),
                    LegendTextSize = table.Column<int>(type: "INTEGER", nullable: false),
                    JpegWidth = table.Column<int>(type: "INTEGER", nullable: false),
                    JpegHeight = table.Column<int>(type: "INTEGER", nullable: false),
                    PlotResolution = table.Column<int>(type: "INTEGER", nullable: false),
                    GraphicsBWLow = table.Column<double>(type: "REAL", nullable: false),
                    GraphicsBWHigh = table.Column<double>(type: "REAL", nullable: false),
                    GraphicsWidthJitter = table.Column<double>(type: "REAL", nullable: false),
                    GraphicsHeightJitter = table.Column<double>(type: "REAL", nullable: false),
                    ErrorBarWidth = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOptions", x => x.UserOptionID);
                });

            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    AnalysisID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnalysisGuid = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    DatasetID = table.Column<int>(type: "INTEGER", nullable: true),
                    DatasetName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ScriptID = table.Column<int>(type: "INTEGER", nullable: false),
                    Tag = table.Column<string>(type: "TEXT", unicode: false, maxLength: 200, nullable: true),
                    RProcessOutput = table.Column<string>(type: "TEXT", nullable: true),
                    HtmlOutput = table.Column<string>(type: "TEXT", nullable: true),
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
                    ArgumentID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnalysisID = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
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
