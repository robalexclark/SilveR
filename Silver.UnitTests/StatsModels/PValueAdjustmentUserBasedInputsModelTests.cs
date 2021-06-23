using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace SilveR.UnitTests.StatsModels
{
    public class PValueAdjustmentUserBasedInputsModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PValueAdjustmentUserBasedInputsModel sut = new PValueAdjustmentUserBasedInputsModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("PValueAdjustmentUserBasedInputs", result);
        }


        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PValueAdjustmentUserBasedInputsModel sut = new PValueAdjustmentUserBasedInputsModel();

            //Act
            IEnumerable<string> result = sut.SignificancesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "0.1", "0.05", "0.025", "0.01", "0.001" }, result);
        }

        [Fact]
        public void MultipleComparisonTestsList_ReturnsCorrectList()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PValueAdjustmentUserBasedInputsModel sut = new PValueAdjustmentUserBasedInputsModel();

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
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PValueAdjustmentUserBasedInputsModel sut = GetModel();

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
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PValueAdjustmentUserBasedInputsModel sut = new PValueAdjustmentUserBasedInputsModel();

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
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PValueAdjustmentUserBasedInputsModel sut = GetModel();

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("0.01,0.01,0.5 Hochberg 0.01", result);
        }


        private PValueAdjustmentUserBasedInputsModel GetModel()
        {
            var model = new PValueAdjustmentUserBasedInputsModel
            {
                SelectedTest = "Hochberg",
                Significance = "0.01",
                PValues = "0.01,0.01,0.5"
            };

            return model;
        }
    }
}