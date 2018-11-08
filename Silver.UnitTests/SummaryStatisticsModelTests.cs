using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests
{
    public class SummaryStatisticsModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            SummaryStatisticsModel sut = new SummaryStatisticsModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("SummaryStatistics", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            SummaryStatisticsModel sut = new SummaryStatisticsModel();

            //Act
            IEnumerable<string> result = sut.TransformationsList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine" }, result);
        }


        [Fact]
        public void ExportData_ReturnsCorrectStringArray()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(1);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(TestDataTable.GetTestDataTable());

            SummaryStatisticsModel sut = new SummaryStatisticsModel(mockDataset.Object);

            //Act
            sut.Responses = new List<string>() { "Resp1", "Resp3" };
            sut.FirstCatFactor = "Cat1";
            sut.SecondCatFactor = "Cat2";
            sut.ThirdCatFactor = "Cat3";
            sut.FourthCatFactor = "Cat5";

            sut.Transformation = "None";

            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Resp1,Resp3,Cat1,Cat2,Cat3,Cat5,catfact", result[0]);
            Assert.Equal(34, result.Count());
            Assert.StartsWith("0.928850779,0.009809005", result[33]);

            //catfactor check
            Assert.EndsWith("D E D 2", result[24]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            SummaryStatisticsModel sut = new SummaryStatisticsModel(null);

            //Act
            sut.Responses = new List<string>() { "Resp1", "Resp 2" };
            sut.FirstCatFactor = "Cat1";
            sut.SecondCatFactor = "Cat2";
            sut.Mean = true;
            sut.N = false;
            sut.StandardDeviation = true;
            sut.Variance = false;
            sut.StandardErrorOfMean = true;
            sut.MinAndMax = false;
            sut.MedianAndQuartiles = true;
            sut.Significance = 80;
            sut.Transformation = "None";

            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var respResult = result.Single(x => x.Name == "Responses");
            Assert.Equal("Resp1,Resp 2", respResult.Value);

            var firstCatFactorResult = result.Single(x => x.Name == "FirstCatFactor");
            Assert.Equal("Cat1", firstCatFactorResult.Value);

            var secondCatFactorResult = result.Single(x => x.Name == "SecondCatFactor");
            Assert.Equal("Cat2", secondCatFactorResult.Value);

            var meanResult = result.Single(x => x.Name == "Mean");
            Assert.Equal("True", meanResult.Value);

            var nResult = result.Single(x => x.Name == "N");
            Assert.Equal("False", nResult.Value);

            var significanceResult = result.Single(x => x.Name == "Significance");
            Assert.Equal("80", significanceResult.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            SummaryStatisticsModel sut = new SummaryStatisticsModel(null);

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "Responses", Value = "Resp1,Resp 2" });
            arguments.Add(new Argument { Name = "FirstCatFactor", Value = "Cat1" });
            arguments.Add(new Argument { Name = "SecondCatFactor", Value = "Cat2" });
            arguments.Add(new Argument { Name = "ThirdCatFactor", Value = "Cat3" });
            arguments.Add(new Argument { Name = "FourthCatFactor", Value = "Cat4" });
            arguments.Add(new Argument { Name = "Transformation", Value = "Log10" });
            arguments.Add(new Argument { Name = "ByCategoriesAndOverall", Value = "True" });
            arguments.Add(new Argument { Name = "CoefficientOfVariation", Value = "False" });
            arguments.Add(new Argument { Name = "ConfidenceInterval", Value = "True" });
            arguments.Add(new Argument { Name = "Mean", Value = "False" });
            arguments.Add(new Argument { Name = "MedianAndQuartiles", Value = "True" });
            arguments.Add(new Argument { Name = "MinAndMax", Value = "False" });
            arguments.Add(new Argument { Name = "N", Value = "True" });
            arguments.Add(new Argument { Name = "NormalProbabilityPlot", Value = "False" });
            arguments.Add(new Argument { Name = "Significance", Value = "90" });
            arguments.Add(new Argument { Name = "StandardDeviation", Value = "True" });
            arguments.Add(new Argument { Name = "StandardErrorOfMean", Value = "False" });
            arguments.Add(new Argument { Name = "Variance", Value = "True" });

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal(new List<string>() { "Resp1", "Resp 2" }, sut.Responses);
            Assert.Equal("Cat1", sut.FirstCatFactor);
            Assert.Equal("Cat2", sut.SecondCatFactor);
            Assert.Equal("Cat3", sut.ThirdCatFactor);
            Assert.Equal("Cat4", sut.FourthCatFactor);
            Assert.Equal("Log10", sut.Transformation);
            Assert.True(sut.ByCategoriesAndOverall);
            Assert.False(sut.CoefficientOfVariation);
            Assert.True(sut.ConfidenceInterval);
            Assert.False(sut.Mean);
            Assert.True(sut.MedianAndQuartiles);
            Assert.False(sut.MinAndMax);
            Assert.True(sut.N);
            Assert.False(sut.NormalProbabilityPlot);
            Assert.Equal(90, sut.Significance);
            Assert.True(sut.StandardDeviation);
            Assert.False(sut.StandardErrorOfMean);
            Assert.True(sut.Variance);
        }

        //[Fact]
        //public void GetCommandLineArguments_ReturnsCorrectString()
        //{
        //    //Arrange
        //    SummaryStatisticsModel sut = new SummaryStatisticsModel(null);

        //    sut.Responses = new List<string>() { "Resp1", "Resp 2" };
        //    sut.FirstCatFactor = "Cat1";
        //    sut.SecondCatFactor = "Cat2";
        //    sut.Mean = true;
        //    sut.N = false;
        //    sut.StandardDeviation = true;
        //    sut.Variance = false;
        //    sut.StandardErrorOfMean = true;
        //    sut.MinAndMax = false;
        //    sut.MedianAndQuartiles = true;
        //    sut.Significance = 80;

        //    sut.Transformation = "None";

        //    //Act
        //    string result = sut.GetCommandLineArguments();

        //    //Assert
        //   // Assert.Equal(, result);
        //}

        //[Fact]
        //public void VariablesUsedOnceOnly_ReturnsTrue()
        //{
        //    //Arrange
        //    SummaryStatisticsModel sut = new SummaryStatisticsModel(null);

        //    sut.Responses = new List<string>() { "Resp1", "Resp 2" };
        //    sut.FirstCatFactor = "Cat1";
        //    sut.SecondCatFactor = "Cat2";
        //    sut.ThirdCatFactor = "Cat3";
        //    sut.FourthCatFactor = "Cat4";

        //    //Act
        //    bool resultResponses = sut.VariablesUsedOnceOnly("Responses");
        //    bool resultCat1 = sut.VariablesUsedOnceOnly("FirstCatFactor");
        //    bool resultCat2 = sut.VariablesUsedOnceOnly("SecondCatFactor");
        //    bool resultCat3 = sut.VariablesUsedOnceOnly("ThirdCatFactor");
        //    bool resultCat4 = sut.VariablesUsedOnceOnly("FourthCatFactor");

        //    //Assert
        //    Assert.True(resultResponses);
        //    Assert.True(resultCat1);
        //    Assert.True(resultCat2);
        //    Assert.True(resultCat3);
        //    Assert.True(resultCat4);
        //}

        //[Fact]
        //public void VariablesUsedOnceOnly_ReturnsFalse()
        //{
        //    //Arrange
        //    SummaryStatisticsModel sut = new SummaryStatisticsModel(null);

        //    sut.Responses = new List<string>() { "Resp1", "Resp2" };
        //    sut.FirstCatFactor = "Resp1";
        //    sut.SecondCatFactor = "Cat2";
        //    sut.ThirdCatFactor = "Cat2";

        //    //Act
        //    bool resultResponses = sut.VariablesUsedOnceOnly("Responses");
        //    bool resultCat1 = sut.VariablesUsedOnceOnly("FirstCatFactor");
        //    bool resultCat2 = sut.VariablesUsedOnceOnly("SecondCatFactor");
        //    bool resultCat3 = sut.VariablesUsedOnceOnly("ThirdCatFactor");

        //    //Assert
        //    Assert.False(resultResponses);
        //    Assert.False(resultCat1);
        //    Assert.False(resultCat2);
        //    Assert.False(resultCat3);
        //}
    }
}