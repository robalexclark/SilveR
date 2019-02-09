using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{

    public class ComparisonOfMeansPowerAnalysisUserBasedInputsModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            ComparisonOfMeansPowerAnalysisUserBasedInputsModel sut = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("ComparisonOfMeansPowerAnalysisUserBasedInputs", result);
        }

        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            ComparisonOfMeansPowerAnalysisUserBasedInputsModel sut = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();

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
            ComparisonOfMeansPowerAnalysisUserBasedInputsModel sut = GetModel();

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var absoluteChange = result.Single(x => x.Name == "AbsoluteChange");
            Assert.Equal("5", absoluteChange.Value);

            var deviationType = result.Single(x => x.Name == "DeviationType");
            Assert.Equal("Variance", deviationType.Value);

            var changeType = result.Single(x => x.Name == "ChangeType");
            Assert.Equal("Absolute", changeType.Value);

            var graphTitle = result.Single(x => x.Name == "GraphTitle");
            Assert.Equal("Test title", graphTitle.Value);

            var groupMean = result.Single(x => x.Name == "GroupMean");
            Assert.Equal("2", groupMean.Value);

            var percentChange = result.Single(x => x.Name == "PercentChange");
            Assert.Null(percentChange.Value);

            var plottingRangeType = result.Single(x => x.Name == "PlottingRangeType");
            Assert.Equal("SampleSize", plottingRangeType.Value);

            var powerFrom = result.Single(x => x.Name == "PowerFrom");
            Assert.Null(powerFrom.Value);

            var powerTo = result.Single(x => x.Name == "PowerTo");
            Assert.Null(powerTo.Value);

            var sampleSizeFrom = result.Single(x => x.Name == "SampleSizeFrom");
            Assert.Equal("6", sampleSizeFrom.Value);

            var sampleSizeTo = result.Single(x => x.Name == "SampleSizeTo");
            Assert.Equal("15", sampleSizeTo.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);

            var standardDeviation = result.Single(x => x.Name == "StandardDeviation");
            Assert.Null(standardDeviation.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            ComparisonOfMeansPowerAnalysisUserBasedInputsModel sut = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "AbsoluteChange", Value = "5" });
            arguments.Add(new Argument { Name = "DeviationType", Value = "Variance" });
            arguments.Add(new Argument { Name = "ChangeType", Value = "Absolute" });
            arguments.Add(new Argument { Name = "GraphTitle", Value = "Test title" });
            arguments.Add(new Argument { Name = "GroupMean", Value = "2" });
            arguments.Add(new Argument { Name = "PercentChange" });
            arguments.Add(new Argument { Name = "PlottingRangeType", Value = "SampleSize" });
            arguments.Add(new Argument { Name = "PowerFrom" });
            arguments.Add(new Argument { Name = "PowerTo"});
            arguments.Add(new Argument { Name = "SampleSizeFrom", Value = "6" });
            arguments.Add(new Argument { Name = "SampleSizeTo", Value = "15" });
            arguments.Add(new Argument { Name = "Significance", Value = "Log10" });
            arguments.Add(new Argument { Name = "StandardDeviation" });
            arguments.Add(new Argument { Name = "Variance", Value = "1" });

            Assert.Equal(14, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("5", sut.AbsoluteChange);
            Assert.Equal(DeviationType.Variance, sut.DeviationType);
            Assert.Equal(ChangeTypeOption.Absolute, sut.ChangeType);
            Assert.Equal("Test title", sut.GraphTitle);
            Assert.Equal(2, sut.GroupMean);
            Assert.Null(sut.PercentChange);
            Assert.Equal(PlottingRangeTypeOption.SampleSize, sut.PlottingRangeType);
            Assert.Null(sut.PowerFrom);
            Assert.Null(sut.PowerTo);
            Assert.Equal(6, sut.SampleSizeFrom);
            Assert.Equal(15, sut.SampleSizeTo);
            Assert.Equal("Log10", sut.Significance);
            Assert.Null(sut.StandardDeviation);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            ComparisonOfMeansPowerAnalysisUserBasedInputsModel sut = GetModel();

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("UserValues 2 Variance 1 0.05 Absolute 5 SampleSize 6 15 \"Test title\"", result);
        }


        private ComparisonOfMeansPowerAnalysisUserBasedInputsModel GetModel()
        {
            var model = new SilveR.StatsModels.ComparisonOfMeansPowerAnalysisUserBasedInputsModel
            {
                AbsoluteChange = "5",
                DeviationType = DeviationType.Variance,
                ChangeType = ChangeTypeOption.Absolute,
                GraphTitle = "Test title",
                GroupMean = 2m,
                PercentChange = null,
                PlottingRangeType = PlottingRangeTypeOption.SampleSize,
                PowerFrom = null,
                PowerTo = null,
                SampleSizeFrom = 6,
                SampleSizeTo = 15,
                Significance = "0.05",
                StandardDeviation = null,
                Variance = 1m
            };

            return model;
        }
    }
}