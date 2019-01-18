using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace Silver.UnitTests.StatsModels
{

    public class SurvivalAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            SurvivalAnalysisModel sut = new SurvivalAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("SurvivalAnalysis", result);
        }


        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            SurvivalAnalysisModel sut = new SurvivalAnalysisModel();

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
            mockDataset.Setup(x => x.DatasetID).Returns(It.IsAny<int>);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            SurvivalAnalysisModel sut = GetModel(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Resp2,Censor1,Groupivs_sp_ivs1", result[0]);
            Assert.Equal(12, result.Count()); //as blank reponses are removed
            Assert.Equal("5,0,A", result[3]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            SurvivalAnalysisModel sut = GetModel(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var censorship = result.Single(x => x.Name == "Censorship");
            Assert.Equal("Censor1", censorship.Value);

            var compareSurvivalCurves = result.Single(x => x.Name == "CompareSurvivalCurves");
            Assert.Equal("False", compareSurvivalCurves.Value);

            var response = result.Single(x => x.Name == "Response");
            Assert.Equal("Resp2", response.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);

            var summaryResults = result.Single(x => x.Name == "SummaryResults");
            Assert.Equal("True", summaryResults.Value);

            var survivalPlot = result.Single(x => x.Name == "SurvivalPlot");
            Assert.Equal("True", survivalPlot.Value);

            var treatment = result.Single(x => x.Name == "Treatment");
            Assert.Equal("Group 1", treatment.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            SurvivalAnalysisModel sut = new SurvivalAnalysisModel(GetDataset());

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "Censorship", Value = "Censor1" });
            arguments.Add(new Argument { Name = "CompareSurvivalCurves", Value = "False" });
            arguments.Add(new Argument { Name = "Response", Value = "Resp2" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.05" });
            arguments.Add(new Argument { Name = "SummaryResults", Value = "True" });
            arguments.Add(new Argument { Name = "SurvivalPlot", Value = "True" });
            arguments.Add(new Argument { Name = "Treatment", Value = "Group 1" });

            Assert.Equal(7, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("Censor1", sut.Censorship);
            Assert.False(sut.CompareSurvivalCurves);
            Assert.Equal("Resp2", sut.Response);
            Assert.Equal("0.05", sut.Significance);
            Assert.True(sut.SummaryResults);
            Assert.True(sut.SurvivalPlot);
            Assert.Equal("Group 1", sut.Treatment);
        }


        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            SurvivalAnalysisModel sut = GetModel(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Resp2 Groupivs_sp_ivs1 Censor1 Y Y N 0.05", result);
        }

        private SurvivalAnalysisModel GetModel(IDataset dataset)
        {
            var model = new SurvivalAnalysisModel(dataset)
            {
                Censorship = "Censor1",
                CompareSurvivalCurves = false,
                Response = "Resp2",
                Significance = "0.05",
                SummaryResults = true,
                SurvivalPlot = true,
                Treatment = "Group 1"
            };

            return model;
        }

        private DataTable GetTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SilveRSelected");
            dt.Columns.Add("Resp 1");
            dt.Columns.Add("Resp2");
            dt.Columns.Add("Resp3");
            dt.Columns.Add("Resp4");
            dt.Columns.Add("Resp £5");
            dt.Columns.Add("Censor1");
            dt.Columns.Add("Censor2");
            dt.Columns.Add("Censor3");
            dt.Columns.Add("Censor4");
            dt.Columns.Add("Censor5");
            dt.Columns.Add("Censor6");
            dt.Columns.Add("Censor7");
            dt.Columns.Add("Censor8");
            dt.Columns.Add("Censor9");
            dt.Columns.Add("Group 1");
            dt.Columns.Add("Group 2");
            dt.Columns.Add("Group 3");
            dt.Columns.Add("Group 4");
            dt.Columns.Add("Group 5");
            dt.Columns.Add("Group 6");
            dt.Columns.Add("Group 7");
            dt.Columns.Add("Group 8");
            dt.Rows.Add(new object[] { "True", "A", "2", "", "8", "8", "0", "2", "0", "1", "", "1", "0", "1", "1", "A", "A", "B", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "4", "4", "4", "16", "16", "0", "0", "0", "1", "0", "1", "0", "1", "1", "A", "A", "A", "A", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "5", "5", "5", "23", "23", "0", "0", "0", "1", "0", "1", "0", "1", "1", "A", "A", "A", "A", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "6", "6", "6", "13", "13", "1", "1", "0", "1", "1", "0", "0", "1", "1", "A", "A", "A", "A", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "4", "4", "4", "22", "22", "1", "1", "0", "1", "1", "1", "0", "1", "1", "A", "A", "A", "A", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "8", "8", "8", "28", "28", "0", "0", "0", "1", "0", "1", "0", "1", "1", "A", "A", "A", "A", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "3", "3", "3", "447", "447", "1", "1", "0", "1", "1", "1", "0", "1", "1", "B", "A", "A", "B", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "5", "5", "5", "318", "318", "1", "1", "0", "1", "1", "1", "0", "1", "1", "B", "A", "A", "B", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "6", "6", "6", "30", "30", "0", "0", "0", "1", "0", "1", "0", "1", "1", "B", "A", "A", "B", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "8", "8", "8", "12", "12", "0", "0", "0", "1", "0", "1", "0", "1", "1", "B", "A", "A", "B", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "9", "9", "9", "24", "24", "0", "0", "0", "1", "0", "1", "0", "1", "1", "B", "A", "A", "B", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "245", "245", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "7", "7", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "9", "9", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "511", "511", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "30", "30", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "53", "53", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "196", "196", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "15", "15", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "154", "154", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "7", "7", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "333", "333", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "141", "141", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "8", "8", "", "", "", "", "", "0", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "96", "96", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "38", "38", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "149", "149", "", "", "", "", "", "0", "0", "1", "0", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "70", "70", "", "", "", "", "", "0", "0", "1", "0", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "536", "536", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "25", "25", "", "", "", "", "", "0", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "17", "17", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "4", "4", "", "", "", "", "", "0", "0", "1", "0", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "185", "185", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "177", "177", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "292", "292", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "114", "114", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "22", "22", "", "", "", "", "", "0", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "159", "159", "", "", "", "", "", "0", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "15", "15", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "108", "108", "", "", "", "", "", "0", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "152", "152", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "PKD", "PKD", "4", "PKD", });
            dt.Rows.Add(new object[] { "True", "", "", "", "562", "562", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "PKD", "PKD", "4", "PKD", });
            dt.Rows.Add(new object[] { "True", "", "", "", "402", "402", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "24", "24", "", "", "", "", "", "0", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "13", "13", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "66", "66", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "39", "39", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "46", "46", "", "", "", "", "", "0", "0", "1", "0", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "12", "12", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "40", "40", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "113", "113", "", "", "", "", "", "0", "0", "1", "0", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "201", "201", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "132", "132", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "156", "156", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "34", "34", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "30", "30", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "2", "2", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "25", "25", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "130", "130", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "26", "26", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "27", "27", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "58", "58", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "5", "5", "", "", "", "", "", "0", "0", "1", "0", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "43", "43", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "AN", "AN", "1", "AN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "152", "152", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "PKD", "PKD", "4", "PKD", });
            dt.Rows.Add(new object[] { "True", "", "", "", "30", "30", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "PKD", "PKD", "4", "PKD", });
            dt.Rows.Add(new object[] { "True", "", "", "", "190", "190", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "5", "5", "", "", "", "", "", "0", "0", "1", "1", "", "", "", "", "GN", "xGN", "2", "GN", });
            dt.Rows.Add(new object[] { "True", "", "", "", "119", "119", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "8", "8", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "54", "54", "", "", "", "", "", "0", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "16", "16", "", "", "", "", "", "0", "0", "1", "1", "", "", "", "", "Other", "Other", "3", "Other", });
            dt.Rows.Add(new object[] { "True", "", "", "", "6", "6", "", "", "", "", "", "0", "0", "1", "0", "", "", "", "", "PKD", "PKD", "4", "PKD", });
            dt.Rows.Add(new object[] { "True", "", "", "", "78", "78", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "PKD", "PKD", "4", "PKD", });
            dt.Rows.Add(new object[] { "True", "", "", "", "63", "63", "", "", "", "", "", "1", "0", "1", "1", "", "", "", "", "PKD", "PKD", "4", "PKD", });
            dt.Rows.Add(new object[] { "True", "", "", "", "8", "8", "", "", "", "", "", "0", "0", "1", "0", "", "", "", "", "PKD", "PKD", "4", "PKD", });

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