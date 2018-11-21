using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
    public class NonParametricAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            NonParametricAnalysisModel sut = new NonParametricAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("NonParametricAnalysis", result);
        }

    }
}