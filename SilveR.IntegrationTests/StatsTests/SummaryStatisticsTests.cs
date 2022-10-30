using SilveR.StatsModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class SummaryStatisticsTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public SummaryStatisticsTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task SS1()
        {
            string testName = "SS1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp1) contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS2()
        {
            string testName = "SS2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The categorisation factor (Cat1) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS3()
        {
            string testName = "SS3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the categorisation factor (Cat2). Please select another factor.", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS4()
        {
            string testName = "SS4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat3";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the categorisation factor (Cat3). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS5()
        {
            string testName = "SS5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp3" };
            model.Transformation = "Log10";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp3 variable. Unfortunately some of the Resp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SummaryStatistics", testName, warnings);
        }

        [Fact]
        public async Task SS6()
        {
            string testName = "SS6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp3" };
            model.Transformation = "Square Root";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp3 variable. Unfortunately some of the Resp3 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SummaryStatistics", testName, warnings);
        }

        [Fact]
        public async Task SS7()
        {
            string testName = "SS7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp3" };
            model.Transformation = "Loge";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp3 variable. Unfortunately some of the Resp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SummaryStatistics", testName, warnings);
        }

        [Fact]
        public async Task SS8()
        {
            string testName = "SS8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp3" };
            model.Transformation = "ArcSine";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp3 variable. Unfortunately some of the Resp3 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SummaryStatistics", testName, warnings);
        }

        [Fact]
        public async Task SS9()
        {
            string testName = "SS9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp11" };
            model.Transformation = "ArcSine";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp11 variable. Unfortunately some of the Resp11 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SummaryStatistics", testName, warnings);
        }

        [Fact]
        public async Task SS10()
        {
            string testName = "SS10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp1", "Resp 2" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp1) contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS11()
        {
            string testName = "SS11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2", "Resp5" };
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The categorisation factor (Cat1) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS12()
        {
            string testName = "SS12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat2";
            model.SecondCatFactor = "Cat4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the categorisation factor (Cat2). Please select another factor.", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS13()
        {
            string testName = "SS13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat3";
            model.SecondCatFactor = "Cat4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the categorisation factor (Cat3). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS14()
        {
            string testName = "SS14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2" };
            model.Significance = 0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("You have selected a confidence limit that is less than 1. Note that this value should be entered as a percentage and not a fraction.", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS15()
        {
            string testName = "SS15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp3", "Resp 2" };
            model.Transformation = "Log10";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp3 variable. Unfortunately some of the Resp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SummaryStatistics", testName, warnings);
        }

        [Fact]
        public async Task SS16()
        {
            string testName = "SS16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp3", "Resp 2" };
            model.Transformation = "Square Root";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp3 variable. Unfortunately some of the Resp3 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SummaryStatistics", testName, warnings);
        }

        [Fact]
        public async Task SS17()
        {
            string testName = "SS17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp4", "Resp 2" };
            model.Transformation = "Loge";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp4 variable. Unfortunately some of the Resp4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SummaryStatistics", testName, warnings);
        }

        [Fact]
        public async Task SS18()
        {
            string testName = "SS18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp6", "Resp7" };
            model.FirstCatFactor = "Cat4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the categorisation factor (Cat4). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS19()
        {
            string testName = "SS19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp1" };
            model.Mean = false;
            model.N = false;
            model.StandardDeviation = false;
            model.Variance = false;
            model.StandardErrorOfMean = false;
            model.MinAndMax = false;
            model.MedianAndQuartiles = false;
            model.CoefficientOfVariation = false;
            model.NormalProbabilityPlot = false;
            model.CoefficientOfVariation = false;
            model.ByCategoriesAndOverall = false;
            model.ConfidenceInterval = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("You have not selected anything to output!", errors);
            Helpers.SaveOutput("SummaryStatistics", testName, errors);
        }

        [Fact]
        public async Task SS20()
        {
            string testName = "SS20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2", "Resp10" };
            model.FirstCatFactor = "Cat4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp10) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("SummaryStatistics", testName, warnings);
        }

        [Fact]
        public async Task SS21()
        {
            string testName = "SS21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2" };
            model.Transformation = "Loge";
            model.Mean = true;
            model.N = true;
            model.StandardDeviation = true;
            model.Variance = true;
            model.StandardErrorOfMean = true;
            model.MinAndMax = true;
            model.MedianAndQuartiles = true;
            model.CoefficientOfVariation = true;
            model.NormalProbabilityPlot = true;
            model.CoefficientOfVariation = true;
            model.ByCategoriesAndOverall = true;
            model.ConfidenceInterval = true;
            model.Significance = 90;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SummaryStatistics", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SummaryStatistics", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SS22()
        {
            string testName = "SS22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2", "Resp8", "Resp9" };
            model.Transformation = "Log10";
            model.Mean = true;
            model.N = true;
            model.StandardDeviation = true;
            model.Variance = true;
            model.StandardErrorOfMean = true;
            model.MinAndMax = true;
            model.MedianAndQuartiles = true;
            model.CoefficientOfVariation = true;
            model.NormalProbabilityPlot = true;
            model.CoefficientOfVariation = true;
            model.ByCategoriesAndOverall = true;
            model.ConfidenceInterval = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SummaryStatistics", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SummaryStatistics", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SS23()
        {
            string testName = "SS23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat4";
            model.Transformation = "Square Root";
            model.Mean = true;
            model.N = true;
            model.StandardDeviation = true;
            model.Variance = true;
            model.StandardErrorOfMean = true;
            model.MinAndMax = true;
            model.MedianAndQuartiles = true;
            model.CoefficientOfVariation = true;
            model.NormalProbabilityPlot = true;
            model.CoefficientOfVariation = true;
            model.ByCategoriesAndOverall = true;
            model.ConfidenceInterval = true;
            model.Significance = 90;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SummaryStatistics", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SummaryStatistics", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SS24()
        {
            string testName = "SS24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2", "Resp8", "Resp9" };
            model.FirstCatFactor = "Cat4";
            model.Transformation = "Square Root";
            model.Mean = true;
            model.N = true;
            model.StandardDeviation = true;
            model.Variance = true;
            model.StandardErrorOfMean = true;
            model.MinAndMax = true;
            model.MedianAndQuartiles = true;
            model.CoefficientOfVariation = true;
            model.NormalProbabilityPlot = true;
            model.CoefficientOfVariation = true;
            model.ByCategoriesAndOverall = true;
            model.ConfidenceInterval = true;
            model.Significance = 99;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SummaryStatistics", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SummaryStatistics", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SS25()
        {
            string testName = "SS25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat4";
            model.SecondCatFactor = "Cat5";
            model.ThirdCatFactor = "Cat6";
            model.Transformation = "Square Root";
            model.Mean = true;
            model.N = true;
            model.StandardDeviation = true;
            model.Variance = true;
            model.StandardErrorOfMean = true;
            model.MinAndMax = true;
            model.MedianAndQuartiles = true;
            model.CoefficientOfVariation = true;
            model.NormalProbabilityPlot = true;
            model.CoefficientOfVariation = true;
            model.ByCategoriesAndOverall = true;
            model.ConfidenceInterval = true;
            model.Significance = 95;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SummaryStatistics", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SummaryStatistics", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SS26()
        {
            string testName = "SS26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2", "Resp8", "Resp9" };
            model.FirstCatFactor = "Cat4";
            model.SecondCatFactor = "Cat5";
            model.ThirdCatFactor = "Cat6";
            model.Transformation = "Log10";
            model.Mean = true;
            model.N = true;
            model.StandardDeviation = true;
            model.Variance = true;
            model.StandardErrorOfMean = true;
            model.MinAndMax = true;
            model.MedianAndQuartiles = true;
            model.CoefficientOfVariation = true;
            model.NormalProbabilityPlot = true;
            model.CoefficientOfVariation = true;
            model.ByCategoriesAndOverall = true;
            model.ConfidenceInterval = true;
            model.Significance = 95;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SummaryStatistics", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SummaryStatistics", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SS27()
        {
            string testName = "SS27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2", "Resp8", "Resp9" };
            model.FirstCatFactor = "Cat6";
            model.Transformation = "None";
            model.Mean = true;
            model.N = true;
            model.StandardDeviation = true;
            model.Variance = true;
            model.StandardErrorOfMean = true;
            model.MinAndMax = true;
            model.MedianAndQuartiles = true;
            model.CoefficientOfVariation = true;
            model.NormalProbabilityPlot = true;
            model.CoefficientOfVariation = true;
            model.ByCategoriesAndOverall = true;
            model.ConfidenceInterval = true;
            model.Significance = 95;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SummaryStatistics", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SummaryStatistics", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SS28()
        {
            string testName = "SS28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Summary Statistics").Key;
            model.Responses = new string[] { "Resp 2", "Resp3", "Resp6" };
            model.FirstCatFactor = "Cat6";
            model.Transformation = "ArcSine";
            model.Mean = true;
            model.N = true;
            model.StandardDeviation = true;
            model.Variance = true;
            model.StandardErrorOfMean = true;
            model.MinAndMax = true;
            model.MedianAndQuartiles = true;
            model.CoefficientOfVariation = true;
            model.NormalProbabilityPlot = true;
            model.CoefficientOfVariation = true;
            model.ByCategoriesAndOverall = true;
            model.ConfidenceInterval = true;
            model.Significance = 95;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp3 variable. Unfortunately some of the Resp3 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SummaryStatistics", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SummaryStatistics", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SummaryStatistics", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SummaryStatistics", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }
    }
}