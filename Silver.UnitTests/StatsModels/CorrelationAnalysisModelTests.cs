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
    
    public class CorrelationAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            CorrelationAnalysisModel sut = new CorrelationAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("CorrelationAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            CorrelationAnalysisModel sut = new CorrelationAnalysisModel();

            //Act
            IEnumerable<string> result = sut.TransformationsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine" }, result);
        }

        [Fact]
        public void HypothesesList_ReturnsCorrectList()
        {
            //Arrange
            CorrelationAnalysisModel sut = new CorrelationAnalysisModel();

            //Act
            IEnumerable<string> result = sut.HypothesesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "2-sided", "Less than", "Greater than" }, result);
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
        public void MethodsList_ReturnsCorrectList()
        {
            //Arrange
            CorrelationAnalysisModel sut = new CorrelationAnalysisModel();

            //Act
            IEnumerable<string> result = sut.MethodsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "Pearson", "Kendall", "Spearman" }, result);
        }

        [Fact]
        public void ExportData_ReturnsCorrectStringArray()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(It.IsAny<int>);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(GetTestDataTable());

            CorrelationAnalysisModel sut = GetModel(mockDataset.Object);

            //Act
            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Respivs_sp_ivs2,Factorivs_sp_ivs1,Factorivs_sp_ivs2,Factorivs_sp_ivs3,Factorivs_sp_ivs4,catfact", result[0]);
            Assert.Equal(33, result.Count()); //as blank responses are removed
            Assert.StartsWith("0.51,0.86,D,2,2,D,D 2 2 D", result[32]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            CorrelationAnalysisModel sut = GetModel(GetDataset());

            //Act
            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var responses = result.Single(x => x.Name == "Responses");
            Assert.Equal("Resp 1,Resp 2", responses.Value);

            var firstCatFactor = result.Single(x => x.Name == "FirstCatFactor");
            Assert.Equal("Factor 1", firstCatFactor.Value);

            var secondCatFactor = result.Single(x => x.Name == "SecondCatFactor");
            Assert.Equal("Factor 2", secondCatFactor.Value);

            var thirdCatFactor = result.Single(x => x.Name == "ThirdCatFactor");
            Assert.Equal("Factor 3", thirdCatFactor.Value);

            var fourthCatFactor = result.Single(x => x.Name == "FourthCatFactor");
            Assert.Equal("Factor 4", fourthCatFactor.Value);

            var transformation = result.Single(x => x.Name == "Transformation");
            Assert.Equal("None", transformation.Value);

            var byCategoriesAndOverall = result.Single(x => x.Name == "ByCategoriesAndOverall");
            Assert.Equal("False", byCategoriesAndOverall.Value);

            var estimate = result.Single(x => x.Name == "CorrelationCoefficient");
            Assert.Equal("True", estimate.Value);

            var hypothesis = result.Single(x => x.Name == "Hypothesis");
            Assert.Equal("2-sided", hypothesis.Value);

            var matrixplot = result.Single(x => x.Name == "Matrixplot");
            Assert.Equal("False", matrixplot.Value);

            var method = result.Single(x => x.Name == "Method");
            Assert.Equal("Pearson", method.Value);

            var pValue = result.Single(x => x.Name == "PValue");
            Assert.Equal("True", pValue.Value);

            var significance = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.05", significance.Value);

            var testStatistic = result.Single(x => x.Name == "TestStatistic");
            Assert.Equal("True", testStatistic.Value);

            var scatterplot = result.Single(x => x.Name == "Scatterplot");
            Assert.Equal("False", scatterplot.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            CorrelationAnalysisModel sut = new CorrelationAnalysisModel(GetDataset());

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "Responses", Value = "Resp 1,Resp2" });
            arguments.Add(new Argument { Name = "FirstCatFactor", Value = "Factor 1" });
            arguments.Add(new Argument { Name = "SecondCatFactor", Value = "Factor 2" });
            arguments.Add(new Argument { Name = "ThirdCatFactor", Value = "Factor 3" });
            arguments.Add(new Argument { Name = "FourthCatFactor", Value = "Factor 4" });
            arguments.Add(new Argument { Name = "Transformation", Value = "Log10" });
            arguments.Add(new Argument { Name = "ByCategoriesAndOverall", Value = "False" });
            arguments.Add(new Argument { Name = "CorrelationCoefficient", Value = "True" });
            arguments.Add(new Argument { Name = "Hypothesis", Value = "2-sided" });
            arguments.Add(new Argument { Name = "Matrixplot", Value = "False" });
            arguments.Add(new Argument { Name = "Method", Value = "Pearson" });
            arguments.Add(new Argument { Name = "PValue", Value = "True" });
            arguments.Add(new Argument { Name = "TestStatistic", Value = "True" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.05" });
            arguments.Add(new Argument { Name = "Scatterplot", Value = "False" });

            Assert.Equal(15, arguments.Count);

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal(new List<string> { "Resp 1", "Resp2" }, sut.Responses);
            Assert.Equal("Factor 1", sut.FirstCatFactor);
            Assert.Equal("Factor 2", sut.SecondCatFactor);
            Assert.Equal("Factor 3", sut.ThirdCatFactor);
            Assert.Equal("Factor 4", sut.FourthCatFactor);
            Assert.Equal("Log10", sut.Transformation);
            Assert.False(sut.ByCategoriesAndOverall);
            Assert.True(sut.CorrelationCoefficient);
            Assert.False(sut.Matrixplot);
            Assert.Equal("2-sided", sut.Hypothesis);
            Assert.Equal("Pearson", sut.Method);
            Assert.True(sut.PValue);
            Assert.True(sut.TestStatistic);
            Assert.Equal("0.05", sut.Significance);
            Assert.False(sut.Scatterplot);
        }

        [Fact]
        public void GetCommandLineArguments_ReturnsCorrectString()
        {
            //Arrange
            CorrelationAnalysisModel sut = GetModel(GetDataset());

            //Act
            string result = sut.GetCommandLineArguments();

            //Assert
            Assert.Equal("Respivs_sp_ivs1,Respivs_sp_ivs2 None Factorivs_sp_ivs1 Factorivs_sp_ivs2 Factorivs_sp_ivs3 Factorivs_sp_ivs4 Pearson 2-sided Y Y Y N N 0.05 N", result);
        }


        private CorrelationAnalysisModel GetModel(IDataset dataset)
        {
            var model = new CorrelationAnalysisModel(dataset)
            {
                ByCategoriesAndOverall = false,
                CorrelationCoefficient = true,
                FirstCatFactor = "Factor 1",
                FourthCatFactor = "Factor 4",
                Hypothesis = "2-sided",
                Matrixplot = false,
                Method = "Pearson",
                PValue = true,
                Responses = new List<string>
                {
                    "Resp 1",
                    "Resp 2"
                },
                Scatterplot = false,
                SecondCatFactor = "Factor 2",
                Significance = "0.05",
                TestStatistic = true,
                ThirdCatFactor = "Factor 3",
                Transformation = "None"
            };

            return model;
        }

        private DataTable GetTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SilveRSelected");
            dt.Columns.Add("Resp 1");
            dt.Columns.Add("Resp 2");
            dt.Columns.Add("Resp 3");
            dt.Columns.Add("Resp 4");
            dt.Columns.Add("Resp 5");
            dt.Columns.Add("Resp 6");
            dt.Columns.Add("Resp 7");
            dt.Columns.Add("Resp 8");
            dt.Columns.Add("Factor 1");
            dt.Columns.Add("Factor 2");
            dt.Columns.Add("Factor 3");
            dt.Columns.Add("Factor 4");
            dt.Columns.Add("Factor 5");
            dt.Columns.Add("Factor 6");
            dt.Columns.Add("Factor 7");
            dt.Columns.Add("Factor 8");
            dt.Columns.Add("Factor 9");
            dt.Columns.Add("Fact1fact2");
            dt.Columns.Add("Fact2Fact3");
            dt.Columns.Add("Resp10");
            dt.Columns.Add("Resp11");
            dt.Columns.Add("Resp12");
            dt.Columns.Add("Resp13");
            dt.Columns.Add("Resp14");
            dt.Columns.Add("Resp15");
            dt.Columns.Add("Resp16");
            dt.Columns.Add("Resp17");
            dt.Columns.Add("Resp18");
            dt.Columns.Add("Resp19");
            dt.Columns.Add("Factor 10");
            dt.Columns.Add("Resp20");
            dt.Columns.Add("Resp21");
            dt.Columns.Add("Factor 11");
            dt.Columns.Add("Resp22");
            dt.Columns.Add("Resp23");
            dt.Columns.Add("Factor 12");
            dt.Columns.Add("Resp24");
            dt.Columns.Add("Resp25");
            dt.Columns.Add("Factor 13");
            dt.Columns.Add("Resp28");
            dt.Columns.Add("Resp29");
            dt.Columns.Add("Factor 15");
            dt.Columns.Add("Resp30");
            dt.Columns.Add("Resp31");
            dt.Columns.Add("Factor 16");
            dt.Columns.Add("Resp32");
            dt.Columns.Add("Resp33");
            dt.Columns.Add("Factor 17");
            dt.Columns.Add("Resp34");
            dt.Columns.Add("Resp35");
            dt.Columns.Add("Factor 18");
            dt.Columns.Add("Resp36");
            dt.Columns.Add("Resp37");
            dt.Columns.Add("Resp38");
            dt.Columns.Add("Resp39");
            dt.Columns.Add("Resp40");
            dt.Columns.Add("Resp41");
            dt.Columns.Add("Resp42");
            dt.Columns.Add("Resp43");
            dt.Columns.Add("Resp44");
            dt.Columns.Add("Resp45");
            dt.Columns.Add("Resp46");
            dt.Columns.Add("Resp47");
            dt.Rows.Add(new object[] { "True", "0.26", "0.04", "0.58", "", "x", "-1", "0", "2", "A", "1", "1", "", "f", "3", "A", "A", "1", "A1", "11", "3", "2", "4", "4", "3", "3", "34", "234", "2", "0.61", "1", "2", "0.61", "1", "2", "0.61", "1", "2", "0.61", "1", "2", "", "1", "2", "", "1", "2", "", "1", "2", "", "1", "0.45", "", "0.57", "", "0.57", "", "0.57", "", "0.26", "0.04", "0.58", "0.26", });
            dt.Rows.Add(new object[] { "True", "0.25", "0.93", "0.47", "", "0.25", "0.25", "0.25", "0.25", "A", "1", "2", "A", "A", "3", "A", "A", "1", "A1", "12", "", "", "3", "3", "4", "5", "234", "34", "0.25", "0.66", "2", "0.25", "0.66", "1", "0.25", "0.66", "1", "0.25", "0.66", "1", "0.25", "", "1", "0.25", "", "1", "0.25", "", "1", "0.25", "0.66", "1", "0.88", "", "0.88", "", "0.88", "", "0.88", "", "0.25", "0.93", "0.47", "0.79", });
            dt.Rows.Add(new object[] { "True", "0.21", "0.44", "0.26", "", "0.21", "0.21", "0.21", "0.21", "A", "1", "1", "A", "A", "3", "A", "A", "1", "A1", "11", "", "", "", "", "6", "2", "543", "234", "0.21", "0.57", "2", "0.21", "0.57", "2", "0.21", "0.57", "1", "0.21", "0.57", "1", "0.21", "", "1", "0.21", "", "1", "0.21", "0.57", "1", "0.21", "0.57", "1", "0.93", "", "0.93", "", "0.93", "", "0.93", "", "0.21", "0.44", "0.26", "0.01", });
            dt.Rows.Add(new object[] { "True", "0.61", "0.05", "0.45", "0.45", "0.61", "0.61", "0.61", "0.61", "A", "1", "2", "A", "A", "2", "A", "A", "1", "A1", "12", "", "", "", "", "", "", "2", "3", "0.61", "0.88", "2", "0.61", "0.88", "2", "0.61", "0.88", "2", "0.61", "0.88", "1", "0.61", "", "1", "0.61", "0.88", "1", "0.61", "0.88", "1", "0.61", "0.88", "1", "0.45", "", "0.45", "", "0.45", "", "0.45", "", "0.61", "0.05", "0.45", "0.55", });
            dt.Rows.Add(new object[] { "True", "0.66", "0.35", "0.2", "0.2", "0.66", "0.66", "0.66", "0.66", "A", "2", "1", "A", "A", "1", "A", "A", "2", "A2", "21", "", "", "", "", "", "", "", "", "0.66", "0.93", "2", "0.66", "0.93", "2", "0.66", "0.93", "2", "0.66", "0.93", "2", "0.66", "0.93", "1", "0.66", "0.93", "1", "0.66", "0.93", "1", "0.66", "0.93", "1", "0.08", "", "0.88", "", "0.88", "", "0.88", "", "0.66", "0.35", "0.2", "0.2", });
            dt.Rows.Add(new object[] { "True", "0.57", "0", "0.26", "0.26", "0.57", "0.57", "0.57", "0.57", "A", "2", "2", "A", "A", "2", "A", "A", "2", "A2", "22", "", "", "", "", "", "", "", "", "0.57", "0.45", "2", "0.57", "0.45", "2", "0.57", "0.45", "2", "0.57", "0.45", "2", "0.57", "0.45", "2", "0.57", "0.45", "2", "0.57", "0.45", "2", "0.57", "0.45", "2", "0.54", "", "0.93", "", "0.93", "", "0.93", "0.53", "0.57", "0", "0.26", "0.26", });
            dt.Rows.Add(new object[] { "True", "0.88", "0.4", "0.79", "0.79", "0.88", "0.88", "0.88", "0.88", "A", "2", "1", "A", "A", "1", "A", "A", "2", "A2", "21", "", "", "", "", "", "", "", "", "0.88", "0.08", "3", "0.88", "0.08", "2", "0.88", "0.08", "2", "0.88", "0.08", "2", "0.88", "0.08", "2", "0.88", "0.08", "2", "0.88", "0.08", "2", "0.88", "0.08", "2", "0.55", "", "0.45", "", "0.45", "0.15", "0.45", "0.15", "0.88", "0.4", "0.79", "0.79", });
            dt.Rows.Add(new object[] { "True", "0.93", "0.94", "0.01", "0.01", "0.93", "0.93", "0.93", "0.93", "A", "2", "2", "A", "A", "2", "A", "A", "2", "A2", "22", "", "", "", "", "", "", "", "", "0.93", "0.54", "3", "0.93", "0.54", "2", "0.93", "0.54", "2", "0.93", "0.54", "2", "0.93", "0.54", "2", "0.93", "0.54", "2", "0.93", "0.54", "2", "0.93", "0.54", "2", "0.54", "", "0.08", "0.61", "0.08", "0.61", "0.08", "0.61", "0.93", "0.94", "0.01", "0.01", });
            dt.Rows.Add(new object[] { "True", "0.45", "0.57", "0.55", "0.55", "0.45", "0.45", "0.45", "0.45", "B", "1", "1", "B", "A", "1", "A", "B", "1", "B1", "11", "", "", "", "", "", "", "", "", "0.45", "0.55", "3", "0.45", "0.55", "3", "0.45", "0.55", "3", "0.45", "0.55", "3", "0.45", "0.55", "2", "0.45", "0.55", "2", "0.45", "0.55", "2", "0.45", "0.55", "2", "0.45", "0.64", "0.54", "0.64", "0.54", "0.64", "0.54", "0.64", "0.45", "0.57", "0.55", "0.55", });
            dt.Rows.Add(new object[] { "True", "0.08", "0.94", "0.21", "0.21", "0.08", "0.08", "0.08", "0.08", "B", "1", "2", "B", "A", "2", "A", "B", "1", "B1", "12", "", "", "", "", "", "", "", "", "0.08", "0.7", "3", "0.88", "0.7", "3", "0.88", "0.7", "3", "0.88", "0.7", "3", "0.88", "0.7", "2", "0.88", "0.7", "2", "0.88", "0.7", "2", "0.88", "0.7", "2", "", "", "", "", "", "", "", "", "0.08", "0.94", "0.21", "0.21", });
            dt.Rows.Add(new object[] { "True", "0.54", "0.13", "1", "0.59", "0.54", "0.54", "0.54", "0.54", "B", "1", "1", "B", "A", "1", "A", "B", "1", "B1", "11", "", "", "", "", "", "", "", "", "0.54", "0.53", "3", "0.93", "0.53", "3", "0.93", "0.53", "3", "0.93", "0.53", "3", "0.93", "0.53", "3", "0.93", "0.53", "3", "0.93", "0.53", "3", "0.93", "0.53", "3", "", "", "", "", "", "", "", "", "0.54", "0.13", "1", "0.59", });
            dt.Rows.Add(new object[] { "True", "0.55", "0.17", "2", "0.11", "0.55", "0.55", "0.55", "0.55", "B", "1", "2", "B", "A", "2", "A", "B", "1", "B1", "12", "", "", "", "", "", "", "", "", "0.55", "0.15", "3", "0.45", "0.15", "3", "0.45", "0.15", "3", "0.45", "0.15", "3", "0.45", "0.15", "3", "0.45", "0.15", "3", "0.45", "0.15", "3", "0.45", "0.15", "3", "", "", "", "", "", "", "", "", "0.55", "0.17", "2", "0.11", });
            dt.Rows.Add(new object[] { "True", "0.7", "0.17", "3", "0.68", "0.7", "0.7", "0.7", "0.7", "B", "2", "1", "B", "A", "1", "A", "B", "2", "B2", "21", "", "", "", "", "", "", "", "", "", "", "", "0.08", "0.61", "3", "0.08", "0.61", "3", "0.08", "0.61", "3", "0.08", "0.61", "3", "0.08", "0.61", "3", "0.08", "0.61", "3", "0.08", "0.61", "3", "", "", "", "", "", "", "", "", "0.7", "0.17", "3", "0.68", });
            dt.Rows.Add(new object[] { "True", "0.53", "0.94", "1", "1", "0.53", "0.53", "0.53", "0.53", "B", "2", "2", "B", "A", "2", "A", "B", "2", "B2", "22", "", "", "", "", "", "", "", "", "", "", "", "0.54", "0.64", "3", "0.54", "0.64", "3", "0.54", "0.64", "3", "0.54", "0.64", "3", "0.54", "0.64", "3", "0.54", "0.64", "3", "0.54", "0.64", "3", "", "", "", "", "", "", "", "", "0.53", "0.94", "1", "1", });
            dt.Rows.Add(new object[] { "True", "0.15", "0.07", "0.81", "0.81", "0.15", "0.15", "0.15", "0.15", "B", "2", "1", "B", "A", "1", "A", "B", "2", "B2", "21", "", "", "", "", "", "", "", "", "", "", "", "0.55", "0.49", "3", "0.55", "0.49", "3", "0.55", "0.49", "3", "0.55", "0.49", "3", "0.55", "0.49", "3", "0.55", "0.49", "3", "0.55", "0.49", "3", "", "", "", "", "", "", "", "", "0.15", "0.07", "0.81", "0.81", });
            dt.Rows.Add(new object[] { "True", "0.61", "0.96", "0.82", "0.82", "0.61", "0.61", "0.61", "0.61", "B", "2", "2", "B", "A", "2", "A", "B", "2", "B2", "22", "", "", "", "", "", "", "", "", "", "", "", "0.54", "0.98", "3", "0.54", "0.98", "3", "0.54", "0.98", "3", "0.54", "0.98", "3", "0.54", "0.98", "3", "0.54", "0.98", "3", "0.54", "0.98", "3", "", "", "", "", "", "", "", "", "0.61", "0.96", "0.82", "0.82", });
            dt.Rows.Add(new object[] { "True", "0.64", "0.98", "0.18", "0.18", "0.64", "0.64", "0.64", "0.64", "C", "1", "1", "C", "A", "1", "A", "C", "1", "C1", "11", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.49", "0.36", "0.56", "0.56", "0.49", "0.49", "0.49", "0.49", "C", "1", "2", "C", "A", "2", "A", "C", "1", "C1", "12", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.98", "0.62", "0.16", "0.16", "0.98", "0.98", "0.98", "0.98", "C", "1", "1", "C", "A", "1", "A", "C", "1", "C1", "11", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", });
            dt.Rows.Add(new object[] { "True", "0.82", "0.14", "0.62", "0.62", "0.82", "0.82", "0.82", "0.82", "C", "1", "2", "C", "A", "2", "A", "C", "1", "C1", "12", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.82", "0.14", "0.62", "0.62", });
            dt.Rows.Add(new object[] { "True", "0.74", "0.13", "0.53", "0.53", "0.74", "0.74", "0.74", "0.74", "C", "2", "1", "C", "A", "1", "A", "C", "2", "C2", "21", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.74", "0.13", "0.53", "0.53", });
            dt.Rows.Add(new object[] { "True", "0.9", "0.96", "0.26", "0.26", "0.9", "0.9", "0.9", "0.9", "C", "2", "2", "C", "A", "2", "A", "C", "2", "C2", "22", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.9", "0.96", "0.26", "0.26", });
            dt.Rows.Add(new object[] { "True", "0.86", "0.49", "0.5", "0.5", "0.86", "0.86", "0.86", "0.86", "C", "2", "1", "C", "A", "1", "A", "C", "2", "C2", "21", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.86", "0.49", "0.5", "0.5", });
            dt.Rows.Add(new object[] { "True", "0.95", "0.57", "0.04", "0.04", "0.95", "0.95", "0.95", "0.95", "C", "2", "2", "C", "A", "2", "A", "C", "2", "C2", "22", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.95", "0.57", "0.04", "0.04", });
            dt.Rows.Add(new object[] { "True", "0.77", "0.84", "0.61", "0.61", "0.77", "0.77", "0.77", "0.77", "D", "1", "1", "D", "A", "1", "A", "D", "1", "D1", "11", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.77", "0.84", "0.61", "0.61", });
            dt.Rows.Add(new object[] { "True", "0.45", "0.23", "0", "0", "0.45", "0.45", "0.45", "0.45", "D", "1", "2", "D", "A", "2", "A", "D", "1", "D1", "12", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.45", "0.23", "0", "0", });
            dt.Rows.Add(new object[] { "True", "0.76", "0.69", "0.57", "0.57", "0.76", "0.76", "0.76", "0.76", "D", "1", "1", "D", "A", "1", "A", "D", "1", "D1", "11", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.76", "0.69", "0.57", "0.57", });
            dt.Rows.Add(new object[] { "True", "0.63", "0.36", "0.82", "0.82", "0.63", "0.63", "0.63", "0.63", "D", "1", "2", "D", "A", "2", "A", "D", "1", "D1", "12", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.63", "0.36", "0.82", "0.82", });
            dt.Rows.Add(new object[] { "True", "0.59", "0.11", "0.16", "0.16", "0.59", "0.59", "0.59", "0.59", "D", "2", "1", "D", "A", "1", "A", "D", "1", "D2", "21", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.59", "0.11", "0.16", "0.16", });
            dt.Rows.Add(new object[] { "True", "0.5", "0.16", "0.71", "0.71", "0.5", "0.5", "0.5", "0.5", "D", "2", "2", "D", "A", "2", "A", "D", "1", "D2", "22", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.5", "0.16", "0.71", "0.71", });
            dt.Rows.Add(new object[] { "True", "0.95", "0.53", "0.25", "0.25", "0.95", "0.95", "0.95", "0.95", "D", "2", "1", "D", "A", "1", "A", "D", "1", "D2", "21", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.95", "0.53", "0.25", "0.25", });
            dt.Rows.Add(new object[] { "True", "0.51", "0.86", "0.58", "0.58", "0.51", "0.51", "0.51", "0.51", "D", "2", "2", "D", "A", "2", "A", "D", "2", "D2", "22", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0.51", "0.86", "0.58", "0.58", });

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