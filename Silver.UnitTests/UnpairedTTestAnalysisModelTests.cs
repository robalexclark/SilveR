using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests
{
    public class UnpairedTTestAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            UnpairedTTestAnalysisModel sut = new UnpairedTTestAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("UnpairedTTestAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            UnpairedTTestAnalysisModel sut = new UnpairedTTestAnalysisModel();

            //Act
            IEnumerable<string> result = sut.TransformationsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }, result);
        }
    }
}