using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

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
        public string ColourFill { get; set; } = "Blue";

        [Required]
        public string BWFill { get; set; } = "Gray";

        [Required]
        public double FillTransparency { get; set; } = 1;

        [Required]
        public string CategoryBarFill { get; set; } = "Ivory";

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

        public bool OutputData { get; set; } = false;

        public bool OutputAnalysisOptions { get; set; } = false;

        public bool OutputPlotsInBW { get; set; } = false;

        public bool GeometryDisplay { get; set; } = false;

        public bool DisplayModelCoefficients { get; set; } = false;

        public bool CovariateRegressionCoefficients { get; set; } = false;

        public bool AssessCovariateInteractions { get; set; } = false;

        public bool DisplayLSMeansLines { get; set; } = false;

        public bool DisplaySEMLines { get; set; } = false;

        public bool DisplayPointLabels { get; set; } = false;



        public int TitleSize { get; set; } = 20;
        public int XAxisTitleFontSize { get; set; } = 15;
        public int YAxisTitleFontSize { get; set; } = 15;
        public int XLabelsFontSize { get; set; } = 15;

        public int YLabelsFontSize { get; set; } = 15;
        public int GraphicsXAngle { get; set; } = 0;
        public double GraphicsXHorizontalJust { get; set; } = 0.5;
        public int GraphicsYAngle { get; set; } = 0;
        public double GraphicsYVerticalJust { get; set; } = 0.5;
        public int PointSize { get; set; } = 4;
        public int PointShape { get; set; } = 21;
        public int LineSize { get; set; } = 1;
        public int LegendTextSize { get; set; } = 15;

        public int JpegWidth { get; set; } = 6;
        public int JpegHeight { get; set; } = 6;
        public int PlotResolution { get; set; } = 300;
        public double GraphicsBWLow { get; set; } = 0.1;
        public double GraphicsBWHigh { get; set; } = 0.8;
        public double GraphicsWidthJitter { get; set; } = 0.1;
        public double GraphicsHeightJitter { get; set; } = 0.1;
        public double ErrorBarWidth { get; set; } = 0.7;


        public IEnumerable<string> GetOptionLines()
        {
            if(System.Threading.Thread.CurrentThread.CurrentCulture != CultureInfo.InvariantCulture)
            {
                throw new InvalidOperationException("This method can only be run when the current thread is InvariantCulture");
            }

            List<string> optionLines = new List<string>();
            optionLines.Add(nameof(this.LineTypeSolid) + " " + this.LineTypeSolid);
            optionLines.Add(nameof(this.LineTypeDashed) + " " + this.LineTypeDashed);
            optionLines.Add(nameof(this.GraphicsFont) + " " + this.GraphicsFont);
            optionLines.Add(nameof(this.FontStyle) + " " + this.FontStyle);
            optionLines.Add(nameof(this.GraphicsTextColour) + " " + this.GraphicsTextColour);
            optionLines.Add(nameof(this.ColourFill) + " " + this.ColourFill);
            optionLines.Add(nameof(this.BWFill) + " " + this.BWFill);
            optionLines.Add(nameof(this.CategoryBarFill) + " " + this.CategoryBarFill);
            optionLines.Add(nameof(this.ColourLine) + " " + this.ColourLine);
            optionLines.Add(nameof(this.BWLine) + " " + this.BWLine);
            optionLines.Add(nameof(this.LegendTextColour) + " " + this.LegendTextColour);
            optionLines.Add(nameof(this.LegendPosition) + " " + this.LegendPosition);
            optionLines.Add(nameof(this.PaletteSet) + " " + this.PaletteSet);
            optionLines.Add(nameof(this.OutputData) + " " + (this.OutputData ? "Y" : "N"));
            optionLines.Add(nameof(this.OutputAnalysisOptions) + " " + (this.OutputAnalysisOptions ? "Y" : "N"));
            optionLines.Add(nameof(this.OutputPlotsInBW) + " " + (this.OutputPlotsInBW ? "Y" : "N"));
            optionLines.Add(nameof(this.GeometryDisplay) + " " + (this.GeometryDisplay ? "Y" : "N"));
            optionLines.Add(nameof(this.DisplayModelCoefficients) + " " + (this.DisplayModelCoefficients ? "Y" : "N"));
            optionLines.Add(nameof(this.CovariateRegressionCoefficients) + " " + (this.CovariateRegressionCoefficients ? "Y" : "N"));
            optionLines.Add(nameof(this.AssessCovariateInteractions) + " " + (this.AssessCovariateInteractions ? "Y" : "N"));
            optionLines.Add(nameof(this.DisplayLSMeansLines) + " " + (this.DisplayLSMeansLines ? "Y" : "N"));
            optionLines.Add(nameof(this.DisplaySEMLines) + " " + (this.DisplaySEMLines ? "Y" : "N"));
            optionLines.Add(nameof(this.DisplayPointLabels) + " " + (this.DisplayPointLabels ? "Y" : "N"));

            optionLines.Add(nameof(this.TitleSize) + " " + this.TitleSize.ToString());
            optionLines.Add(nameof(this.XAxisTitleFontSize) + " " + this.XAxisTitleFontSize.ToString());
            optionLines.Add(nameof(this.YAxisTitleFontSize) + " " + this.YAxisTitleFontSize.ToString());
            optionLines.Add(nameof(this.XLabelsFontSize) + " " + this.XLabelsFontSize.ToString());
            optionLines.Add(nameof(this.YLabelsFontSize) + " " + this.YLabelsFontSize.ToString());
            optionLines.Add(nameof(this.GraphicsXAngle) + " " + this.GraphicsXAngle.ToString());
            optionLines.Add(nameof(this.GraphicsXHorizontalJust) + " " + this.GraphicsXHorizontalJust.ToString());
            optionLines.Add(nameof(this.GraphicsYAngle) + " " + this.GraphicsYAngle.ToString());
            optionLines.Add(nameof(this.GraphicsYVerticalJust) + " " + this.GraphicsYVerticalJust.ToString());
            optionLines.Add(nameof(this.PointSize) + " " + this.PointSize.ToString());
            optionLines.Add(nameof(this.PointShape) + " " + this.PointShape.ToString());
            optionLines.Add(nameof(this.LineSize) + " " + this.LineSize.ToString());
            optionLines.Add(nameof(this.LegendTextSize) + " " + this.LegendTextSize.ToString());
            optionLines.Add(nameof(this.JpegWidth) + " " + this.JpegWidth.ToString());
            optionLines.Add(nameof(this.JpegHeight) + " " + this.JpegHeight.ToString());
            optionLines.Add(nameof(this.PlotResolution) + " " + this.PlotResolution.ToString());
            optionLines.Add(nameof(this.GraphicsBWLow) + " " + this.GraphicsBWLow.ToString());
            optionLines.Add(nameof(this.GraphicsBWHigh) + " " + this.GraphicsBWHigh.ToString());
            optionLines.Add(nameof(this.GraphicsWidthJitter) + " " + this.GraphicsWidthJitter.ToString());
            optionLines.Add(nameof(this.GraphicsHeightJitter) + " " + this.GraphicsHeightJitter.ToString());
            optionLines.Add(nameof(this.ErrorBarWidth) + " " + this.ErrorBarWidth.ToString());
            optionLines.Add(nameof(this.FillTransparency) + " " + this.FillTransparency.ToString());

            return optionLines;
        }
    }
}