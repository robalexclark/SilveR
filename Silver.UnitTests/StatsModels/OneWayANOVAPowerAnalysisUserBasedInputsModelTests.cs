using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
    public class OneWayANOVAPowerAnalysisUserBasedInputsModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            OneWayANOVAPowerAnalysisUserBasedInputsModel sut = new OneWayANOVAPowerAnalysisUserBasedInputsModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("OneWayANOVAPowerAnalysisUserBasedInputs", result);
        }

        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            OneWayANOVAPowerAnalysisUserBasedInputsModel sut = new OneWayANOVAPowerAnalysisUserBasedInputsModel();

            //Act
            IEnumerable<string> result = sut.SignificancesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "0.1", "0.05", "0.01", "0.001" }, result);
        }


        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            OneWayANOVAPowerAnalysisUserBasedInputsModel sut = GetModel();

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var graphTitle = result.Single(x => x.Name == "GraphTitle");
            Assert.Equal("TitleTest", graphTitle.Value);

            var means = result.Single(x => x.Name == "Means");
            Assert.Equal("55", means.Value);

            var variabilityEstimate = result.Single(x => x.Name == "VariabilityEstimate");
            Assert.Equal("Variance", variabilityEstimate.Value);

            var variance = result.Single(x => x.Name == "Variance");
            Assert.Equal("42", variance.Value);

            var standardDeviation = result.Single(x => x.Name == "StandardDeviation");
            Assert.Null(standardDeviation.Value);

            var plottingRangeType = result.Single(x => x.Name == "PlottingRangeType");
            Assert.Equal("Power", plottingRangeType.Value);

            var powerFrom = result.Single(x => x.Name == "PowerFrom");
            Assert.Equal("70", powerFrom.Value);

            var powerTo = result.Single(x => x.Name == "PowerTo");
            Assert.Equal("90", powerTo.Value);

            var sampleSizeFrom = result.Single(x => x.Name == "SampleSizeFrom");
            Assert.Null(sampleSizeFrom.Value);

            var sampleSizeTo = result.Single(x => x.Name == "SampleSizeTo");
            Assert.Null(sampleSizeTo.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            OneWayANOVAPowerAnalysisUserBasedInputsModel sut = new OneWayANOVAPowerAnalysisUserBasedInputsModel();

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "GraphTitle", Value = "TitleTest" });
            arguments.Add(new Argument { Name = "EffectSizeEstimate", Value = "TreatmentMeanSquare" });
            arguments.Add(new Argument { Name = "TreatmentMeanSquare", Value = "12.5" });
            arguments.Add(new Argument { Name = "Means", Value = "55" });
            arguments.Add(new Argument { Name = "VariabilityEstimate", Value = "Variance" });
            arguments.Add(new Argument { Name = "ResidualMeanSquare" });
            arguments.Add(new Argument { Name = "Variance", Value = "42" });
            arguments.Add(new Argument { Name = "StandardDeviation" });
            arguments.Add(new Argument { Name = "NoOfTreatmentGroups", Value = "5" });
            arguments.Add(new Argument { Name = "PlottingRangeType", Value = "Power" });
            arguments.Add(new Argument { Name = "PowerFrom", Value = "70" });
            arguments.Add(new Argument { Name = "PowerTo", Value = "90" });
            arguments.Add(new Argument { Name = "SampleSizeFrom" });
            arguments.Add(new Argument { Name = "SampleSizeTo" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.05" });

            Assert.Equal(15, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("55", sut.Means);
            Assert.Equal("Variance", sut.VariabilityEstimate.ToString());
            Assert.Equal("42", sut.Variance.ToString());
            Assert.Null(sut.StandardDeviation);
            Assert.Equal("55", sut.Means);
            Assert.Equal("TitleTest", sut.GraphTitle);
            Assert.Equal(PlottingRangeTypeOption.Power, sut.PlottingRangeType);
            Assert.Equal(70, sut.PowerFrom);
            Assert.Equal(90, sut.PowerTo);
            Assert.Null(sut.SampleSizeFrom);
            Assert.Null(sut.SampleSizeTo);
            Assert.Equal("0.05", sut.Significance);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            OneWayANOVAPowerAnalysisUserBasedInputsModel sut = GetModel();

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("UserValues 55 Variance 42 NULL 0.05 PowerAxis 70 90 TitleTest", result);
        }


        private OneWayANOVAPowerAnalysisUserBasedInputsModel GetModel()
        {
            var model = new OneWayANOVAPowerAnalysisUserBasedInputsModel
            {
                Means = "55",
                VariabilityEstimate = VariabilityEstimate.Variance,
                Variance = 42,
                StandardDeviation = null,
                GraphTitle = "TitleTest",
                PlottingRangeType = PlottingRangeTypeOption.Power,
                PowerFrom = 70,
                PowerTo = 90,
                SampleSizeFrom = null,
                SampleSizeTo = null,
                Significance = "0.05"
            };

            return model;
        }
    }
}