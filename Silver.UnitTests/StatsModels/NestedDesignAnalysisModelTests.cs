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
    public class NestedDesignAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            NestedDesignAnalysisModel sut = new NestedDesignAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("NestedDesignAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            NestedDesignAnalysisModel sut = new NestedDesignAnalysisModel();

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
            NestedDesignAnalysisModel sut = new NestedDesignAnalysisModel();

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

            NestedDesignAnalysisModel sut = GetModel(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Response1,Covariate1,Treat1,Random1", result[0]);
            Assert.Equal(769, result.Count()); //as blank reponses are removed
            Assert.StartsWith("56.32", result[32]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            NestedDesignAnalysisModel sut = GetModel(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var covariateTransformation = result.Single(x => x.Name == "CovariateTransformation");
            Assert.Equal("Log10", covariateTransformation.Value);

            var covariates = result.Single(x => x.Name == "Covariates");
            Assert.Equal("Covariate1", covariates.Value);

            var designOption1 = result.Single(x => x.Name == "DesignOption1");
            Assert.Null(designOption1.Value);

            var designOption2 = result.Single(x => x.Name == "DesignOption2");
            Assert.Null(designOption2.Value);

            var designOption3 = result.Single(x => x.Name == "DesignOption3");
            Assert.Null(designOption3.Value);

            var designOption4 = result.Single(x => x.Name == "DesignOption4");
            Assert.Null(designOption4.Value);

            var otherDesignFactors = result.Single(x => x.Name == "OtherDesignFactors");
            Assert.Null(otherDesignFactors.Value);

            var randomFactor1 = result.Single(x => x.Name == "RandomFactor1");
            Assert.Equal("Random1", randomFactor1.Value);

            var randomFactor2 = result.Single(x => x.Name == "RandomFactor2");
            Assert.Null(randomFactor2.Value);

            var randomFactor3 = result.Single(x => x.Name == "RandomFactor3");
            Assert.Null(randomFactor3.Value);

            var randomFactor4 = result.Single(x => x.Name == "RandomFactor4");
            Assert.Null(randomFactor4.Value);

            var response = result.Single(x => x.Name == "Response");
            Assert.Equal("Response1", response.Value);

            var responseTransformation = result.Single(x => x.Name == "ResponseTransformation");
            Assert.Equal("None", responseTransformation.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);

            var treatments = result.Single(x => x.Name == "Treatments");
            Assert.Equal("Treat1", treatments.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            NestedDesignAnalysisModel sut = new NestedDesignAnalysisModel(GetDataset());

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "CovariateTransformation", Value = "Log 10" });
            arguments.Add(new Argument { Name = "Covariates", Value = "Covariate1" });
            arguments.Add(new Argument { Name = "DesignOption1" });
            arguments.Add(new Argument { Name = "DesignOption2" });
            arguments.Add(new Argument { Name = "DesignOption3" });
            arguments.Add(new Argument { Name = "DesignOption4" });
            arguments.Add(new Argument { Name = "OtherDesignFactors" });
            arguments.Add(new Argument { Name = "RandomFactor1" });
            arguments.Add(new Argument { Name = "RandomFactor2" });
            arguments.Add(new Argument { Name = "RandomFactor3" });
            arguments.Add(new Argument { Name = "RandomFactor4" });
            arguments.Add(new Argument { Name = "Response", Value = "Response1" });
            arguments.Add(new Argument { Name = "ResponseTransformation", Value = "None" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.05" });
            arguments.Add(new Argument { Name = "Treatments", Value = "Treat1" });

            Assert.Equal(15, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("Log 10", sut.CovariateTransformation);
            Assert.Equal(new List<string> { "Covariate1" }, sut.Covariates);
            Assert.Null(sut.OtherDesignFactors);
            Assert.Null(sut.DesignOption1);
            Assert.Null(sut.DesignOption2);
            Assert.Null(sut.DesignOption3);
            Assert.Null(sut.DesignOption4);
            Assert.Null(sut.OtherDesignFactors);
            Assert.Null(sut.RandomFactor1);
            Assert.Null(sut.RandomFactor2);
            Assert.Null(sut.RandomFactor3);
            Assert.Null(sut.RandomFactor4);
            Assert.Equal("Response1", sut.Response);
            Assert.Equal("None", sut.ResponseTransformation);
            Assert.Equal("0.05", sut.Significance);
            Assert.Equal(new List<string> { "Treat1" }, sut.Treatments);
        }


        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            NestedDesignAnalysisModel sut = GetModel(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Response1~Covariate1+Treat1+Random1 None Log10 Treat1 NULL Covariate1 0.05 Random1 NULL NULL NULL NULL NULL NULL NULL", result);
        }

        private NestedDesignAnalysisModel GetModel(IDataset dataset)
        {
            var model = new SilveR.StatsModels.NestedDesignAnalysisModel(dataset)
            {
                CovariateTransformation = "Log10",
                Covariates = new System.Collections.Generic.List<string>
                {
                    "Covariate1"
                },
                DatasetID = 2,
                DesignOption1 = null,
                DesignOption2 = null,
                DesignOption3 = null,
                DesignOption4 = null,
                OtherDesignFactors = null,
                RandomFactor1 = "Random1",
                RandomFactor2 = null,
                RandomFactor3 = null,
                RandomFactor4 = null,
                Response = "Response1",
                ResponseTransformation = "None",
                Significance = "0.05",
                Treatments = new System.Collections.Generic.List<string>
                {
                    "Treat1"
                }
            };

            return model;
        }

        private DataTable GetTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SilveRSelected");
            dt.Columns.Add("Response1");
            dt.Columns.Add("Covariate1");
            dt.Columns.Add("Treat1");
            dt.Columns.Add("Treat2");
            dt.Columns.Add("Block1");
            dt.Columns.Add("Block2");
            dt.Columns.Add("Random1");
            dt.Columns.Add("Random2");
            dt.Columns.Add("Random3");
            dt.Columns.Add("Random4");
            dt.Rows.Add(new object[] { "True", "32.47", "0.49", "A", "1", "1", "x", "1", "1", "1", "1", });
            dt.Rows.Add(new object[] { "True", "27.65", "0.15", "B", "1", "1", "x", "2", "2", "2", "2", });
            dt.Rows.Add(new object[] { "True", "81.67", "0.8", "A", "2", "1", "x", "3", "3", "3", "3", });
            dt.Rows.Add(new object[] { "True", "42.29", "0.61", "B", "2", "1", "x", "4", "4", "4", "4", });
            dt.Rows.Add(new object[] { "True", "56.76", "0.46", "A", "1", "2", "x", "5", "5", "5", "5", });
            dt.Rows.Add(new object[] { "True", "6.85", "0.61", "B", "1", "2", "x", "6", "6", "6", "6", });
            dt.Rows.Add(new object[] { "True", "50.75", "0.28", "A", "2", "2", "x", "7", "7", "7", "7", });
            dt.Rows.Add(new object[] { "True", "2.8", "0.83", "B", "2", "2", "x", "8", "8", "8", "8", });
            dt.Rows.Add(new object[] { "True", "67.89", "0.18", "A", "1", "1", "c", "9", "9", "9", "9", });
            dt.Rows.Add(new object[] { "True", "2.09", "0.38", "B", "1", "1", "c", "10", "10", "10", "10", });
            dt.Rows.Add(new object[] { "True", "70.73", "0.58", "A", "2", "1", "c", "11", "11", "11", "11", });
            dt.Rows.Add(new object[] { "True", "56.97", "0.96", "B", "2", "1", "c", "12", "12", "12", "12", });
            dt.Rows.Add(new object[] { "True", "9.94", "0.7", "A", "1", "2", "c", "13", "13", "13", "13", });
            dt.Rows.Add(new object[] { "True", "16.41", "0.31", "B", "1", "2", "c", "14", "14", "14", "14", });
            dt.Rows.Add(new object[] { "True", "40.28", "0.42", "A", "2", "2", "c", "15", "15", "15", "15", });
            dt.Rows.Add(new object[] { "True", "68.06", "0.72", "B", "2", "2", "c", "16", "16", "16", "16", });
            dt.Rows.Add(new object[] { "True", "48.68", "0.3", "A", "1", "1", "x", "17", "17", "17", "17", });
            dt.Rows.Add(new object[] { "True", "16.34", "0.67", "B", "1", "1", "x", "18", "18", "18", "18", });
            dt.Rows.Add(new object[] { "True", "45.97", "0.98", "A", "2", "1", "x", "19", "19", "19", "19", });
            dt.Rows.Add(new object[] { "True", "90.68", "0.8", "B", "2", "1", "x", "20", "20", "20", "20", });
            dt.Rows.Add(new object[] { "True", "47.38", "0.51", "A", "1", "2", "x", "21", "21", "21", "21", });
            dt.Rows.Add(new object[] { "True", "50.08", "0.14", "B", "1", "2", "x", "22", "22", "22", "22", });
            dt.Rows.Add(new object[] { "True", "22.39", "0.86", "A", "2", "2", "x", "23", "23", "23", "23", });
            dt.Rows.Add(new object[] { "True", "6.87", "0.67", "B", "2", "2", "x", "24", "24", "24", "24", });
            dt.Rows.Add(new object[] { "True", "4.61", "0.32", "A", "1", "1", "c", "25", "25", "25", "25", });
            dt.Rows.Add(new object[] { "True", "6.32", "0.69", "B", "1", "1", "c", "26", "26", "26", "26", });
            dt.Rows.Add(new object[] { "True", "9.53", "0.45", "A", "2", "1", "c", "27", "27", "27", "27", });
            dt.Rows.Add(new object[] { "True", "18.21", "0.07", "B", "2", "1", "c", "28", "28", "28", "28", });
            dt.Rows.Add(new object[] { "True", "16.4", "0.11", "A", "1", "2", "c", "29", "29", "29", "29", });
            dt.Rows.Add(new object[] { "True", "28.2", "0.3", "B", "1", "2", "c", "30", "30", "30", "30", });
            dt.Rows.Add(new object[] { "True", "12.21", "0.01", "A", "2", "2", "c", "31", "31", "31", "31", });
            dt.Rows.Add(new object[] { "True", "56.32", "0.25", "B", "2", "2", "c", "32", "32", "32", "32", });
            dt.Rows.Add(new object[] { "True", "28.94", "0.49", "A", "1", "1", "x", "1", "33", "33", "33", });
            dt.Rows.Add(new object[] { "True", "78.31", "0.15", "B", "1", "1", "x", "2", "34", "34", "34", });
            dt.Rows.Add(new object[] { "True", "93.23", "0.8", "A", "2", "1", "x", "3", "35", "35", "35", });
            dt.Rows.Add(new object[] { "True", "90.87", "0.61", "B", "2", "1", "x", "4", "36", "36", "36", });
            dt.Rows.Add(new object[] { "True", "42.62", "0.46", "A", "1", "2", "x", "5", "37", "37", "37", });
            dt.Rows.Add(new object[] { "True", "51.95", "0.61", "B", "1", "2", "x", "6", "38", "38", "38", });
            dt.Rows.Add(new object[] { "True", "6.34", "0.28", "A", "2", "2", "x", "7", "39", "39", "39", });
            dt.Rows.Add(new object[] { "True", "30.53", "0.83", "B", "2", "2", "x", "8", "40", "40", "40", });
            dt.Rows.Add(new object[] { "True", "70.74", "0.18", "A", "1", "1", "c", "9", "41", "41", "41", });
            dt.Rows.Add(new object[] { "True", "56.92", "0.38", "B", "1", "1", "c", "10", "42", "42", "42", });
            dt.Rows.Add(new object[] { "True", "21.61", "0.58", "A", "2", "1", "c", "11", "43", "43", "43", });
            dt.Rows.Add(new object[] { "True", "31.88", "0.96", "B", "2", "1", "c", "12", "44", "44", "44", });
            dt.Rows.Add(new object[] { "True", "99.86", "0.7", "A", "1", "2", "c", "13", "45", "45", "45", });
            dt.Rows.Add(new object[] { "True", "21.91", "0.31", "B", "1", "2", "c", "14", "46", "46", "46", });
            dt.Rows.Add(new object[] { "True", "69.56", "0.42", "A", "2", "2", "c", "15", "47", "47", "47", });
            dt.Rows.Add(new object[] { "True", "0.84", "0.72", "B", "2", "2", "c", "16", "48", "48", "48", });
            dt.Rows.Add(new object[] { "True", "95.58", "0.3", "A", "1", "1", "x", "17", "49", "49", "49", });
            dt.Rows.Add(new object[] { "True", "99.04", "0.67", "B", "1", "1", "x", "18", "50", "50", "50", });
            dt.Rows.Add(new object[] { "True", "32.57", "0.98", "A", "2", "1", "x", "19", "51", "51", "51", });
            dt.Rows.Add(new object[] { "True", "4.21", "0.8", "B", "2", "1", "x", "20", "52", "52", "52", });
            dt.Rows.Add(new object[] { "True", "99.43", "0.51", "A", "1", "2", "x", "21", "53", "53", "53", });
            dt.Rows.Add(new object[] { "True", "78.64", "0.14", "B", "1", "2", "x", "22", "54", "54", "54", });
            dt.Rows.Add(new object[] { "True", "64.14", "0.86", "A", "2", "2", "x", "23", "55", "55", "55", });
            dt.Rows.Add(new object[] { "True", "43.29", "0.67", "B", "2", "2", "x", "24", "56", "56", "56", });
            dt.Rows.Add(new object[] { "True", "35.56", "0.32", "A", "1", "1", "c", "25", "57", "57", "57", });
            dt.Rows.Add(new object[] { "True", "21.39", "0.69", "B", "1", "1", "c", "26", "58", "58", "58", });
            dt.Rows.Add(new object[] { "True", "71.04", "0.45", "A", "2", "1", "c", "27", "59", "59", "59", });
            dt.Rows.Add(new object[] { "True", "65.53", "0.07", "B", "2", "1", "c", "28", "60", "60", "60", });
            dt.Rows.Add(new object[] { "True", "43.75", "0.11", "A", "1", "2", "c", "29", "61", "61", "61", });
            dt.Rows.Add(new object[] { "True", "62.81", "0.3", "B", "1", "2", "c", "30", "62", "62", "62", });
            dt.Rows.Add(new object[] { "True", "61.57", "0.01", "A", "2", "2", "c", "31", "63", "63", "63", });
            dt.Rows.Add(new object[] { "True", "97.93", "0.25", "B", "2", "2", "c", "32", "64", "64", "64", });
            dt.Rows.Add(new object[] { "True", "7.61", "0.49", "A", "1", "1", "x", "1", "1", "65", "65", });
            dt.Rows.Add(new object[] { "True", "48.4", "0.15", "B", "1", "1", "x", "2", "2", "66", "66", });
            dt.Rows.Add(new object[] { "True", "80.51", "0.8", "A", "2", "1", "x", "3", "3", "67", "67", });
            dt.Rows.Add(new object[] { "True", "56.62", "0.61", "B", "2", "1", "x", "4", "4", "68", "68", });
            dt.Rows.Add(new object[] { "True", "41.56", "0.46", "A", "1", "2", "x", "5", "5", "69", "69", });
            dt.Rows.Add(new object[] { "True", "35.09", "0.61", "B", "1", "2", "x", "6", "6", "70", "70", });
            dt.Rows.Add(new object[] { "True", "59.04", "0.28", "A", "2", "2", "x", "7", "7", "71", "71", });
            dt.Rows.Add(new object[] { "True", "40.44", "0.83", "B", "2", "2", "x", "8", "8", "72", "72", });
            dt.Rows.Add(new object[] { "True", "65.84", "0.18", "A", "1", "1", "c", "9", "9", "73", "73", });
            dt.Rows.Add(new object[] { "True", "26.32", "0.38", "B", "1", "1", "c", "10", "10", "74", "74", });
            dt.Rows.Add(new object[] { "True", "57.58", "0.58", "A", "2", "1", "c", "11", "11", "75", "75", });
            dt.Rows.Add(new object[] { "True", "64.96", "0.96", "B", "2", "1", "c", "12", "12", "76", "76", });
            dt.Rows.Add(new object[] { "True", "88.75", "0.7", "A", "1", "2", "c", "13", "13", "77", "77", });
            dt.Rows.Add(new object[] { "True", "36.3", "0.31", "B", "1", "2", "c", "14", "14", "78", "78", });
            dt.Rows.Add(new object[] { "True", "73.28", "0.42", "A", "2", "2", "c", "15", "15", "79", "79", });
            dt.Rows.Add(new object[] { "True", "72.82", "0.72", "B", "2", "2", "c", "16", "16", "80", "80", });
            dt.Rows.Add(new object[] { "True", "32.65", "0.3", "A", "1", "1", "x", "17", "17", "81", "81", });
            dt.Rows.Add(new object[] { "True", "87.83", "0.67", "B", "1", "1", "x", "18", "18", "82", "82", });
            dt.Rows.Add(new object[] { "True", "81.67", "0.98", "A", "2", "1", "x", "19", "19", "83", "83", });
            dt.Rows.Add(new object[] { "True", "78.33", "0.8", "B", "2", "1", "x", "20", "20", "84", "84", });
            dt.Rows.Add(new object[] { "True", "49.07", "0.51", "A", "1", "2", "x", "21", "21", "85", "85", });
            dt.Rows.Add(new object[] { "True", "8.06", "0.14", "B", "1", "2", "x", "22", "22", "86", "86", });
            dt.Rows.Add(new object[] { "True", "73.42", "0.86", "A", "2", "2", "x", "23", "23", "87", "87", });
            dt.Rows.Add(new object[] { "True", "8.07", "0.67", "B", "2", "2", "x", "24", "24", "88", "88", });
            dt.Rows.Add(new object[] { "True", "62.67", "0.32", "A", "1", "1", "c", "25", "25", "89", "89", });
            dt.Rows.Add(new object[] { "True", "76.9", "0.69", "B", "1", "1", "c", "26", "26", "90", "90", });
            dt.Rows.Add(new object[] { "True", "38.96", "0.45", "A", "2", "1", "c", "27", "27", "91", "91", });
            dt.Rows.Add(new object[] { "True", "68.02", "0.07", "B", "2", "1", "c", "28", "28", "92", "92", });
            dt.Rows.Add(new object[] { "True", "16.72", "0.11", "A", "1", "2", "c", "29", "29", "93", "93", });
            dt.Rows.Add(new object[] { "True", "16.29", "0.3", "B", "1", "2", "c", "30", "30", "94", "94", });
            dt.Rows.Add(new object[] { "True", "40.24", "0.01", "A", "2", "2", "c", "31", "31", "95", "95", });
            dt.Rows.Add(new object[] { "True", "54.85", "0.25", "B", "2", "2", "c", "32", "32", "96", "96", });
            dt.Rows.Add(new object[] { "True", "12.09", "0.49", "A", "1", "1", "x", "1", "33", "97", "97", });
            dt.Rows.Add(new object[] { "True", "76.15", "0.15", "B", "1", "1", "x", "2", "34", "98", "98", });
            dt.Rows.Add(new object[] { "True", "8.27", "0.8", "A", "2", "1", "x", "3", "35", "99", "99", });
            dt.Rows.Add(new object[] { "True", "31.36", "0.61", "B", "2", "1", "x", "4", "36", "100", "100", });
            dt.Rows.Add(new object[] { "True", "83.33", "0.46", "A", "1", "2", "x", "5", "37", "101", "101", });
            dt.Rows.Add(new object[] { "True", "66.23", "0.61", "B", "1", "2", "x", "6", "38", "102", "102", });
            dt.Rows.Add(new object[] { "True", "64.04", "0.28", "A", "2", "2", "x", "7", "39", "103", "103", });
            dt.Rows.Add(new object[] { "True", "2.85", "0.83", "B", "2", "2", "x", "8", "40", "104", "104", });
            dt.Rows.Add(new object[] { "True", "87.43", "0.18", "A", "1", "1", "c", "9", "41", "105", "105", });
            dt.Rows.Add(new object[] { "True", "1.12", "0.38", "B", "1", "1", "c", "10", "42", "106", "106", });
            dt.Rows.Add(new object[] { "True", "19.15", "0.58", "A", "2", "1", "c", "11", "43", "107", "107", });
            dt.Rows.Add(new object[] { "True", "64.88", "0.96", "B", "2", "1", "c", "12", "44", "108", "108", });
            dt.Rows.Add(new object[] { "True", "63.22", "0.7", "A", "1", "2", "c", "13", "45", "109", "109", });
            dt.Rows.Add(new object[] { "True", "66.27", "0.31", "B", "1", "2", "c", "14", "46", "110", "110", });
            dt.Rows.Add(new object[] { "True", "57.14", "0.42", "A", "2", "2", "c", "15", "47", "111", "111", });
            dt.Rows.Add(new object[] { "True", "56.78", "0.72", "B", "2", "2", "c", "16", "48", "112", "112", });
            dt.Rows.Add(new object[] { "True", "93.68", "0.3", "A", "1", "1", "x", "17", "49", "113", "113", });
            dt.Rows.Add(new object[] { "True", "71.65", "0.67", "B", "1", "1", "x", "18", "50", "114", "114", });
            dt.Rows.Add(new object[] { "True", "56.98", "0.98", "A", "2", "1", "x", "19", "51", "115", "115", });
            dt.Rows.Add(new object[] { "True", "94.02", "0.8", "B", "2", "1", "x", "20", "52", "116", "116", });
            dt.Rows.Add(new object[] { "True", "78.33", "0.51", "A", "1", "2", "x", "21", "53", "117", "117", });
            dt.Rows.Add(new object[] { "True", "77.34", "0.14", "B", "1", "2", "x", "22", "54", "118", "118", });
            dt.Rows.Add(new object[] { "True", "89.01", "0.86", "A", "2", "2", "x", "23", "55", "119", "119", });
            dt.Rows.Add(new object[] { "True", "79.48", "0.67", "B", "2", "2", "x", "24", "56", "120", "120", });
            dt.Rows.Add(new object[] { "True", "50.12", "0.32", "A", "1", "1", "c", "25", "57", "121", "121", });
            dt.Rows.Add(new object[] { "True", "7.54", "0.69", "B", "1", "1", "c", "26", "58", "122", "122", });
            dt.Rows.Add(new object[] { "True", "78.82", "0.45", "A", "2", "1", "c", "27", "59", "123", "123", });
            dt.Rows.Add(new object[] { "True", "43.8", "0.07", "B", "2", "1", "c", "28", "60", "124", "124", });
            dt.Rows.Add(new object[] { "True", "96.22", "0.11", "A", "1", "2", "c", "29", "61", "125", "125", });
            dt.Rows.Add(new object[] { "True", "81.91", "0.3", "B", "1", "2", "c", "30", "62", "126", "126", });
            dt.Rows.Add(new object[] { "True", "49.38", "0.01", "A", "2", "2", "c", "31", "63", "127", "127", });
            dt.Rows.Add(new object[] { "True", "82.35", "0.25", "B", "2", "2", "c", "32", "64", "128", "128", });
            dt.Rows.Add(new object[] { "True", "46.71", "0.49", "A", "1", "1", "x", "1", "1", "129", "129", });
            dt.Rows.Add(new object[] { "True", "97.85", "0.15", "B", "1", "1", "x", "2", "2", "130", "130", });
            dt.Rows.Add(new object[] { "True", "29.48", "0.8", "A", "2", "1", "x", "3", "3", "131", "131", });
            dt.Rows.Add(new object[] { "True", "66.08", "0.61", "B", "2", "1", "x", "4", "4", "132", "132", });
            dt.Rows.Add(new object[] { "True", "73.26", "0.46", "A", "1", "2", "x", "5", "5", "133", "133", });
            dt.Rows.Add(new object[] { "True", "67.75", "0.61", "B", "1", "2", "x", "6", "6", "134", "134", });
            dt.Rows.Add(new object[] { "True", "84.77", "0.28", "A", "2", "2", "x", "7", "7", "135", "135", });
            dt.Rows.Add(new object[] { "True", "17.21", "0.83", "B", "2", "2", "x", "8", "8", "136", "136", });
            dt.Rows.Add(new object[] { "True", "45.66", "0.18", "A", "1", "1", "c", "9", "9", "137", "137", });
            dt.Rows.Add(new object[] { "True", "52.44", "0.38", "B", "1", "1", "c", "10", "10", "138", "138", });
            dt.Rows.Add(new object[] { "True", "73.33", "0.58", "A", "2", "1", "c", "11", "11", "139", "139", });
            dt.Rows.Add(new object[] { "True", "33.76", "0.96", "B", "2", "1", "c", "12", "12", "140", "140", });
            dt.Rows.Add(new object[] { "True", "14.16", "0.7", "A", "1", "2", "c", "13", "13", "141", "141", });
            dt.Rows.Add(new object[] { "True", "49.47", "0.31", "B", "1", "2", "c", "14", "14", "142", "142", });
            dt.Rows.Add(new object[] { "True", "24.65", "0.42", "A", "2", "2", "c", "15", "15", "143", "143", });
            dt.Rows.Add(new object[] { "True", "20.77", "0.72", "B", "2", "2", "c", "16", "16", "144", "144", });
            dt.Rows.Add(new object[] { "True", "67.49", "0.3", "A", "1", "1", "x", "17", "17", "145", "145", });
            dt.Rows.Add(new object[] { "True", "53.21", "0.67", "B", "1", "1", "x", "18", "18", "146", "146", });
            dt.Rows.Add(new object[] { "True", "5.29", "0.98", "A", "2", "1", "x", "19", "19", "147", "147", });
            dt.Rows.Add(new object[] { "True", "13.89", "0.8", "B", "2", "1", "x", "20", "20", "148", "148", });
            dt.Rows.Add(new object[] { "True", "91.36", "0.51", "A", "1", "2", "x", "21", "21", "149", "149", });
            dt.Rows.Add(new object[] { "True", "72.02", "0.14", "B", "1", "2", "x", "22", "22", "150", "150", });
            dt.Rows.Add(new object[] { "True", "86.17", "0.86", "A", "2", "2", "x", "23", "23", "151", "151", });
            dt.Rows.Add(new object[] { "True", "82.41", "0.67", "B", "2", "2", "x", "24", "24", "152", "152", });
            dt.Rows.Add(new object[] { "True", "9.38", "0.32", "A", "1", "1", "c", "25", "25", "153", "153", });
            dt.Rows.Add(new object[] { "True", "85.66", "0.69", "B", "1", "1", "c", "26", "26", "154", "154", });
            dt.Rows.Add(new object[] { "True", "25.79", "0.45", "A", "2", "1", "c", "27", "27", "155", "155", });
            dt.Rows.Add(new object[] { "True", "1.8", "0.07", "B", "2", "1", "c", "28", "28", "156", "156", });
            dt.Rows.Add(new object[] { "True", "44.71", "0.11", "A", "1", "2", "c", "29", "29", "157", "157", });
            dt.Rows.Add(new object[] { "True", "87.02", "0.3", "B", "1", "2", "c", "30", "30", "158", "158", });
            dt.Rows.Add(new object[] { "True", "95.29", "0.01", "A", "2", "2", "c", "31", "31", "159", "159", });
            dt.Rows.Add(new object[] { "True", "1.19", "0.25", "B", "2", "2", "c", "32", "32", "160", "160", });
            dt.Rows.Add(new object[] { "True", "77.35", "0.49", "A", "1", "1", "x", "1", "33", "161", "161", });
            dt.Rows.Add(new object[] { "True", "64.58", "0.15", "B", "1", "1", "x", "2", "34", "162", "162", });
            dt.Rows.Add(new object[] { "True", "63.05", "0.8", "A", "2", "1", "x", "3", "35", "163", "163", });
            dt.Rows.Add(new object[] { "True", "96.1", "0.61", "B", "2", "1", "x", "4", "36", "164", "164", });
            dt.Rows.Add(new object[] { "True", "95.2", "0.46", "A", "1", "2", "x", "5", "37", "165", "165", });
            dt.Rows.Add(new object[] { "True", "90.79", "0.61", "B", "1", "2", "x", "6", "38", "166", "166", });
            dt.Rows.Add(new object[] { "True", "30.68", "0.28", "A", "2", "2", "x", "7", "39", "167", "167", });
            dt.Rows.Add(new object[] { "True", "99.79", "0.83", "B", "2", "2", "x", "8", "40", "168", "168", });
            dt.Rows.Add(new object[] { "True", "49.15", "0.18", "A", "1", "1", "c", "9", "41", "169", "169", });
            dt.Rows.Add(new object[] { "True", "75.26", "0.38", "B", "1", "1", "c", "10", "42", "170", "170", });
            dt.Rows.Add(new object[] { "True", "74.43", "0.58", "A", "2", "1", "c", "11", "43", "171", "171", });
            dt.Rows.Add(new object[] { "True", "92.2", "0.96", "B", "2", "1", "c", "12", "44", "172", "172", });
            dt.Rows.Add(new object[] { "True", "63.49", "0.7", "A", "1", "2", "c", "13", "45", "173", "173", });
            dt.Rows.Add(new object[] { "True", "28.16", "0.31", "B", "1", "2", "c", "14", "46", "174", "174", });
            dt.Rows.Add(new object[] { "True", "73.8", "0.42", "A", "2", "2", "c", "15", "47", "175", "175", });
            dt.Rows.Add(new object[] { "True", "74.79", "0.72", "B", "2", "2", "c", "16", "48", "176", "176", });
            dt.Rows.Add(new object[] { "True", "74.45", "0.3", "A", "1", "1", "x", "17", "49", "177", "177", });
            dt.Rows.Add(new object[] { "True", "74.32", "0.67", "B", "1", "1", "x", "18", "50", "178", "178", });
            dt.Rows.Add(new object[] { "True", "67.78", "0.98", "A", "2", "1", "x", "19", "51", "179", "179", });
            dt.Rows.Add(new object[] { "True", "94.99", "0.8", "B", "2", "1", "x", "20", "52", "180", "180", });
            dt.Rows.Add(new object[] { "True", "88.9", "0.51", "A", "1", "2", "x", "21", "53", "181", "181", });
            dt.Rows.Add(new object[] { "True", "78.01", "0.14", "B", "1", "2", "x", "22", "54", "182", "182", });
            dt.Rows.Add(new object[] { "True", "66.72", "0.86", "A", "2", "2", "x", "23", "55", "183", "183", });
            dt.Rows.Add(new object[] { "True", "21.09", "0.67", "B", "2", "2", "x", "24", "56", "184", "184", });
            dt.Rows.Add(new object[] { "True", "66.05", "0.32", "A", "1", "1", "c", "25", "57", "185", "185", });
            dt.Rows.Add(new object[] { "True", "88.38", "0.69", "B", "1", "1", "c", "26", "58", "186", "186", });
            dt.Rows.Add(new object[] { "True", "1.68", "0.45", "A", "2", "1", "c", "27", "59", "187", "187", });
            dt.Rows.Add(new object[] { "True", "85.63", "0.07", "B", "2", "1", "c", "28", "60", "188", "188", });
            dt.Rows.Add(new object[] { "True", "15.34", "0.11", "A", "1", "2", "c", "29", "61", "189", "189", });
            dt.Rows.Add(new object[] { "True", "17.62", "0.3", "B", "1", "2", "c", "30", "62", "190", "190", });
            dt.Rows.Add(new object[] { "True", "29.4", "0.01", "A", "2", "2", "c", "31", "63", "191", "191", });
            dt.Rows.Add(new object[] { "True", "76.93", "0.25", "B", "2", "2", "c", "32", "64", "192", "192", });
            dt.Rows.Add(new object[] { "True", "21.68", "0.49", "A", "1", "1", "x", "1", "1", "1", "193", });
            dt.Rows.Add(new object[] { "True", "43.29", "0.15", "B", "1", "1", "x", "2", "2", "2", "194", });
            dt.Rows.Add(new object[] { "True", "30.43", "0.8", "A", "2", "1", "x", "3", "3", "3", "195", });
            dt.Rows.Add(new object[] { "True", "63.06", "0.61", "B", "2", "1", "x", "4", "4", "4", "196", });
            dt.Rows.Add(new object[] { "True", "86.27", "0.46", "A", "1", "2", "x", "5", "5", "5", "197", });
            dt.Rows.Add(new object[] { "True", "21.47", "0.61", "B", "1", "2", "x", "6", "6", "6", "198", });
            dt.Rows.Add(new object[] { "True", "98.35", "0.28", "A", "2", "2", "x", "7", "7", "7", "199", });
            dt.Rows.Add(new object[] { "True", "10.4", "0.83", "B", "2", "2", "x", "8", "8", "8", "200", });
            dt.Rows.Add(new object[] { "True", "55.77", "0.18", "A", "1", "1", "c", "9", "9", "9", "201", });
            dt.Rows.Add(new object[] { "True", "39.34", "0.38", "B", "1", "1", "c", "10", "10", "10", "202", });
            dt.Rows.Add(new object[] { "True", "4.68", "0.58", "A", "2", "1", "c", "11", "11", "11", "203", });
            dt.Rows.Add(new object[] { "True", "41.9", "0.96", "B", "2", "1", "c", "12", "12", "12", "204", });
            dt.Rows.Add(new object[] { "True", "39.46", "0.7", "A", "1", "2", "c", "13", "13", "13", "205", });
            dt.Rows.Add(new object[] { "True", "39.4", "0.31", "B", "1", "2", "c", "14", "14", "14", "206", });
            dt.Rows.Add(new object[] { "True", "51.6", "0.42", "A", "2", "2", "c", "15", "15", "15", "207", });
            dt.Rows.Add(new object[] { "True", "46.39", "0.72", "B", "2", "2", "c", "16", "16", "16", "208", });
            dt.Rows.Add(new object[] { "True", "31.67", "0.3", "A", "1", "1", "x", "17", "17", "17", "209", });
            dt.Rows.Add(new object[] { "True", "87.96", "0.67", "B", "1", "1", "x", "18", "18", "18", "210", });
            dt.Rows.Add(new object[] { "True", "21.55", "0.98", "A", "2", "1", "x", "19", "19", "19", "211", });
            dt.Rows.Add(new object[] { "True", "33.4", "0.8", "B", "2", "1", "x", "20", "20", "20", "212", });
            dt.Rows.Add(new object[] { "True", "25.05", "0.51", "A", "1", "2", "x", "21", "21", "21", "213", });
            dt.Rows.Add(new object[] { "True", "60.14", "0.14", "B", "1", "2", "x", "22", "22", "22", "214", });
            dt.Rows.Add(new object[] { "True", "2.14", "0.86", "A", "2", "2", "x", "23", "23", "23", "215", });
            dt.Rows.Add(new object[] { "True", "5.57", "0.67", "B", "2", "2", "x", "24", "24", "24", "216", });
            dt.Rows.Add(new object[] { "True", "13.37", "0.32", "A", "1", "1", "c", "25", "25", "25", "217", });
            dt.Rows.Add(new object[] { "True", "56.67", "0.69", "B", "1", "1", "c", "26", "26", "26", "218", });
            dt.Rows.Add(new object[] { "True", "64.9", "0.45", "A", "2", "1", "c", "27", "27", "27", "219", });
            dt.Rows.Add(new object[] { "True", "72.75", "0.07", "B", "2", "1", "c", "28", "28", "28", "220", });
            dt.Rows.Add(new object[] { "True", "17.72", "0.11", "A", "1", "2", "c", "29", "29", "29", "221", });
            dt.Rows.Add(new object[] { "True", "0.31", "0.3", "B", "1", "2", "c", "30", "30", "30", "222", });
            dt.Rows.Add(new object[] { "True", "3.66", "0.01", "A", "2", "2", "c", "31", "31", "31", "223", });
            dt.Rows.Add(new object[] { "True", "20.63", "0.25", "B", "2", "2", "c", "32", "32", "32", "224", });
            dt.Rows.Add(new object[] { "True", "83.56", "0.49", "A", "1", "1", "x", "1", "33", "33", "225", });
            dt.Rows.Add(new object[] { "True", "44.13", "0.15", "B", "1", "1", "x", "2", "34", "34", "226", });
            dt.Rows.Add(new object[] { "True", "69.14", "0.8", "A", "2", "1", "x", "3", "35", "35", "227", });
            dt.Rows.Add(new object[] { "True", "20.74", "0.61", "B", "2", "1", "x", "4", "36", "36", "228", });
            dt.Rows.Add(new object[] { "True", "32.01", "0.46", "A", "1", "2", "x", "5", "37", "37", "229", });
            dt.Rows.Add(new object[] { "True", "78.46", "0.61", "B", "1", "2", "x", "6", "38", "38", "230", });
            dt.Rows.Add(new object[] { "True", "26.61", "0.28", "A", "2", "2", "x", "7", "39", "39", "231", });
            dt.Rows.Add(new object[] { "True", "81.76", "0.83", "B", "2", "2", "x", "8", "40", "40", "232", });
            dt.Rows.Add(new object[] { "True", "10.97", "0.18", "A", "1", "1", "c", "9", "41", "41", "233", });
            dt.Rows.Add(new object[] { "True", "95.81", "0.38", "B", "1", "1", "c", "10", "42", "42", "234", });
            dt.Rows.Add(new object[] { "True", "72.67", "0.58", "A", "2", "1", "c", "11", "43", "43", "235", });
            dt.Rows.Add(new object[] { "True", "73.78", "0.96", "B", "2", "1", "c", "12", "44", "44", "236", });
            dt.Rows.Add(new object[] { "True", "20.09", "0.7", "A", "1", "2", "c", "13", "45", "45", "237", });
            dt.Rows.Add(new object[] { "True", "93.63", "0.31", "B", "1", "2", "c", "14", "46", "46", "238", });
            dt.Rows.Add(new object[] { "True", "79.56", "0.42", "A", "2", "2", "c", "15", "47", "47", "239", });
            dt.Rows.Add(new object[] { "True", "52.71", "0.72", "B", "2", "2", "c", "16", "48", "48", "240", });
            dt.Rows.Add(new object[] { "True", "84.17", "0.3", "A", "1", "1", "x", "17", "49", "49", "241", });
            dt.Rows.Add(new object[] { "True", "66.35", "0.67", "B", "1", "1", "x", "18", "50", "50", "242", });
            dt.Rows.Add(new object[] { "True", "66.99", "0.98", "A", "2", "1", "x", "19", "51", "51", "243", });
            dt.Rows.Add(new object[] { "True", "8.05", "0.8", "B", "2", "1", "x", "20", "52", "52", "244", });
            dt.Rows.Add(new object[] { "True", "26.62", "0.51", "A", "1", "2", "x", "21", "53", "53", "245", });
            dt.Rows.Add(new object[] { "True", "37.22", "0.14", "B", "1", "2", "x", "22", "54", "54", "246", });
            dt.Rows.Add(new object[] { "True", "47.29", "0.86", "A", "2", "2", "x", "23", "55", "55", "247", });
            dt.Rows.Add(new object[] { "True", "64.25", "0.67", "B", "2", "2", "x", "24", "56", "56", "248", });
            dt.Rows.Add(new object[] { "True", "74.13", "0.32", "A", "1", "1", "c", "25", "57", "57", "249", });
            dt.Rows.Add(new object[] { "True", "92.27", "0.69", "B", "1", "1", "c", "26", "58", "58", "250", });
            dt.Rows.Add(new object[] { "True", "13.16", "0.45", "A", "2", "1", "c", "27", "59", "59", "251", });
            dt.Rows.Add(new object[] { "True", "33.63", "0.07", "B", "2", "1", "c", "28", "60", "60", "252", });
            dt.Rows.Add(new object[] { "True", "96.8", "0.11", "A", "1", "2", "c", "29", "61", "61", "253", });
            dt.Rows.Add(new object[] { "True", "59.39", "0.3", "B", "1", "2", "c", "30", "62", "62", "254", });
            dt.Rows.Add(new object[] { "True", "97.46", "0.01", "A", "2", "2", "c", "31", "63", "63", "255", });
            dt.Rows.Add(new object[] { "True", "53.82", "0.25", "B", "2", "2", "c", "32", "64", "64", "256", });
            dt.Rows.Add(new object[] { "True", "37.21", "0.49", "A", "1", "1", "x", "1", "1", "65", "257", });
            dt.Rows.Add(new object[] { "True", "24.67", "0.15", "B", "1", "1", "x", "2", "2", "66", "258", });
            dt.Rows.Add(new object[] { "True", "49.78", "0.8", "A", "2", "1", "x", "3", "3", "67", "259", });
            dt.Rows.Add(new object[] { "True", "97.61", "0.61", "B", "2", "1", "x", "4", "4", "68", "260", });
            dt.Rows.Add(new object[] { "True", "31.01", "0.46", "A", "1", "2", "x", "5", "5", "69", "261", });
            dt.Rows.Add(new object[] { "True", "59.28", "0.61", "B", "1", "2", "x", "6", "6", "70", "262", });
            dt.Rows.Add(new object[] { "True", "80.59", "0.28", "A", "2", "2", "x", "7", "7", "71", "263", });
            dt.Rows.Add(new object[] { "True", "44", "0.83", "B", "2", "2", "x", "8", "8", "72", "264", });
            dt.Rows.Add(new object[] { "True", "80.3", "0.18", "A", "1", "1", "c", "9", "9", "73", "265", });
            dt.Rows.Add(new object[] { "True", "84.11", "0.38", "B", "1", "1", "c", "10", "10", "74", "266", });
            dt.Rows.Add(new object[] { "True", "9", "0.58", "A", "2", "1", "c", "11", "11", "75", "267", });
            dt.Rows.Add(new object[] { "True", "53.44", "0.96", "B", "2", "1", "c", "12", "12", "76", "268", });
            dt.Rows.Add(new object[] { "True", "40.72", "0.7", "A", "1", "2", "c", "13", "13", "77", "269", });
            dt.Rows.Add(new object[] { "True", "0.69", "0.31", "B", "1", "2", "c", "14", "14", "78", "270", });
            dt.Rows.Add(new object[] { "True", "96.86", "0.42", "A", "2", "2", "c", "15", "15", "79", "271", });
            dt.Rows.Add(new object[] { "True", "92.9", "0.72", "B", "2", "2", "c", "16", "16", "80", "272", });
            dt.Rows.Add(new object[] { "True", "35.08", "0.3", "A", "1", "1", "x", "17", "17", "81", "273", });
            dt.Rows.Add(new object[] { "True", "86.52", "0.67", "B", "1", "1", "x", "18", "18", "82", "274", });
            dt.Rows.Add(new object[] { "True", "97.03", "0.98", "A", "2", "1", "x", "19", "19", "83", "275", });
            dt.Rows.Add(new object[] { "True", "54.63", "0.8", "B", "2", "1", "x", "20", "20", "84", "276", });
            dt.Rows.Add(new object[] { "True", "20.83", "0.51", "A", "1", "2", "x", "21", "21", "85", "277", });
            dt.Rows.Add(new object[] { "True", "3.07", "0.14", "B", "1", "2", "x", "22", "22", "86", "278", });
            dt.Rows.Add(new object[] { "True", "1.73", "0.86", "A", "2", "2", "x", "23", "23", "87", "279", });
            dt.Rows.Add(new object[] { "True", "20.98", "0.67", "B", "2", "2", "x", "24", "24", "88", "280", });
            dt.Rows.Add(new object[] { "True", "2.55", "0.32", "A", "1", "1", "c", "25", "25", "89", "281", });
            dt.Rows.Add(new object[] { "True", "13.72", "0.69", "B", "1", "1", "c", "26", "26", "90", "282", });
            dt.Rows.Add(new object[] { "True", "91.78", "0.45", "A", "2", "1", "c", "27", "27", "91", "283", });
            dt.Rows.Add(new object[] { "True", "73.83", "0.07", "B", "2", "1", "c", "28", "28", "92", "284", });
            dt.Rows.Add(new object[] { "True", "97.1", "0.11", "A", "1", "2", "c", "29", "29", "93", "285", });
            dt.Rows.Add(new object[] { "True", "50.32", "0.3", "B", "1", "2", "c", "30", "30", "94", "286", });
            dt.Rows.Add(new object[] { "True", "71.52", "0.01", "A", "2", "2", "c", "31", "31", "95", "287", });
            dt.Rows.Add(new object[] { "True", "99.18", "0.25", "B", "2", "2", "c", "32", "32", "96", "288", });
            dt.Rows.Add(new object[] { "True", "74.47", "0.49", "A", "1", "1", "x", "1", "33", "97", "289", });
            dt.Rows.Add(new object[] { "True", "21.29", "0.15", "B", "1", "1", "x", "2", "34", "98", "290", });
            dt.Rows.Add(new object[] { "True", "0.83", "0.8", "A", "2", "1", "x", "3", "35", "99", "291", });
            dt.Rows.Add(new object[] { "True", "78.69", "0.61", "B", "2", "1", "x", "4", "36", "100", "292", });
            dt.Rows.Add(new object[] { "True", "59.21", "0.46", "A", "1", "2", "x", "5", "37", "101", "293", });
            dt.Rows.Add(new object[] { "True", "62.25", "0.61", "B", "1", "2", "x", "6", "38", "102", "294", });
            dt.Rows.Add(new object[] { "True", "44.78", "0.28", "A", "2", "2", "x", "7", "39", "103", "295", });
            dt.Rows.Add(new object[] { "True", "53.87", "0.83", "B", "2", "2", "x", "8", "40", "104", "296", });
            dt.Rows.Add(new object[] { "True", "97.61", "0.18", "A", "1", "1", "c", "9", "41", "105", "297", });
            dt.Rows.Add(new object[] { "True", "99.31", "0.38", "B", "1", "1", "c", "10", "42", "106", "298", });
            dt.Rows.Add(new object[] { "True", "48.74", "0.58", "A", "2", "1", "c", "11", "43", "107", "299", });
            dt.Rows.Add(new object[] { "True", "53.75", "0.96", "B", "2", "1", "c", "12", "44", "108", "300", });
            dt.Rows.Add(new object[] { "True", "13.61", "0.7", "A", "1", "2", "c", "13", "45", "109", "301", });
            dt.Rows.Add(new object[] { "True", "60.41", "0.31", "B", "1", "2", "c", "14", "46", "110", "302", });
            dt.Rows.Add(new object[] { "True", "64.41", "0.42", "A", "2", "2", "c", "15", "47", "111", "303", });
            dt.Rows.Add(new object[] { "True", "3.68", "0.72", "B", "2", "2", "c", "16", "48", "112", "304", });
            dt.Rows.Add(new object[] { "True", "14.79", "0.3", "A", "1", "1", "x", "17", "49", "113", "305", });
            dt.Rows.Add(new object[] { "True", "33.87", "0.67", "B", "1", "1", "x", "18", "50", "114", "306", });
            dt.Rows.Add(new object[] { "True", "70.6", "0.98", "A", "2", "1", "x", "19", "51", "115", "307", });
            dt.Rows.Add(new object[] { "True", "61.31", "0.8", "B", "2", "1", "x", "20", "52", "116", "308", });
            dt.Rows.Add(new object[] { "True", "27.12", "0.51", "A", "1", "2", "x", "21", "53", "117", "309", });
            dt.Rows.Add(new object[] { "True", "5.18", "0.14", "B", "1", "2", "x", "22", "54", "118", "310", });
            dt.Rows.Add(new object[] { "True", "76.18", "0.86", "A", "2", "2", "x", "23", "55", "119", "311", });
            dt.Rows.Add(new object[] { "True", "8.12", "0.67", "B", "2", "2", "x", "24", "56", "120", "312", });
            dt.Rows.Add(new object[] { "True", "71.31", "0.32", "A", "1", "1", "c", "25", "57", "121", "313", });
            dt.Rows.Add(new object[] { "True", "6.29", "0.69", "B", "1", "1", "c", "26", "58", "122", "314", });
            dt.Rows.Add(new object[] { "True", "42.79", "0.45", "A", "2", "1", "c", "27", "59", "123", "315", });
            dt.Rows.Add(new object[] { "True", "30", "0.07", "B", "2", "1", "c", "28", "60", "124", "316", });
            dt.Rows.Add(new object[] { "True", "6.23", "0.11", "A", "1", "2", "c", "29", "61", "125", "317", });
            dt.Rows.Add(new object[] { "True", "27.72", "0.3", "B", "1", "2", "c", "30", "62", "126", "318", });
            dt.Rows.Add(new object[] { "True", "3.87", "0.01", "A", "2", "2", "c", "31", "63", "127", "319", });
            dt.Rows.Add(new object[] { "True", "59.31", "0.25", "B", "2", "2", "c", "32", "64", "128", "320", });
            dt.Rows.Add(new object[] { "True", "49.81", "0.49", "A", "1", "1", "x", "1", "1", "129", "321", });
            dt.Rows.Add(new object[] { "True", "54.73", "0.15", "B", "1", "1", "x", "2", "2", "130", "322", });
            dt.Rows.Add(new object[] { "True", "40.46", "0.8", "A", "2", "1", "x", "3", "3", "131", "323", });
            dt.Rows.Add(new object[] { "True", "90.13", "0.61", "B", "2", "1", "x", "4", "4", "132", "324", });
            dt.Rows.Add(new object[] { "True", "46.77", "0.46", "A", "1", "2", "x", "5", "5", "133", "325", });
            dt.Rows.Add(new object[] { "True", "2.54", "0.61", "B", "1", "2", "x", "6", "6", "134", "326", });
            dt.Rows.Add(new object[] { "True", "72.61", "0.28", "A", "2", "2", "x", "7", "7", "135", "327", });
            dt.Rows.Add(new object[] { "True", "1.6", "0.83", "B", "2", "2", "x", "8", "8", "136", "328", });
            dt.Rows.Add(new object[] { "True", "36.94", "0.18", "A", "1", "1", "c", "9", "9", "137", "329", });
            dt.Rows.Add(new object[] { "True", "22.06", "0.38", "B", "1", "1", "c", "10", "10", "138", "330", });
            dt.Rows.Add(new object[] { "True", "68.49", "0.58", "A", "2", "1", "c", "11", "11", "139", "331", });
            dt.Rows.Add(new object[] { "True", "60.83", "0.96", "B", "2", "1", "c", "12", "12", "140", "332", });
            dt.Rows.Add(new object[] { "True", "48.72", "0.7", "A", "1", "2", "c", "13", "13", "141", "333", });
            dt.Rows.Add(new object[] { "True", "81.14", "0.31", "B", "1", "2", "c", "14", "14", "142", "334", });
            dt.Rows.Add(new object[] { "True", "40.14", "0.42", "A", "2", "2", "c", "15", "15", "143", "335", });
            dt.Rows.Add(new object[] { "True", "11.54", "0.72", "B", "2", "2", "c", "16", "16", "144", "336", });
            dt.Rows.Add(new object[] { "True", "39.46", "0.3", "A", "1", "1", "x", "17", "17", "145", "337", });
            dt.Rows.Add(new object[] { "True", "96.21", "0.67", "B", "1", "1", "x", "18", "18", "146", "338", });
            dt.Rows.Add(new object[] { "True", "40.36", "0.98", "A", "2", "1", "x", "19", "19", "147", "339", });
            dt.Rows.Add(new object[] { "True", "69.75", "0.8", "B", "2", "1", "x", "20", "20", "148", "340", });
            dt.Rows.Add(new object[] { "True", "66.86", "0.51", "A", "1", "2", "x", "21", "21", "149", "341", });
            dt.Rows.Add(new object[] { "True", "70.19", "0.14", "B", "1", "2", "x", "22", "22", "150", "342", });
            dt.Rows.Add(new object[] { "True", "81.52", "0.86", "A", "2", "2", "x", "23", "23", "151", "343", });
            dt.Rows.Add(new object[] { "True", "26.32", "0.67", "B", "2", "2", "x", "24", "24", "152", "344", });
            dt.Rows.Add(new object[] { "True", "45.87", "0.32", "A", "1", "1", "c", "25", "25", "153", "345", });
            dt.Rows.Add(new object[] { "True", "38.63", "0.69", "B", "1", "1", "c", "26", "26", "154", "346", });
            dt.Rows.Add(new object[] { "True", "25.53", "0.45", "A", "2", "1", "c", "27", "27", "155", "347", });
            dt.Rows.Add(new object[] { "True", "34.37", "0.07", "B", "2", "1", "c", "28", "28", "156", "348", });
            dt.Rows.Add(new object[] { "True", "98.86", "0.11", "A", "1", "2", "c", "29", "29", "157", "349", });
            dt.Rows.Add(new object[] { "True", "13.01", "0.3", "B", "1", "2", "c", "30", "30", "158", "350", });
            dt.Rows.Add(new object[] { "True", "73.1", "0.01", "A", "2", "2", "c", "31", "31", "159", "351", });
            dt.Rows.Add(new object[] { "True", "51.77", "0.25", "B", "2", "2", "c", "32", "32", "160", "352", });
            dt.Rows.Add(new object[] { "True", "29.94", "0.49", "A", "1", "1", "x", "1", "33", "161", "353", });
            dt.Rows.Add(new object[] { "True", "96.21", "0.15", "B", "1", "1", "x", "2", "34", "162", "354", });
            dt.Rows.Add(new object[] { "True", "37.16", "0.8", "A", "2", "1", "x", "3", "35", "163", "355", });
            dt.Rows.Add(new object[] { "True", "17.53", "0.61", "B", "2", "1", "x", "4", "36", "164", "356", });
            dt.Rows.Add(new object[] { "True", "22.54", "0.46", "A", "1", "2", "x", "5", "37", "165", "357", });
            dt.Rows.Add(new object[] { "True", "44.72", "0.61", "B", "1", "2", "x", "6", "38", "166", "358", });
            dt.Rows.Add(new object[] { "True", "39.84", "0.28", "A", "2", "2", "x", "7", "39", "167", "359", });
            dt.Rows.Add(new object[] { "True", "70.96", "0.83", "B", "2", "2", "x", "8", "40", "168", "360", });
            dt.Rows.Add(new object[] { "True", "17.24", "0.18", "A", "1", "1", "c", "9", "41", "169", "361", });
            dt.Rows.Add(new object[] { "True", "75.57", "0.38", "B", "1", "1", "c", "10", "42", "170", "362", });
            dt.Rows.Add(new object[] { "True", "2.81", "0.58", "A", "2", "1", "c", "11", "43", "171", "363", });
            dt.Rows.Add(new object[] { "True", "16.51", "0.96", "B", "2", "1", "c", "12", "44", "172", "364", });
            dt.Rows.Add(new object[] { "True", "17.21", "0.7", "A", "1", "2", "c", "13", "45", "173", "365", });
            dt.Rows.Add(new object[] { "True", "42.99", "0.31", "B", "1", "2", "c", "14", "46", "174", "366", });
            dt.Rows.Add(new object[] { "True", "57.75", "0.42", "A", "2", "2", "c", "15", "47", "175", "367", });
            dt.Rows.Add(new object[] { "True", "33", "0.72", "B", "2", "2", "c", "16", "48", "176", "368", });
            dt.Rows.Add(new object[] { "True", "29.4", "0.3", "A", "1", "1", "x", "17", "49", "177", "369", });
            dt.Rows.Add(new object[] { "True", "72.46", "0.67", "B", "1", "1", "x", "18", "50", "178", "370", });
            dt.Rows.Add(new object[] { "True", "89.9", "0.98", "A", "2", "1", "x", "19", "51", "179", "371", });
            dt.Rows.Add(new object[] { "True", "96.04", "0.8", "B", "2", "1", "x", "20", "52", "180", "372", });
            dt.Rows.Add(new object[] { "True", "65.29", "0.51", "A", "1", "2", "x", "21", "53", "181", "373", });
            dt.Rows.Add(new object[] { "True", "57.31", "0.14", "B", "1", "2", "x", "22", "54", "182", "374", });
            dt.Rows.Add(new object[] { "True", "89.02", "0.86", "A", "2", "2", "x", "23", "55", "183", "375", });
            dt.Rows.Add(new object[] { "True", "22.25", "0.67", "B", "2", "2", "x", "24", "56", "184", "376", });
            dt.Rows.Add(new object[] { "True", "3.95", "0.32", "A", "1", "1", "c", "25", "57", "185", "377", });
            dt.Rows.Add(new object[] { "True", "63.87", "0.69", "B", "1", "1", "c", "26", "58", "186", "378", });
            dt.Rows.Add(new object[] { "True", "25.98", "0.45", "A", "2", "1", "c", "27", "59", "187", "379", });
            dt.Rows.Add(new object[] { "True", "14.37", "0.07", "B", "2", "1", "c", "28", "60", "188", "380", });
            dt.Rows.Add(new object[] { "True", "14.48", "0.11", "A", "1", "2", "c", "29", "61", "189", "381", });
            dt.Rows.Add(new object[] { "True", "92.14", "0.3", "B", "1", "2", "c", "30", "62", "190", "382", });
            dt.Rows.Add(new object[] { "True", "35.6", "0.01", "A", "2", "2", "c", "31", "63", "191", "383", });
            dt.Rows.Add(new object[] { "True", "8.89", "0.25", "B", "2", "2", "c", "32", "64", "192", "384", });
            dt.Rows.Add(new object[] { "True", "66.47", "0.49", "A", "1", "1", "x", "1", "1", "1", "385", });
            dt.Rows.Add(new object[] { "True", "46.25", "0.15", "B", "1", "1", "x", "2", "2", "2", "386", });
            dt.Rows.Add(new object[] { "True", "19.94", "0.8", "A", "2", "1", "x", "3", "3", "3", "387", });
            dt.Rows.Add(new object[] { "True", "73.65", "0.61", "B", "2", "1", "x", "4", "4", "4", "388", });
            dt.Rows.Add(new object[] { "True", "85.53", "0.46", "A", "1", "2", "x", "5", "5", "5", "389", });
            dt.Rows.Add(new object[] { "True", "77.85", "0.61", "B", "1", "2", "x", "6", "6", "6", "390", });
            dt.Rows.Add(new object[] { "True", "31.48", "0.28", "A", "2", "2", "x", "7", "7", "7", "391", });
            dt.Rows.Add(new object[] { "True", "86.41", "0.83", "B", "2", "2", "x", "8", "8", "8", "392", });
            dt.Rows.Add(new object[] { "True", "64.75", "0.18", "A", "1", "1", "c", "9", "9", "9", "393", });
            dt.Rows.Add(new object[] { "True", "0.01", "0.38", "B", "1", "1", "c", "10", "10", "10", "394", });
            dt.Rows.Add(new object[] { "True", "69.45", "0.58", "A", "2", "1", "c", "11", "11", "11", "395", });
            dt.Rows.Add(new object[] { "True", "53.69", "0.96", "B", "2", "1", "c", "12", "12", "12", "396", });
            dt.Rows.Add(new object[] { "True", "2.63", "0.7", "A", "1", "2", "c", "13", "13", "13", "397", });
            dt.Rows.Add(new object[] { "True", "88.66", "0.31", "B", "1", "2", "c", "14", "14", "14", "398", });
            dt.Rows.Add(new object[] { "True", "8.54", "0.42", "A", "2", "2", "c", "15", "15", "15", "399", });
            dt.Rows.Add(new object[] { "True", "21.96", "0.72", "B", "2", "2", "c", "16", "16", "16", "400", });
            dt.Rows.Add(new object[] { "True", "0.01", "0.3", "A", "1", "1", "x", "17", "17", "17", "401", });
            dt.Rows.Add(new object[] { "True", "62.38", "0.67", "B", "1", "1", "x", "18", "18", "18", "402", });
            dt.Rows.Add(new object[] { "True", "59.72", "0.98", "A", "2", "1", "x", "19", "19", "19", "403", });
            dt.Rows.Add(new object[] { "True", "44.54", "0.8", "B", "2", "1", "x", "20", "20", "20", "404", });
            dt.Rows.Add(new object[] { "True", "81.59", "0.51", "A", "1", "2", "x", "21", "21", "21", "405", });
            dt.Rows.Add(new object[] { "True", "72.08", "0.14", "B", "1", "2", "x", "22", "22", "22", "406", });
            dt.Rows.Add(new object[] { "True", "84.36", "0.86", "A", "2", "2", "x", "23", "23", "23", "407", });
            dt.Rows.Add(new object[] { "True", "5.85", "0.67", "B", "2", "2", "x", "24", "24", "24", "408", });
            dt.Rows.Add(new object[] { "True", "29.7", "0.32", "A", "1", "1", "c", "25", "25", "25", "409", });
            dt.Rows.Add(new object[] { "True", "33.11", "0.69", "B", "1", "1", "c", "26", "26", "26", "410", });
            dt.Rows.Add(new object[] { "True", "91.4", "0.45", "A", "2", "1", "c", "27", "27", "27", "411", });
            dt.Rows.Add(new object[] { "True", "51.69", "0.07", "B", "2", "1", "c", "28", "28", "28", "412", });
            dt.Rows.Add(new object[] { "True", "22.86", "0.11", "A", "1", "2", "c", "29", "29", "29", "413", });
            dt.Rows.Add(new object[] { "True", "97.82", "0.3", "B", "1", "2", "c", "30", "30", "30", "414", });
            dt.Rows.Add(new object[] { "True", "30.07", "0.01", "A", "2", "2", "c", "31", "31", "31", "415", });
            dt.Rows.Add(new object[] { "True", "91.55", "0.25", "B", "2", "2", "c", "32", "32", "32", "416", });
            dt.Rows.Add(new object[] { "True", "78.42", "0.49", "A", "1", "1", "x", "1", "33", "33", "417", });
            dt.Rows.Add(new object[] { "True", "74.73", "0.15", "B", "1", "1", "x", "2", "34", "34", "418", });
            dt.Rows.Add(new object[] { "True", "75.35", "0.8", "A", "2", "1", "x", "3", "35", "35", "419", });
            dt.Rows.Add(new object[] { "True", "68.25", "0.61", "B", "2", "1", "x", "4", "36", "36", "420", });
            dt.Rows.Add(new object[] { "True", "43.28", "0.46", "A", "1", "2", "x", "5", "37", "37", "421", });
            dt.Rows.Add(new object[] { "True", "89.84", "0.61", "B", "1", "2", "x", "6", "38", "38", "422", });
            dt.Rows.Add(new object[] { "True", "2.67", "0.28", "A", "2", "2", "x", "7", "39", "39", "423", });
            dt.Rows.Add(new object[] { "True", "25.3", "0.83", "B", "2", "2", "x", "8", "40", "40", "424", });
            dt.Rows.Add(new object[] { "True", "66.67", "0.18", "A", "1", "1", "c", "9", "41", "41", "425", });
            dt.Rows.Add(new object[] { "True", "82.68", "0.38", "B", "1", "1", "c", "10", "42", "42", "426", });
            dt.Rows.Add(new object[] { "True", "60.14", "0.58", "A", "2", "1", "c", "11", "43", "43", "427", });
            dt.Rows.Add(new object[] { "True", "74.39", "0.96", "B", "2", "1", "c", "12", "44", "44", "428", });
            dt.Rows.Add(new object[] { "True", "51.86", "0.7", "A", "1", "2", "c", "13", "45", "45", "429", });
            dt.Rows.Add(new object[] { "True", "42.77", "0.31", "B", "1", "2", "c", "14", "46", "46", "430", });
            dt.Rows.Add(new object[] { "True", "84.46", "0.42", "A", "2", "2", "c", "15", "47", "47", "431", });
            dt.Rows.Add(new object[] { "True", "78.79", "0.72", "B", "2", "2", "c", "16", "48", "48", "432", });
            dt.Rows.Add(new object[] { "True", "55.32", "0.3", "A", "1", "1", "x", "17", "49", "49", "433", });
            dt.Rows.Add(new object[] { "True", "49.65", "0.67", "B", "1", "1", "x", "18", "50", "50", "434", });
            dt.Rows.Add(new object[] { "True", "40.55", "0.98", "A", "2", "1", "x", "19", "51", "51", "435", });
            dt.Rows.Add(new object[] { "True", "8.46", "0.8", "B", "2", "1", "x", "20", "52", "52", "436", });
            dt.Rows.Add(new object[] { "True", "37.67", "0.51", "A", "1", "2", "x", "21", "53", "53", "437", });
            dt.Rows.Add(new object[] { "True", "29.72", "0.14", "B", "1", "2", "x", "22", "54", "54", "438", });
            dt.Rows.Add(new object[] { "True", "98.78", "0.86", "A", "2", "2", "x", "23", "55", "55", "439", });
            dt.Rows.Add(new object[] { "True", "21.4", "0.67", "B", "2", "2", "x", "24", "56", "56", "440", });
            dt.Rows.Add(new object[] { "True", "38.09", "0.32", "A", "1", "1", "c", "25", "57", "57", "441", });
            dt.Rows.Add(new object[] { "True", "12.98", "0.69", "B", "1", "1", "c", "26", "58", "58", "442", });
            dt.Rows.Add(new object[] { "True", "62.92", "0.45", "A", "2", "1", "c", "27", "59", "59", "443", });
            dt.Rows.Add(new object[] { "True", "64.36", "0.07", "B", "2", "1", "c", "28", "60", "60", "444", });
            dt.Rows.Add(new object[] { "True", "57.58", "0.11", "A", "1", "2", "c", "29", "61", "61", "445", });
            dt.Rows.Add(new object[] { "True", "40.12", "0.3", "B", "1", "2", "c", "30", "62", "62", "446", });
            dt.Rows.Add(new object[] { "True", "32.44", "0.01", "A", "2", "2", "c", "31", "63", "63", "447", });
            dt.Rows.Add(new object[] { "True", "37.68", "0.25", "B", "2", "2", "c", "32", "64", "64", "448", });
            dt.Rows.Add(new object[] { "True", "16.04", "0.49", "A", "1", "1", "x", "1", "1", "65", "449", });
            dt.Rows.Add(new object[] { "True", "83.53", "0.15", "B", "1", "1", "x", "2", "2", "66", "450", });
            dt.Rows.Add(new object[] { "True", "92.47", "0.8", "A", "2", "1", "x", "3", "3", "67", "451", });
            dt.Rows.Add(new object[] { "True", "48.78", "0.61", "B", "2", "1", "x", "4", "4", "68", "452", });
            dt.Rows.Add(new object[] { "True", "95.94", "0.46", "A", "1", "2", "x", "5", "5", "69", "453", });
            dt.Rows.Add(new object[] { "True", "50.09", "0.61", "B", "1", "2", "x", "6", "6", "70", "454", });
            dt.Rows.Add(new object[] { "True", "92.18", "0.28", "A", "2", "2", "x", "7", "7", "71", "455", });
            dt.Rows.Add(new object[] { "True", "2.69", "0.83", "B", "2", "2", "x", "8", "8", "72", "456", });
            dt.Rows.Add(new object[] { "True", "41.69", "0.18", "A", "1", "1", "c", "9", "9", "73", "457", });
            dt.Rows.Add(new object[] { "True", "53.6", "0.38", "B", "1", "1", "c", "10", "10", "74", "458", });
            dt.Rows.Add(new object[] { "True", "49.88", "0.58", "A", "2", "1", "c", "11", "11", "75", "459", });
            dt.Rows.Add(new object[] { "True", "55.23", "0.96", "B", "2", "1", "c", "12", "12", "76", "460", });
            dt.Rows.Add(new object[] { "True", "23.55", "0.7", "A", "1", "2", "c", "13", "13", "77", "461", });
            dt.Rows.Add(new object[] { "True", "38.2", "0.31", "B", "1", "2", "c", "14", "14", "78", "462", });
            dt.Rows.Add(new object[] { "True", "28.79", "0.42", "A", "2", "2", "c", "15", "15", "79", "463", });
            dt.Rows.Add(new object[] { "True", "43.26", "0.72", "B", "2", "2", "c", "16", "16", "80", "464", });
            dt.Rows.Add(new object[] { "True", "94.51", "0.3", "A", "1", "1", "x", "17", "17", "81", "465", });
            dt.Rows.Add(new object[] { "True", "6.36", "0.67", "B", "1", "1", "x", "18", "18", "82", "466", });
            dt.Rows.Add(new object[] { "True", "31.57", "0.98", "A", "2", "1", "x", "19", "19", "83", "467", });
            dt.Rows.Add(new object[] { "True", "11.52", "0.8", "B", "2", "1", "x", "20", "20", "84", "468", });
            dt.Rows.Add(new object[] { "True", "71.98", "0.51", "A", "1", "2", "x", "21", "21", "85", "469", });
            dt.Rows.Add(new object[] { "True", "20.42", "0.14", "B", "1", "2", "x", "22", "22", "86", "470", });
            dt.Rows.Add(new object[] { "True", "89.27", "0.86", "A", "2", "2", "x", "23", "23", "87", "471", });
            dt.Rows.Add(new object[] { "True", "55.5", "0.67", "B", "2", "2", "x", "24", "24", "88", "472", });
            dt.Rows.Add(new object[] { "True", "67.81", "0.32", "A", "1", "1", "c", "25", "25", "89", "473", });
            dt.Rows.Add(new object[] { "True", "3.63", "0.69", "B", "1", "1", "c", "26", "26", "90", "474", });
            dt.Rows.Add(new object[] { "True", "43.16", "0.45", "A", "2", "1", "c", "27", "27", "91", "475", });
            dt.Rows.Add(new object[] { "True", "21.75", "0.07", "B", "2", "1", "c", "28", "28", "92", "476", });
            dt.Rows.Add(new object[] { "True", "80.38", "0.11", "A", "1", "2", "c", "29", "29", "93", "477", });
            dt.Rows.Add(new object[] { "True", "58.82", "0.3", "B", "1", "2", "c", "30", "30", "94", "478", });
            dt.Rows.Add(new object[] { "True", "40.49", "0.01", "A", "2", "2", "c", "31", "31", "95", "479", });
            dt.Rows.Add(new object[] { "True", "64.74", "0.25", "B", "2", "2", "c", "32", "32", "96", "480", });
            dt.Rows.Add(new object[] { "True", "95.79", "0.49", "A", "1", "1", "x", "1", "33", "97", "481", });
            dt.Rows.Add(new object[] { "True", "91.67", "0.15", "B", "1", "1", "x", "2", "34", "98", "482", });
            dt.Rows.Add(new object[] { "True", "19.81", "0.8", "A", "2", "1", "x", "3", "35", "99", "483", });
            dt.Rows.Add(new object[] { "True", "73.07", "0.61", "B", "2", "1", "x", "4", "36", "100", "484", });
            dt.Rows.Add(new object[] { "True", "26.21", "0.46", "A", "1", "2", "x", "5", "37", "101", "485", });
            dt.Rows.Add(new object[] { "True", "8.86", "0.61", "B", "1", "2", "x", "6", "38", "102", "486", });
            dt.Rows.Add(new object[] { "True", "76.4", "0.28", "A", "2", "2", "x", "7", "39", "103", "487", });
            dt.Rows.Add(new object[] { "True", "46.91", "0.83", "B", "2", "2", "x", "8", "40", "104", "488", });
            dt.Rows.Add(new object[] { "True", "22.13", "0.18", "A", "1", "1", "c", "9", "41", "105", "489", });
            dt.Rows.Add(new object[] { "True", "20.06", "0.38", "B", "1", "1", "c", "10", "42", "106", "490", });
            dt.Rows.Add(new object[] { "True", "26.45", "0.58", "A", "2", "1", "c", "11", "43", "107", "491", });
            dt.Rows.Add(new object[] { "True", "73.42", "0.96", "B", "2", "1", "c", "12", "44", "108", "492", });
            dt.Rows.Add(new object[] { "True", "24.52", "0.7", "A", "1", "2", "c", "13", "45", "109", "493", });
            dt.Rows.Add(new object[] { "True", "30.7", "0.31", "B", "1", "2", "c", "14", "46", "110", "494", });
            dt.Rows.Add(new object[] { "True", "1.54", "0.42", "A", "2", "2", "c", "15", "47", "111", "495", });
            dt.Rows.Add(new object[] { "True", "26.88", "0.72", "B", "2", "2", "c", "16", "48", "112", "496", });
            dt.Rows.Add(new object[] { "True", "56.36", "0.3", "A", "1", "1", "x", "17", "49", "113", "497", });
            dt.Rows.Add(new object[] { "True", "1.27", "0.67", "B", "1", "1", "x", "18", "50", "114", "498", });
            dt.Rows.Add(new object[] { "True", "54.89", "0.98", "A", "2", "1", "x", "19", "51", "115", "499", });
            dt.Rows.Add(new object[] { "True", "77.69", "0.8", "B", "2", "1", "x", "20", "52", "116", "500", });
            dt.Rows.Add(new object[] { "True", "63.47", "0.51", "A", "1", "2", "x", "21", "53", "117", "501", });
            dt.Rows.Add(new object[] { "True", "93.93", "0.14", "B", "1", "2", "x", "22", "54", "118", "502", });
            dt.Rows.Add(new object[] { "True", "87.09", "0.86", "A", "2", "2", "x", "23", "55", "119", "503", });
            dt.Rows.Add(new object[] { "True", "6.9", "0.67", "B", "2", "2", "x", "24", "56", "120", "504", });
            dt.Rows.Add(new object[] { "True", "52.03", "0.32", "A", "1", "1", "c", "25", "57", "121", "505", });
            dt.Rows.Add(new object[] { "True", "46.57", "0.69", "B", "1", "1", "c", "26", "58", "122", "506", });
            dt.Rows.Add(new object[] { "True", "74.7", "0.45", "A", "2", "1", "c", "27", "59", "123", "507", });
            dt.Rows.Add(new object[] { "True", "93.89", "0.07", "B", "2", "1", "c", "28", "60", "124", "508", });
            dt.Rows.Add(new object[] { "True", "16.55", "0.11", "A", "1", "2", "c", "29", "61", "125", "509", });
            dt.Rows.Add(new object[] { "True", "61.02", "0.3", "B", "1", "2", "c", "30", "62", "126", "510", });
            dt.Rows.Add(new object[] { "True", "97.28", "0.01", "A", "2", "2", "c", "31", "63", "127", "511", });
            dt.Rows.Add(new object[] { "True", "80.93", "0.25", "B", "2", "2", "c", "32", "64", "128", "512", });
            dt.Rows.Add(new object[] { "True", "59.59", "0.49", "A", "1", "1", "x", "1", "1", "129", "513", });
            dt.Rows.Add(new object[] { "True", "64.06", "0.15", "B", "1", "1", "x", "2", "2", "130", "514", });
            dt.Rows.Add(new object[] { "True", "68.32", "0.8", "A", "2", "1", "x", "3", "3", "131", "515", });
            dt.Rows.Add(new object[] { "True", "13.93", "0.61", "B", "2", "1", "x", "4", "4", "132", "516", });
            dt.Rows.Add(new object[] { "True", "2.82", "0.46", "A", "1", "2", "x", "5", "5", "133", "517", });
            dt.Rows.Add(new object[] { "True", "17.46", "0.61", "B", "1", "2", "x", "6", "6", "134", "518", });
            dt.Rows.Add(new object[] { "True", "31.49", "0.28", "A", "2", "2", "x", "7", "7", "135", "519", });
            dt.Rows.Add(new object[] { "True", "8.3", "0.83", "B", "2", "2", "x", "8", "8", "136", "520", });
            dt.Rows.Add(new object[] { "True", "67.95", "0.18", "A", "1", "1", "c", "9", "9", "137", "521", });
            dt.Rows.Add(new object[] { "True", "42.43", "0.38", "B", "1", "1", "c", "10", "10", "138", "522", });
            dt.Rows.Add(new object[] { "True", "65.38", "0.58", "A", "2", "1", "c", "11", "11", "139", "523", });
            dt.Rows.Add(new object[] { "True", "9.73", "0.96", "B", "2", "1", "c", "12", "12", "140", "524", });
            dt.Rows.Add(new object[] { "True", "2.27", "0.7", "A", "1", "2", "c", "13", "13", "141", "525", });
            dt.Rows.Add(new object[] { "True", "68.36", "0.31", "B", "1", "2", "c", "14", "14", "142", "526", });
            dt.Rows.Add(new object[] { "True", "87.68", "0.42", "A", "2", "2", "c", "15", "15", "143", "527", });
            dt.Rows.Add(new object[] { "True", "43.76", "0.72", "B", "2", "2", "c", "16", "16", "144", "528", });
            dt.Rows.Add(new object[] { "True", "6.99", "0.3", "A", "1", "1", "x", "17", "17", "145", "529", });
            dt.Rows.Add(new object[] { "True", "7.71", "0.67", "B", "1", "1", "x", "18", "18", "146", "530", });
            dt.Rows.Add(new object[] { "True", "94.71", "0.98", "A", "2", "1", "x", "19", "19", "147", "531", });
            dt.Rows.Add(new object[] { "True", "48.42", "0.8", "B", "2", "1", "x", "20", "20", "148", "532", });
            dt.Rows.Add(new object[] { "True", "51.09", "0.51", "A", "1", "2", "x", "21", "21", "149", "533", });
            dt.Rows.Add(new object[] { "True", "35.1", "0.14", "B", "1", "2", "x", "22", "22", "150", "534", });
            dt.Rows.Add(new object[] { "True", "92.3", "0.86", "A", "2", "2", "x", "23", "23", "151", "535", });
            dt.Rows.Add(new object[] { "True", "18.69", "0.67", "B", "2", "2", "x", "24", "24", "152", "536", });
            dt.Rows.Add(new object[] { "True", "5.55", "0.32", "A", "1", "1", "c", "25", "25", "153", "537", });
            dt.Rows.Add(new object[] { "True", "66.99", "0.69", "B", "1", "1", "c", "26", "26", "154", "538", });
            dt.Rows.Add(new object[] { "True", "57.19", "0.45", "A", "2", "1", "c", "27", "27", "155", "539", });
            dt.Rows.Add(new object[] { "True", "64.43", "0.07", "B", "2", "1", "c", "28", "28", "156", "540", });
            dt.Rows.Add(new object[] { "True", "75.48", "0.11", "A", "1", "2", "c", "29", "29", "157", "541", });
            dt.Rows.Add(new object[] { "True", "5.09", "0.3", "B", "1", "2", "c", "30", "30", "158", "542", });
            dt.Rows.Add(new object[] { "True", "96", "0.01", "A", "2", "2", "c", "31", "31", "159", "543", });
            dt.Rows.Add(new object[] { "True", "4.74", "0.25", "B", "2", "2", "c", "32", "32", "160", "544", });
            dt.Rows.Add(new object[] { "True", "38.26", "0.49", "A", "1", "1", "x", "1", "33", "161", "545", });
            dt.Rows.Add(new object[] { "True", "47.33", "0.15", "B", "1", "1", "x", "2", "34", "162", "546", });
            dt.Rows.Add(new object[] { "True", "12.64", "0.8", "A", "2", "1", "x", "3", "35", "163", "547", });
            dt.Rows.Add(new object[] { "True", "49.76", "0.61", "B", "2", "1", "x", "4", "36", "164", "548", });
            dt.Rows.Add(new object[] { "True", "94.49", "0.46", "A", "1", "2", "x", "5", "37", "165", "549", });
            dt.Rows.Add(new object[] { "True", "11.28", "0.61", "B", "1", "2", "x", "6", "38", "166", "550", });
            dt.Rows.Add(new object[] { "True", "4.88", "0.28", "A", "2", "2", "x", "7", "39", "167", "551", });
            dt.Rows.Add(new object[] { "True", "17.98", "0.83", "B", "2", "2", "x", "8", "40", "168", "552", });
            dt.Rows.Add(new object[] { "True", "91.61", "0.18", "A", "1", "1", "c", "9", "41", "169", "553", });
            dt.Rows.Add(new object[] { "True", "92.92", "0.38", "B", "1", "1", "c", "10", "42", "170", "554", });
            dt.Rows.Add(new object[] { "True", "90.7", "0.58", "A", "2", "1", "c", "11", "43", "171", "555", });
            dt.Rows.Add(new object[] { "True", "10.48", "0.96", "B", "2", "1", "c", "12", "44", "172", "556", });
            dt.Rows.Add(new object[] { "True", "4.87", "0.7", "A", "1", "2", "c", "13", "45", "173", "557", });
            dt.Rows.Add(new object[] { "True", "49.14", "0.31", "B", "1", "2", "c", "14", "46", "174", "558", });
            dt.Rows.Add(new object[] { "True", "7.26", "0.42", "A", "2", "2", "c", "15", "47", "175", "559", });
            dt.Rows.Add(new object[] { "True", "76.72", "0.72", "B", "2", "2", "c", "16", "48", "176", "560", });
            dt.Rows.Add(new object[] { "True", "57.27", "0.3", "A", "1", "1", "x", "17", "49", "177", "561", });
            dt.Rows.Add(new object[] { "True", "79.35", "0.67", "B", "1", "1", "x", "18", "50", "178", "562", });
            dt.Rows.Add(new object[] { "True", "19.36", "0.98", "A", "2", "1", "x", "19", "51", "179", "563", });
            dt.Rows.Add(new object[] { "True", "29.8", "0.8", "B", "2", "1", "x", "20", "52", "180", "564", });
            dt.Rows.Add(new object[] { "True", "31.29", "0.51", "A", "1", "2", "x", "21", "53", "181", "565", });
            dt.Rows.Add(new object[] { "True", "6.29", "0.14", "B", "1", "2", "x", "22", "54", "182", "566", });
            dt.Rows.Add(new object[] { "True", "6.53", "0.86", "A", "2", "2", "x", "23", "55", "183", "567", });
            dt.Rows.Add(new object[] { "True", "10.32", "0.67", "B", "2", "2", "x", "24", "56", "184", "568", });
            dt.Rows.Add(new object[] { "True", "16.53", "0.32", "A", "1", "1", "c", "25", "57", "185", "569", });
            dt.Rows.Add(new object[] { "True", "19.55", "0.69", "B", "1", "1", "c", "26", "58", "186", "570", });
            dt.Rows.Add(new object[] { "True", "86.54", "0.45", "A", "2", "1", "c", "27", "59", "187", "571", });
            dt.Rows.Add(new object[] { "True", "33.54", "0.07", "B", "2", "1", "c", "28", "60", "188", "572", });
            dt.Rows.Add(new object[] { "True", "2.27", "0.11", "A", "1", "2", "c", "29", "61", "189", "573", });
            dt.Rows.Add(new object[] { "True", "85.53", "0.3", "B", "1", "2", "c", "30", "62", "190", "574", });
            dt.Rows.Add(new object[] { "True", "35.8", "0.01", "A", "2", "2", "c", "31", "63", "191", "575", });
            dt.Rows.Add(new object[] { "True", "70.8", "0.25", "B", "2", "2", "c", "32", "64", "192", "576", });
            dt.Rows.Add(new object[] { "True", "65.11", "0.49", "A", "1", "1", "x", "1", "1", "1", "577", });
            dt.Rows.Add(new object[] { "True", "53.38", "0.15", "B", "1", "1", "x", "2", "2", "2", "578", });
            dt.Rows.Add(new object[] { "True", "46.65", "0.8", "A", "2", "1", "x", "3", "3", "3", "579", });
            dt.Rows.Add(new object[] { "True", "1.18", "0.61", "B", "2", "1", "x", "4", "4", "4", "580", });
            dt.Rows.Add(new object[] { "True", "94.79", "0.46", "A", "1", "2", "x", "5", "5", "5", "581", });
            dt.Rows.Add(new object[] { "True", "99.5", "0.61", "B", "1", "2", "x", "6", "6", "6", "582", });
            dt.Rows.Add(new object[] { "True", "83.01", "0.28", "A", "2", "2", "x", "7", "7", "7", "583", });
            dt.Rows.Add(new object[] { "True", "24.84", "0.83", "B", "2", "2", "x", "8", "8", "8", "584", });
            dt.Rows.Add(new object[] { "True", "27.21", "0.18", "A", "1", "1", "c", "9", "9", "9", "585", });
            dt.Rows.Add(new object[] { "True", "85.16", "0.38", "B", "1", "1", "c", "10", "10", "10", "586", });
            dt.Rows.Add(new object[] { "True", "31.27", "0.58", "A", "2", "1", "c", "11", "11", "11", "587", });
            dt.Rows.Add(new object[] { "True", "65.97", "0.96", "B", "2", "1", "c", "12", "12", "12", "588", });
            dt.Rows.Add(new object[] { "True", "39.45", "0.7", "A", "1", "2", "c", "13", "13", "13", "589", });
            dt.Rows.Add(new object[] { "True", "82.9", "0.31", "B", "1", "2", "c", "14", "14", "14", "590", });
            dt.Rows.Add(new object[] { "True", "31.77", "0.42", "A", "2", "2", "c", "15", "15", "15", "591", });
            dt.Rows.Add(new object[] { "True", "33.03", "0.72", "B", "2", "2", "c", "16", "16", "16", "592", });
            dt.Rows.Add(new object[] { "True", "18.21", "0.3", "A", "1", "1", "x", "17", "17", "17", "593", });
            dt.Rows.Add(new object[] { "True", "87.18", "0.67", "B", "1", "1", "x", "18", "18", "18", "594", });
            dt.Rows.Add(new object[] { "True", "78.34", "0.98", "A", "2", "1", "x", "19", "19", "19", "595", });
            dt.Rows.Add(new object[] { "True", "72.26", "0.8", "B", "2", "1", "x", "20", "20", "20", "596", });
            dt.Rows.Add(new object[] { "True", "17.58", "0.51", "A", "1", "2", "x", "21", "21", "21", "597", });
            dt.Rows.Add(new object[] { "True", "49.18", "0.14", "B", "1", "2", "x", "22", "22", "22", "598", });
            dt.Rows.Add(new object[] { "True", "26.03", "0.86", "A", "2", "2", "x", "23", "23", "23", "599", });
            dt.Rows.Add(new object[] { "True", "40.25", "0.67", "B", "2", "2", "x", "24", "24", "24", "600", });
            dt.Rows.Add(new object[] { "True", "91.96", "0.32", "A", "1", "1", "c", "25", "25", "25", "601", });
            dt.Rows.Add(new object[] { "True", "92.01", "0.69", "B", "1", "1", "c", "26", "26", "26", "602", });
            dt.Rows.Add(new object[] { "True", "42.31", "0.45", "A", "2", "1", "c", "27", "27", "27", "603", });
            dt.Rows.Add(new object[] { "True", "58.03", "0.07", "B", "2", "1", "c", "28", "28", "28", "604", });
            dt.Rows.Add(new object[] { "True", "14.22", "0.11", "A", "1", "2", "c", "29", "29", "29", "605", });
            dt.Rows.Add(new object[] { "True", "29.37", "0.3", "B", "1", "2", "c", "30", "30", "30", "606", });
            dt.Rows.Add(new object[] { "True", "40.01", "0.01", "A", "2", "2", "c", "31", "31", "31", "607", });
            dt.Rows.Add(new object[] { "True", "89.41", "0.25", "B", "2", "2", "c", "32", "32", "32", "608", });
            dt.Rows.Add(new object[] { "True", "93.45", "0.49", "A", "1", "1", "x", "1", "33", "33", "609", });
            dt.Rows.Add(new object[] { "True", "43.38", "0.15", "B", "1", "1", "x", "2", "34", "34", "610", });
            dt.Rows.Add(new object[] { "True", "74.58", "0.8", "A", "2", "1", "x", "3", "35", "35", "611", });
            dt.Rows.Add(new object[] { "True", "64.85", "0.61", "B", "2", "1", "x", "4", "36", "36", "612", });
            dt.Rows.Add(new object[] { "True", "22.24", "0.46", "A", "1", "2", "x", "5", "37", "37", "613", });
            dt.Rows.Add(new object[] { "True", "28", "0.61", "B", "1", "2", "x", "6", "38", "38", "614", });
            dt.Rows.Add(new object[] { "True", "34.13", "0.28", "A", "2", "2", "x", "7", "39", "39", "615", });
            dt.Rows.Add(new object[] { "True", "85.31", "0.83", "B", "2", "2", "x", "8", "40", "40", "616", });
            dt.Rows.Add(new object[] { "True", "93.08", "0.18", "A", "1", "1", "c", "9", "41", "41", "617", });
            dt.Rows.Add(new object[] { "True", "24.13", "0.38", "B", "1", "1", "c", "10", "42", "42", "618", });
            dt.Rows.Add(new object[] { "True", "5.31", "0.58", "A", "2", "1", "c", "11", "43", "43", "619", });
            dt.Rows.Add(new object[] { "True", "84.71", "0.96", "B", "2", "1", "c", "12", "44", "44", "620", });
            dt.Rows.Add(new object[] { "True", "75.76", "0.7", "A", "1", "2", "c", "13", "45", "45", "621", });
            dt.Rows.Add(new object[] { "True", "66.18", "0.31", "B", "1", "2", "c", "14", "46", "46", "622", });
            dt.Rows.Add(new object[] { "True", "80.18", "0.42", "A", "2", "2", "c", "15", "47", "47", "623", });
            dt.Rows.Add(new object[] { "True", "9.72", "0.72", "B", "2", "2", "c", "16", "48", "48", "624", });
            dt.Rows.Add(new object[] { "True", "95.52", "0.3", "A", "1", "1", "x", "17", "49", "49", "625", });
            dt.Rows.Add(new object[] { "True", "71.59", "0.67", "B", "1", "1", "x", "18", "50", "50", "626", });
            dt.Rows.Add(new object[] { "True", "89.15", "0.98", "A", "2", "1", "x", "19", "51", "51", "627", });
            dt.Rows.Add(new object[] { "True", "59.08", "0.8", "B", "2", "1", "x", "20", "52", "52", "628", });
            dt.Rows.Add(new object[] { "True", "18.6", "0.51", "A", "1", "2", "x", "21", "53", "53", "629", });
            dt.Rows.Add(new object[] { "True", "1.25", "0.14", "B", "1", "2", "x", "22", "54", "54", "630", });
            dt.Rows.Add(new object[] { "True", "62.1", "0.86", "A", "2", "2", "x", "23", "55", "55", "631", });
            dt.Rows.Add(new object[] { "True", "47.76", "0.67", "B", "2", "2", "x", "24", "56", "56", "632", });
            dt.Rows.Add(new object[] { "True", "64.52", "0.32", "A", "1", "1", "c", "25", "57", "57", "633", });
            dt.Rows.Add(new object[] { "True", "33.19", "0.69", "B", "1", "1", "c", "26", "58", "58", "634", });
            dt.Rows.Add(new object[] { "True", "22.57", "0.45", "A", "2", "1", "c", "27", "59", "59", "635", });
            dt.Rows.Add(new object[] { "True", "82.56", "0.07", "B", "2", "1", "c", "28", "60", "60", "636", });
            dt.Rows.Add(new object[] { "True", "70.68", "0.11", "A", "1", "2", "c", "29", "61", "61", "637", });
            dt.Rows.Add(new object[] { "True", "47.26", "0.3", "B", "1", "2", "c", "30", "62", "62", "638", });
            dt.Rows.Add(new object[] { "True", "86.59", "0.01", "A", "2", "2", "c", "31", "63", "63", "639", });
            dt.Rows.Add(new object[] { "True", "82.33", "0.25", "B", "2", "2", "c", "32", "64", "64", "640", });
            dt.Rows.Add(new object[] { "True", "30.63", "0.49", "A", "1", "1", "x", "1", "1", "65", "641", });
            dt.Rows.Add(new object[] { "True", "22.01", "0.15", "B", "1", "1", "x", "2", "2", "66", "642", });
            dt.Rows.Add(new object[] { "True", "86.67", "0.8", "A", "2", "1", "x", "3", "3", "67", "643", });
            dt.Rows.Add(new object[] { "True", "83.29", "0.61", "B", "2", "1", "x", "4", "4", "68", "644", });
            dt.Rows.Add(new object[] { "True", "98.6", "0.46", "A", "1", "2", "x", "5", "5", "69", "645", });
            dt.Rows.Add(new object[] { "True", "89.9", "0.61", "B", "1", "2", "x", "6", "6", "70", "646", });
            dt.Rows.Add(new object[] { "True", "82.63", "0.28", "A", "2", "2", "x", "7", "7", "71", "647", });
            dt.Rows.Add(new object[] { "True", "66.63", "0.83", "B", "2", "2", "x", "8", "8", "72", "648", });
            dt.Rows.Add(new object[] { "True", "20.92", "0.18", "A", "1", "1", "c", "9", "9", "73", "649", });
            dt.Rows.Add(new object[] { "True", "89.5", "0.38", "B", "1", "1", "c", "10", "10", "74", "650", });
            dt.Rows.Add(new object[] { "True", "94.16", "0.58", "A", "2", "1", "c", "11", "11", "75", "651", });
            dt.Rows.Add(new object[] { "True", "47.49", "0.96", "B", "2", "1", "c", "12", "12", "76", "652", });
            dt.Rows.Add(new object[] { "True", "67.61", "0.7", "A", "1", "2", "c", "13", "13", "77", "653", });
            dt.Rows.Add(new object[] { "True", "61.29", "0.31", "B", "1", "2", "c", "14", "14", "78", "654", });
            dt.Rows.Add(new object[] { "True", "74.56", "0.42", "A", "2", "2", "c", "15", "15", "79", "655", });
            dt.Rows.Add(new object[] { "True", "12.22", "0.72", "B", "2", "2", "c", "16", "16", "80", "656", });
            dt.Rows.Add(new object[] { "True", "43.84", "0.3", "A", "1", "1", "x", "17", "17", "81", "657", });
            dt.Rows.Add(new object[] { "True", "35.95", "0.67", "B", "1", "1", "x", "18", "18", "82", "658", });
            dt.Rows.Add(new object[] { "True", "25.9", "0.98", "A", "2", "1", "x", "19", "19", "83", "659", });
            dt.Rows.Add(new object[] { "True", "64.49", "0.8", "B", "2", "1", "x", "20", "20", "84", "660", });
            dt.Rows.Add(new object[] { "True", "91.14", "0.51", "A", "1", "2", "x", "21", "21", "85", "661", });
            dt.Rows.Add(new object[] { "True", "66.2", "0.14", "B", "1", "2", "x", "22", "22", "86", "662", });
            dt.Rows.Add(new object[] { "True", "94.01", "0.86", "A", "2", "2", "x", "23", "23", "87", "663", });
            dt.Rows.Add(new object[] { "True", "33.87", "0.67", "B", "2", "2", "x", "24", "24", "88", "664", });
            dt.Rows.Add(new object[] { "True", "71.83", "0.32", "A", "1", "1", "c", "25", "25", "89", "665", });
            dt.Rows.Add(new object[] { "True", "17.34", "0.69", "B", "1", "1", "c", "26", "26", "90", "666", });
            dt.Rows.Add(new object[] { "True", "70.73", "0.45", "A", "2", "1", "c", "27", "27", "91", "667", });
            dt.Rows.Add(new object[] { "True", "29.7", "0.07", "B", "2", "1", "c", "28", "28", "92", "668", });
            dt.Rows.Add(new object[] { "True", "6.76", "0.11", "A", "1", "2", "c", "29", "29", "93", "669", });
            dt.Rows.Add(new object[] { "True", "71.1", "0.3", "B", "1", "2", "c", "30", "30", "94", "670", });
            dt.Rows.Add(new object[] { "True", "95.33", "0.01", "A", "2", "2", "c", "31", "31", "95", "671", });
            dt.Rows.Add(new object[] { "True", "55.03", "0.25", "B", "2", "2", "c", "32", "32", "96", "672", });
            dt.Rows.Add(new object[] { "True", "49.73", "0.49", "A", "1", "1", "x", "1", "33", "97", "673", });
            dt.Rows.Add(new object[] { "True", "86.38", "0.15", "B", "1", "1", "x", "2", "34", "98", "674", });
            dt.Rows.Add(new object[] { "True", "47.08", "0.8", "A", "2", "1", "x", "3", "35", "99", "675", });
            dt.Rows.Add(new object[] { "True", "90.41", "0.61", "B", "2", "1", "x", "4", "36", "100", "676", });
            dt.Rows.Add(new object[] { "True", "78.71", "0.46", "A", "1", "2", "x", "5", "37", "101", "677", });
            dt.Rows.Add(new object[] { "True", "67.06", "0.61", "B", "1", "2", "x", "6", "38", "102", "678", });
            dt.Rows.Add(new object[] { "True", "47.4", "0.28", "A", "2", "2", "x", "7", "39", "103", "679", });
            dt.Rows.Add(new object[] { "True", "50.23", "0.83", "B", "2", "2", "x", "8", "40", "104", "680", });
            dt.Rows.Add(new object[] { "True", "24.17", "0.18", "A", "1", "1", "c", "9", "41", "105", "681", });
            dt.Rows.Add(new object[] { "True", "94.03", "0.38", "B", "1", "1", "c", "10", "42", "106", "682", });
            dt.Rows.Add(new object[] { "True", "84.32", "0.58", "A", "2", "1", "c", "11", "43", "107", "683", });
            dt.Rows.Add(new object[] { "True", "20.47", "0.96", "B", "2", "1", "c", "12", "44", "108", "684", });
            dt.Rows.Add(new object[] { "True", "66.43", "0.7", "A", "1", "2", "c", "13", "45", "109", "685", });
            dt.Rows.Add(new object[] { "True", "75.56", "0.31", "B", "1", "2", "c", "14", "46", "110", "686", });
            dt.Rows.Add(new object[] { "True", "29", "0.42", "A", "2", "2", "c", "15", "47", "111", "687", });
            dt.Rows.Add(new object[] { "True", "39.68", "0.72", "B", "2", "2", "c", "16", "48", "112", "688", });
            dt.Rows.Add(new object[] { "True", "10.83", "0.3", "A", "1", "1", "x", "17", "49", "113", "689", });
            dt.Rows.Add(new object[] { "True", "11.61", "0.67", "B", "1", "1", "x", "18", "50", "114", "690", });
            dt.Rows.Add(new object[] { "True", "40.71", "0.98", "A", "2", "1", "x", "19", "51", "115", "691", });
            dt.Rows.Add(new object[] { "True", "35.03", "0.8", "B", "2", "1", "x", "20", "52", "116", "692", });
            dt.Rows.Add(new object[] { "True", "37.27", "0.51", "A", "1", "2", "x", "21", "53", "117", "693", });
            dt.Rows.Add(new object[] { "True", "55.2", "0.14", "B", "1", "2", "x", "22", "54", "118", "694", });
            dt.Rows.Add(new object[] { "True", "27.89", "0.86", "A", "2", "2", "x", "23", "55", "119", "695", });
            dt.Rows.Add(new object[] { "True", "46.03", "0.67", "B", "2", "2", "x", "24", "56", "120", "696", });
            dt.Rows.Add(new object[] { "True", "31.17", "0.32", "A", "1", "1", "c", "25", "57", "121", "697", });
            dt.Rows.Add(new object[] { "True", "2.35", "0.69", "B", "1", "1", "c", "26", "58", "122", "698", });
            dt.Rows.Add(new object[] { "True", "63.31", "0.45", "A", "2", "1", "c", "27", "59", "123", "699", });
            dt.Rows.Add(new object[] { "True", "66.11", "0.07", "B", "2", "1", "c", "28", "60", "124", "700", });
            dt.Rows.Add(new object[] { "True", "32.16", "0.11", "A", "1", "2", "c", "29", "61", "125", "701", });
            dt.Rows.Add(new object[] { "True", "86.7", "0.3", "B", "1", "2", "c", "30", "62", "126", "702", });
            dt.Rows.Add(new object[] { "True", "88.48", "0.01", "A", "2", "2", "c", "31", "63", "127", "703", });
            dt.Rows.Add(new object[] { "True", "15.25", "0.25", "B", "2", "2", "c", "32", "64", "128", "704", });
            dt.Rows.Add(new object[] { "True", "88.69", "0.49", "A", "1", "1", "x", "1", "1", "129", "705", });
            dt.Rows.Add(new object[] { "True", "29.81", "0.15", "B", "1", "1", "x", "2", "2", "130", "706", });
            dt.Rows.Add(new object[] { "True", "25.95", "0.8", "A", "2", "1", "x", "3", "3", "131", "707", });
            dt.Rows.Add(new object[] { "True", "39.93", "0.61", "B", "2", "1", "x", "4", "4", "132", "708", });
            dt.Rows.Add(new object[] { "True", "22.87", "0.46", "A", "1", "2", "x", "5", "5", "133", "709", });
            dt.Rows.Add(new object[] { "True", "18.26", "0.61", "B", "1", "2", "x", "6", "6", "134", "710", });
            dt.Rows.Add(new object[] { "True", "44.03", "0.28", "A", "2", "2", "x", "7", "7", "135", "711", });
            dt.Rows.Add(new object[] { "True", "83.86", "0.83", "B", "2", "2", "x", "8", "8", "136", "712", });
            dt.Rows.Add(new object[] { "True", "33.26", "0.18", "A", "1", "1", "c", "9", "9", "137", "713", });
            dt.Rows.Add(new object[] { "True", "78.27", "0.38", "B", "1", "1", "c", "10", "10", "138", "714", });
            dt.Rows.Add(new object[] { "True", "33.21", "0.58", "A", "2", "1", "c", "11", "11", "139", "715", });
            dt.Rows.Add(new object[] { "True", "1.05", "0.96", "B", "2", "1", "c", "12", "12", "140", "716", });
            dt.Rows.Add(new object[] { "True", "71.21", "0.7", "A", "1", "2", "c", "13", "13", "141", "717", });
            dt.Rows.Add(new object[] { "True", "84.68", "0.31", "B", "1", "2", "c", "14", "14", "142", "718", });
            dt.Rows.Add(new object[] { "True", "88.33", "0.42", "A", "2", "2", "c", "15", "15", "143", "719", });
            dt.Rows.Add(new object[] { "True", "77.81", "0.72", "B", "2", "2", "c", "16", "16", "144", "720", });
            dt.Rows.Add(new object[] { "True", "5.57", "0.3", "A", "1", "1", "x", "17", "17", "145", "721", });
            dt.Rows.Add(new object[] { "True", "46.25", "0.67", "B", "1", "1", "x", "18", "18", "146", "722", });
            dt.Rows.Add(new object[] { "True", "25.5", "0.98", "A", "2", "1", "x", "19", "19", "147", "723", });
            dt.Rows.Add(new object[] { "True", "82.98", "0.8", "B", "2", "1", "x", "20", "20", "148", "724", });
            dt.Rows.Add(new object[] { "True", "98.94", "0.51", "A", "1", "2", "x", "21", "21", "149", "725", });
            dt.Rows.Add(new object[] { "True", "46.67", "0.14", "B", "1", "2", "x", "22", "22", "150", "726", });
            dt.Rows.Add(new object[] { "True", "53.51", "0.86", "A", "2", "2", "x", "23", "23", "151", "727", });
            dt.Rows.Add(new object[] { "True", "70.95", "0.67", "B", "2", "2", "x", "24", "24", "152", "728", });
            dt.Rows.Add(new object[] { "True", "81.61", "0.32", "A", "1", "1", "c", "25", "25", "153", "729", });
            dt.Rows.Add(new object[] { "True", "72.26", "0.69", "B", "1", "1", "c", "26", "26", "154", "730", });
            dt.Rows.Add(new object[] { "True", "68.55", "0.45", "A", "2", "1", "c", "27", "27", "155", "731", });
            dt.Rows.Add(new object[] { "True", "64.74", "0.07", "B", "2", "1", "c", "28", "28", "156", "732", });
            dt.Rows.Add(new object[] { "True", "82.31", "0.11", "A", "1", "2", "c", "29", "29", "157", "733", });
            dt.Rows.Add(new object[] { "True", "5.44", "0.3", "B", "1", "2", "c", "30", "30", "158", "734", });
            dt.Rows.Add(new object[] { "True", "28.71", "0.01", "A", "2", "2", "c", "31", "31", "159", "735", });
            dt.Rows.Add(new object[] { "True", "8.35", "0.25", "B", "2", "2", "c", "32", "32", "160", "736", });
            dt.Rows.Add(new object[] { "True", "99.94", "0.49", "A", "1", "1", "x", "1", "33", "161", "737", });
            dt.Rows.Add(new object[] { "True", "14.61", "0.15", "B", "1", "1", "x", "2", "34", "162", "738", });
            dt.Rows.Add(new object[] { "True", "42.93", "0.8", "A", "2", "1", "x", "3", "35", "163", "739", });
            dt.Rows.Add(new object[] { "True", "62.73", "0.61", "B", "2", "1", "x", "4", "36", "164", "740", });
            dt.Rows.Add(new object[] { "True", "24.97", "0.46", "A", "1", "2", "x", "5", "37", "165", "741", });
            dt.Rows.Add(new object[] { "True", "93.6", "0.61", "B", "1", "2", "x", "6", "38", "166", "742", });
            dt.Rows.Add(new object[] { "True", "53.59", "0.28", "A", "2", "2", "x", "7", "39", "167", "743", });
            dt.Rows.Add(new object[] { "True", "85.89", "0.83", "B", "2", "2", "x", "8", "40", "168", "744", });
            dt.Rows.Add(new object[] { "True", "24.99", "0.18", "A", "1", "1", "c", "9", "41", "169", "745", });
            dt.Rows.Add(new object[] { "True", "90.31", "0.38", "B", "1", "1", "c", "10", "42", "170", "746", });
            dt.Rows.Add(new object[] { "True", "23.65", "0.58", "A", "2", "1", "c", "11", "43", "171", "747", });
            dt.Rows.Add(new object[] { "True", "7.81", "0.96", "B", "2", "1", "c", "12", "44", "172", "748", });
            dt.Rows.Add(new object[] { "True", "57.39", "0.7", "A", "1", "2", "c", "13", "45", "173", "749", });
            dt.Rows.Add(new object[] { "True", "57.43", "0.31", "B", "1", "2", "c", "14", "46", "174", "750", });
            dt.Rows.Add(new object[] { "True", "66.6", "0.42", "A", "2", "2", "c", "15", "47", "175", "751", });
            dt.Rows.Add(new object[] { "True", "48.17", "0.72", "B", "2", "2", "c", "16", "48", "176", "752", });
            dt.Rows.Add(new object[] { "True", "78.59", "0.3", "A", "1", "1", "x", "17", "49", "177", "753", });
            dt.Rows.Add(new object[] { "True", "45.93", "0.67", "B", "1", "1", "x", "18", "50", "178", "754", });
            dt.Rows.Add(new object[] { "True", "45.23", "0.98", "A", "2", "1", "x", "19", "51", "179", "755", });
            dt.Rows.Add(new object[] { "True", "44.93", "0.8", "B", "2", "1", "x", "20", "52", "180", "756", });
            dt.Rows.Add(new object[] { "True", "42.74", "0.51", "A", "1", "2", "x", "21", "53", "181", "757", });
            dt.Rows.Add(new object[] { "True", "35.08", "0.14", "B", "1", "2", "x", "22", "54", "182", "758", });
            dt.Rows.Add(new object[] { "True", "77.14", "0.86", "A", "2", "2", "x", "23", "55", "183", "759", });
            dt.Rows.Add(new object[] { "True", "26.99", "0.67", "B", "2", "2", "x", "24", "56", "184", "760", });
            dt.Rows.Add(new object[] { "True", "88.38", "0.32", "A", "1", "1", "c", "25", "57", "185", "761", });
            dt.Rows.Add(new object[] { "True", "6.95", "0.69", "B", "1", "1", "c", "26", "58", "186", "762", });
            dt.Rows.Add(new object[] { "True", "95.75", "0.45", "A", "2", "1", "c", "27", "59", "187", "763", });
            dt.Rows.Add(new object[] { "True", "29.86", "0.07", "B", "2", "1", "c", "28", "60", "188", "764", });
            dt.Rows.Add(new object[] { "True", "66.36", "0.11", "A", "1", "2", "c", "29", "61", "189", "765", });
            dt.Rows.Add(new object[] { "True", "71.27", "0.3", "B", "1", "2", "c", "30", "62", "190", "766", });
            dt.Rows.Add(new object[] { "True", "12.11", "0.01", "A", "2", "2", "c", "31", "63", "191", "767", });
            dt.Rows.Add(new object[] { "True", "90.32", "0.25", "B", "2", "2", "c", "32", "64", "192", "768", });

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