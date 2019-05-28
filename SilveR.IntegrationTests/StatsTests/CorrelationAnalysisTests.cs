using ControlledForms.IntegrationTests;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class CorrelationAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public CorrelationAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task COR1()
        {
            string testName = "COR1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 5" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The response selected (Resp 5) contains non-numerical data. Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("CorrelationAnalysis", testName, errors);
        }

        [Fact]
        public async Task COR2()
        {
            string testName = "COR2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 5" };
            model.FirstCatFactor = "Factor 4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The categorisation factor selected (Factor 4) contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("CorrelationAnalysis", testName, errors);
        }

        [Fact]
        public async Task COR3()
        {
            string testName = "COR3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2" };
            model.FirstCatFactor = "Factor 5";            

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the categorical (Factor 5). Please select another factor.", errors);
            Helpers.SaveOutput("CorrelationAnalysis", testName, errors);
        }

        [Fact]
        public async Task COR4()
        {
            string testName = "COR4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 4" };
            model.FirstCatFactor = "Factor 6";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the categorical (Factor 6). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("CorrelationAnalysis", testName, errors);
        }

        [Fact]
        public async Task COR5()
        {
            string testName = "COR5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 4", "Resp 6" };
            model.Transformation = "Log10";
            model.FirstCatFactor = "Factor 1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp 4 variable. Unfortunately some of the Resp 4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);
        }

        [Fact]
        public async Task COR6()
        {
            string testName = "COR6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 4", "Resp 6" };
            model.Transformation = "Square Root";
            model.FirstCatFactor = "Factor 1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp 6 variable. Unfortunately some of the Resp 6 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);
        }

        [Fact]
        public async Task COR7()
        {
            string testName = "COR7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 4", "Resp 6" };
            model.Transformation = "Loge";
            model.FirstCatFactor = "Factor 1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp 4 variable. Unfortunately some of the Resp 4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);
        }

        [Fact]
        public async Task COR8()
        {
            string testName = "COR8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 4", "Resp 6" };
            model.Transformation = "ArcSine";
            model.FirstCatFactor = "Factor 1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp 6 variable. Unfortunately some of the Resp 6 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);
        }

        [Fact]
        public async Task COR9()
        {
            string testName = "COR9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 4", "Resp 8" };
            model.Transformation = "ArcSine";
            model.FirstCatFactor = "Factor 1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp 8 variable. Unfortunately some of the Resp 8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);
        }

        [Fact]
        public async Task COR10()
        {
            string testName = "COR10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The categorisation factor selected (Factor 4) contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("CorrelationAnalysis", testName, errors);
        }

        [Fact]
        public async Task COR11()
        {
            string testName = "COR11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 7";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("At least one of your categorisation factors only has one level. Please remove it from the analysis.", errors);
            Helpers.SaveOutput("CorrelationAnalysis", testName, errors);
        }

        [Fact]
        public async Task COR12()
        {
            string testName = "COR12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 8";
            model.SecondCatFactor = "Factor 9";
            model.ByCategoriesAndOverall = true;
            model.Hypothesis = "2-sided";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR13()
        {
            string testName = "COR13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1" };
            model.Transformation = "None";
            model.Hypothesis = "2-sided";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Responses requires at least two entries.", errors);
            Helpers.SaveOutput("CorrelationAnalysis", testName, errors);
        }

        [Fact]
        public async Task COR14()
        {
            string testName = "COR14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 4" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 1";
            model.SecondCatFactor = "Factor 2";
            model.Hypothesis = "2-sided";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR15()
        {
            string testName = "COR15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "2-sided";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR16()
        {
            string testName = "COR16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 1";
            model.SecondCatFactor = "Factor 2";
            model.Method = "Pearson";
            model.Hypothesis = "2-sided";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR17()
        {
            string testName = "COR17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "Log10";
            model.Method = "Pearson";
            model.Hypothesis = "2-sided";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp 4 variable. Unfortunately some of the Resp 4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR18()
        {
            string testName = "COR18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "Square Root";
            model.FirstCatFactor = "Factor 1";
            model.SecondCatFactor = "Factor 2";
            model.Method = "Pearson";
            model.Hypothesis = "2-sided";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR19()
        {
            string testName = "COR19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "ArcSine";
            model.Method = "Pearson";
            model.Hypothesis = "2-sided";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp 3 variable. Unfortunately some of the Resp 3 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR20()
        {
            string testName = "COR20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "Loge";
            model.FirstCatFactor = "Factor 1";
            model.SecondCatFactor = "Factor 2";
            model.Method = "Pearson";
            model.Hypothesis = "2-sided";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR21()
        {
            string testName = "COR21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "Less than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR22()
        {
            string testName = "COR22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 1";
            model.SecondCatFactor = "Factor 2";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR23()
        {
            string testName = "COR23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.Method = "Kendall";
            model.Hypothesis = "2-sided";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR24()
        {
            string testName = "COR24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 1";
            model.SecondCatFactor = "Factor 2";
            model.Method = "Kendall";
            model.Hypothesis = "2-sided";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR25()
        {
            string testName = "COR25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.Method = "Kendall";
            model.Hypothesis = "Less than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR26()
        {
            string testName = "COR26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 1";
            model.SecondCatFactor = "Factor 2";
            model.Method = "Kendall";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR27()
        {
            string testName = "COR27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.Method = "Spearman";
            model.Hypothesis = "2-sided";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR28()
        {
            string testName = "COR28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 1";
            model.SecondCatFactor = "Factor 2";
            model.Method = "Spearman";
            model.Hypothesis = "2-sided";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR29()
        {
            string testName = "COR29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.Transformation = "None";
            model.Method = "Spearman";
            model.Hypothesis = "Less than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR30()
        {
            string testName = "COR30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.FirstCatFactor = "Factor 1";
            model.SecondCatFactor = "Factor 2";
            model.Transformation = "None";
            model.Method = "Spearman";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR31()
        {
            string testName = "COR31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp 1", "Resp 2", "Resp 3", "Resp 4" };
            model.FirstCatFactor = "Factor 2";
            model.SecondCatFactor = "Factor 3";
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp 4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR32()
        {
            string testName = "COR32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp10", "Resp11" };
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR33()
        {
            string testName = "COR33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp12", "Resp13" };
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR34()
        {
            string testName = "COR34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp14", "Resp15" };
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR35()
        {
            string testName = "COR35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp16", "Resp17" };
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR36()
        {
            string testName = "COR36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp18", "Resp19" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 10";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the categorical (Factor 10). Please select another factor.", errors);
            Helpers.SaveOutput("CorrelationAnalysis", testName, errors);
        }

        [Fact]
        public async Task COR37()
        {
            string testName = "COR37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp20", "Resp21" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 11";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR38()
        {
            string testName = "COR38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp22", "Resp23" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 12";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR39()
        {
            string testName = "COR39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp24", "Resp25" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 13";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR40()
        {
            string testName = "COR40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp25", "Resp28", "Resp29" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 15";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";



            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the categorical (Factor 15). Please select another factor.", errors);
            Helpers.SaveOutput("CorrelationAnalysis", testName, errors);
        }

        [Fact]
        public async Task COR41()
        {
            string testName = "COR41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp25", "Resp30", "Resp31" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 16";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp31) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR42()
        {
            string testName = "COR42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp25", "Resp32", "Resp33" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 17";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp33) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR43()
        {
            string testName = "COR43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp25", "Resp34", "Resp35" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 18";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp35) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR44()
        {
            string testName = "COR44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp36", "Resp37", "Resp38" };
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR45()
        {
            string testName = "COR45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp36", "Resp38", "Resp39" };
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR46()
        {
            string testName = "COR46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp36", "Resp40", "Resp41" };
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR47()
        {
            string testName = "COR47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp36", "Resp42", "Resp43" };
            model.Transformation = "None";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task COR48()
        {
            string testName = "COR48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            CorrelationAnalysisModel model = new CorrelationAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Correlation").Key;
            model.Responses = new string[] { "Resp44", "Resp45", "Resp46", "Resp47" };
            model.Transformation = "None";
            model.FirstCatFactor = "Factor 1";
            model.SecondCatFactor = "Factor 2";
            model.Method = "Pearson";
            model.Hypothesis = "Greater than";
            model.Estimate = true;
            model.Statistic = true;
            model.PValue = true;
            model.Scatterplot = true;
            model.Matrixplot = true;
            model.ByCategoriesAndOverall = true;
            model.Significance = "0.01";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/CorrelationAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Resp44) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("CorrelationAnalysis", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "CorrelationAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("CorrelationAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "CorrelationAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }
    }
}