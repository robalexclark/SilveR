using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
    [ExcludeFromCodeCoverageAttribute]
    public class PValueAdjustmentModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            PValueAdjustmentModel sut = new PValueAdjustmentModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("PValueAdjustment", result);
        }

    }
}