using Moq;
using SilveR.Models;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Silver.UnitTests
{
    public class RepeatedMeasuresParametricAnalysisModelTests
    {
        [Fact]
        public void ScriptFileName_ReturnsCorrectString()
        {
            //Arrange
            RepeatedMeasuresParametricAnalysisModel sut = new RepeatedMeasuresParametricAnalysisModel();

            //Act
            string result = sut.ScriptFileName;

            //Assert
            Assert.Equal("SingleMeasuresParametricAnalysis", result);
        }

        [Fact]
        public void TransformationsList_ReturnsCorrectList()
        {
            //Arrange
            RepeatedMeasuresParametricAnalysisModel sut = new RepeatedMeasuresParametricAnalysisModel();

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
            RepeatedMeasuresParametricAnalysisModel sut = new RepeatedMeasuresParametricAnalysisModel();

            //Act
            IEnumerable<string> result = sut.SignificancesList;

            //Assert
            Assert.IsAssignableFrom<IEnumerable<string>>(result);
            Assert.Equal(new List<string>() { "0.1", "0.05", "0.01", "0.001" }, result);
        }

        [Fact]
        public void CovariancesList_ReturnsCorrectList()
        {
            //Arrange
            RepeatedMeasuresParametricAnalysisModel sut = new RepeatedMeasuresParametricAnalysisModel();

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
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(1);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(TestDataTable.GetTestDataTable());

            RepeatedMeasuresParametricAnalysisModel sut = new RepeatedMeasuresParametricAnalysisModel(mockDataset.Object);

            //Act
            sut.Response = "Resp1";
            sut.Treatments = new List<string>() { "Cat1", "Cat2" };
            sut.OtherDesignFactors = new List<string>() { "Cat3" };
            sut.Covariates = new List<string>() { "Resp3" };
            sut.PrimaryFactor = "Cat1";

            sut.SelectedEffect = "Cat1";

            sut.ResponseTransformation = "None";
            sut.CovariateTransformation = "None";

            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Resp1,Resp3,Cat1,Cat2,Cat3,catfact,scatterPlotColumn,mainEffect", result[0]);
            Assert.Equal(33, result.Count()); //as blank reponses are removed
            Assert.StartsWith("0.928850779,0.009809005", result[32]);

            //scatterplot check
            Assert.Contains(",D E,", result[24]);

            //mainEffect check
            Assert.EndsWith(",D", result[24]);
        }

        [Fact]
        public void ExportData_MultipleTreatments_ReturnsCorrectStringArray()
        {
            //Arrange
            Mock<IDataset> mockDataset = new Mock<IDataset>();
            mockDataset.Setup(x => x.DatasetID).Returns(1);
            mockDataset.Setup(x => x.DatasetToDataTable()).Returns(TestDataTable.GetTestDataTable());

            RepeatedMeasuresParametricAnalysisModel sut = new RepeatedMeasuresParametricAnalysisModel(mockDataset.Object);

            //Act
            sut.Response = "Resp1";
            sut.Treatments = new List<string>() { "Cat1", "Cat2" };
            sut.PrimaryFactor = "Cat1";

            sut.SelectedEffect = "Cat1 * Cat2";

            sut.ResponseTransformation = "None";
            sut.CovariateTransformation = "None";

            string[] result = sut.ExportData();

            //Assert
            Assert.Equal("Resp1,Cat1,Cat2,catfact,scatterPlotColumn,mainEffect", result[0]);

            //mainEffect check
            Assert.EndsWith(",D E", result[24]);
        }

        [Fact]
        public void GetArguments_ReturnsCorrectArguments()
        {
            //Arrange
            RepeatedMeasuresParametricAnalysisModel sut = new RepeatedMeasuresParametricAnalysisModel(null);

            //Act
            sut.Response = "Resp1";
            sut.Treatments = new List<string>() { "Cat1", "Cat2" };
            sut.PrimaryFactor = "Cat1";
            sut.OtherDesignFactors = new List<string>() { "Cat3" };
            sut.Covariates = new List<string>() { "Resp3" };
            sut.SelectedEffect = "Cat1 * Cat2";
            sut.ResponseTransformation = "Log10";
            sut.CovariateTransformation = "ArcSine";

            sut.ANOVASelected = true;
            sut.Significance = "0.2";

            List<Argument> result = sut.GetArguments().ToList();

            //Assert
            var respResult = result.Single(x => x.Name == "Response");
            Assert.Equal("Resp1", respResult.Value);

            var treatmentsResult = result.Single(x => x.Name == "Treatments");
            Assert.Equal("Cat1,Cat2", treatmentsResult.Value);

            var otherDesignFactorsResult = result.Single(x => x.Name == "OtherDesignFactors");
            Assert.Equal("Cat3", otherDesignFactorsResult.Value);

            var covariatesResult = result.Single(x => x.Name == "Covariates");
            Assert.Equal("Resp3", covariatesResult.Value);

            var responseTransformationResult = result.Single(x => x.Name == "ResponseTransformation");
            Assert.Equal("Log10", responseTransformationResult.Value);

            var covariateTransformationResult = result.Single(x => x.Name == "CovariateTransformation");
            Assert.Equal("ArcSine", covariateTransformationResult.Value);

            var primaryFactorResult = result.Single(x => x.Name == "PrimaryFactor");
            Assert.Equal("Cat1", primaryFactorResult.Value);

            var selectedEffectResult = result.Single(x => x.Name == "SelectedEffect");
            Assert.Equal("Cat1 * Cat2", selectedEffectResult.Value);

            var anovaSelectedResult = result.Single(x => x.Name == "ANOVASelected");
            Assert.Equal("True", anovaSelectedResult.Value);

            var significanceResult = result.Single(x => x.Name == "Significance");
            Assert.Equal("0.2", significanceResult.Value);
        }

        [Fact]
        public void LoadArguments_ReturnsCorrectArguments()
        {
            //Arrange
            RepeatedMeasuresParametricAnalysisModel sut = new RepeatedMeasuresParametricAnalysisModel(null);

            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument { Name = "Response", Value = "Resp1" });
            arguments.Add(new Argument { Name = "Treatments", Value = "Cat1,Cat2" });
            arguments.Add(new Argument { Name = "OtherDesignFactors", Value = "Cat3,Cat4" });
            arguments.Add(new Argument { Name = "ResponseTransformation", Value = "Log10" });
            arguments.Add(new Argument { Name = "Covariates", Value = "Resp3" });
            arguments.Add(new Argument { Name = "PrimaryFactor", Value = "Cat2" });
            arguments.Add(new Argument { Name = "CovariateTransformation", Value = "ArcSine" });
            arguments.Add(new Argument { Name = "ANOVASelected", Value = "False" });
            arguments.Add(new Argument { Name = "PRPlotSelected", Value = "True" });
            arguments.Add(new Argument { Name = "NormalPlotSelected", Value = "False" });
            arguments.Add(new Argument { Name = "Significance", Value = "0.9" });
            arguments.Add(new Argument { Name = "SelectedEffect", Value = "Cat1 * Cat2" });
            arguments.Add(new Argument { Name = "LSMeansSelected", Value = "True" });
            //arguments.Add(new Argument { Name = "AllPairwise", Value = "Tukey" });
            //arguments.Add(new Argument { Name = "ComparisonsBackToControl", Value = "Bonferroni" });
            //arguments.Add(new Argument { Name = "ControlGroup", Value = "A" });

            //Act
            sut.LoadArguments(arguments);

            //Assert
            Assert.Equal("Resp1", sut.Response);
            Assert.Equal(new List<string> { "Cat1", "Cat2" }, sut.Treatments);
            Assert.Equal(new List<string> { "Cat3", "Cat4" }, sut.OtherDesignFactors);
            Assert.Equal("Log10", sut.ResponseTransformation);
            Assert.Equal(new List<string> { "Resp3" }, sut.Covariates);
            Assert.Equal("Cat2", sut.PrimaryFactor);
            Assert.Equal("ArcSine", sut.CovariateTransformation);
            Assert.False(sut.ANOVASelected);
            Assert.True(sut.PRPlotSelected);
            Assert.False(sut.NormalPlotSelected);
            Assert.Equal("0.9", sut.Significance);
            Assert.Equal("Cat1 * Cat2", sut.SelectedEffect);
            Assert.True(sut.LSMeansSelected);
            //Assert.Equal("Tukey", sut.AllPairwise);
            //Assert.Equal("Bonferroni", sut.ComparisonsBackToControl);
            //Assert.Equal("A", sut.ControlGroup);
        }


        //[Fact]
        //public void GetCommandLineArguments_ReturnsCorrectString()
        //{
        //    //Arrange
        //    RepeatedMeasuresParametricAnalysisModel sut = new RepeatedMeasuresParametricAnalysisModel(null);

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
    }
}