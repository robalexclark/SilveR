using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
    [ExcludeFromCodeCoverageAttribute]
    public class MeansComparisonModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            MeansComparisonModel sut = new MeansComparisonModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("MeansComparison", result);
        }

    }
}