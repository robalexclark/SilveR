using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
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


        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            PValueAdjustmentModel sut = new PValueAdjustmentModel();

            //Act
            IEnumerable<string> result = sut.SignificancesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "0.1", "0.05", "0.01", "0.001" }, result);
        }

        [Fact]
        public void MultipleComparisonTestsList_ReturnsCorrectList()
        {
            //Arrange
            PValueAdjustmentModel sut = new PValueAdjustmentModel();

            //Act
            IEnumerable<string> result = sut.MultipleComparisonTests;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "Holm", "Hochberg", "Hommel", "Benjamini-Hochberg", "Bonferroni" }, result);
        }


        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            PValueAdjustmentModel sut = GetModel();

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var selectedTest = result.Single(x => x.Name == "SelectedTest");
            Assert.Equal("Hochberg", selectedTest.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.01", significance.Value);

            var pValues = result.Single(x => x.Name == "PValues");
            Assert.Equal("0.01,0.01,0.5", pValues.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            PValueAdjustmentModel sut = new PValueAdjustmentModel();

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "SelectedTest", Value = "Hochberg" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.01" });
            arguments.Add(new Argument { Name = "PValues", Value = "0.01,0.01,0.5" });

            Assert.Equal(3, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("Hochberg", sut.SelectedTest);
            Assert.Equal("0.01", sut.Significance);
            Assert.Equal("0.01,0.01,0.5", sut.PValues);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            PValueAdjustmentModel sut = GetModel();

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("0.01,0.01,0.5 Hochberg 0.01", result);
        }


        private PValueAdjustmentModel GetModel()
        {
            var model = new PValueAdjustmentModel
            {
                SelectedTest = "Hochberg",
                Significance = "0.01",
                PValues = "0.01,0.01,0.5"
            };

            return model;
        }
    }
}