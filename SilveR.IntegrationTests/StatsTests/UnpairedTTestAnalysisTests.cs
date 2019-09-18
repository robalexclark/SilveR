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
    public class UnpairedTTestAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public UnpairedTTestAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task UPTT1()
        {
            string testName = "UPTT1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 1";
            model.Treatment = null;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment field is required.", errors);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task UPTT2()
        {
            string testName = "UPTT2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp2";
            model.Treatment = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment (Treat1). Please select another factor.", errors);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, errors);
        }


        [Fact]
        public async Task UPTT3()
        {
            string testName = "UPTT3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 3";
            model.Treatment = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the Treatment (Treat1). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task UPTT4()
        {
            string testName = "UPTT4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment (Treat2) contains missing data where there are observations present in the Response. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task UPTT5()
        {
            string testName = "UPTT5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp4";
            model.Treatment = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task UPTT6()
        {
            string testName = "UPTT6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 5";
            model.Treatment = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp 5) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task UPTT7()
        {
            string testName = "UPTT7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 6";
            model.ResponseTransformation = "Log10";
            model.Treatment = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp 6 variable. Unfortunately some of the Resp 6 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task UPTT8()
        {
            string testName = "UPTT8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 7";
            model.ResponseTransformation = "Log10";
            model.Treatment = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp 7 variable. Unfortunately some of the Resp 7 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, warnings);
        }
        [Fact]
        public async Task UPTT9()
        {
            string testName = "UPTT9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 6";
            model.ResponseTransformation = "Square Root";
            model.Treatment = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp 6 variable. Unfortunately some of the Resp 6 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task UPTT10()
        {
            string testName = "UPTT10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "ArcSine";
            model.Treatment = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task UPTT11()
        {
            string testName = "UPTT11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 7";
            model.ResponseTransformation = "ArcSine";
            model.Treatment = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp 7 variable. Unfortunately some of the Resp 7 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task UPTT12()
        {
            string testName = "UPTT12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatment = "Treat4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The treatment (Treat4) has more than two levels, please analyse using Single Measure Parametric Analysis module.", errors);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task UPTT13()
        {
            string testName = "UPTT13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatment = "Treat(5";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The treatment (Treat(5) has more than two levels, please analyse using Single Measure Parametric Analysis module.", errors);
            Helpers.SaveOutput("UnpairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task UPTT14()
        {
            string testName = "UPTT14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatment = "Treat1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("UnpairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "UnpairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UPTT15()
        {
            string testName = "UPTT15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatment = "Treat1";
            model.Significance = "0.1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("UnpairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "UnpairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UPTT16()
        {
            string testName = "UPTT16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp:9";
            model.ResponseTransformation = "Square Root";
            model.Treatment = "Treat3";
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("UnpairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "UnpairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UPTT17()
        {
            string testName = "UPTT17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp-10";
            model.ResponseTransformation = "Loge";
            model.Treatment = "Treat£6";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("UnpairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "UnpairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UPTT18()
        {
            string testName = "UPTT18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp-10";
            model.ResponseTransformation = "Loge";
            model.Treatment = "Treat:7";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("UnpairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "UnpairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UPTT19()
        {
            string testName = "UPTT19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "Resp^11";
            model.ResponseTransformation = "ArcSine";
            model.Treatment = "Treat:7";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("UnpairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "UnpairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UPTT20()
        {
            string testName = "UPTT20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "PVTestresponse1";
            model.ResponseTransformation = "None";
            model.Treatment = "PVTestgroup";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("UnpairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "UnpairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UPTT21()
        {
            string testName = "UPTT21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            model.Response = "PVTestresponse2";
            model.ResponseTransformation = "None";
            model.Treatment = "PVTestgroup";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "UnpairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("UnpairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "UnpairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }
    }
}