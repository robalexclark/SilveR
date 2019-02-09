using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
    public class OneWayANOVAPowerAnalysisDatasetBasedInputsModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            OneWayANOVAPowerAnalysisDatasetBasedInputsModel sut = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("OneWayANOVAPowerAnalysisDatasetBasedInputs", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            OneWayANOVAPowerAnalysisDatasetBasedInputsModel sut = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();

            //Act
            IEnumerable<string> result = sut.TransformationsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }, result);
        }

        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            OneWayANOVAPowerAnalysisDatasetBasedInputsModel sut = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();

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
            OneWayANOVAPowerAnalysisDatasetBasedInputsModel sut = GetModel();

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var graphTitle = result.Single(x => x.Name == "GraphTitle");
            Assert.Null(graphTitle.Value);

            var plottingRangeType = result.Single(x => x.Name == "PlottingRangeType");
            Assert.Equal("SampleSize", plottingRangeType.Value);

            var powerFrom = result.Single(x => x.Name == "PowerFrom");
            Assert.Null(powerFrom.Value);

            var powerTo = result.Single(x => x.Name == "PowerTo");
            Assert.Null(powerTo.Value);

            var response = result.Single(x => x.Name == "Response");
            Assert.Equal("Resp1", response.Value);

            var responseTransformation = result.Single(x => x.Name == "ResponseTransformation");
            Assert.Equal("Log10", responseTransformation.Value);

            var sampleSizeFrom = result.Single(x => x.Name == "SampleSizeFrom");
            Assert.Equal("6", sampleSizeFrom.Value);

            var sampleSizeTo = result.Single(x => x.Name == "SampleSizeTo");
            Assert.Equal("15", sampleSizeTo.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);

            var treatment = result.Single(x => x.Name == "Treatment");
            Assert.Equal("Treat2", treatment.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            OneWayANOVAPowerAnalysisDatasetBasedInputsModel sut = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "GraphTitle" });
            arguments.Add(new Argument { Name = "PlottingRangeType", Value = "SampleSize" });
            arguments.Add(new Argument { Name = "PowerFrom" });
            arguments.Add(new Argument { Name = "PowerTo" });
            arguments.Add(new Argument { Name = "Response", Value = "Resp1" });
            arguments.Add(new Argument { Name = "ResponseTransformation", Value = "Log10" });
            arguments.Add(new Argument { Name = "SampleSizeFrom", Value = "6" });
            arguments.Add(new Argument { Name = "SampleSizeTo", Value = "15" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.05" });
            arguments.Add(new Argument { Name = "Treatment", Value = "Treat2" });

            Assert.Equal(10, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Null(sut.GraphTitle);
            Assert.Equal(PlottingRangeTypeOption.SampleSize, sut.PlottingRangeType);
            Assert.Null(sut.PowerFrom);
            Assert.Null(sut.PowerTo);
            Assert.Equal("Resp1", sut.Response);
            Assert.Equal("Log10", sut.ResponseTransformation);
            Assert.Equal(6, sut.SampleSizeFrom);
            Assert.Equal(15, sut.SampleSizeTo);
            Assert.Equal("0.05", sut.Significance);
            Assert.Equal("Treat2", sut.Treatment);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            OneWayANOVAPowerAnalysisDatasetBasedInputsModel sut = GetModel();

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("DatasetValues Resp1 Log10 Treat2 0.05 SampleSize 6 15 NULL", result);
        }


        private OneWayANOVAPowerAnalysisDatasetBasedInputsModel GetModel()
        {
            var model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel
            {
                GraphTitle = null,
                PlottingRangeType = PlottingRangeTypeOption.SampleSize,
                PowerFrom = null,
                PowerTo = null,
                Response = "Resp1",
                ResponseTransformation = "Log10",
                SampleSizeFrom = 6,
                SampleSizeTo = 15,
                Significance = "0.05",
                Treatment = "Treat2"
            };

            return model;
        }
    }
}