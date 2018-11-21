using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{
    [ExcludeFromCodeCoverageAttribute]
    public class OneSampleTTestAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            OneSampleTTestAnalysisModel sut = new OneSampleTTestAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("OneSampleTTestAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            OneSampleTTestAnalysisModel sut = new OneSampleTTestAnalysisModel();

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
            OneSampleTTestAnalysisModel sut = new OneSampleTTestAnalysisModel();

            //Act
            IEnumerable<string> result = sut.SignificancesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "0.1", "0.05", "0.01", "0.001" }, result);
        }

        [Fact]
        public void ExportData_ReturnsCorrectStringArray()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(1);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            OneSampleTTestAnalysisModel sut = GetModel(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Resp2", result[0]);
            Assert.Equal(13, result.Count()); //as blank reponses are removed
            Assert.Equal("543,", result[3]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            OneSampleTTestAnalysisModel sut = GetModel(null);

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var normalPlot = result.Single(x => x.Name == "NormalPlot");
            Assert.Equal("False", normalPlot.Value);

            var confidenceInterval = result.Single(x => x.Name == "ConfidenceInterval");
            Assert.Equal("True", confidenceInterval.Value);

            var responses = result.Single(x => x.Name == "Responses");
            Assert.Equal("Resp 1,Resp2", responses.Value);

            var responseTransformation = result.Single(x => x.Name == "ResponseTransformation");
            Assert.Equal("None", responseTransformation.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);

            var testValue = result.Single(x => x.Name == "TestValue");
            Assert.Equal("3.142", testValue.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            OneSampleTTestAnalysisModel sut = new OneSampleTTestAnalysisModel(null);

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "NormalPlot", Value = "False" });
            arguments.Add(new Argument { Name = "ConfidenceInterval", Value = "True" });
            arguments.Add(new Argument { Name = "Responses", Value = "Resp 1,Resp2" });
            arguments.Add(new Argument { Name = "ResponseTransformation", Value = "None" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.05" });
            arguments.Add(new Argument { Name = "TestValue", Value = "3.142" });

            Assert.Equal(6, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.False(sut.NormalPlot);
            Assert.True(sut.ConfidenceInterval);
            Assert.Equal(new List<string> { "Resp 1","Resp2" }, sut.Responses);
            Assert.Equal("None", sut.ResponseTransformation);
            Assert.Equal("0.05", sut.Significance);
            Assert.Equal(3.142m, sut.TestValue);
        }


        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            OneSampleTTestAnalysisModel sut = GetModel(null);

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Resp2 None 3.142 Y N 0.05", result);
        }


        private OneSampleTTestAnalysisModel GetModel(IDataset dataset)
        {
            var model = new OneSampleTTestAnalysisModel(dataset)
            {
                NormalPlot = false,
                ConfidenceInterval = true,
                Responses = new List<string> { "Resp 1", "Resp2" },
                ResponseTransformation = "None",
                Significance = "0.05",
                TestValue = 3.142m
            };

            return model;
        }

        private DataTable GetTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SilveRSelected");
            dt.Columns.Add("Resp 1");
            dt.Columns.Add("Resp2");
            dt.Columns.Add("Resp 3");
            dt.Columns.Add("Resp4");
            dt.Columns.Add("Resp 5");
            dt.Columns.Add("Resp 6");
            dt.Columns.Add("Resp 7");
            dt.Columns.Add("Resp8");
            dt.Columns.Add("Resp:9");
            dt.Columns.Add("Resp-10");
            dt.Columns.Add("Resp^11");
            dt.Columns.Add("Treat1");
            dt.Columns.Add("Treat2");
            dt.Columns.Add("Treat3");
            dt.Columns.Add("Treat4");
            dt.Columns.Add("Treat(5");
            dt.Columns.Add("Treat£6");
            dt.Columns.Add("Treat:7");
            dt.Columns.Add("Treat}8");
            dt.Columns.Add("PVTestresponse1");
            dt.Columns.Add("PVTestresponse2");
            dt.Columns.Add("PVTestgroup");

            dt.Rows.Add(new object[] { "True", "65", "65", "65", "x", "", "-2", "0", "-2", "65", "65", "0.1", "A", "A", "1", "A", "1", "A", "A", "A", "1", "1", "1", });
            dt.Rows.Add(new object[] { "True", "32", "", "32", "32", "32", "32", "32", "0.1", "32", "32", "0.1", "A", "A", "1", "A", "1", "A", "A", "A", "2", "2", "1", });
            dt.Rows.Add(new object[] { "True", "543", "", "543", "543", "543", "543", "543", "0.2", "543", "543", "0.2", "A", "A", "1", "A", "1", "A", "A", "A", "3", "3", "1", });
            dt.Rows.Add(new object[] { "True", "675", "", "675", "675", "675", "675", "675", "0.1", "675", "675", "0.1", "A", "A", "1", "B", "1", "A", "A", "A", "4", "4", "1", });
            dt.Rows.Add(new object[] { "True", "876", "", "876", "876", "876", "876", "876", "0.2", "876", "876", "0.2", "A", "A", "1", "B", "1", "A", "A", "A", "11", "10", "2", });
            dt.Rows.Add(new object[] { "True", "54", "", "54", "54", "54", "54", "54", "0.3", "54", "54", "0.3", "A", "A", "1", "B", "1", "A", "A", "A", "12", "11", "2", });
            dt.Rows.Add(new object[] { "True", "432", "", "", "432", "432", "432", "432", "0.45", "432", "432", "0.45", "B", "B", "2", "C", "2", "B", "B", "B", "13", "12", "2", });
            dt.Rows.Add(new object[] { "True", "564", "", "", "564", "564", "564", "564", "0.2", "564", "564", "0.2", "B", "B", "2", "C", "2", "B", "B", "", "14", "13", "2", });
            dt.Rows.Add(new object[] { "True", "76", "", "", "76", "76", "76", "76", "0.14", "76", "76", "0.14", "B", "B", "2", "C", "2", "B", "B", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "54", "", "", "54", "54", "54", "54", "0.2", "54", "54", "0.2", "B", "B", "2", "D", "3", "B", "B", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "32", "", "", "32", "32", "32", "32", "0.1", "32", "32", "0.1", "B", "B", "2", "D", "3", "B", "B", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "234", "", "", "234", "234", "234", "234", "0.4", "234", "234", "0.4", "B", "", "2", "D", "3", "B", "B", "", "", "", "", });

            return dt;
        }
    }
}