using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
    public class PValueAdjustmentDatasetBasedInputsModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            PValueAdjustmentDatasetBasedInputsModel sut = new PValueAdjustmentDatasetBasedInputsModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("PValueAdjustmentDatasetBasedInputs", result);
        }


        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            PValueAdjustmentDatasetBasedInputsModel sut = new PValueAdjustmentDatasetBasedInputsModel();

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
            PValueAdjustmentDatasetBasedInputsModel sut = new PValueAdjustmentDatasetBasedInputsModel();

            //Act
            IEnumerable<string> result = sut.MultipleComparisonTests;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "Holm", "Hochberg", "Hommel", "Benjamini-Hochberg", "Bonferroni" }, result);
        }

        [Fact]
        public void ExportData_MultipleTreatments_ReturnsCorrectStringArray()
        {
            ////Arrange
            //Mock<IDataset> mockDataset = new Mock<IDataset>();
            //mockDataset.Setup(x => x.DatasetID).Returns(1);
            //mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            //SingleMeasuresParametricAnalysisModel sut = GetModel(mockDataset.Object);

            ////Act
            //string[] result = sut.ExportData();

            ////Assert
            //Assert.Equal("Respivs_sp_ivs1", result[0]);

            ////scatterplot check
            //Assert.Contains(",D1 F,", result[24]);

            ////mainEffect check
            //Assert.EndsWith(",D1 F", result[24]);
        }


        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            PValueAdjustmentDatasetBasedInputsModel sut = GetModel();

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var selectedTest = result.Single(x => x.Name == "SelectedTest");
            Assert.Equal("Hochberg", selectedTest.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.01", significance.Value);

            var pValues = result.Single(x => x.Name == "PValues");
            Assert.Equal("Resp1", pValues.Value);

            var datasetValues = result.Single(x => x.Name == "DatasetLabels");
            Assert.Equal("Treat1", datasetValues.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            PValueAdjustmentDatasetBasedInputsModel sut = new PValueAdjustmentDatasetBasedInputsModel();

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "SelectedTest", Value = "Hochberg" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.01" });
            arguments.Add(new Argument { Name = "PValues", Value = "Resp1" });
            arguments.Add(new Argument { Name = "DatasetLabels", Value = "Treat1" });

            Assert.Equal(4, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("Hochberg", sut.SelectedTest);
            Assert.Equal("0.01", sut.Significance);
            Assert.Equal("Resp1", sut.PValues);
            Assert.Equal("Treat1", sut.DatasetLabels);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            PValueAdjustmentDatasetBasedInputsModel sut = GetModel();

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Resp1 Treat1 Hochberg 0.01", result);
        }


        private PValueAdjustmentDatasetBasedInputsModel GetModel()
        {
            var model = new PValueAdjustmentDatasetBasedInputsModel
            {
                SelectedTest = "Hochberg",
                Significance = "0.01",
                PValues = "Resp1",
                DatasetLabels = "Treat1"
            };

            return model;
        }
    }
}