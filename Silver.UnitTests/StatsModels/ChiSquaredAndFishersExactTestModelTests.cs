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

    public class ChiSquaredAndFishersExactTestModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            ChiSquaredAndFishersExactTestModel sut = new ChiSquaredAndFishersExactTestModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("ChiSquaredAndFishersExactTest", result);
        }

        [Fact]
        public void SignificancesList_ReturnsCorrectList()
        {
            //Arrange
            ChiSquaredAndFishersExactTestModel sut = new ChiSquaredAndFishersExactTestModel();

            //Act
            IEnumerable<string> result = sut.SignificancesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "0.1", "0.05", "0.01", "0.001" }, result);
        }

        [Fact]
        public void HypothesesList_ReturnsCorrectList()
        {
            //Arrange
            ChiSquaredAndFishersExactTestModel sut = new ChiSquaredAndFishersExactTestModel();

            //Act
            IEnumerable<string> result = sut.HypothesesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "Two-sided", "Less-than", "Greater-than" }, result);
        }

        [Fact]
        public void ExportData_ReturnsCorrectStringArray()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(It.IsAny<int>);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            ChiSquaredAndFishersExactTestModel sut = GetModel(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Count,Group,Respcat", result[0]);
            Assert.Equal(9, result.Count()); //as blank reponses are removed
            Assert.StartsWith("6,a_Tr,N", result[3]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            ChiSquaredAndFishersExactTestModel sut = GetModel(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var  barnardsTest = result.Single(x => x.Name == "BarnardsTest");
            Assert.Equal("False", barnardsTest.Value);

            var chiSquaredTest = result.Single(x => x.Name == "ChiSquaredTest");
            Assert.Equal("True", chiSquaredTest.Value);

            var fishersExactTest = result.Single(x => x.Name == "FishersExactTest");
            Assert.Equal("True", fishersExactTest.Value);

            var groupingFactor = result.Single(x => x.Name == "GroupingFactor");
            Assert.Equal("Group", groupingFactor.Value);

            var hypothesis = result.Single(x => x.Name == "Hypothesis");
            Assert.Equal("Two-sided", hypothesis.Value);

            var response = result.Single(x => x.Name == "Response");
            Assert.Equal("Count", response.Value);

            var responseCategories = result.Single(x => x.Name == "ResponseCategories");
            Assert.Equal("Respcat", responseCategories.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            ChiSquaredAndFishersExactTestModel sut = new ChiSquaredAndFishersExactTestModel(GetDataset());

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "BarnardsTest", Value = "False" });
            arguments.Add(new Argument { Name = "ChiSquaredTest", Value = "True" });
            arguments.Add(new Argument { Name = "FishersExactTest", Value = "True" });
            arguments.Add(new Argument { Name = "GroupingFactor", Value = "Group" });
            arguments.Add(new Argument { Name = "Hypothesis", Value = "Two-sided" });
            arguments.Add(new Argument { Name = "Response", Value = "Count" });
            arguments.Add(new Argument { Name = "ResponseCategories", Value = "Respcat" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.05" });

            Assert.Equal(8, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.False(sut.BarnardsTest);
            Assert.True(sut.ChiSquaredTest);
            Assert.True(sut.FishersExactTest);
            Assert.Equal("Group", sut.GroupingFactor);
            Assert.Equal("Two-sided", sut.Hypothesis);
            Assert.Equal("Count", sut.Response);
            Assert.Equal("Respcat", sut.ResponseCategories);
            Assert.Equal("0.05", sut.Significance);
        }


        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            ChiSquaredAndFishersExactTestModel sut = GetModel(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Count Group Respcat Y Y Two-sided N 0.05", result);
        }

        private ChiSquaredAndFishersExactTestModel GetModel(IDataset dataset)
        {
            var model = new ChiSquaredAndFishersExactTestModel(dataset)
            {
                BarnardsTest = false,
                ChiSquaredTest = true,
                FishersExactTest = true,
                GroupingFactor = "Group",
                Hypothesis = "Two-sided",
                Response = "Count",
                ResponseCategories = "Respcat",
                Significance = "0.05"
            };

            return model;
        }

        private DataTable GetTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SilveRSelected");
            dt.Columns.Add("CountAB");
            dt.Columns.Add("TreatmentA");
            dt.Columns.Add("TreatmentB");
            dt.Columns.Add("Treat A");
            dt.Columns.Add("Treat B");
            dt.Columns.Add("T_A");
            dt.Columns.Add("T_B");
            dt.Columns.Add("CountCD");
            dt.Columns.Add("TreatmentC");
            dt.Columns.Add("TreatmentD");
            dt.Columns.Add("BinC");
            dt.Columns.Add("BinD");
            dt.Columns.Add("CountEF");
            dt.Columns.Add("BinE");
            dt.Columns.Add("BinF");
            dt.Columns.Add("Bin(E");
            dt.Columns.Add("Bin(F");
            dt.Columns.Add("CountGH");
            dt.Columns.Add("TreatmentG");
            dt.Columns.Add("TreatmentH");
            dt.Columns.Add("Count_1");
            dt.Columns.Add("Treatment x");
            dt.Columns.Add("Treatmenty");
            dt.Columns.Add("Count_2");
            dt.Columns.Add("BinD2");
            dt.Columns.Add("Count");
            dt.Columns.Add("Group");
            dt.Columns.Add("Respcat");
            dt.Rows.Add(new object[] { "True", "76", "B", "3", "B^", "_3", "1", "3", "4", "1", "s", "x", "1", "4", "2", "1", "1", "1", "7", "B", "3", "76x", "", "1", "", "a", "2", "a_Tr", "Y", });
            dt.Rows.Add(new object[] { "True", "9", "B", "2", "B^", "*2", "1", "2", "2", "2", "s", "x", "1", "5", "1", "1", "1", "1", "9", "B", "2", "9", "1", "2", "9", "a", "1", "a_Tr", "Y", });
            dt.Rows.Add(new object[] { "True", "3", "C", "1", "C_", "[1", "3", "1", "3", "3", "s", "x", "2", "7", "2", "2", "2", "1", "3", "C", "1", "3", "1", "3", "3", "s", "6", "a_Tr", "N", });
            dt.Rows.Add(new object[] { "True", "5", "C", "3", "C_", "_3", "3", "3", "4", "1", "d", "x", "2", "4", "1", "2", "2", "2", "5", "C", "3", "5", "1", "1", "5", "s", "4", "a_Tr", "N", });
            dt.Rows.Add(new object[] { "True", "8", "C", "2", "C_", "*2", "3", "2", "65", "2", "d", "y", "1", "", "", "", "", "", "8", "C", "2", "8", "1", "2", "8", "a", "6", "a_Tr", "N", });
            dt.Rows.Add(new object[] { "True", "4", "A", "1", "A", "[1", "2", "1", "4", "3", "d", "y", "1", "", "", "", "", "", "4", "A", "1", "4", "2", "3", "4", "a", "8", "b_Con", "Y", });
            dt.Rows.Add(new object[] { "True", "12", "A", "2", "A", "*2", "2", "2", "3", "4", "d", "y", "2", "", "", "", "", "", "5", "A", "2", "12", "2", "1", "5", "s", "5", "b_Con", "N", });
            dt.Rows.Add(new object[] { "True", "87", "A", "3", "A", "_3", "2", "3", "", "", "", "", "", "", "", "", "", "", "4", "A", "3", "87", "2", "2", "4", "", "4", "b_Con", "Y", });
            dt.Rows.Add(new object[] { "True", "4", "A", "1", "A", "[1", "2", "1", "", "", "", "", "", "", "", "", "", "", "4", "A", "1", "4", "2", "3", "4", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "3", "A", "4", "A", "4", "2", "4", "", "", "", "", "", "", "", "", "", "", "3", "A", "4", "3", "2", "1", "3", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "5", "B", "4", "B^", "4", "1", "4", "", "", "", "", "", "", "", "", "", "", "5", "B", "4", "5", "1", "2", "5", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "7", "C", "4", "C_", "4", "3", "4", "", "", "", "", "", "", "", "", "", "", "7", "C", "4", "7", "1", "3", "7", "", "", "", "", });

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