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
    public class OneSampleTTestAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public OneSampleTTestAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task OSTT1()
        {
            string testName = "OSTT1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses= new string[] { "Resp 5" };
            model.TargetValue = 0.1m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in the response variable (Resp 5). Please select another factor.", errors);
            Helpers.SaveOutput("OneSampleTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task OSTT2()
        {
            string testName = "OSTT2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 6" };
            model.TargetValue = 0.1m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp 6) contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.", errors);
            Helpers.SaveOutput("OneSampleTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task OSTT3()
        {
            string testName = "OSTT3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp8" };
            model.ResponseTransformation = "Log10";
            model.TargetValue = 0.1m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp8 variable. Unfortunately some of the Resp8 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("OneSampleTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task OSTT4()
        {
            string testName = "OSTT4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 9" };
            model.ResponseTransformation = "Log10";
            model.TargetValue = 0.1m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp 9 variable. Unfortunately some of the Resp 9 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("OneSampleTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task OSTT5()
        {
            string testName = "OSTT5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp8" };
            model.ResponseTransformation = "Square Root";
            model.TargetValue = 0.1m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp8 variable. Unfortunately some of the Resp8 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("OneSampleTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task OSTT6()
        {
            string testName = "OSTT6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp8" };
            model.ResponseTransformation = "ArcSine";
            model.TargetValue = 0.1m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("OneSampleTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task OSTT7()
        {
            string testName = "OSTT7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 10" };
            model.ResponseTransformation = "ArcSine";
            model.TargetValue = 0.1m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp 10 variable. Unfortunately some of the Resp 10 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("OneSampleTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task OSTT8()
        {
            string testName = "OSTT8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 1" };
            model.ResponseTransformation = "None";
            model.TargetValue = 0.1m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneSampleTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneSampleTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task OSTT9()
        {
            string testName = "OSTT9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp2" };
            model.ResponseTransformation = "None";
            model.Significance = "0.1";
            model.TargetValue = 0.1m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneSampleTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneSampleTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task OSTT10()
        {
            string testName = "OSTT10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 1" };
            model.ResponseTransformation = "Square Root";
            model.Significance = "0.01";
            model.TargetValue = 0.1m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneSampleTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneSampleTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task OSTT11()
        {
            string testName = "OSTT11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 1" };
            model.ResponseTransformation = "Log10";
            model.Significance = "0.05";
            model.TargetValue = 0.1m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneSampleTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneSampleTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task OSTT12()
        {
            string testName = "OSTT12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 1" };
            model.ResponseTransformation = "Loge";
            model.Significance = "0.05";
            model.TargetValue = 0.1m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneSampleTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneSampleTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task OSTT13()
        {
            string testName = "OSTT13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 1" };
            model.ResponseTransformation = "ArcSine";
            model.Significance = "0.05";
            model.TargetValue = 0.1m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneSampleTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneSampleTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task OSTT14()
        {
            string testName = "OSTT14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 1" };
            model.ResponseTransformation = "None";
            model.Significance = "0.05";
            model.TargetValue = 10m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneSampleTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneSampleTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task OSTT15()
        {
            string testName = "OSTT15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 1", "Resp2" };
            model.ResponseTransformation = "None";
            model.Significance = "0.05";
            model.TargetValue = 1m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneSampleTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneSampleTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task OSTT16()
        {
            string testName = "OSTT16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 1", "Resp2", "Resp 3" };
            model.ResponseTransformation = "None";
            model.Significance = "0.05";
            model.TargetValue = 1m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneSampleTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneSampleTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task OSTT17()
        {
            string testName = "OSTT17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneSampleTTestAnalysisModel model = new OneSampleTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "One Sample t-test").Key;
            model.Responses = new string[] { "Resp 1", "Resp2", "Resp 3", "Resp4" };
            model.ResponseTransformation = "None";
            model.Significance = "0.05";
            model.TargetValue = 1m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneSampleTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneSampleTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneSampleTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }
    }
}