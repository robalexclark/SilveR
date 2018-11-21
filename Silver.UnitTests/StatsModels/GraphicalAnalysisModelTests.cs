using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
    [ExcludeFromCodeCoverageAttribute]
    public class GraphicalAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            GraphicalAnalysisModel sut = new GraphicalAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("GraphicalAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            GraphicalAnalysisModel sut = new GraphicalAnalysisModel();

            //Act
            IEnumerable<string> result = sut.TransformationsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine" }, result);
        }
    }
}