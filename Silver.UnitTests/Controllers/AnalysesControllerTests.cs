﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SilveR.Controllers;
using SilveR.Models;
using SilveR.Services;
using SilveR.StatsModels;
using SilveR.Validators;
using SilveR.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.UnitTests.Controllers
{
    public class AnalysesControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.HasAnalyses()).ReturnsAsync(It.IsAny<bool>());

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);
            sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            IActionResult result = await sut.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task GetAnalyses_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetAnalyses()).ReturnsAsync(GetAnalyses());

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = await sut.GetAnalyses();

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);

            IEnumerable<AnalysisViewModel> analysesViewModel = (IEnumerable<AnalysisViewModel>)jsonResult.Value;
            AnalysisViewModel analysis = analysesViewModel.First();
            Assert.Equal(45, analysis.AnalysisID);
        }

        [Fact]
        public async Task AnalysisDataSelector_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetDatasetViewModels()).ReturnsAsync(GetDatasets());
            mockRepository.Setup(x => x.GetScripts()).ReturnsAsync(GetScripts());

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = await sut.AnalysisDataSelector("SummaryStatistics");

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            AnalysisDataSelectorViewModel analysisDataSelectorViewModel = (AnalysisDataSelectorViewModel)viewResult.Model;

            Assert.Equal(19, analysisDataSelectorViewModel.Scripts.Count());
            Assert.Equal("_test dataset.xlsx [regression] v1", analysisDataSelectorViewModel.Datasets.First().DatasetNameVersion);
            Assert.Equal("Summary Statistics", analysisDataSelectorViewModel.AnalysisDisplayName);
            Assert.Equal("SummaryStatistics", analysisDataSelectorViewModel.AnalysisName);
        }

        [Fact]
        public void AnalysisDataSelectorPost_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = sut.AnalysisDataSelector(new AnalysisDataSelectorViewModel());

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Analysis_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            AnalysisDataSelectorViewModel analysisDataSelectorViewModel = new AnalysisDataSelectorViewModel { AnalysisName = "SummaryStatistics", SelectedDatasetID = 1 };

            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetDatasetByID(It.IsAny<int>())).ReturnsAsync(GetDataset());

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = await sut.Analysis(analysisDataSelectorViewModel);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<AnalysisModelBase>(viewResult.Model);

            Assert.Equal("SummaryStatistics", viewResult.ViewName);
        }

        [Fact]
        public async Task SummaryStatistics_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<SummaryStatisticsModel> mockModel = new Mock<SummaryStatisticsModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.SummaryStatistics(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task SingleMeasuresParametricAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<SingleMeasuresParametricAnalysisModel> mockModel = new Mock<SingleMeasuresParametricAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.SingleMeasuresParametricAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task RepeatedMeasuresParametricAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<RepeatedMeasuresParametricAnalysisModel> mockModel = new Mock<RepeatedMeasuresParametricAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.RepeatedMeasuresParametricAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task PValueAdjustment_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(null);

            Mock<PValueAdjustmentUserBasedInputsModel> mockModel = new Mock<PValueAdjustmentUserBasedInputsModel>();
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.PValueAdjustmentUserBasedInputs(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task PairedTTestAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<PairedTTestAnalysisModel> mockModel = new Mock<PairedTTestAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.PairedTTestAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task UnpairedTTestAnalysis_ReturnsARedirectToActionResultt()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<UnpairedTTestAnalysisModel> mockModel = new Mock<UnpairedTTestAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.UnpairedTTestAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task OneSampleTTestAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<OneSampleTTestAnalysisModel> mockModel = new Mock<OneSampleTTestAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.OneSampleTTestAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task CorrelationAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<CorrelationAnalysisModel> mockModel = new Mock<CorrelationAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.CorrelationAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task LinearRegressionAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<LinearRegressionAnalysisModel> mockModel = new Mock<LinearRegressionAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.LinearRegressionAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task DoseResponseAndNonLinearRegressionAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<DoseResponseAndNonLinearRegressionAnalysisModel> mockModel = new Mock<DoseResponseAndNonLinearRegressionAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.DoseResponseAndNonLinearRegressionAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task NonParametricAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<NonParametricAnalysisModel> mockModel = new Mock<NonParametricAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.NonParametricAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task ChiSquaredAndFishersExactTest_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<ChiSquaredAndFishersExactTestModel> mockModel = new Mock<ChiSquaredAndFishersExactTestModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.ChiSquaredAndFishersExactTest(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task SurvivalAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<SurvivalAnalysisModel> mockModel = new Mock<SurvivalAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.SurvivalAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task GraphicalAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<GraphicalAnalysisModel> mockModel = new Mock<GraphicalAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.GraphicalAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task ComparisonOfMeansPowerAnalysisDatasetBasedInputs_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel> mockModel = new Mock<ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.ComparisonOfMeansPowerAnalysisDatasetBasedInputs(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task ComparisonOfMeansPowerAnalysisUserBasedInputs_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<ComparisonOfMeansPowerAnalysisUserBasedInputsModel> mockModel = new Mock<ComparisonOfMeansPowerAnalysisUserBasedInputsModel>();
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.ComparisonOfMeansPowerAnalysisUserBasedInputs(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task NestedDesignAnalysis_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<NestedDesignAnalysisModel> mockModel = new Mock<NestedDesignAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.NestedDesignAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task IncompleteFactorialParametricAnalysis_ReturnsARedirectToActionResultt()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<IncompleteFactorialParametricAnalysisModel> mockModel = new Mock<IncompleteFactorialParametricAnalysisModel>(dataset);
            mockModel.Setup(x => x.Validate()).Returns(new ValidationInfo());

            //Act
            IActionResult result = await sut.IncompleteFactorialParametricAnalysis(mockModel.Object, false);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        [Fact]
        public async Task SummaryStatistics_ValidationFailed_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<SummaryStatisticsModel> mockModel = new Mock<SummaryStatisticsModel>(dataset);
            var validationInfo = new ValidationInfo();
            validationInfo.AddErrorMessage("Test Error");
            mockModel.Setup(x => x.Validate()).Returns(validationInfo);

            //Act
            IActionResult result = await sut.SummaryStatistics(mockModel.Object, false);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AnalysisModelBase>(viewResult.Model);

            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task SummaryStatistics_HasWarnings_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<SummaryStatisticsModel> mockModel = new Mock<SummaryStatisticsModel>(dataset);
            var validationInfo = new ValidationInfo();
            validationInfo.AddWarningMessage("Test Warning");
            mockModel.Setup(x => x.Validate()).Returns(validationInfo);

            //Act
            IActionResult result = await sut.SummaryStatistics(mockModel.Object, false);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<AnalysisModelBase>(viewResult.Model);

            Assert.NotNull(viewResult.ViewData["WarningMessages"]);
        }

        [Fact]
        public async Task SummaryStatistics_HasWarningsIgnored_ReturnsARedirectToActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var dataset = GetDataset();
            AnalysesController sut = SetupAnalysisControllerForAnalysisRun(dataset);

            Mock<SummaryStatisticsModel> mockModel = new Mock<SummaryStatisticsModel>(dataset);
            var validationInfo = new ValidationInfo();
            validationInfo.AddWarningMessage("Test Warning");
            mockModel.Setup(x => x.Validate()).Returns(validationInfo);

            //Act
            IActionResult result = await sut.SummaryStatistics(mockModel.Object, true); //ignore warnings

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Processing", redirectToActionResult.ActionName);
            Assert.NotNull(redirectToActionResult.RouteValues.Single(x => x.Key == "analysisGuid").Value);
        }

        private AnalysesController SetupAnalysisControllerForAnalysisRun(Dataset dataset)
        {
            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetDatasetByID(It.IsAny<int>())).ReturnsAsync(dataset);
            mockRepository.Setup(x => x.GetScriptByName(It.IsAny<string>())).ReturnsAsync(It.IsAny<Script>());
            mockRepository.Setup(x => x.AddAnalysis(It.IsAny<Analysis>())).Returns(Task.CompletedTask);

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            mockBackgroundTaskQueue.Setup(x => x.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()));

            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();
            mockProcessorService.Setup(x => x.Execute(It.IsAny<String>()));

            return new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);
        }

        [Fact]
        public async Task Reanalyse_DatasetRequired_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Analysis analysis = GetSummaryStatsAnalysis();

            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetAnalysisComplete(It.IsAny<string>())).ReturnsAsync(analysis);

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = await sut.ReAnalyse(analysis.AnalysisGuid);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("SummaryStatistics", viewResult.ViewName);
            Assert.IsType<SummaryStatisticsModel>(viewResult.Model);
        }

        [Fact]
        public async Task Reanalyse_NoDatasetRequired_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Analysis analysis = GetAnalyses().Single(x => x.AnalysisID == 34);

            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetAnalysisComplete(It.IsAny<string>())).ReturnsAsync(analysis);

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = await sut.ReAnalyse(analysis.AnalysisGuid);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("PValueAdjustmentUserBasedInputs", viewResult.ViewName);
            Assert.IsType<PValueAdjustmentUserBasedInputsModel>(viewResult.Model);
        }

        [Fact]
        public async Task Reanalyse_DatasetMissing_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Analysis analysis = GetAnalyses().First();
            analysis.Dataset = null;

            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetAnalysisComplete(It.IsAny<string>())).ReturnsAsync(analysis);

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);
            sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            IActionResult result = await sut.ReAnalyse(analysis.AnalysisGuid);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ViewResults_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Analysis analysis = GetAnalyses().First();

            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetAnalysis(It.IsAny<string>())).ReturnsAsync(analysis);

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = await sut.ViewResults(analysis.AnalysisGuid);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Analysis analysisReturned = Assert.IsType<Analysis>(viewResult.Model);

            Assert.Contains("InVivoStat Single Measure Parametric Analysis</h1>", analysisReturned.HtmlOutput);
        }

        [Fact]
        public async Task ViewResults_NoOutput_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Analysis analysis = GetAnalyses().First();
            analysis.HtmlOutput = null;

            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetAnalysis(It.IsAny<string>())).ReturnsAsync(analysis);

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);
            sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            IActionResult result = await sut.ViewResults(analysis.AnalysisGuid);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewLog", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ResultsForExport_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Analysis analysis = GetAnalyses().First();

            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetAnalysis(It.IsAny<string>())).ReturnsAsync(analysis);

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = await sut.ResultsForExport(It.IsAny<string>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Analysis analysisReturned = Assert.IsType<Analysis>(viewResult.Model);

            Assert.Contains("InVivoStat Single Measure Parametric Analysis</h1>", analysisReturned.HtmlOutput);
        }


        [Fact]
        public void Processing_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.HasAnalysisCompleted(It.IsAny<string>())).ReturnsAsync(It.IsAny<bool>);

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = sut.Processing(It.IsAny<string>());

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AnalysisCompleted_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.HasAnalysisCompleted(It.IsAny<string>())).ReturnsAsync(It.IsAny<bool>);

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = await sut.AnalysisCompleted(It.IsAny<string>());

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            Assert.False((bool)jsonResult.Value);
        }

        [Fact]
        public async Task ViewLog_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.GetAnalysis(It.IsAny<string>())).ReturnsAsync(new Analysis(It.IsAny<Dataset>()) { RProcessOutput = "Test Output" });

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);
            sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            IActionResult result = await sut.ViewLog(It.IsAny<string>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Test Output", viewResult.ViewData["AnalysisLog"]);
        }

        [Fact]
        public async Task Destroy_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Mock<ISilveRRepository> mockRepository = new Mock<ISilveRRepository>();
            mockRepository.Setup(x => x.DeleteAnalysis(It.IsAny<int>())).Returns(Task.CompletedTask);

            Mock<IBackgroundTaskQueue> mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            Mock<IRProcessorService> mockProcessorService = new Mock<IRProcessorService>();

            AnalysesController sut = new AnalysesController(mockRepository.Object, mockBackgroundTaskQueue.Object, mockProcessorService.Object);

            //Act
            IActionResult result = await sut.Destroy(It.IsAny<int>());

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            Assert.True((bool)jsonResult.Value);
        }


        private Dataset GetDataset()
        {
            var dataset = new Dataset
            {
                DatasetID = 6,
                DatasetName = "_test dataset.xlsx [unpairedttest]",
                DateUpdated = new DateTime(2018, 11, 16, 9, 14, 35),
                TheData = "SilveRSelected,Resp 1,Resp2,Resp 3,Resp4,Resp 5,Resp 6,Resp 7,Resp8,Resp:9,Resp-10,Resp^11,Treat1,Treat2,Treat3,Treat4,Treat(5,Treat£6,Treat:7,Treat}8,PVTestresponse1,PVTestresponse2,PVTestgroup\r\nTrue,65,65,65,x,,-2,0,-2,65,65,0.1,A,A,1,A,1,A,A,A,1,1,1\r\nTrue,32,,32,32,32,32,32,0.1,32,32,0.1,A,A,1,A,1,A,A,A,2,2,1\r\nTrue,543,,543,543,543,543,543,0.2,543,543,0.2,A,A,1,A,1,A,A,A,3,3,1\r\nTrue,675,,675,675,675,675,675,0.1,675,675,0.1,A,A,1,B,1,A,A,A,4,4,1\r\nTrue,876,,876,876,876,876,876,0.2,876,876,0.2,A,A,1,B,1,A,A,A,11,10,2\r\nTrue,54,,54,54,54,54,54,0.3,54,54,0.3,A,A,1,B,1,A,A,A,12,11,2\r\nTrue,432,,,432,432,432,432,0.45,432,432,0.45,B,B,2,C,2,B,B,B,13,12,2\r\nTrue,564,,,564,564,564,564,0.2,564,564,0.2,B,B,2,C,2,B,B,,14,13,2\r\nTrue,76,,,76,76,76,76,0.14,76,76,0.14,B,B,2,C,2,B,B,,,,\r\nTrue,54,,,54,54,54,54,0.2,54,54,0.2,B,B,2,D,3,B,B,,,,\r\nTrue,32,,,32,32,32,32,0.1,32,32,0.1,B,B,2,D,3,B,B,,,,\r\nTrue,234,,,234,234,234,234,0.4,234,234,0.4,B,,2,D,3,B,B,,,,",
                VersionNo = 1
            };

            return dataset;
        }

        private IList<Analysis> GetAnalyses()
        {
            var analyses = new List<Analysis>
            {
                new Analysis(It.IsAny<Dataset>())
                {
                    AnalysisID = 45,
                    Dataset = GetDataset(),
                    DatasetID = 3,
                    DatasetName = "_test dataset.xlsx [singlemeasures]",
                    DateAnalysed = new DateTime(2018, 11, 19, 18, 5, 27),
                    HtmlOutput = "\r\n<link rel=\"stylesheet\" type=\"text/css\" href='r2html.css'>\r\n\r\n <h1> InVivoStat Single Measure Parametric Analysis</h1>\r\n\r\n <h2> Response</h2>\r\n\r\n<p class='character'>The  Resp 1 response is currently being analysed by the Single Measures Parametric Analysis module.</p>\r\n\r\n <h2> Scatterplot of the raw data</h2>\r\n\r\n<p align=\"centre\"><img src='data:image/png;base64,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' border=\"1\" width=\"500\">\r\n<p class='character'><a href=\"C:\\Users\\robal\\AppData\\Local\\Temp\\2fbe4cd1-1b0c-4b03-82ad-24d594ae5195scatterPlot.pdf\">Click here to view the PDF of the scatterplot</a></p>\r\n\r\n<p class='character'>Tip: Use this plot to identify possible outliers.</p>\r\n\r\n <h2> Analysis of variance (ANOVA) table</h2>\r\n\r\n\r\n<p align=\"left\">\r\n<table cellspacing=\"0\" border=\"1\">\r\n<caption align=\"bottom\" class=\"captiondataframe\"></caption>\r\n<tr><td>\r\n\t<table border=\"0\" class=\"dataframe\">\r\n\t<tbody> \r\n\t<tr class=\"second\"> \r\n\t\t<th>Effect  </th>\r\n\t\t<th>Sums of squares  </th>\r\n\t\t<th>Degrees of freedom  </th>\r\n\t\t<th>Mean square  </th>\r\n\t\t<th>F-value  </th>\r\n\t\t<th>p-value</th> \r\n\t</tr> \r\n<tr> \r\n<td class=\"cellinside\">Treat1\r\n</td>\r\n<td class=\"cellinside\">3.07\r\n</td>\r\n<td class=\"cellinside\"> 3\r\n</td>\r\n<td class=\"cellinside\">1.023\r\n</td>\r\n<td class=\"cellinside\">18.60\r\n</td>\r\n<td class=\"cellinside\">< 0.0001\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">Residual\r\n</td>\r\n<td class=\"cellinside\">4.07\r\n</td>\r\n<td class=\"cellinside\">74\r\n</td>\r\n<td class=\"cellinside\">0.055\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n\t</tbody>\r\n</table>\r\n </td></tr></table>\r\n <br>\r\n\r\n<p class='character'>Comment: ANOVA table calculated using a Type III model fit, see Armitage et al. (2001).</p>\r\n\r\n<p class='character'>Conclusion: There is a statistically significant overall difference between the levels of Treat1. </p>\r\n\r\n<p class='character'>Tip: While it is a good idea to consider the overall tests in the ANOVA table, we should not rely on them when deciding whether or not to make pairwise comparisons.</p>\r\n\r\n <h2> Diagnostic plot</h2>\r\n\r\n <h3> Residuals vs. predicted plot</h3>\r\n\r\n<p align=\"centre\"><img src='data:image/png;base64,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' border=\"1\" width=\"500\">\r\n<p class='character'><a href=\"C:\\Users\\robal\\AppData\\Local\\Temp\\2fbe4cd1-1b0c-4b03-82ad-24d594ae5195residualplot.pdf\">Click here to view the PDF of the residuals vs. predicted plot</a></p>\r\n\r\n<p class='character'>Tip: On this plot look to see if the spread of the points increases as the predicted values increase. If so the response may need transforming.</p>\r\n\r\n<p class='character'>Tip: Any observation with a residual less than -3 or greater than 3 (SD) should be investigated as a possible outlier.</p>\r\n\r\n <h2> Analysis description</h2>\r\n\r\n<p class='character'>The data were analysed using a 1-way ANOVA approach, with Treat1 as the treatment factor. </p>\r\n\r\n<p class='character'>For more information on the theoretical approaches that are implemented within this module, see Bate and Clark (2014).</p>\r\n\r\n <h2> Statistical references</h2>\r\n\r\n<p class='character'>Bate ST and Clark RA. (2014). The Design and Statistical Analysis of Animal Experiments. Cambridge University Press.</p>\r\n\r\n<p class='character'><bf> Armitage P, Matthews JNS and Berry G. (2001). Statistical Methods in Medical Research. 4th edition; John Wiley & Sons. New York.<p>\r\n\r\n <h2> R references</h2>\r\n\r\n<p class='character'>R Development Core Team (2013). R: A language and environment for statistical computing. R Foundation for Statistical Computing, Vienna, Austria. URL http://www.R-project.org.</p>\r\n\r\n<p class='character'>Barret Schloerke, Jason Crowley, Di Cook, Heike Hofmann, Hadley Wickham, Francois Briatte, Moritz Marbach and Edwin Thoen (2014). GGally: Extension to ggplot2. R package version 0.4.5. http://CRAN.R-project.org/package=GGally</p>\r\n\r\n<p class='character'>Erich Neuwirth (2011). RColorBrewer: ColorBrewer palettes. R package  version 1.0-5. http://CRAN.R-project.org/package=RColorBrewer</p>\r\n\r\n<p class='character'>H. Wickham. ggplot2: elegant graphics for data analysis. Springer New York, 2009.</p>\r\n\r\n<p class='character'>Kamil Slowikowski (2018). ggrepel: Automatically Position Non-Overlapping Text Labels with 'ggplot2'. R package version 0.8.0. https://CRAN.R-project.org/package=ggrepel</p>\r\n\r\n<p class='character'>H. Wickham. Reshaping data with the reshape package. Journal of Statistical Software, 21(12), 2007.</p>\r\n\r\n<p class='character'>Hadley Wickham (2011). The Split-Apply-Combine Strategy for Data Analysis. Journal of Statistical Software, 40(1), 1-29. URL http://www.jstatsoft.org/v40/i01/.</p>\r\n\r\n<p class='character'>Hadley Wickham (2012). scales: Scale functions for graphics. R package version 0.2.3. http://CRAN.R-project.org/package=scales</p>\r\n\r\n<p class='character'>John Fox and Sanford Weisberg (2011). An {R} Companion to Applied Regression, Second Edition. Thousand Oaks CA: Sage. URL: http://socserv.socsci.mcmaster.ca/jfox/Books/Companion</p>\r\n\r\n<p class='character'>Lecoutre, Eric (2003). The R2HTML Package. R News, Vol 3. N. 3, Vienna, Austria.</p>\r\n\r\n<p class='character'>Louis Kates and Thomas Petzoldt (2012). proto: Prototype object-based programming. R package version 0.3-10. http://CRAN.R-project.org/package=proto</p>\r\n\r\n<p class='character'>Russell V. Lenth (2014). lsmeans: Least-Squares Means. R package version 2.00-1. http://CRAN.R-project.org/package=lsmeans</p>\r\n\r\n<p class='character'>Torsten Hothorn, Frank Bretz and Peter Westfall (2008). Simultaneous  Inference in General Parametric Models. Biometrical Journal 50(3),  346--363.</p>\r\n\r\n<p class='character'>Spencer Graves, Hans-Peter Piepho and Luciano Selzer with help from Sundar Dorai-Raj (2015). multcompView: Visualizations of Paired Comparisons. R package version 0.1-7. https://CRAN.R-project.org/package=multcompView</p>\r\n\r\n <h2> Analysis dataset</h2>\r\n\r\n\r\n<p align=\"left\">\r\n<table cellspacing=\"0\" border=\"1\">\r\n<caption align=\"bottom\" class=\"captiondataframe\"></caption>\r\n<tr><td>\r\n\t<table border=\"0\" class=\"dataframe\">\r\n\t<tbody> \r\n\t<tr class=\"second\"> \r\n\t\t<th>Observation  </th>\r\n\t\t<th>Resp 1  </th>\r\n\t\t<th>Treat1</th> \r\n\t</tr> \r\n<tr> \r\n<td class=\"cellinside\"> 1\r\n</td>\r\n<td class=\"cellinside\">1.17\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 2\r\n</td>\r\n<td class=\"cellinside\">1.30\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 3\r\n</td>\r\n<td class=\"cellinside\">1.72\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 4\r\n</td>\r\n<td class=\"cellinside\">1.17\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 5\r\n</td>\r\n<td class=\"cellinside\">1.39\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 6\r\n</td>\r\n<td class=\"cellinside\">1.44\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 7\r\n</td>\r\n<td class=\"cellinside\">1.60\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 8\r\n</td>\r\n<td class=\"cellinside\">1.57\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 9\r\n</td>\r\n<td class=\"cellinside\">1.33\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">10\r\n</td>\r\n<td class=\"cellinside\">1.57\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">11\r\n</td>\r\n<td class=\"cellinside\">1.66\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">12\r\n</td>\r\n<td class=\"cellinside\">1.47\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">13\r\n</td>\r\n<td class=\"cellinside\">1.72\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">14\r\n</td>\r\n<td class=\"cellinside\">1.17\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">15\r\n</td>\r\n<td class=\"cellinside\">1.06\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">16\r\n</td>\r\n<td class=\"cellinside\">1.24\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">17\r\n</td>\r\n<td class=\"cellinside\">1.60\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">18\r\n</td>\r\n<td class=\"cellinside\">0.99\r\n</td>\r\n<td class=\"cellinside\">D0\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">19\r\n</td>\r\n<td class=\"cellinside\">1.37\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">20\r\n</td>\r\n<td class=\"cellinside\">1.50\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">21\r\n</td>\r\n<td class=\"cellinside\">1.59\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">22\r\n</td>\r\n<td class=\"cellinside\">1.37\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">23\r\n</td>\r\n<td class=\"cellinside\">1.50\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">24\r\n</td>\r\n<td class=\"cellinside\">1.37\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">25\r\n</td>\r\n<td class=\"cellinside\">1.64\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">26\r\n</td>\r\n<td class=\"cellinside\">1.53\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">27\r\n</td>\r\n<td class=\"cellinside\">1.50\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">28\r\n</td>\r\n<td class=\"cellinside\">1.59\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">29\r\n</td>\r\n<td class=\"cellinside\">1.77\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">30\r\n</td>\r\n<td class=\"cellinside\">1.86\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">31\r\n</td>\r\n<td class=\"cellinside\">1.26\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">32\r\n</td>\r\n<td class=\"cellinside\">1.64\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">33\r\n</td>\r\n<td class=\"cellinside\">1.53\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">34\r\n</td>\r\n<td class=\"cellinside\">1.77\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">35\r\n</td>\r\n<td class=\"cellinside\">1.44\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">36\r\n</td>\r\n<td class=\"cellinside\">1.92\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">37\r\n</td>\r\n<td class=\"cellinside\">1.86\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">38\r\n</td>\r\n<td class=\"cellinside\">1.26\r\n</td>\r\n<td class=\"cellinside\">D1\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">39\r\n</td>\r\n<td class=\"cellinside\">1.26\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">40\r\n</td>\r\n<td class=\"cellinside\">1.77\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">41\r\n</td>\r\n<td class=\"cellinside\">1.67\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">42\r\n</td>\r\n<td class=\"cellinside\">1.64\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">43\r\n</td>\r\n<td class=\"cellinside\">1.53\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">44\r\n</td>\r\n<td class=\"cellinside\">1.77\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">45\r\n</td>\r\n<td class=\"cellinside\">1.92\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">46\r\n</td>\r\n<td class=\"cellinside\">1.37\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">47\r\n</td>\r\n<td class=\"cellinside\">1.86\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">48\r\n</td>\r\n<td class=\"cellinside\">1.26\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">49\r\n</td>\r\n<td class=\"cellinside\">1.80\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">50\r\n</td>\r\n<td class=\"cellinside\">1.19\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">51\r\n</td>\r\n<td class=\"cellinside\">1.37\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">52\r\n</td>\r\n<td class=\"cellinside\">1.44\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">53\r\n</td>\r\n<td class=\"cellinside\">1.92\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">54\r\n</td>\r\n<td class=\"cellinside\">1.37\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">55\r\n</td>\r\n<td class=\"cellinside\">1.50\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">56\r\n</td>\r\n<td class=\"cellinside\">1.59\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">57\r\n</td>\r\n<td class=\"cellinside\">1.80\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">58\r\n</td>\r\n<td class=\"cellinside\">1.19\r\n</td>\r\n<td class=\"cellinside\">D10\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">59\r\n</td>\r\n<td class=\"cellinside\">2.03\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">60\r\n</td>\r\n<td class=\"cellinside\">2.20\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">61\r\n</td>\r\n<td class=\"cellinside\">2.28\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">62\r\n</td>\r\n<td class=\"cellinside\">1.72\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">63\r\n</td>\r\n<td class=\"cellinside\">1.75\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">64\r\n</td>\r\n<td class=\"cellinside\">1.81\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">65\r\n</td>\r\n<td class=\"cellinside\">1.86\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">66\r\n</td>\r\n<td class=\"cellinside\">1.64\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">67\r\n</td>\r\n<td class=\"cellinside\">2.25\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">68\r\n</td>\r\n<td class=\"cellinside\">1.88\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">69\r\n</td>\r\n<td class=\"cellinside\">1.69\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">70\r\n</td>\r\n<td class=\"cellinside\">1.82\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">71\r\n</td>\r\n<td class=\"cellinside\">2.26\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">72\r\n</td>\r\n<td class=\"cellinside\">1.61\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">73\r\n</td>\r\n<td class=\"cellinside\">2.06\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">74\r\n</td>\r\n<td class=\"cellinside\">2.18\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">75\r\n</td>\r\n<td class=\"cellinside\">1.99\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">76\r\n</td>\r\n<td class=\"cellinside\">1.69\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">77\r\n</td>\r\n<td class=\"cellinside\">1.62\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">78\r\n</td>\r\n<td class=\"cellinside\">2.48\r\n</td>\r\n<td class=\"cellinside\">D3\r\n</td></tr>\r\n \r\n\t</tbody>\r\n</table>\r\n </td></tr></table>\r\n <br>\r\n\r\n <h2> Analysis options</h2>\r\n\r\n<p class='character'>Response variable: Resp 1</p>\r\n\r\n<p class='character'>Treatment factors: Treat1</p>\r\n\r\n<p class='character'>Model fitted: Treat1</p>\r\n\r\n<p class='character'>Output ANOVA table (Y/N): Y</p>\r\n\r\n<p class='character'>Output predicted vs. residual plot (Y/N): Y</p>\r\n\r\n<p class='character'>Output normal probability plot (Y/N): N</p>\r\n\r\n<p class='character'>Show Least Square predicted means (Y/N): N</p>\r\n\r\n<p class='character'>Significance level: 0.05</p>\r\n</bf>",
                    RProcessOutput = "[1] \"--vanilla\"                                                                       \r\n [2] \"--args\"                                                                          \r\n [3] \"C:\\\\Users\\\\robal\\\\AppData\\\\Local\\\\Temp\\\\2fbe4cd1-1b0c-4b03-82ad-24d594ae5195.csv\"\r\n [4] \"Respivs_sp_ivs1~Treat1\"                                                          \r\n [5] \"Respivs_sp_ivs1~scatterPlotColumn\"                                               \r\n [6] \"NULL\"                                                                            \r\n [7] \"None\"                                                                            \r\n [8] \"None\"                                                                            \r\n [9] \"NULL\"                                                                            \r\n[10] \"Treat1\"                                                                          \r\n[11] \"NULL\"                                                                            \r\n[12] \"Y\"                                                                               \r\n[13] \"Y\"                                                                               \r\n[14] \"N\"                                                                               \r\n[15] \"0.05\"                                                                            \r\n[16] \"Respivs_sp_ivs1~mainEffect\"                                                      \r\n[17] \"Treat1\"                                                                          \r\n[18] \"N\"                                                                               \r\n[19] \"NULL\"                                                                            \r\n[20] \"NULL\"                                                                            \r\n[21] \"NULL\"                                                                            \r\n[1] \"C:\\\\Users\\\\robal\\\\AppData\\\\Local\\\\Temp\\\\2fbe4cd1-1b0c-4b03-82ad-24d594ae5195.html\"\r\n\r\n\r\n\r\nAttaching package: 'reshape'\r\n\r\nThe following objects are masked from 'package:plyr':\r\n\r\n    rename, round_any\r\n\r\nLoading required package: mvtnorm\r\nLoading required package: survival\r\nLoading required package: TH.data\r\nLoading required package: MASS\r\n\r\nAttaching package: 'TH.data'\r\n\r\nThe following object is masked from 'package:MASS':\r\n\r\n    geyser\r\n\r\nLoading required package: carData\r\nThe 'lsmeans' package is being deprecated.\r\nUsers are encouraged to switch to 'emmeans'.\r\nSee help('transition') for more information, including how\r\nto convert 'lsmeans' objects and scripts to work with 'emmeans'.\r\n\r\nAnalysis by the R Processor took 16.29 seconds.",
                    Script = new Script
                    {
                        ScriptDisplayName = "Single Measures Parametric Analysis",
                        ScriptFileName = "SingleMeasuresParametricAnalysis",
                        ScriptID = 16,
                        RequiresDataset = true
                    },
                    ScriptID = 16,
                    Tag = null
                },
                new Analysis(It.IsAny<Dataset>())
                {
                    AnalysisID = 43,
                    Arguments = new System.Collections.Generic.HashSet<SilveR.Models.Argument>
                    {
                    },
                    Dataset = null,
                    DatasetID = 6,
                    DatasetName = "_test dataset.xlsx [unpairedttest]",
                    DateAnalysed = new DateTime(2018, 11, 18, 11, 0, 6),
                    HtmlOutput = "\r\n<link rel=\"stylesheet\" type=\"text/css\" href='r2html.css'>\r\n\r\n <h1> InVivoStat Correlation Analysis</h1>\r\n\r\n <h2> Responses</h2>\r\n\r\n<p class='character'>This module assesses the correlation between the response variables Resp 1 and Resp2. </p>\r\n\r\n <h2> Warning</h2>\r\n\r\n<p class='character'>Warning: One or more of the responses consists of only one or two observations, please remove these prior to the analysis as it is not possible to calculate correlations with them.</p>\r\n\r\n <h3> Scatterplot of the raw data</h3>\r\n\r\n<p align=\"centre\"><img src='data:image/png;base64,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' border=\"1\" width=\"500\">\r\n<p class='character'><a href=\"C:\\Users\\robal\\AppData\\Local\\Temp\\3d78da4f-bf93-4324-a7c9-73ffd19798ea12scatterPlot.pdf\">Click here to view the PDF of the scatterplot</a></p>\r\n\r\n<p class='character'>Warning: One or more of the lines have not been fitted as there is only one pair of responses available.</p>\r\n\r\n<p align=\"centre\"><img src='data:image/png;base64,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' border=\"1\" width=\"500\">\r\n<p class='character'><a href=\"C:\\Users\\robal\\AppData\\Local\\Temp\\3d78da4f-bf93-4324-a7c9-73ffd19798ea21scatterPlot.pdf\">Click here to view the PDF of the scatterplot</a></p>\r\n\r\n<p class='character'>Warning: One or more of the lines have not been fitted as there is only one pair of responses available.</p>\r\n\r\n<p class='character'>For more information on the theoretical approaches that are implemented within this module, see Bate and Clark (2014).</p>\r\n\r\n <h2> Statistical references</h2>\r\n\r\n<p class='character'>Bate ST and Clark RA. (2014). The Design and Statistical Analysis of Animal Experiments. Cambridge University Press.</p>\r\n\r\n<p class='character'>Snedecor GW and Cochran WG. (1989). Statistical Methods. 8th edition; Iowa State University Press, Iowa, USA.</p>\r\n\r\n <h2> R references</h2>\r\n\r\n<p class='character'>R Development Core Team (2013). R: A language and environment for statistical computing. R Foundation for Statistical Computing, Vienna, Austria. URL http://www.R-project.org.</p>\r\n\r\n<p class='character'>Barret Schloerke, Jason Crowley, Di Cook, Heike Hofmann, Hadley Wickham, Francois Briatte, Moritz Marbach and Edwin Thoen (2014). GGally: Extension to ggplot2. R package version 0.4.5. http://CRAN.R-project.org/package=GGally</p>\r\n\r\n<p class='character'>Erich Neuwirth (2011). RColorBrewer: ColorBrewer palettes. R package  version 1.0-5. http://CRAN.R-project.org/package=RColorBrewer</p>\r\n\r\n<p class='character'>H. Wickham. ggplot2: elegant graphics for data analysis. Springer New York, 2009.</p>\r\n\r\n<p class='character'>Kamil Slowikowski (2018). ggrepel: Automatically Position Non-Overlapping Text Labels with 'ggplot2'. R package version 0.8.0. https://CRAN.R-project.org/package=ggrepel</p>\r\n\r\n<p class='character'>H. Wickham. Reshaping data with the reshape package. Journal of Statistical Software, 21(12), 2007.</p>\r\n\r\n<p class='character'>Hadley Wickham (2011). The Split-Apply-Combine Strategy for Data Analysis. Journal of Statistical Software, 40(1), 1-29. URL http://www.jstatsoft.org/v40/i01/.</p>\r\n\r\n<p class='character'>Hadley Wickham (2012). scales: Scale functions for graphics. R package version 0.2.3. http://CRAN.R-project.org/package=scales</p>\r\n\r\n<p class='character'>Lecoutre, Eric (2003). The R2HTML Package. R News, Vol 3. N. 3, Vienna, Austria.</p>\r\n\r\n<p class='character'>Louis Kates and Thomas Petzoldt (2012). proto: Prototype object-based programming. R package version 0.3-10. http://CRAN.R-project.org/package=proto</p>\r\n\r\n <h2> Analysis dataset</h2>\r\n\r\n\r\n<p align=\"left\">\r\n<table cellspacing=\"0\" border=\"1\">\r\n<caption align=\"bottom\" class=\"captiondataframe\"></caption>\r\n<tr><td>\r\n\t<table border=\"0\" class=\"dataframe\">\r\n\t<tbody> \r\n\t<tr class=\"second\"> \r\n\t\t<th>Observation  </th>\r\n\t\t<th>Resp 1  </th>\r\n\t\t<th>Resp2</th> \r\n\t</tr> \r\n<tr> \r\n<td class=\"cellinside\"> 1\r\n</td>\r\n<td class=\"cellinside\"> 65\r\n</td>\r\n<td class=\"cellinside\">65\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 2\r\n</td>\r\n<td class=\"cellinside\"> 32\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 3\r\n</td>\r\n<td class=\"cellinside\">543\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 4\r\n</td>\r\n<td class=\"cellinside\">675\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 5\r\n</td>\r\n<td class=\"cellinside\">876\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 6\r\n</td>\r\n<td class=\"cellinside\"> 54\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 7\r\n</td>\r\n<td class=\"cellinside\">432\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 8\r\n</td>\r\n<td class=\"cellinside\">564\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 9\r\n</td>\r\n<td class=\"cellinside\"> 76\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">10\r\n</td>\r\n<td class=\"cellinside\"> 54\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">11\r\n</td>\r\n<td class=\"cellinside\"> 32\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">12\r\n</td>\r\n<td class=\"cellinside\">234\r\n</td>\r\n<td class=\"cellinside\"> \r\n</td></tr>\r\n \r\n\t</tbody>\r\n</table>\r\n </td></tr></table>\r\n <br>\r\n\r\n <h2> Analysis options</h2>\r\n\r\n<p class='character'>Response variables: Resp 1,Resp2</p>\r\n\r\n<p class='character'>Significance level: 0.05</p>\r\n\r\n<p class='character'>Method used: Pearson</p>\r\n\r\n<p class='character'>Hypotheses type: 2-sided</p>\r\n\r\n<p class='character'>Display correlation coefficient (Y/N): Y</p>\r\n\r\n<p class='character'>Display test statistic (Y/N): Y</p>\r\n\r\n<p class='character'>Display p-values (Y/N): Y</p>\r\n\r\n<p class='character'>Generate scatterplots (Y/N): Y</p>\r\n\r\n<p class='character'>Generate matrixplot (Y/N): N</p>\r\n\r\n<p class='character'>Generate categorised output (Y/N): N</p>\r\n",
                    RProcessOutput = "[1] \"--vanilla\"                                                                       \r\n [2] \"--args\"                                                                          \r\n [3] \"C:\\\\Users\\\\robal\\\\AppData\\\\Local\\\\Temp\\\\3d78da4f-bf93-4324-a7c9-73ffd19798ea.csv\"\r\n [4] \"Respivs_sp_ivs1,Resp2\"                                                           \r\n [5] \"None\"                                                                            \r\n [6] \"NULL\"                                                                            \r\n [7] \"NULL\"                                                                            \r\n [8] \"NULL\"                                                                            \r\n [9] \"NULL\"                                                                            \r\n[10] \"Pearson\"                                                                         \r\n[11] \"2-sided\"                                                                         \r\n[12] \"Y\"                                                                               \r\n[13] \"Y\"                                                                               \r\n[14] \"Y\"                                                                               \r\n[15] \"Y\"                                                                               \r\n[16] \"N\"                                                                               \r\n[17] \"0.05\"                                                                            \r\n[18] \"N\"                                                                               \r\n[1] \"C:\\\\Users\\\\robal\\\\AppData\\\\Local\\\\Temp\\\\3d78da4f-bf93-4324-a7c9-73ffd19798ea.html\"\r\n\r\n\r\n\r\nAttaching package: 'reshape'\r\n\r\nThe following objects are masked from 'package:plyr':\r\n\r\n    rename, round_any\r\n\r\nAnalysis by the R Processor took 4.16 seconds.",
                   Script =  new SilveR.Models.Script
                    {
                        ScriptDisplayName = "Correlation Analysis",
                        ScriptFileName = "CorrelationAnalysis",
                        ScriptID = 10,
                        RequiresDataset = true
                    },
                    ScriptID = 10,
                    Tag = null
                },
                new Analysis(It.IsAny<Dataset>())
                {
                    AnalysisID = 39,
                    Dataset = null,
                    DatasetID = 6,
                    DatasetName = "_test dataset.xlsx [unpairedttest]",
                    DateAnalysed = new DateTime(2018, 11, 17, 8, 15, 13),
                    HtmlOutput = "\r\n<link rel=\"stylesheet\" type=\"text/css\" href='r2html.css'>\r\n\r\n <h1> InVivoStat One-sample t-test Analysis</h1>\r\n\r\n <h2> Response</h2>\r\n\r\n<p class='character'>The  Resp 1,Resp2 response is currently being analysed by the One-sample t-test Analysis module. The sample mean for Resp 1,Resp2 is compared back to the fixed value 0.</p>\r\n\r\n<p class='character'>For more information on the theoretical approaches that are implemented within this module, see Bate and Clark (2014).</p>\r\n\r\n <h2> One-sample t-test summary results</h2>\r\n",
                    RProcessOutput = "[1] \"--vanilla\"                                                                       \r\n[2] \"--args\"                                                                          \r\n[3] \"C:\\\\Users\\\\robal\\\\AppData\\\\Local\\\\Temp\\\\0e5acaf6-ed64-4bc9-93b2-63a277393728.csv\"\r\n[4] \"Respivs_sp_ivs1,Resp2\"                                                           \r\n[5] \"None\"                                                                            \r\n[6] \"0\"                                                                               \r\n[7] \"Y\"                                                                               \r\n[8] \"N\"                                                                               \r\n[9] \"0.05\"                                                                            \r\n[1] \"C:\\\\Users\\\\robal\\\\AppData\\\\Local\\\\Temp\\\\0e5acaf6-ed64-4bc9-93b2-63a277393728.html\"\r\n\r\n\r\n\r\nAttaching package: 'reshape'\r\n\r\nThe following objects are masked from 'package:plyr':\r\n\r\n    rename, round_any\r\n\r\nError in parse(text = paste(\"statdata$\", xxxresponsexxx)) : \r\n  <text>:1:26: unexpected ','\r\n1: statdata$ Respivs_sp_ivs1,\r\n                             ^\r\nCalls: t.test -> eval -> parse\r\nExecution halted\r\n\r\nAnalysis by the R Processor took 6.51 seconds.",
                    Script = new SilveR.Models.Script
                    {
                        ScriptDisplayName = "One-sample t-test Analysis",
                        ScriptFileName = "OneSampleTTestAnalysis",
                        ScriptID = 19,
                        RequiresDataset = true
                    },
                    ScriptID = 19,
                    Tag = null
                },
                new Analysis(It.IsAny<Dataset>())
                {
                    AnalysisID = 33,
                    Dataset = null,
                    DatasetID = 1,
                    DatasetName = "_test dataset.xlsx [doseresponse]",
                    DateAnalysed = new DateTime(2018, 11, 15, 11, 10, 36),
                    HtmlOutput = "\r\n<link rel=\"stylesheet\" type=\"text/css\" href='r2html.css'>\r\n\r\n <h1> InVivoStat Dose-Response Analysis</h1>\r\n\r\n <h2> Response and dose variables</h2>\r\n\r\n<p class='character'>The  Resp 1 response is currently being analysed by the Dose-Response Analysis module.</p>\r\n\r\n<p class='character'>The dose variable (Dose1) has been Log10 transformed prior to analysis.</p>\r\n",
                    RProcessOutput = "[1] \"--vanilla\"                                                                       \r\n [2] \"--args\"                                                                          \r\n [3] \"C:\\\\Users\\\\robal\\\\AppData\\\\Local\\\\Temp\\\\5ed8b44e-2674-4bfa-a4d7-72b892b5a97f.csv\"\r\n [4] \"FourParameter\"                                                                   \r\n [5] \"Respivs_sp_ivs1\"                                                                 \r\n [6] \"None\"                                                                            \r\n [7] \"Dose1\"                                                                           \r\n [8] \"NULL\"                                                                            \r\n [9] \"Log10\"                                                                           \r\n[10] \"NULL\"                                                                            \r\n[11] \"NULL\"                                                                            \r\n[12] \"NULL\"                                                                            \r\n[13] \"10\"                                                                              \r\n[14] \"1\"                                                                               \r\n[15] \"NULL\"                                                                            \r\n[16] \"NULL\"                                                                            \r\n[17] \"NULL\"                                                                            \r\n[18] \"NULL\"                                                                            \r\n[19] \"NULL\"                                                                            \r\n[20] \"NULL\"                                                                            \r\n[21] \"NULL\"                                                                            \r\n[22] \"NULL\"                                                                            \r\n[23] \"NULL\"                                                                            \r\n[24] \"NULL\"                                                                            \r\n[1] \"C:\\\\Users\\\\robal\\\\AppData\\\\Local\\\\Temp\\\\5ed8b44e-2674-4bfa-a4d7-72b892b5a97f.html\"\r\n\r\n\r\n\r\nAttaching package: 'reshape'\r\n\r\nThe following objects are masked from 'package:plyr':\r\n\r\n    rename, round_any\r\n\r\nError in nls(responsezzzz ~ MinCoeffp + (MaxCoeffp - MinCoeffp)/(1 + 10^((C -  : \r\n  step factor 0.000488281 reduced below 'minFactor' of 0.000976562\r\nExecution halted\r\n\r\nAnalysis by the R Processor took 7.95 seconds.",
                    Script = new SilveR.Models.Script
                    {
                        ScriptDisplayName = "Dose-response and Non-linear Regression Analysis",
                        ScriptFileName = "DoseResponseAndNonLinearRegressionAnalysis",
                        ScriptID = 8,
                        RequiresDataset = true
                    },
                    ScriptID = 8,
                    Tag = null
                },
                new Analysis(null)
                {
                    AnalysisID = 34,
                    DateAnalysed = new DateTime(2018, 11, 15, 11, 10, 36),
                    HtmlOutput = "\r\n<link rel=\"stylesheet\" type=\"text/css\" href='r2html.css'>\r\n\r\n <h1> InVivoStat Dose-Response Analysis</h1>\r\n\r\n <h2> Response and dose variables</h2>\r\n\r\n<p class='character'>The  Resp 1 response is currently being analysed by the Dose-Response Analysis module.</p>\r\n\r\n<p class='character'>The dose variable (Dose1) has been Log10 transformed prior to analysis.</p>\r\n",
                    RProcessOutput = "[1] \"--vanilla\"                                                                       \r\n [2] \"--args\"                                                                          \r\n [3] \"C:\\\\Users\\\\robal\\\\AppData\\\\Local\\\\Temp\\\\5ed8b44e-2674-4bfa-a4d7-72b892b5a97f.csv\"\r\n [4] \"FourParameter\"                                                                   \r\n [5] \"Respivs_sp_ivs1\"                                                                 \r\n [6] \"None\"                                                                            \r\n [7] \"Dose1\"                                                                           \r\n [8] \"NULL\"                                                                            \r\n [9] \"Log10\"                                                                           \r\n[10] \"NULL\"                                                                            \r\n[11] \"NULL\"                                                                            \r\n[12] \"NULL\"                                                                            \r\n[13] \"10\"                                                                              \r\n[14] \"1\"                                                                               \r\n[15] \"NULL\"                                                                            \r\n[16] \"NULL\"                                                                            \r\n[17] \"NULL\"                                                                            \r\n[18] \"NULL\"                                                                            \r\n[19] \"NULL\"                                                                            \r\n[20] \"NULL\"                                                                            \r\n[21] \"NULL\"                                                                            \r\n[22] \"NULL\"                                                                            \r\n[23] \"NULL\"                                                                            \r\n[24] \"NULL\"                                                                            \r\n[1] \"C:\\\\Users\\\\robal\\\\AppData\\\\Local\\\\Temp\\\\5ed8b44e-2674-4bfa-a4d7-72b892b5a97f.html\"\r\n\r\n\r\n\r\nAttaching package: 'reshape'\r\n\r\nThe following objects are masked from 'package:plyr':\r\n\r\n    rename, round_any\r\n\r\nError in nls(responsezzzz ~ MinCoeffp + (MaxCoeffp - MinCoeffp)/(1 + 10^((C -  : \r\n  step factor 0.000488281 reduced below 'minFactor' of 0.000976562\r\nExecution halted\r\n\r\nAnalysis by the R Processor took 7.95 seconds.",
                    Script = new SilveR.Models.Script
                    {
                        ScriptDisplayName = "P-value Adjustment (User Based Inputs)",
                        ScriptFileName = "PValueAdjustmentUserBasedInputs",
                        ScriptID = 9,
                        RequiresDataset = false
                    },
                    ScriptID = 8,
                    Tag = null,
                    Arguments = new System.Collections.Generic.HashSet<SilveR.Models.Argument>
                    {
                        new SilveR.Models.Argument
                        {
                            AnalysisID = 34,
                            ArgumentID = 1121,
                            Name = "SelectedTest",
                            Value = "Holm"
                        },
                        new SilveR.Models.Argument
                        {
                            AnalysisID = 34,
                            ArgumentID = 1122,
                            Name = "Significance",
                            Value = "0.05"
                        },
                        new SilveR.Models.Argument
                        {
                            AnalysisID = 34,
                            ArgumentID = 1123,
                            Name = "PValues",
                            Value = "0.01"
                        }
                    }
                }
            };

            return analyses;
        }

        private IList<DatasetViewModel> GetDatasets()
        {
            var datasets = new List<SilveR.ViewModels.DatasetViewModel>
            {
                new SilveR.ViewModels.DatasetViewModel
                {
                    DatasetID = 1,
                    DatasetName = "_test dataset.xlsx [doseresponse]",
                    DateUpdated = new DateTime(2018, 11, 11, 10, 58, 48),
                    VersionNo = 1
                },
                new SilveR.ViewModels.DatasetViewModel
                {
                    DatasetID = 2,
                    DatasetName = "_test dataset.xlsx [nonpara]",
                    DateUpdated = new DateTime(2018, 11, 11, 11, 12, 16),
                    VersionNo = 1
                },
                new SilveR.ViewModels.DatasetViewModel
                {
                    DatasetID = 3,
                    DatasetName = "_test dataset.xlsx [singlemeasures]",
                    DateUpdated = new DateTime(2018, 11, 11, 11, 14, 2),
                    VersionNo = 1
                },
                new SilveR.ViewModels.DatasetViewModel
                {
                    DatasetID = 4,
                    DatasetName = "_test dataset.xlsx [regression]",
                    DateUpdated = new DateTime(2018, 11, 12, 11, 53, 6),
                    VersionNo = 1
                }
            };

            return datasets;
        }

        private List<Script> GetScripts()
        {
            var scripts = new List<Script>
            {
                new Script{ ScriptFileName = "SummaryStatistics",ScriptDisplayName = "Summary Statistics", RequiresDataset = true },
                new Script{ ScriptFileName = "MultivariateAnalysis",ScriptDisplayName = "MultivariateAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "ComparisonOfMeansPowerAnalysisUserBasedInputs",ScriptDisplayName = "ComparisonOfMeansPowerAnalysisUserBasedInputs", RequiresDataset = false },
                new Script{ ScriptFileName =  "ComparisonOfMeansPowerAnalysisDatasetBasedInputs",ScriptDisplayName = "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", RequiresDataset = true },
                new Script{ ScriptFileName =  "GraphicalAnalysis",ScriptDisplayName = "GraphicalAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "SurvivalAnalysis" ,ScriptDisplayName = "SurvivalAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "ChiSquaredAndFishersExactTest",ScriptDisplayName = "ChiSquaredAndFishersExactTest", RequiresDataset = true },
                new Script{ ScriptFileName =  "NonParametricAnalysis",ScriptDisplayName = "NonParametricAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "DoseResponseAndNonLinearRegressionAnalysis" ,ScriptDisplayName = "DoseResponseAndNonLinearRegressionAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "LinearRegressionAnalysis" ,ScriptDisplayName = "LinearRegressionAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "CorrelationAnalysis",ScriptDisplayName = "CorrelationAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "UnpairedTTestAnalysis" ,ScriptDisplayName = "UnpairedTTestAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "PairedTTestAnalysis",ScriptDisplayName = "PairedTTestAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "PValueAdjustmentUserBasedInputs",ScriptDisplayName = "PValueAdjustmentUserBasedInputs", RequiresDataset = false },
                new Script{ ScriptFileName =  "RepeatedMeasuresParametricAnalysis" ,ScriptDisplayName = "RepeatedMeasuresParametricAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "SingleMeasuresParametricAnalysis",ScriptDisplayName = "SingleMeasuresParametricAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "NestedDesignAnalysis",ScriptDisplayName = "NestedDesignAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "IncompleteFactorialParametricAnalysis",ScriptDisplayName = "IncompleteFactorialParametricAnalysis", RequiresDataset = true },
                new Script{ ScriptFileName =  "OneSampleTTestAnalysis",ScriptDisplayName = "OneSampleTTestAnalysis", RequiresDataset = true }
            };

            return scripts;
        }

        private Analysis GetSummaryStatsAnalysis()
        {
            Analysis analysis = new Analysis(It.IsAny<Dataset>())
            {
                AnalysisID = 67,
                Arguments = new System.Collections.Generic.HashSet<SilveR.Models.Argument>
                {
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1121,
                        Name = "Responses",
                        Value = "Resp 2"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1122,
                        Name = "Significance",
                        Value = "95"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1123,
                        Name = "ConfidenceInterval",
                        Value = "True"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1124,
                        Name = "CoefficientOfVariation",
                        Value = "False"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1125,
                        Name = "MedianAndQuartiles",
                        Value = "False"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1126,
                        Name = "MinAndMax",
                        Value = "False"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1127,
                        Name = "StandardErrorOfMean",
                        Value = "False"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1128,
                        Name = "Variance",
                        Value = "False"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1129,
                        Name = "StandardDeviation",
                        Value = "True"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1130,
                        Name = "N",
                        Value = "True"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1131,
                        Name = "Sum",
                        Value = "False"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1132,
                        Name = "Mean",
                        Value = "True"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1133,
                        Name = "FourthCatFactor",
                        Value = null
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1134,
                        Name = "ThirdCatFactor",
                        Value = null
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1135,
                        Name = "SecondCatFactor",
                        Value = null
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1136,
                        Name = "FirstCatFactor",
                        Value = null
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1137,
                        Name = "Transformation",
                        Value = "None"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1138,
                        Name = "NormalProbabilityPlot",
                        Value = "False"
                    },
                    new SilveR.Models.Argument
                    {
                        AnalysisID = 67,
                        ArgumentID = 1139,
                        Name = "ByCategoriesAndOverall",
                        Value = "False"
                    }
                },
                Dataset = GetDataset(),
                DatasetID = 9,
                DatasetName = "_test dataset.xlsx [summary]",
                DateAnalysed = new DateTime(2018, 11, 23, 12, 33, 13),
                HtmlOutput = "\r\n <h1> InVivoStat Summary Statistics</h1>\r\n\r\n <h2> Variable selection</h2>\r\n\r\n<p class='character'>Response Resp 2 is analysed in this module.</p>\r\n\r\n <h2> Summary statistics for Resp 2</h2>\r\n\r\n\r\n<p align=\"left\">\r\n<table cellspacing=\"0\" border=\"1\"><caption align=\"bottom\" class=\"captiondataframe\"></caption>\r\n<tr><td>\r\n\t<table border=\"0\" class=\"dataframe\">\r\n\t<tbody> <tr class=\"second\"> <th>Response</th><th>Mean</th><th>N</th><th>Std dev</th><th>Lower 95% CI</th><th>Upper 95% CI</th> </tr>\r\n <tr> \r\n<td class=\"cellinside\">Resp 2</td>\r\n<td class=\"cellinside\">0.4732         </td>\r\n<td class=\"cellinside\">32             </td>\r\n<td class=\"cellinside\">0.3408         </td>\r\n<td class=\"cellinside\">0.3503         </td>\r\n<td class=\"cellinside\">0.5960         </td> </tr>\r\n \r\n\t</tbody>\r\n</table>\r\n </td></tr></table>\r\n <br>\r\n\r\n<p class='character'>For more information on the theoretical approaches that are implemented within this module, see Bate and Clark (2014).</p>\r\n\r\n <h2> Statistical references</h2>\r\n\r\n<p class='character'>Bate ST and Clark RA. (2014). The Design and Statistical Analysis of Animal Experiments. Cambridge University Press.</p>\r\n\r\n <h2> R references</h2>\r\n\r\n<p class='character'>R Development Core Team (2013). R: A language and environment for statistical computing. R Foundation for Statistical Computing, Vienna, Austria. URL http://www.R-project.org.</p>\r\n\r\n<p class='character'>Barret Schloerke, Jason Crowley, Di Cook, Heike Hofmann, Hadley Wickham, Francois Briatte, Moritz Marbach and Edwin Thoen (2014). GGally: Extension to ggplot2. R package version 0.4.5. http://CRAN.R-project.org/package=GGally</p>\r\n\r\n<p class='character'>Erich Neuwirth (2011). RColorBrewer: ColorBrewer palettes. R package  version 1.0-5. http://CRAN.R-project.org/package=RColorBrewer</p>\r\n\r\n<p class='character'>H. Wickham. ggplot2: elegant graphics for data analysis. Springer New York, 2009.</p>\r\n\r\n<p class='character'>Kamil Slowikowski (2018). ggrepel: Automatically Position Non-Overlapping Text Labels with 'ggplot2'. R package version 0.8.0. https://CRAN.R-project.org/package=ggrepel</p>\r\n\r\n<p class='character'>H. Wickham. Reshaping data with the reshape package. Journal of Statistical Software, 21(12), 2007.</p>\r\n\r\n<p class='character'>Hadley Wickham (2011). The Split-Apply-Combine Strategy for Data Analysis. Journal of Statistical Software, 40(1), 1-29. URL http://www.jstatsoft.org/v40/i01/.</p>\r\n\r\n<p class='character'>Hadley Wickham (2012). scales: Scale functions for graphics. R package version 0.2.3. http://CRAN.R-project.org/package=scales</p>\r\n\r\n<p class='character'>Lecoutre, Eric (2003). The R2HTML Package. R News, Vol 3. N. 3, Vienna, Austria.</p>\r\n\r\n<p class='character'>Louis Kates and Thomas Petzoldt (2012). proto: Prototype object-based programming. R package version 0.3-10. http://CRAN.R-project.org/package=proto</p>\r\n\r\n <h2> Analysis dataset</h2>\r\n\r\n\r\n<p align=\"left\">\r\n<table cellspacing=\"0\" border=\"1\">\r\n<caption align=\"bottom\" class=\"captiondataframe\"></caption>\r\n<tr><td>\r\n\t<table border=\"0\" class=\"dataframe\">\r\n\t<tbody> \r\n\t<tr class=\"second\"> \r\n\t\t<th>Observation  </th>\r\n\t\t<th>Resp 2</th> \r\n\t</tr> \r\n<tr> \r\n<td class=\"cellinside\"> 1\r\n</td>\r\n<td class=\"cellinside\">0.5742\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 2\r\n</td>\r\n<td class=\"cellinside\">0.9414\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 3\r\n</td>\r\n<td class=\"cellinside\">0.1276\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 4\r\n</td>\r\n<td class=\"cellinside\">0.1665\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 5\r\n</td>\r\n<td class=\"cellinside\">0.1726\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 6\r\n</td>\r\n<td class=\"cellinside\">0.9369\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 7\r\n</td>\r\n<td class=\"cellinside\">0.0667\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 8\r\n</td>\r\n<td class=\"cellinside\">0.9584\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\"> 9\r\n</td>\r\n<td class=\"cellinside\">0.9822\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">10\r\n</td>\r\n<td class=\"cellinside\">0.3582\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">11\r\n</td>\r\n<td class=\"cellinside\">0.6249\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">12\r\n</td>\r\n<td class=\"cellinside\">0.1419\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">13\r\n</td>\r\n<td class=\"cellinside\">0.1289\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">14\r\n</td>\r\n<td class=\"cellinside\">0.9643\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">15\r\n</td>\r\n<td class=\"cellinside\">0.4917\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">16\r\n</td>\r\n<td class=\"cellinside\">0.5714\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">17\r\n</td>\r\n<td class=\"cellinside\">0.8445\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">18\r\n</td>\r\n<td class=\"cellinside\">0.2338\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">19\r\n</td>\r\n<td class=\"cellinside\">0.6860\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">20\r\n</td>\r\n<td class=\"cellinside\">0.3566\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">21\r\n</td>\r\n<td class=\"cellinside\">0.1150\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">22\r\n</td>\r\n<td class=\"cellinside\">0.1570\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">23\r\n</td>\r\n<td class=\"cellinside\">0.5324\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">24\r\n</td>\r\n<td class=\"cellinside\">0.8592\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">25\r\n</td>\r\n<td class=\"cellinside\">    NA\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">26\r\n</td>\r\n<td class=\"cellinside\">0.0405\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">27\r\n</td>\r\n<td class=\"cellinside\">0.9347\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">28\r\n</td>\r\n<td class=\"cellinside\">0.4400\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">29\r\n</td>\r\n<td class=\"cellinside\">0.0482\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">30\r\n</td>\r\n<td class=\"cellinside\">0.3480\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">31\r\n</td>\r\n<td class=\"cellinside\">0.0043\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">32\r\n</td>\r\n<td class=\"cellinside\">0.3950\r\n</td></tr>\r\n \r\n<tr> \r\n<td class=\"cellinside\">33\r\n</td>\r\n<td class=\"cellinside\">0.9394\r\n</td></tr>\r\n \r\n\t</tbody>\r\n</table>\r\n </td></tr></table>\r\n <br>\r\n\r\n <h2> Analysis options</h2>\r\n\r\n<p class='character'>Response variables: Resp 2</p>\r\n\r\n<p class='character'>Display mean (Y/N): Y</p>\r\n\r\n<p class='character'>Display sample size  (Y/N): Y</p>\r\n\r\n<p class='character'>Display standard deviation (Y/N): Y</p>\r\n\r\n<p class='character'>Display variance (Y/N): N</p>\r\n\r\n<p class='character'>Display standard error (Y/N): N</p>\r\n\r\n<p class='character'>Display minimum/maximum  (Y/N): N</p>\r\n\r\n<p class='character'>Display median and quartiles (Y/N): N</p>\r\n\r\n<p class='character'>Display coefficient of variation (Y/N): N</p>\r\n\r\n<p class='character'>Display confidence interval (Y/N): Y</p>\r\n\r\n<p class='character'>Confidence level: 95</p>\r\n\r\n<p class='character'>Display normal probability plot (Y/N): N</p>\r\n\r\n<p class='character'>Display results by categories & overall (Y/N): N</p>\r\n",
                RProcessOutput = "[1] \"--vanilla\"                                                                                   \r\n [2] \"--args\"                                                                                      \r\n [3] \"C:\\\\Users\\\\Robin Clark.HLSUK\\\\AppData\\\\Local\\\\Temp\\\\36b332be-41f0-4f36-b581-0a6067ac9402.csv\"\r\n [4] \"Respivs_sp_ivs2\"                                                                             \r\n [5] \"None\"                                                                                        \r\n [6] \"NULL\"                                                                                        \r\n [7] \"NULL\"                                                                                        \r\n [8] \"NULL\"                                                                                        \r\n [9] \"NULL\"                                                                                        \r\n[10] \"Y\"                                                                                           \r\n[11] \"Y\"                                                                                           \r\n[12] \"Y\"                                                                                           \r\n[13] \"N\"                                                                                           \r\n[14] \"N\"                                                                                           \r\n[15] \"N\"                                                                                           \r\n[16] \"N\"                                                                                           \r\n[17] \"N\"                                                                                           \r\n[18] \"Y\"                                                                                           \r\n[19] \"95\"                                                                                          \r\n[20] \"N\"                                                                                           \r\n[21] \"N\"                                                                                           \r\n[1] \"C:\\\\Users\\\\Robin Clark.HLSUK\\\\AppData\\\\Local\\\\Temp\\\\36b332be-41f0-4f36-b581-0a6067ac9402.html\"\r\n\r\n\r\n\r\nAttaching package: 'reshape'\r\n\r\nThe following objects are masked from 'package:plyr':\r\n\r\n    rename, round_any\r\n\r\nAnalysis by the R Processor took 6.14 seconds.",
                Script = new SilveR.Models.Script
                {
                    ScriptDisplayName = "Summary Statistics",
                    ScriptFileName = "SummaryStatistics",
                    ScriptID = 1,
                    RequiresDataset = true
                },
                ScriptID = 1,
                Tag = null
            };

            return analysis;
        }
    }
}