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
    public class SurvivalAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public SurvivalAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SURV1()
        {
            string testName = "SURV1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp 1";
            model.Treatment = "Group 1";
            model.Censorship = "Censor1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp 1) contains non-numeric data that cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("SurvivalAnalysis", testName, errors);
        }

        [Fact]
        public async Task SURV2()
        {
            string testName = "SURV2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp2";
            model.Treatment = "Group 1";
            model.Censorship = "Censor2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Censorship variable contains values other than 0 and 1. Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("SurvivalAnalysis", testName, errors);
        }

        [Fact]
        public async Task SURV3()
        {
            string testName = "SURV3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp2";
            model.Treatment = "Group 2";
            model.Censorship = "Censor1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Group 2) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("SurvivalAnalysis", testName, errors);
        }

        [Fact]
        public async Task SURV4()
        {
            string testName = "SURV4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp2";
            model.Treatment = "Group 3";
            model.Censorship = "Censor1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Group 3). Please select another factor.", errors);
            Helpers.SaveOutput("SurvivalAnalysis", testName, errors);
        }

        [Fact]
        public async Task SURV5()
        {
            string testName = "SURV5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp2";
            model.Treatment = "Group 1";
            model.Censorship = "Censor5";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is a missing value in the Censorship variable when there is a corresponding Response. Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("SurvivalAnalysis", testName, errors);
        }

        [Fact]
        public async Task SURV6()
        {
            string testName = "SURV6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp2";
            model.Treatment = "Group 4";
            model.Censorship = "Censor1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Group 4) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("SurvivalAnalysis", testName, errors);
        }

        [Fact]
        public async Task SURV7()
        {
            string testName = "SURV7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp3";
            model.Treatment = "Group 1";
            model.Censorship = "Censor1";
            model.CompareSurvivalCurves = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("SurvivalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SurvivalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SurvivalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SurvivalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SURV8()
        {
            string testName = "SURV8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp2";
            model.Censorship = "Censor1";
            model.CompareSurvivalCurves = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor field is required.", errors);
            Helpers.SaveOutput("SurvivalAnalysis", testName, errors);
        }

        [Fact]
        public async Task SURV9()
        {
            string testName = "SURV9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp2";
            model.Treatment = "Group 1";
            model.CompareSurvivalCurves = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Censorship field is required.", errors);
            Helpers.SaveOutput("SurvivalAnalysis", testName, errors);
        }


        [Fact]
        public async Task SURV10()
        {
            string testName = "SURV10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Treatment = "Group 1";
            model.Censorship = "Censor1";
            model.CompareSurvivalCurves = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response field is required.", errors);
            Helpers.SaveOutput("SurvivalAnalysis", testName, errors);
        }

        [Fact]
        public async Task SURV11()
        {
            string testName = "SURV11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp4";
            model.Treatment = "Group 5";
            model.Censorship = "Censor6";
            model.CompareSurvivalCurves = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SurvivalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SurvivalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SURV12()
        {
            string testName = "SURV12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp4";
            model.Treatment = "Group 5";
            model.Censorship = "Censor7";
            model.CompareSurvivalCurves = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SurvivalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SurvivalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SURV13()
        {
            string testName = "SURV13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp4";
            model.Treatment = "Group 5";
            model.Censorship = "Censor8";
            model.CompareSurvivalCurves = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SurvivalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SurvivalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SURV14()
        {
            string testName = "SURV14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp4";
            model.Treatment = "Group 6";
            model.Censorship = "Censor8";
            model.CompareSurvivalCurves = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SurvivalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SurvivalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SURV15()
        {
            string testName = "SURV15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp4";
            model.Treatment = "Group 7";
            model.Censorship = "Censor8";
            model.CompareSurvivalCurves = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SurvivalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SurvivalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SURV16()
        {
            string testName = "SURV16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SurvivalAnalysisModel model = new SurvivalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Survival").Key;
            model.Response = "Resp4";
            model.Treatment = "Group 8";
            model.Censorship = "Censor9";
            model.CompareSurvivalCurves = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SurvivalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SurvivalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SurvivalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }
    }
}