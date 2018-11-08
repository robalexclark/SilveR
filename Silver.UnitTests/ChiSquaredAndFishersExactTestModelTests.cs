using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests
{
    public class ChiSquaredAndFishersExactTestModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            ChiSquaredAndFishersExactTestModel sut = new ChiSquaredAndFishersExactTestModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("ChiSquaredAndFishersExactTest", result);
        }


    }
}