using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace SilveR.UnitTests.StatsModels
{
    
    public class UnpairedTTestAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            UnpairedTTestAnalysisModel sut = new UnpairedTTestAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("UnpairedTTestAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            UnpairedTTestAnalysisModel sut = new UnpairedTTestAnalysisModel();

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
            UnpairedTTestAnalysisModel sut = new UnpairedTTestAnalysisModel();

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

            UnpairedTTestAnalysisModel sut = GetModel(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Treat1", result[0]);
            Assert.Equal(13, result.Count()); //as blank reponses are removed
            Assert.StartsWith("234,B", result[12]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            UnpairedTTestAnalysisModel sut = GetModel(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var response = result.Single(x => x.Name == "Response");
            Assert.Equal("Resp 1", response.Value);

            var treatment = result.Single(x => x.Name == "Treatment");
            Assert.Equal("Treat1", treatment.Value);

            var responseTransformation = result.Single(x => x.Name == "ResponseTransformation");
            Assert.Equal("None", responseTransformation.Value);

            var equalVarianceCaseSelected = result.Single(x => x.Name == "EqualVarianceCaseSelected");
            Assert.Equal("True", equalVarianceCaseSelected.Value);

            var normalProbabilityPlotSelected = result.Single(x => x.Name == "NormalProbabilityPlotSelected");
            Assert.Equal("False", normalProbabilityPlotSelected.Value);

            var residualsVsPredictedPlotSelected = result.Single(x => x.Name == "ResidualsVsPredictedPlotSelected");
            Assert.Equal("False", residualsVsPredictedPlotSelected.Value);

            var unequalVarianceCaseSelected = result.Single(x => x.Name == "UnequalVarianceCaseSelected");
            Assert.Equal("True", unequalVarianceCaseSelected.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);

            var controlGroup = result.Single(x => x.Name == "ControlGroup");
            Assert.Equal("B", controlGroup.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            UnpairedTTestAnalysisModel sut = new UnpairedTTestAnalysisModel(GetDataset());

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "Response", Value = "Resp 1" });
            arguments.Add(new Argument { Name = "Treatment", Value = "Treat1" });
            arguments.Add(new Argument { Name = "ResponseTransformation", Value = "Log10" });
            arguments.Add(new Argument { Name = "NormalProbabilityPlotSelected", Value = "False" });
            arguments.Add(new Argument { Name = "ResidualsVsPredictedPlotSelected", Value = "True" });
            arguments.Add(new Argument { Name = "UnequalVarianceCaseSelected", Value = "False" });
            arguments.Add(new Argument { Name = "EqualVarianceCaseSelected", Value = "True" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.05" });
            arguments.Add(new Argument { Name = "ControlGroup", Value = "B" });

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("Resp 1", sut.Response);
            Assert.Equal("Treat1", sut.Treatment);
            Assert.Equal("Log10", sut.ResponseTransformation);
            Assert.False(sut.NormalProbabilityPlotSelected);
            Assert.True(sut.ResidualsVsPredictedPlotSelected);
            Assert.False(sut.UnequalVarianceCaseSelected);
            Assert.True(sut.EqualVarianceCaseSelected);
            Assert.Equal("0.05", sut.Significance);
            Assert.Equal("B", sut.ControlGroup);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            UnpairedTTestAnalysisModel sut = GetModel(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Respivs_sp_ivs1 None Treat1 Y Y N N B 0.05", result);
        }

        private UnpairedTTestAnalysisModel GetModel(IDataset dataset)
        {
            var model = new UnpairedTTestAnalysisModel(dataset)
            {
                ControlGroup = "B",
                EqualVarianceCaseSelected = true,
                NormalProbabilityPlotSelected = false,
                ResidualsVsPredictedPlotSelected = false,
                Response = "Resp 1",
                ResponseTransformation = "None",
                Significance = "0.05",
                Treatment = "Treat1",
                UnequalVarianceCaseSelected = true
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

        private Dataset GetDataset()
        {
            Dataset dataset = new Dataset
            {
                DatasetID = 6,
                DatasetName = "_test dataset.xlsx [unpairedttest]",
                DateUpdated = new DateTime(2018, 11, 16, 9, 14, 35),
                TheData = "SilveRSelected,Resp 1,Resp2,Resp 3,Resp4,Resp 5,Resp 6,Resp 7,Resp8,Resp:9,Resp-10,Resp^11,Treat1,Treat2,Treat3,Treat4,Treat(5,Treat£6,Treat:7,Treat}8,PVTestresponse1,PVTestresponse2,PVTestgroup\r\nTrue,65,65,65,x,,-2,0,-2,65,65,0.1,A,A,1,A,1,A,A,A,1,1,1\r\nTrue,32,,32,32,32,32,32,0.1,32,32,0.1,A,A,1,A,1,A,A,A,2,2,1\r\nTrue,543,,543,543,543,543,543,0.2,543,543,0.2,A,A,1,A,1,A,A,A,3,3,1\r\nTrue,675,,675,675,675,675,675,0.1,675,675,0.1,A,A,1,B,1,A,A,A,4,4,1\r\nTrue,876,,876,876,876,876,876,0.2,876,876,0.2,A,A,1,B,1,A,A,A,11,10,2\r\nTrue,54,,54,54,54,54,54,0.3,54,54,0.3,A,A,1,B,1,A,A,A,12,11,2\r\nTrue,432,,,432,432,432,432,0.45,432,432,0.45,B,B,2,C,2,B,B,B,13,12,2\r\nTrue,564,,,564,564,564,564,0.2,564,564,0.2,B,B,2,C,2,B,B,,14,13,2\r\nTrue,76,,,76,76,76,76,0.14,76,76,0.14,B,B,2,C,2,B,B,,,,\r\nTrue,54,,,54,54,54,54,0.2,54,54,0.2,B,B,2,D,3,B,B,,,,\r\nTrue,32,,,32,32,32,32,0.1,32,32,0.1,B,B,2,D,3,B,B,,,,\r\nTrue,234,,,234,234,234,234,0.4,234,234,0.4,B,,2,D,3,B,B,,,,",
                VersionNo = 1
            };

            return dataset;
        }
    }
}