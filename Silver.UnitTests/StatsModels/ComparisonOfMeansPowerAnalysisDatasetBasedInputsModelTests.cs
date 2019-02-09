using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{

    public class ComparisonOfMeansPowerAnalysisDatasetBasedInputsModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel sut = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", result);
        }



        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel sut = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();

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
            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel sut = GetModel();

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var absoluteChange = result.Single(x => x.Name == "AbsoluteChange");
            Assert.Null(absoluteChange.Value);

            var changeType = result.Single(x => x.Name == "ChangeType");
            Assert.Equal("Percent", changeType.Value);

            var controlGroup = result.Single(x => x.Name == "ControlGroup");
            Assert.Equal("x", controlGroup.Value);

            var graphTitle = result.Single(x => x.Name == "GraphTitle");
            Assert.Null(graphTitle.Value);

            var percentChange = result.Single(x => x.Name == "PercentChange");
            Assert.Equal("5", percentChange.Value);

            var plottingRangeType = result.Single(x => x.Name == "PlottingRangeType");
            Assert.Equal("Power", plottingRangeType.Value);

            var powerFrom = result.Single(x => x.Name == "PowerFrom");
            Assert.Equal("70", powerFrom.Value);

            var powerTo = result.Single(x => x.Name == "PowerTo");
            Assert.Equal("90", powerTo.Value);

            var response = result.Single(x => x.Name == "Response");
            Assert.Equal("Resp1", response.Value);

            var sampleSizeFrom = result.Single(x => x.Name == "SampleSizeFrom");
            Assert.Null(sampleSizeFrom.Value);

            var sampleSizeTo = result.Single(x => x.Name == "SampleSizeTo");
            Assert.Null(sampleSizeTo.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);

            var treatment = result.Single(x => x.Name == "Treatment");
            Assert.Equal("Treat2", treatment.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel sut = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "AbsoluteChange" });
            arguments.Add(new Argument { Name = "ChangeType", Value = "Percent" });
            arguments.Add(new Argument { Name = "ControlGroup", Value = "x" });
            arguments.Add(new Argument { Name = "GraphTitle" });
            arguments.Add(new Argument { Name = "PercentChange", Value = "5" });
            arguments.Add(new Argument { Name = "PlottingRangeType", Value = "Power" });
            arguments.Add(new Argument { Name = "PowerFrom", Value = "70" });
            arguments.Add(new Argument { Name = "PowerTo", Value = "90" });
            arguments.Add(new Argument { Name = "Response", Value = "Resp1" });
            arguments.Add(new Argument { Name = "SampleSizeFrom" });
            arguments.Add(new Argument { Name = "SampleSizeTo" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.05" });
            arguments.Add(new Argument { Name = "Treatment", Value = "Treat2" });

            Assert.Equal(13, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Null(sut.AbsoluteChange);
            Assert.Equal(ChangeTypeOption.Percent, sut.ChangeType);
            Assert.Equal("x", sut.ControlGroup);
            Assert.Null(sut.GraphTitle);
            Assert.Equal("5", sut.PercentChange);
            Assert.Equal(PlottingRangeTypeOption.Power, sut.PlottingRangeType);
            Assert.Equal(70, sut.PowerFrom);
            Assert.Equal(90, sut.PowerTo);
            Assert.Equal("Resp1", sut.Response);
            Assert.Null( sut.SampleSizeFrom);
            Assert.Null( sut.SampleSizeTo);
            Assert.Equal("0.05", sut.Significance);
            Assert.Equal("Treat2", sut.Treatment);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel sut = GetModel();

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("DatasetValues Resp1 Treat2 x 0.05 Percent 5 PowerAxis 70 90 NULL", result);
        }


        private ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel GetModel()
        {
            var model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel
            {
                AbsoluteChange = null,
                ChangeType = ChangeTypeOption.Percent,
                ControlGroup = "x",
                GraphTitle = null,
                PercentChange = "5",
                PlottingRangeType = PlottingRangeTypeOption.Power,
                PowerFrom = 70,
                PowerTo = 90,
                Response = "Resp1",
                SampleSizeFrom = null,
                SampleSizeTo = null,
                Significance = "0.05",
                Treatment = "Treat2"
            };

            return model;
        }
    }
}