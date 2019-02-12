using SilveR.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.Models
{
    public class UserOptionTests
    {
        [Fact]
        public void GetOptionLines()
        {
            //Arrange
            UserOption userOption = new UserOption();

            //Act
            List<string> optionLines = userOption.GetOptionLines().ToList();

            //Assert (test default user options)
            Assert.Equal("LineTypeSolid solid", optionLines[0]);
            Assert.Equal("LineTypeDashed dashed", optionLines[1]);
            Assert.Equal("GraphicsFont Helvetica", optionLines[2]);
            Assert.Equal("FontStyle plain", optionLines[3]);
            Assert.Equal("GraphicsTextColour Black", optionLines[4]);
            Assert.Equal("ColourFill royalblue1", optionLines[5]);
            Assert.Equal("BWFill grey", optionLines[6]);
            Assert.Equal("CategoryBarFill ivory2", optionLines[7]);
            Assert.Equal("ColourLine red", optionLines[8]);
            Assert.Equal("BWLine black", optionLines[9]);
            Assert.Equal("LegendTextColour white", optionLines[10]);
            Assert.Equal("LegendPosition Default", optionLines[11]);
            Assert.Equal("PaletteSet Set1", optionLines[12]);
            Assert.Equal("OutputData N", optionLines[13]);
            Assert.Equal("OutputPlotsInBW N", optionLines[14]);
            Assert.Equal("GeometryDisplay N", optionLines[15]);
            Assert.Equal("CovariateRegressionCoefficients N", optionLines[16]);
            Assert.Equal("AssessCovariateInteractions N", optionLines[17]);
            Assert.Equal("ScatterLabels N", optionLines[18]);
            Assert.Equal("DisplayLSMeansLines N", optionLines[19]);
            Assert.Equal("DisplaySEMLines N", optionLines[20]);
            Assert.Equal("DisplayPointLabels N", optionLines[21]);

            Assert.Equal("TitleSize 20", optionLines[22]);
            Assert.Equal("XAxisTitleFontSize 15", optionLines[23]);
            Assert.Equal("YAxisTitleFontSize 15", optionLines[24]);
            Assert.Equal("XLabelsFontSize 15", optionLines[25]);
            Assert.Equal("YLabelsFontSize 15", optionLines[26]);
            Assert.Equal("GraphicsXAngle 0", optionLines[27]);
            Assert.Equal("GraphicsXHorizontalJust 0.5", optionLines[28]);
            Assert.Equal("GraphicsYAngle 0", optionLines[29]);
            Assert.Equal("GraphicsYVerticalJust 0.5", optionLines[30]);
            Assert.Equal("PointSize 4", optionLines[31]);
            Assert.Equal("PointShape 21", optionLines[32]);
            Assert.Equal("LineSize 1", optionLines[33]);
            Assert.Equal("LegendTextSize 15", optionLines[34]);
            Assert.Equal("JpegWidth 480", optionLines[35]);
            Assert.Equal("JpegHeight 480", optionLines[36]);
            Assert.Equal("GraphicsBWLow 0.1", optionLines[37]);
            Assert.Equal("GraphicsBWHigh 0.8", optionLines[38]);
            Assert.Equal("GraphicsWidthJitter 0.1", optionLines[39]);
            Assert.Equal("GraphicsHeightJitter 0.1", optionLines[40]);
            Assert.Equal("ErrorBarWidth 0.7", optionLines[41]);
        }
    }
}