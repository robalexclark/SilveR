using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Xunit;

namespace SilveR.UnitTests.StatsModels
{    
    public class PairedTTestAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PairedTTestAnalysisModel sut = new PairedTTestAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("PairedTTestAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PairedTTestAnalysisModel sut = new PairedTTestAnalysisModel();

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
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PairedTTestAnalysisModel sut = new PairedTTestAnalysisModel();

            //Act
            IEnumerable<string> result = sut.SignificancesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "0.1", "0.05", "0.025", "0.01", "0.001" }, result);
        }

        [Fact]
        public void CovariancesList_ReturnsCorrectList()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PairedTTestAnalysisModel sut = new PairedTTestAnalysisModel();

            //Act
            IEnumerable<string> result = sut.CovariancesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "Compound Symmetric", "Unstructured", "Autoregressive(1)" }, result);
        }

        [Fact]
        public void ExportData_ReturnsCorrectStringArray()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(It.IsAny<int>);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            PairedTTestAnalysisModel sut = GetModel(mockDataset.Object);         

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Animal1,Day1,Cov1,Block1,Block2", result[0]);
            Assert.Equal(81, result.Count()); //as blank reponses are removed
            Assert.StartsWith("9.681530124,8,6,0.56,x", result[32]);
        }       

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PairedTTestAnalysisModel sut = GetModel(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var response = result.Single(x => x.Name == "Response");
            Assert.Equal("Resp 1", response.Value);

            var treatment = result.Single(x => x.Name == "Treatment");
            Assert.Equal("Day1", treatment.Value);

            var subject = result.Single(x => x.Name == "Subject");
            Assert.Equal("Animal1", subject.Value);

            var otherDesignFactors = result.Single(x => x.Name == "OtherDesignFactors");
            Assert.Equal("Block1,Block2", otherDesignFactors.Value);

            var covariates = result.Single(x => x.Name == "Covariates");
            Assert.Equal("Cov1", covariates.Value);

            var responseTransformation = result.Single(x => x.Name == "ResponseTransformation");
            Assert.Equal("None", responseTransformation.Value);

            var covariateTransformation = result.Single(x => x.Name == "CovariateTransformation");
            Assert.Equal("None", covariateTransformation.Value);
            
            var anovaSelected = result.Single(x => x.Name == "ANOVASelected");
            Assert.Equal("True", anovaSelected.Value);

            var prPlotSelected = result.Single(x => x.Name == "PRPlotSelected");
            Assert.Equal("True", prPlotSelected.Value);

            var allPairwiseComparisonsSelected = result.Single(x => x.Name == "AllPairwiseComparisons");
            Assert.Equal("False", allPairwiseComparisonsSelected.Value);

            var lsMeansSelected = result.Single(x => x.Name == "LSMeansSelected");
            Assert.Equal("False", lsMeansSelected.Value);

            var normalPlotSelectedSelected = result.Single(x => x.Name == "NormalPlotSelected");
            Assert.Equal("False", normalPlotSelectedSelected.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);

            var covariance = result.Single(x => x.Name == "Covariance");
            Assert.Equal("Compound Symmetric", covariance.Value);

            var controlGroup = result.Single(x => x.Name == "ControlGroup");
            Assert.Equal("2", controlGroup.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PairedTTestAnalysisModel sut = new PairedTTestAnalysisModel(GetDataset());

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "Response", Value = "Resp1" });
            arguments.Add(new Argument { Name = "Subject", Value = "Animal1" });
            arguments.Add(new Argument { Name = "Treatment", Value = "Day1" });
            arguments.Add(new Argument { Name = "OtherDesignFactors", Value = "Block1,Block2" });
            arguments.Add(new Argument { Name = "ResponseTransformation", Value = "Log10" });
            arguments.Add(new Argument { Name = "Covariates", Value = "Cov1" });
            arguments.Add(new Argument { Name = "CovariateTransformation", Value = "ArcSine" });
            arguments.Add(new Argument { Name = "ANOVASelected", Value = "False" });
            arguments.Add(new Argument { Name = "PRPlotSelected", Value = "True" });
            arguments.Add(new Argument { Name = "NormalPlotSelected", Value = "False" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.9" });
            arguments.Add(new Argument { Name = "LSMeansSelected", Value = "True" });
            arguments.Add(new Argument { Name = "Covariance", Value = "Compound Symmetric" });
            arguments.Add(new Argument { Name = "CompareCovarianceModels", Value = "False" });
            arguments.Add(new Argument { Name = "ControlGroup", Value = "2" });
            arguments.Add(new Argument { Name = "AllPairwiseComparisons", Value = "True" });

            Assert.Equal(16, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("Resp1", sut.Response);
            Assert.Equal("Day1", sut.Treatment);
            Assert.Equal("Animal1", sut.Subject);
            Assert.Equal(new List<string> { "Block1", "Block2" }, sut.OtherDesignFactors);
            Assert.Equal("Log10", sut.ResponseTransformation);
            Assert.Equal(new List<string> { "Cov1" }, sut.Covariates);
            Assert.Equal("ArcSine", sut.CovariateTransformation);
            Assert.False(sut.ANOVASelected);
            Assert.True(sut.PRPlotSelected);
            Assert.False(sut.NormalPlotSelected);
            Assert.Equal("0.9", sut.Significance);
            Assert.True(sut.LSMeansSelected);
            Assert.Equal("Compound Symmetric", sut.Covariance);
            Assert.False(sut.CompareCovarianceModels);
            Assert.Equal("2", sut.ControlGroup);
            Assert.True(sut.AllPairwiseComparisons);
        }


        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            PairedTTestAnalysisModel sut = GetModel(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Respivs_sp_ivs1~Cov1+Block1+Block2+Timezzz Day1 Animal1 Cov1 \"Compound Symmetric\" N None None Block1,Block2 Y Y N N 2 0.05 N", result);
        }

        private PairedTTestAnalysisModel GetModel(IDataset dataset)
        {
            var model = new PairedTTestAnalysisModel(dataset)
            {
                ANOVASelected = true,
                AllPairwiseComparisons = false,
                ControlGroup = "2",
                Covariance = "Compound Symmetric",
                CovariateTransformation = "None",
                Covariates = new System.Collections.Generic.List<string>
                {
                    "Cov1"
                },
                LSMeansSelected = false,
                NormalPlotSelected = false,
                OtherDesignFactors = new System.Collections.Generic.List<string>
                {
                    "Block1",
                    "Block2"
                },
                PRPlotSelected = true,
                Response = "Resp 1",
                ResponseTransformation = "None",
                Significance = "0.05",
                Subject = "Animal1",
                Treatment = "Day1"
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
            dt.Columns.Add("Resp5");
            dt.Columns.Add("Resp6");
            dt.Columns.Add("Resp7");
            dt.Columns.Add("Resp8");
            dt.Columns.Add("Resp9");
            dt.Columns.Add("Animal1");
            dt.Columns.Add("Animal2");
            dt.Columns.Add("Animal3");
            dt.Columns.Add("Animal4");
            dt.Columns.Add("Animal 5");
            dt.Columns.Add("Day1");
            dt.Columns.Add("Day2");
            dt.Columns.Add("Day3");
            dt.Columns.Add("Day4");
            dt.Columns.Add("Day5");
            dt.Columns.Add("Day6");
            dt.Columns.Add("Cov1");
            dt.Columns.Add("Cov2");
            dt.Columns.Add("Cov3");
            dt.Columns.Add("Cov4");
            dt.Columns.Add("Cov5");
            dt.Columns.Add("Cov6");
            dt.Columns.Add("Block1");
            dt.Columns.Add("Block2");
            dt.Columns.Add("Block3");
            dt.Columns.Add("Resp10");
            dt.Columns.Add("Day7");
            dt.Columns.Add("Animal6");
            dt.Columns.Add("Cov7");
            dt.Columns.Add("PVtestResp1");
            dt.Columns.Add("PVTestTreat1");
            dt.Columns.Add("PVTestAnimal1");
            dt.Columns.Add("PVTestDay1");
            dt.Columns.Add("PVTestCOV1a");
            dt.Columns.Add("PVTestCOV1b");
            dt.Columns.Add("CVResp");
            dt.Columns.Add("CVAnimal1");
            dt.Columns.Add("CVTime1");
            dt.Columns.Add("CVAnimal2");
            dt.Columns.Add("CVTime2");
            dt.Rows.Add(new object[] { "True", "4.998032373", "0.0143", "x", "", "-2", "0", "", "3", "", "1", "", "", "A1", "1", "1", "", "D2", "D1", "1", "1", "0.26", "x", "", "-0.6", "0", "2", "x", "x", "", "0.214717465629762", "1", "1", "0.744034001938249", "8.27030188", "A", "1", "1", "9.038588654", "11", "10", "1", "1", "A", "A", });
            dt.Rows.Add(new object[] { "True", "7.339311161", "", "0.86", "0.86", "0.86", "0.86", "0.6", "0.86", "", "1", "1", "", "A1", "1", "2", "D2", "D2", "D2", "2", "2", "0.75", "0.75", "0.75", "0.75", "0.75", "0.75", "x", "x", "x", "0.80302429949629", "1", "2", "0.606708650284174", "2.47257242", "A", "1", "2", "3.018706714", "7.5", "11", "3", "1", "C", "A", });
            dt.Rows.Add(new object[] { "True", "4.786572071", "", "0.21", "0.21", "0.21", "0.21", "0.94", "0.21", "", "1", "1", "", "A1", "2", "4", "D3", "D3", "D3", "3", "1", "0.98", "0.98", "0.98", "0.98", "0.98", "0.98", "x", "x", "x", "0.254489978652641", "2", "1", "0.154985437623257", "7.719077689", "A", "1", "3", "8.352499099", "8.2", "12", "5", "1", "E", "A", });
            dt.Rows.Add(new object[] { "True", "9.784210352", "", "0.36", "0.36", "0.36", "0.36", "0.29", "0.36", "0.36", "1", "1", "1", "A1", "2", "6", "D4", "D4", "D4", "4", "2", "0.33", "0.33", "0.33", "0.33", "0.33", "0.33", "x", "x", "x", "0.1990955502737", "2", "2", "0.794298877174678", "0.733823824", "A", "2", "1", "1.537598087", "2", "13", "7", "1", "G", "A", });
            dt.Rows.Add(new object[] { "True", "1.825975532", "", "0.46", "0.46", "0.46", "0.46", "", "0.46", "0.46", "2", "2", "2", "A2", "3", "1", "D1", "D1", "D1", "1", "1", "0.06", "0.06", "0.06", "0.06", "0.06", "0.06", "x", "x", "x", "0.974035728527615", "1", "3", "0.724524100713686", "2.407241051", "A", "2", "2", "2.858660958", "7", "0.679468096619325", "1", "2", "A", "B", });
            dt.Rows.Add(new object[] { "True", "7.261284571", "", "0.32", "0.32", "0.32", "0.32", "0.77", "0.32", "0.32", "2", "2", "2", "A2", "3", "2", "D2", "D2", "D2", "2", "2", "0.97", "0.97", "0.97", "0.97", "0.97", "0.97", "x", "x", "x", "0.966035274217998", "2", "3", "0.730616385236094", "6.463195512", "A", "2", "3", "7.36601117", "7.36601117", "0.937718868655884", "3", "2", "C", "B", });
            dt.Rows.Add(new object[] { "True", "7.483543022", "", "0.11", "0.11", "0.11", "0.11", "0.51", "0.11", "0.11", "2", "2", "2", "A2", "4", "4", "D3", "D3", "D3", "3", "1", "0.73", "0.73", "0.73", "0.73", "0.73", "0.73", "x", "x", "x", "", "", "", "", "8.969025291", "B", "3", "1", "9.683744634", "9.683744634", "0.438363156181912", "5", "2", "E", "B", });
            dt.Rows.Add(new object[] { "True", "10.05311469", "", "0.24", "0.24", "0.24", "0.24", "0.01", "0.24", "0.24", "2", "2", "2", "A2", "4", "6", "D4", "D4", "D4", "4", "2", "0.21", "0.21", "0.21", "0.21", "0.21", "0.21", "x", "x", "x", "", "", "", "", "2.595751775", "B", "3", "2", "2.616275921", "2.616275921", "0.977636657227435", "7", "2", "G", "B", });
            dt.Rows.Add(new object[] { "True", "5.511177567", "", "0.36", "0.36", "0.36", "0.36", "", "0.36", "0.36", "3", "3", "3", "A3", "5", "1", "D1", "D1", "D1", "1", "1", "0.44", "0.44", "0.44", "0.44", "0.44", "0.44", "x", "x", "x", "", "", "", "", "5.717092739", "B", "3", "3", "6.066355151", "6.066355151", "0.701240363645833", "1", "3", "A", "C", });
            dt.Rows.Add(new object[] { "True", "6.833200829", "", "0.29", "0.29", "0.29", "0.29", "0.66", "0.29", "0.29", "3", "3", "3", "A3", "5", "2", "D2", "D2", "D2", "2", "2", "0.64", "0.64", "0.64", "0.64", "0.64", "0.64", "x", "x", "x", "", "", "", "", "4.16087994", "B", "4", "1", "4.870291139", "4.870291139", "0.838436761292166", "3", "3", "C", "C", });
            dt.Rows.Add(new object[] { "True", "9.150455657", "", "0.48", "0.48", "0.48", "0.48", "0.91", "0.48", "0.48", "3", "3", "3", "A3", "6", "4", "D3", "D3", "D3", "3", "1", "0.47", "0.47", "0.47", "0.47", "0.47", "0.47", "x", "y", "x", "", "", "", "", "3.951893108", "B", "4", "2", "4.448731133", "4.448731133", "0.328615408557135", "5", "3", "E", "C", });
            dt.Rows.Add(new object[] { "True", "6.118593122", "", "0.93", "0.93", "0.93", "0.93", "0.83", "0.93", "0.93", "3", "3", "3", "A3", "6", "6", "D4", "D4", "D4", "4", "2", "0.96", "0.96", "0.96", "0.96", "0.96", "0.96", "x", "y", "x", "", "", "", "", "6.536920116", "B", "4", "3", "6.718672815", "6.718672815", "0.635137296588129", "7", "3", "G", "C", });
            dt.Rows.Add(new object[] { "True", "3.292410415", "", "0.56", "0.56", "0.56", "0.56", "", "0.56", "0.56", "4", "4", "4", "A4", "7", "1", "D1", "D1", "D1", "1", "1", "0.68", "0.68", "0.68", "0.68", "0.68", "0.68", "y", "y", "y", "", "", "", "", "2.187754576", "C", "5", "1", "2.338702625", "2.338702625", "10", "2", "1", "B", "A", });
            dt.Rows.Add(new object[] { "True", "4.284204212", "", "0.67", "0.67", "0.67", "0.67", "0.89", "0.67", "0.67", "4", "4", "4", "A4", "7", "2", "D2", "D2", "D2", "2", "2", "0.66", "0.66", "0.66", "0.66", "0.66", "0.66", "y", "y", "y", "", "", "", "", "2.365602227", "C", "5", "2", "2.469641191", "2.469641191", "11", "4", "1", "D", "A", });
            dt.Rows.Add(new object[] { "True", "9.601493399", "", "0.36", "0.36", "0.36", "0.36", "0", "0.36", "0.36", "4", "4", "4", "A4", "8", "4", "D3", "D3", "D3", "3", "1", "0.94", "0.94", "0.94", "0.94", "0.94", "0.94", "y", "y", "y", "", "", "", "", "1.819919367", "C", "5", "3", "2.141109042", "2.141109042", "12", "6", "1", "F", "A", });
            dt.Rows.Add(new object[] { "True", "7.677091651", "", "0.55", "0.55", "0.55", "0.55", "0.95", "0.55", "0.55", "4", "4", "4", "A4", "8", "6", "D4", "D4", "D4", "4", "2", "0.47", "0.47", "0.47", "0.47", "0.47", "0.47", "y", "y", "y", "", "", "", "", "9.093133675", "C", "6", "1", "9.58324267", "9.58324267", "13", "8", "1", "Q", "A", });
            dt.Rows.Add(new object[] { "True", "6.277852888", "", "0.51", "0.51", "0.51", "0.51", "", "0.51", "0.51", "5", "5", "5", "A5", "9", "1", "D1", "D1", "D1", "1", "1", "0.47", "0.47", "0.47", "0.47", "0.47", "0.47", "y", "y", "y", "", "", "", "", "0.980650612", "C", "6", "2", "1.048459637", "1.048459637", "0.506656691498279", "2", "2", "B", "B", });
            dt.Rows.Add(new object[] { "True", "7.083701168", "", "0.4", "0.4", "0.4", "0.4", "0.66", "0.4", "0.4", "5", "5", "5", "A5", "9", "2", "D2", "D2", "D2", "2", "2", "0.61", "0.61", "0.61", "0.61", "0.61", "0.61", "y", "y", "y", "", "", "", "", "4.121418956", "C", "6", "3", "4.569624021", "4.569624021", "0.479987420492732", "4", "2", "D", "B", });
            dt.Rows.Add(new object[] { "True", "4.515318914", "", "0.34", "0.34", "0.34", "0.34", "0.8", "0.34", "0.34", "5", "5", "5", "A5", "10", "4", "D3", "D3", "D3", "3", "1", "0.78", "0.78", "0.78", "0.78", "0.78", "0.78", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "0.673124077108385", "6", "2", "F", "B", });
            dt.Rows.Add(new object[] { "True", "10.87272934", "", "0.51", "0.51", "0.51", "0.51", "0.02", "0.51", "0.51", "5", "5", "5", "A5", "10", "6", "D4", "D4", "D4", "4", "2", "0.3", "0.3", "0.3", "0.3", "0.3", "0.3", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "0.192390261122053", "8", "2", "Q", "B", });
            dt.Rows.Add(new object[] { "True", "2.833155261", "", "1", "", "1", "1", "", "1", "", "6", "", "", "A6", "11", "1", "D1", "D2", "D1", "1", "1", "0.2", "x", "", "-0.6", "0", "0.2", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "0.935208350922343", "2", "3", "B", "C", });
            dt.Rows.Add(new object[] { "True", "2.583890328", "", "0.61", "0.61", "0.61", "0.61", "0.6", "0.61", "", "6", "6", "", "A6", "11", "2", "D2", "D2", "D2", "2", "2", "0.41", "0.41", "0.41", "0.41", "0.41", "0.41", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "0.785493831491179", "4", "3", "D", "C", });
            dt.Rows.Add(new object[] { "True", "4.936222294", "", "0.65", "0.65", "0.65", "0.65", "0.94", "0.65", "", "6", "6", "", "A6", "12", "4", "D3", "D3", "D3", "3", "1", "0.65", "0.65", "0.65", "0.65", "0.65", "0.65", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "0.643019469631139", "6", "3", "F", "C", });
            dt.Rows.Add(new object[] { "True", "8.239188535", "", "0.41", "0.41", "0.41", "0.41", "0.29", "0.41", "", "6", "6", "", "A6", "12", "6", "D4", "D4", "D4", "4", "2", "0.89", "0.89", "0.89", "0.89", "0.89", "0.89", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "0.697269327150342", "8", "3", "Q", "C", });
            dt.Rows.Add(new object[] { "True", "6.867032347", "", "0.99", "0.99", "0.99", "0.99", "", "0.99", "0.99", "7", "7", "7", "A7", "13", "1", "D1", "D1", "D1", "1", "1", "0.83", "0.83", "0.83", "0.83", "0.83", "0.83", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.579536541", "", "0.86", "0.86", "0.86", "0.86", "0.77", "0.86", "0.86", "7", "7", "7", "A7", "13", "2", "D2", "D2", "D2", "2", "2", "0.6", "0.6", "0.6", "0.6", "0.6", "0.6", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "8.143669301", "", "0.64", "0.64", "0.64", "0.64", "0.51", "0.64", "0.64", "7", "7", "7", "A7", "14", "4", "D3", "D3", "D3", "3", "1", "0.7", "0.7", "0.7", "0.7", "0.7", "0.7", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "10.09983919", "", "0.86", "0.86", "0.86", "0.86", "0.01", "0.86", "0.86", "7", "7", "7", "A7", "14", "6", "D4", "D4", "D4", "4", "2", "0.85", "0.85", "0.85", "0.85", "0.85", "0.85", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "4.735714824", "", "0.31", "0.31", "0.31", "0.31", "", "0.31", "0.31", "8", "8", "8", "A8", "15", "1", "D1", "D1", "D1", "1", "1", "0.68", "0.68", "0.68", "0.68", "0.68", "0.68", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.80323595", "", "0.19", "0.19", "0.19", "0.19", "0.66", "0.19", "0.19", "8", "8", "8", "A8", "15", "2", "D2", "D2", "D2", "2", "2", "0.2", "0.2", "0.2", "0.2", "0.2", "0.2", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "9.867030164", "", "0.21", "0.21", "0.21", "0.21", "0.91", "0.21", "0.21", "8", "8", "8", "A8", "16", "4", "D3", "D3", "D3", "3", "1", "0.31", "0.31", "0.31", "0.31", "0.31", "0.31", "x", "y", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "9.681530124", "", "0.36", "0.36", "0.36", "0.36", "0.83", "0.36", "0.36", "8", "8", "8", "A8", "16", "6", "D4", "D4", "D4", "4", "2", "0.56", "0.56", "0.56", "0.56", "0.56", "0.56", "x", "y", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.668629944", "", "0.41", "0.41", "0.41", "0.41", "", "0.41", "0.41", "9", "9", "9", "A9", "17", "1", "D1", "D1", "D1", "1", "1", "0.43", "0.43", "0.43", "0.43", "0.43", "0.43", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.42416509", "", "0.47", "0.47", "0.47", "0.47", "0.89", "0.47", "0.47", "9", "9", "9", "A9", "17", "2", "D2", "D2", "D2", "2", "2", "0.28", "0.28", "0.28", "0.28", "0.28", "0.28", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "5.831212286", "", "0.16", "0.16", "0.16", "0.16", "0", "0.16", "0.16", "9", "9", "9", "A9", "18", "4", "D3", "D3", "D3", "3", "1", "0.39", "0.39", "0.39", "0.39", "0.39", "0.39", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "8.145112739", "", "0.81", "0.81", "0.81", "0.81", "0.95", "0.81", "0.81", "9", "9", "9", "A9", "18", "6", "D4", "D4", "D4", "4", "2", "0.78", "0.78", "0.78", "0.78", "0.78", "0.78", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "5.334908831", "", "0.9", "0.9", "0.9", "0.9", "", "0.9", "0.9", "10", "10", "10", "A10", "19", "1", "D1", "D1", "D1", "1", "1", "0.39", "0.39", "0.39", "0.39", "0.39", "0.39", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.922583299", "", "0.72", "0.72", "0.72", "0.72", "0.66", "0.72", "0.72", "10", "10", "10", "A10", "19", "2", "D2", "D2", "D2", "2", "2", "0.71", "0.71", "0.71", "0.71", "0.71", "0.71", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "4.889999703", "", "0.87", "0.87", "0.87", "0.87", "0.8", "0.87", "0.87", "10", "10", "10", "A10", "20", "4", "D3", "D3", "D3", "3", "1", "0.51", "0.51", "0.51", "0.51", "0.51", "0.51", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "9.267634598", "", "0.02", "0.02", "0.02", "0.02", "0.02", "0.02", "0.02", "10", "10", "10", "A10", "20", "6", "D4", "D4", "D4", "4", "2", "0.4", "0.4", "0.4", "0.4", "0.4", "0.4", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.223398993", "", "0.29", "0.14", "0.65", "0.39", "", "0.77", "0.87", "11", "11", "11", "A11", "21", "1", "", "D2", "D1", "1", "1", "0.83", "0.04", "0.84", "0.18", "0.44", "0.19", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "4.999098531", "", "0.25", "0.57", "0.43", "0.58", "0.6", "0.49", "0.13", "11", "11", "11", "A11", "21", "2", "", "D2", "D2", "2", "2", "0.09", "0.88", "0.53", "0.93", "0.37", "0.99", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.424053641", "", "0.76", "0.95", "0.26", "0.67", "0.94", "0.9", "0.04", "11", "11", "11", "A11", "22", "4", "", "D3", "D3", "3", "1", "0.25", "0.68", "0.72", "0.22", "0.07", "0.36", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "7.330363504", "", "0.71", "0.3", "0.99", "0.2", "0.29", "0.6", "0.58", "11", "11", "11", "A11", "22", "6", "", "D4", "D4", "4", "2", "0.01", "0.04", "0.24", "0.73", "0.49", "0.25", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.611501029", "", "0.36", "0.66", "0.23", "0.12", "", "0.1", "0.07", "12", "12", "12", "A12", "23", "1", "", "D1", "D1", "1", "1", "0.13", "0.05", "0.35", "0.06", "0.87", "0.51", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.96553019", "", "0.46", "0.46", "0.27", "1", "0.77", "0.49", "0.68", "12", "12", "12", "A12", "23", "2", "", "D2", "D2", "2", "2", "0.54", "0.89", "0.44", "0.17", "0.76", "0.33", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "4.241386061", "", "0.45", "0.52", "0.44", "0.52", "0.51", "0.58", "0.68", "12", "12", "12", "A12", "24", "4", "", "D3", "D3", "3", "1", "0.92", "0.71", "0.67", "0.5", "0.58", "0.4", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "11.64289215", "", "0.25", "0.13", "0.84", "0.93", "0.01", "0.75", "0.57", "12", "12", "12", "A12", "24", "6", "", "D4", "D4", "4", "2", "0.53", "0.54", "0.43", "0.09", "0.86", "0.91", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "3.775624124", "", "0.34", "0.77", "0.22", "0.26", "", "0.11", "0.38", "13", "13", "13", "A13", "25", "1", "", "D1", "D1", "1", "1", "0.84", "0.81", "0.67", "0.45", "0.66", "0.08", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "5.102299131", "", "0.14", "0.25", "0.91", "0.28", "0.66", "0.56", "0.14", "13", "13", "13", "A13", "25", "2", "", "D2", "D2", "2", "2", "0.16", "0.63", "0.27", "0.43", "0.85", "0.35", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "7.84197955", "", "0.69", "0.55", "0.66", "0.66", "0.91", "0.31", "0.56", "13", "13", "13", "A13", "26", "4", "", "D3", "D3", "3", "1", "0.02", "0.98", "0.24", "0.35", "0.3", "0.83", "x", "y", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.760567005", "", "0.52", "0.41", "0.06", "0.83", "0.83", "0.11", "0.81", "13", "13", "13", "A13", "26", "6", "", "D4", "D4", "4", "2", "0.93", "0.84", "0.9", "0.66", "0.42", "0.71", "x", "y", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.322933035", "", "0.01", "0.57", "0.35", "0.14", "", "0.68", "0.72", "14", "14", "14", "A14", "27", "1", "", "D1", "D1", "1", "1", "0.68", "0.83", "0.65", "0.7", "0.46", "0.91", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.241125364", "", "0.72", "0.41", "0.2", "0.49", "0.89", "0.48", "0.51", "14", "14", "14", "A14", "27", "2", "", "D2", "D2", "2", "2", "0.27", "0.83", "0.49", "0.39", "0.35", "0.96", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.45742267", "", "0.47", "0.99", "0.55", "0.99", "0", "0.85", "0.08", "14", "14", "14", "A14", "28", "4", "", "D3", "D3", "3", "1", "0.76", "0.7", "0.61", "0.72", "0.78", "0.03", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "9.675728671", "", "0.82", "0.84", "0.49", "0.44", "0.95", "0.67", "0.15", "14", "14", "14", "A14", "28", "6", "", "D4", "D4", "4", "2", "0.71", "0.47", "0.9", "0.88", "0.96", "0.86", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "5.977318396", "", "0.72", "0.78", "0.17", "0.76", "", "0.33", "0.69", "15", "15", "15", "A15", "29", "1", "", "D1", "D1", "1", "1", "0.86", "0.52", "0.34", "0.88", "0.92", "0.42", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.383093501", "", "0.96", "0.13", "0.38", "0.28", "0.66", "0.2", "0.11", "15", "15", "15", "A15", "29", "2", "", "D2", "D2", "2", "2", "0.61", "0.27", "0.47", "0.2", "0.88", "0.8", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "7.078017354", "", "0.46", "0.19", "0.04", "0.61", "0.8", "0.98", "0.33", "15", "15", "15", "A15", "30", "4", "", "D3", "D3", "3", "1", "0.83", "0.97", "0.66", "0.79", "0.94", "0.9", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "11.55171565", "", "0.5", "0.52", "0.9", "0.53", "0.02", "0.26", "0.5", "15", "15", "15", "A15", "30", "6", "", "D4", "D4", "4", "2", "0.75", "0.8", "0.73", "0.61", "0.01", "0.2", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "4.36407643", "", "0.23", "0.42", "0.27", "0.23", "", "0.31", "0.14", "16", "16", "16", "A16", "31", "1", "", "D2", "D1", "1", "1", "0.87", "0.74", "0.57", "0.01", "1", "0.51", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "3.388664565", "", "0.42", "0.01", "0.55", "0.25", "0.6", "0.79", "0.65", "16", "16", "16", "A16", "31", "2", "", "D2", "D2", "2", "2", "0.19", "0.19", "0.52", "0.88", "0.72", "0.96", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "8.355323036", "", "0.85", "0.67", "0.38", "0.04", "0.94", "0.02", "0.61", "16", "16", "16", "A16", "32", "4", "", "D3", "D3", "3", "1", "0.23", "0.85", "0.02", "0.21", "0.44", "0.31", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "7.487817071", "", "0.69", "0.48", "0.31", "0.93", "0.29", "0.9", "0.03", "16", "16", "16", "A16", "32", "6", "", "D4", "D4", "4", "2", "0.45", "0.05", "0.8", "0.39", "0.04", "0.83", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "3.57414719", "", "0.59", "0.99", "0.03", "0.95", "", "0.2", "0.97", "17", "17", "17", "A17", "33", "1", "", "D1", "D1", "1", "1", "0.02", "0.81", "0.17", "0.78", "0.13", "0.68", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "4.225716027", "", "0.34", "0.23", "0.88", "0.74", "0.77", "0.29", "0.06", "17", "17", "17", "A17", "33", "2", "", "D2", "D2", "2", "2", "0.62", "0.17", "0.03", "0.93", "0.7", "0.48", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "8.636038048", "", "0.54", "0.01", "0.35", "0.2", "0.51", "0.32", "0.6", "17", "17", "17", "A17", "34", "4", "", "D3", "D3", "3", "1", "0.44", "0.17", "0.29", "0.53", "0.57", "0.11", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.348998633", "", "0.78", "0.51", "0.5", "0.19", "0.01", "0.66", "0.56", "17", "17", "17", "A17", "34", "6", "", "D4", "D4", "4", "2", "0.43", "0.11", "0.45", "0.42", "0.67", "0.57", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.562233371", "", "0.36", "0.83", "0.85", "0.82", "", "0.99", "0.2", "18", "18", "18", "A18", "35", "1", "", "D1", "D1", "1", "1", "0.66", "0.83", "0.45", "0.06", "0.94", "0.92", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "2.203238749", "", "0.4", "0.92", "0.06", "0.33", "0.66", "0.85", "0.05", "18", "18", "18", "A18", "35", "2", "", "D2", "D2", "2", "2", "0.53", "0.36", "0.4", "0.28", "0.84", "0.98", "x", "x", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "9.713285962", "", "0.96", "0.18", "0.21", "0.78", "0.91", "0.71", "0.52", "18", "18", "18", "A18", "36", "4", "", "D3", "D3", "3", "1", "0.49", "0.37", "0.67", "0.16", "0.12", "0.18", "x", "y", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.551917052", "", "0.16", "0.61", "0.51", "0.79", "0.83", "0.35", "0.8", "18", "18", "18", "A18", "36", "6", "", "D4", "D4", "4", "2", "0.24", "0.5", "0.56", "0.72", "0.38", "0.48", "x", "y", "x", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "6.362958082", "", "0.45", "0.5", "0.4", "0.47", "", "0.07", "0.86", "19", "19", "19", "A19", "37", "1", "", "D1", "D1", "1", "1", "0.13", "0.86", "0.81", "0.43", "0.69", "0.19", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "4.949229585", "", "0.48", "0.53", "0.21", "0.06", "0.89", "0.27", "0.37", "19", "19", "19", "A19", "37", "2", "", "D2", "D2", "2", "2", "0.84", "0.34", "0.31", "0.3", "0.29", "0.77", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "7.499543507", "", "0.18", "0.04", "0.88", "0.52", "0", "0.98", "0.23", "19", "19", "19", "A19", "38", "4", "", "D3", "D3", "3", "1", "0.45", "0.53", "0.16", "0.19", "0.36", "0.3", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "9.887500471", "", "0.73", "0.89", "0.15", "0.62", "0.95", "0.04", "0.72", "19", "19", "19", "A19", "38", "6", "", "D4", "D4", "4", "2", "0.37", "0.83", "0.65", "0.59", "0.01", "0.7", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "1.352947907", "", "0.29", "0.64", "0", "0.48", "", "0.41", "0.01", "20", "20", "20", "A20", "39", "1", "", "D1", "D1", "1", "1", "0.34", "0.5", "0.59", "0.88", "0.45", "0.55", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "7.137458619", "", "0.12", "0.92", "0.48", "0.47", "0.66", "0.57", "0.3", "20", "20", "20", "A20", "39", "2", "", "D2", "D2", "2", "2", "0.5", "0.18", "0.55", "0.78", "0.04", "0.38", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "7.581225903", "", "0.12", "0.61", "0.87", "0.6", "0.8", "0.22", "0.9", "20", "20", "20", "A20", "40", "4", "", "D3", "D3", "3", "1", "0.62", "0.69", "0.05", "0.03", "0.42", "0.38", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "9.223098467", "", "0.91", "0.28", "0.7", "0.97", "0.02", "0.63", "0.22", "20", "20", "20", "A20", "40", "6", "", "D4", "D4", "4", "2", "0.45", "0.51", "0.37", "0.6", "0.77", "0.29", "y", "y", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });

            return dt;
        }

        private Dataset GetDataset()
        {
            Dataset dataset = new Dataset
            {
                DatasetID = 6,
                DatasetName = "_test dataset.xlsx [unpairedttest]",
                DateUpdated = new DateTime(2018, 11, 16, 9, 14, 35),
                TheData = "SilveRSelected,Resp 1,Resp2,Resp 3,Resp4,Resp 5,Resp 6,Resp 7,Resp8,Resp:9,Resp-10,Resp^11,Treat1,Treat2,Treat3,Treat4,Treat(5,Treat�6,Treat:7,Treat}8,PVTestresponse1,PVTestresponse2,PVTestgroup\r\nTrue,65,65,65,x,,-2,0,-2,65,65,0.1,A,A,1,A,1,A,A,A,1,1,1\r\nTrue,32,,32,32,32,32,32,0.1,32,32,0.1,A,A,1,A,1,A,A,A,2,2,1\r\nTrue,543,,543,543,543,543,543,0.2,543,543,0.2,A,A,1,A,1,A,A,A,3,3,1\r\nTrue,675,,675,675,675,675,675,0.1,675,675,0.1,A,A,1,B,1,A,A,A,4,4,1\r\nTrue,876,,876,876,876,876,876,0.2,876,876,0.2,A,A,1,B,1,A,A,A,11,10,2\r\nTrue,54,,54,54,54,54,54,0.3,54,54,0.3,A,A,1,B,1,A,A,A,12,11,2\r\nTrue,432,,,432,432,432,432,0.45,432,432,0.45,B,B,2,C,2,B,B,B,13,12,2\r\nTrue,564,,,564,564,564,564,0.2,564,564,0.2,B,B,2,C,2,B,B,,14,13,2\r\nTrue,76,,,76,76,76,76,0.14,76,76,0.14,B,B,2,C,2,B,B,,,,\r\nTrue,54,,,54,54,54,54,0.2,54,54,0.2,B,B,2,D,3,B,B,,,,\r\nTrue,32,,,32,32,32,32,0.1,32,32,0.1,B,B,2,D,3,B,B,,,,\r\nTrue,234,,,234,234,234,234,0.4,234,234,0.4,B,,2,D,3,B,B,,,,",
                VersionNo = 1
            };

            return dataset;
        }
    }
}