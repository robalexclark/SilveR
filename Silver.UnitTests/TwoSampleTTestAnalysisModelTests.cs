using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests
{
    public class TwoSampleTTestAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            TwoSampleTTestAnalysisModel sut = new TwoSampleTTestAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("TwoSampleTTestAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            TwoSampleTTestAnalysisModel sut = new TwoSampleTTestAnalysisModel();

            //Act
            IEnumerable<string> result = sut.TransformationsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }, result);
        }

    }
}