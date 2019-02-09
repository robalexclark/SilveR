using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SilveR.Models
{
    public partial class UserOption
    {
        [Key]
        public int UserOptionID { get; set; }

        [Required]
        public string LineTypeSolid { get; set; } = "Solid";

        [Required]
        public string LineTypeDashed { get; set; } = "Dashed";

        [Required]
        public string GraphicsFont { get; set; } = "Helvetica";

        [Required]
        public string FontStyle { get; set; } = "Plain";

        [Required]
        public string GraphicsTextColour { get; set; } = "Black";

        [Required]
        public string ColourFill { get; set; } = "RoyalBlue1";

        [Required]
        public string BWFill { get; set; } = "Grey";

        [Required]
        public string CategoryBarFill { get; set; } = "Ivory2";

        [Required]
        public string ColourLine { get; set; } = "Red";

        [Required]
        public string BWLine { get; set; } = "Black";

        [Required]
        public string LegendTextColour { get; set; } = "White";

        [Required]
        public string LegendPosition { get; set; } = "Default";

        [Required]
        public string PaletteSet { get; set; } = "Set1";

        [Required]
        public string OutputData { get; set; } = "N";

        [Required]
        public string OutputPlotsAsPdf { get; set; } = "N";

        [Required]
        public string OutputPlotsInBW { get; set; } = "N";

        [Required]
        public string GeometryDisplay { get; set; } = "N";

        [Required]
        public string CovariateRegressionCoefficients { get; set; } = "N";

        [Required]
        public string AssessCovariateInteractions { get; set; } = "N";

        [Required]
        public string ScatterLabels { get; set; } = "N";

        [Required]
        public string DisplayLSMeanslines { get; set; } = "N";

        [Required]
        public string DisplaySEMlines { get; set; } = "N";



        public int TitleSize { get; set; } = 20;
        public int XAxisTitleFontSize { get; set; } = 15;
        public int YAxisTitleFontSize { get; set; } = 15;
        public int XLabelsFontSize { get; set; } = 15;

        public int YLabelsFontSize { get; set; } = 15;
        public int GraphicsXAngle { get; set; } = 0;
        public decimal GraphicsXHorizontalJust { get; set; } = 0.5m;
        public int GraphicsYAngle { get; set; } = 0;
        public decimal GraphicsYVerticalJust { get; set; } = 0.5m;
        public int PointSize { get; set; } = 4;
        public int PointShape { get; set; } = 21;
        public int LineSize { get; set; } = 1;
        public int LegendTextSize { get; set; } = 15;

        public int PdfWidth { get; set; } = 11;
        public int PdfHeight { get; set; } = 8;
        public int JpegWidth { get; set; } = 480;
        public int JpegHeight { get; set; } = 480;
        public decimal GraphicsBWLow { get; set; } = 0.1m;
        public decimal GraphicsBWHigh { get; set; } = 0.8m;
        public decimal GraphicsWidthJitter { get; set; } = 0.1m;
        public decimal GraphicsHeightJitter { get; set; } = 0.1m;
        public decimal ErrorBarWidth { get; set; } = 0.7m;


        public IEnumerable<string> GetOptionLines()
        {
            List<string> optionLines = new List<string>();
            optionLines.Add(nameof(this.LineTypeSolid) + " " + this.LineTypeSolid.ToLower());
            optionLines.Add(nameof(this.LineTypeDashed) + " " + this.LineTypeDashed.ToLower());
            optionLines.Add(nameof(this.GraphicsFont) + " " + this.GraphicsFont);
            optionLines.Add(nameof(this.FontStyle) + " " + this.FontStyle.ToLower());
            optionLines.Add(nameof(this.GraphicsTextColour) + " " + this.GraphicsTextColour);
            optionLines.Add(nameof(this.ColourFill) + " " + this.ColourFill.ToLower());
            optionLines.Add(nameof(this.BWFill) + " " + this.BWFill.ToLower());
            optionLines.Add(nameof(this.CategoryBarFill) + " " + this.CategoryBarFill.ToLower());
            optionLines.Add(nameof(this.ColourLine) + " " + this.ColourLine.ToLower());
            optionLines.Add(nameof(this.BWLine) + " " + this.BWLine.ToLower());
            optionLines.Add(nameof(this.LegendTextColour) + " " + this.LegendTextColour.ToLower());
            optionLines.Add(nameof(this.LegendPosition) + " " + this.LegendPosition);
            optionLines.Add(nameof(this.PaletteSet) + " " + this.PaletteSet);
            optionLines.Add(nameof(this.OutputData) + " " + this.OutputData);
            optionLines.Add(nameof(this.OutputPlotsAsPdf) + " " + this.OutputPlotsAsPdf);
            optionLines.Add(nameof(this.OutputPlotsInBW) + " " + this.OutputPlotsInBW);
            optionLines.Add(nameof(this.GeometryDisplay) + " " + this.GeometryDisplay);
            optionLines.Add(nameof(this.CovariateRegressionCoefficients) + " " + this.CovariateRegressionCoefficients);
            optionLines.Add(nameof(this.AssessCovariateInteractions) + " " + this.AssessCovariateInteractions);
            optionLines.Add(nameof(this.ScatterLabels) + " " + this.ScatterLabels);
            optionLines.Add(nameof(this.DisplayLSMeanslines) + " " + this.DisplayLSMeanslines);
            optionLines.Add(nameof(this.DisplaySEMlines) + " " + this.DisplaySEMlines);

            optionLines.Add(nameof(this.TitleSize) + " " + this.TitleSize);
            optionLines.Add(nameof(this.XAxisTitleFontSize) + " " + this.XAxisTitleFontSize);
            optionLines.Add(nameof(this.YAxisTitleFontSize) + " " + this.YAxisTitleFontSize);
            optionLines.Add(nameof(this.XLabelsFontSize) + " " + this.XLabelsFontSize);
            optionLines.Add(nameof(this.YLabelsFontSize) + " " + this.YLabelsFontSize);
            optionLines.Add(nameof(this.GraphicsXAngle) + " " + this.GraphicsXAngle);
            optionLines.Add(nameof(this.GraphicsXHorizontalJust) + " " + this.GraphicsXHorizontalJust);
            optionLines.Add(nameof(this.GraphicsYAngle) + " " + this.GraphicsYAngle);
            optionLines.Add(nameof(this.GraphicsYVerticalJust) + " " + this.GraphicsYVerticalJust);
            optionLines.Add(nameof(this.PointSize) + " " + this.PointSize);
            optionLines.Add(nameof(this.PointShape) + " " + this.PointShape);
            optionLines.Add(nameof(this.LineSize) + " " + this.LineSize);
            optionLines.Add(nameof(this.LegendTextSize) + " " + this.LegendTextSize);
            optionLines.Add(nameof(this.PdfWidth) + " " + this.PdfWidth);
            optionLines.Add(nameof(this.PdfHeight) + " " + this.PdfHeight);
            optionLines.Add(nameof(this.JpegWidth) + " " + this.JpegWidth);
            optionLines.Add(nameof(this.JpegHeight) + " " + this.JpegHeight);
            optionLines.Add(nameof(this.GraphicsBWLow) + " " + this.GraphicsBWLow);
            optionLines.Add(nameof(this.GraphicsBWHigh) + " " + this.GraphicsBWHigh);
            optionLines.Add(nameof(this.GraphicsWidthJitter) + " " + this.GraphicsWidthJitter);
            optionLines.Add(nameof(this.GraphicsHeightJitter) + " " + this.GraphicsHeightJitter);
            optionLines.Add(nameof(this.ErrorBarWidth) + " " + this.ErrorBarWidth);

            return optionLines;
        }
    }
}