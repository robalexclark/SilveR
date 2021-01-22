using SilveR.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace SilveR.UnitTests.Models
{
    public class UserOptionTests
    {
        [Fact]
        public void GetOptionLines()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            UserOption userOption = new UserOption();

            //Act
            List<string> optionLines = userOption.GetOptionLines().ToList();

            //Assert (test default user options)
            Assert.Equal("LineTypeSolid Solid", optionLines[0]);
            Assert.Equal("LineTypeDashed Dashed", optionLines[1]);
            Assert.Equal("GraphicsFont Helvetica", optionLines[2]);
            Assert.Equal("FontStyle Plain", optionLines[3]);
            Assert.Equal("GraphicsTextColour Black", optionLines[4]);
            Assert.Equal("ColourFill Blue", optionLines[5]);
            Assert.Equal("BWFill Gray", optionLines[6]);
            Assert.Equal("CategoryBarFill Ivory", optionLines[7]);
            Assert.Equal("ColourLine Red", optionLines[8]);
            Assert.Equal("BWLine Black", optionLines[9]);
            Assert.Equal("LegendTextColour White", optionLines[10]);
            Assert.Equal("LegendPosition Default", optionLines[11]);
            Assert.Equal("PaletteSet Set1", optionLines[12]);
            Assert.Equal("OutputData N", optionLines[13]);
            Assert.Equal("OutputAnalysisOptions N", optionLines[14]);
            Assert.Equal("OutputPlotsInBW N", optionLines[15]);
            Assert.Equal("GeometryDisplay N", optionLines[16]);
            Assert.Equal("DisplayModelCoefficients N", optionLines[17]);
            Assert.Equal("CovariateRegressionCoefficients N", optionLines[18]);
            Assert.Equal("AssessCovariateInteractions N", optionLines[19]);
            Assert.Equal("DisplayLSMeansLines N", optionLines[20]);
            Assert.Equal("DisplaySEMLines N", optionLines[21]);
            Assert.Equal("DisplayPointLabels N", optionLines[22]);

            Assert.Equal("TitleSize 20", optionLines[23]);
            Assert.Equal("XAxisTitleFontSize 15", optionLines[24]);
            Assert.Equal("YAxisTitleFontSize 15", optionLines[25]);
            Assert.Equal("XLabelsFontSize 15", optionLines[26]);
            Assert.Equal("YLabelsFontSize 15", optionLines[27]);
            Assert.Equal("GraphicsXAngle 0", optionLines[28]);
            Assert.Equal("GraphicsXHorizontalJust 0.5", optionLines[29]);
            Assert.Equal("GraphicsYAngle 0", optionLines[30]);
            Assert.Equal("GraphicsYVerticalJust 0.5", optionLines[31]);
            Assert.Equal("PointSize 4", optionLines[32]);
            Assert.Equal("PointShape 21", optionLines[33]);
            Assert.Equal("LineSize 1", optionLines[34]);
            Assert.Equal("LegendTextSize 15", optionLines[35]);
            Assert.Equal("JpegWidth 6", optionLines[36]);
            Assert.Equal("JpegHeight 6", optionLines[37]);
            Assert.Equal("PlotResolution 300", optionLines[38]);
            Assert.Equal("GraphicsBWLow 0.1", optionLines[39]);
            Assert.Equal("GraphicsBWHigh 0.8", optionLines[40]);
            Assert.Equal("GraphicsWidthJitter 0.1", optionLines[41]);
            Assert.Equal("GraphicsHeightJitter 0.1", optionLines[42]);
            Assert.Equal("ErrorBarWidth 0.7", optionLines[43]);
            Assert.Equal("FillTransparency 1", optionLines[44]);
        }
    }
}