using SilveR.StatsModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class NonParametricTests_1 : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public NonParametricTests_1(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task NP1()
        {
            string testName = "NP1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat3";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat3) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP2()
        {
            string testName = "NP2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat4";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat4) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP3()
        {
            string testName = "NP3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat4";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat4) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP4()
        {
            string testName = "NP4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat4";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat4) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP5()
        {
            string testName = "NP5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat5) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP6()
        {
            string testName = "NP6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat5) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP7()
        {
            string testName = "NP7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat5";
            model.Control = "x";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat5) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP8()
        {
            string testName = "NP8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat6";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat6). Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP9()
        {
            string testName = "NP9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat7";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat7). Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP10()
        {
            string testName = "NP10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat7";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat7). Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP11()
        {
            string testName = "NP11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat7";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat7). Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP12()
        {
            string testName = "NP12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp3";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains non-numeric data that cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP13()
        {
            string testName = "NP13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp3";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains non-numeric data that cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP14()
        {
            string testName = "NP14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp3";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains non-numeric data that cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP15()
        {
            string testName = "NP15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp3";
            model.Treatment = "Treat 2";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains non-numeric data that cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP16()
        {
            string testName = "NP16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp4";
            model.Treatment = "Resp4";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp4) has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Treatment factor (Resp4) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP17()
        {
            string testName = "NP17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp5";
            model.Treatment = "Resp5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp5) has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Treatment factor (Resp5) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP18()
        {
            string testName = "NP18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp5";
            model.Treatment = "Resp5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp5) has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Treatment factor (Resp5) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP19()
        {
            string testName = "NP19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp5";
            model.Treatment = "Resp5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp5) has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Treatment factor (Resp5) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP20()
        {
            string testName = "NP20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Control level is a required variable when comparing to control.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP21()
        {
            string testName = "NP21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP22()
        {
            string testName = "NP22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP23()
        {
            string testName = "NP23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP24()
        {
            string testName = "NP24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP25()
        {
            string testName = "NP25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP26()
        {
            string testName = "NP26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP27()
        {
            string testName = "NP27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP28()
        {
            string testName = "NP28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP29()
        {
            string testName = "NP29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.10";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP30()
        {
            string testName = "NP30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP31()
        {
            string testName = "NP31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP32()
        {
            string testName = "NP32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP33()
        {
            string testName = "NP33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp2";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP34()
        {
            string testName = "NP34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp2";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP35()
        {
            string testName = "NP35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp2";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP36()
        {
            string testName = "NP36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp2";
            model.Treatment = "Treat 2";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP37()
        {
            string testName = "NP37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat 1) has only two levels so a Mann-Whitney test will be presented.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP38()
        {
            string testName = "NP38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 1";
            model.Control = "x";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat 1) has only two levels so a Mann-Whitney test will be presented.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP39()
        {
            string testName = "NP39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp6";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP40()
        {
            string testName = "NP40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp6";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP41()
        {
            string testName = "NP41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp6";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP42()
        {
            string testName = "NP42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat8";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP43()
        {
            string testName = "NP43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat9";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP44()
        {
            string testName = "NP44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat9";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP45()
        {
            string testName = "NP45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat9";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP46()
        {
            string testName = "NP46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp7";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP47()
        {
            string testName = "NP47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp7";
            model.Treatment = "Treat10";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP48()
        {
            string testName = "NP48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp7";
            model.Treatment = "Treat10";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP49()
        {
            string testName = "NP49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp7";
            model.Treatment = "Treat10";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP50()
        {
            string testName = "NP50";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp7";
            model.Treatment = "Treat10";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP51()
        {
            string testName = "NP51";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp11";
            model.Treatment = "Treat11";
            model.OtherDesignFactor = "Block1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp11) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task NP52()
        {
            string testName = "NP52";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp11";
            model.Treatment = "Treat12";
            model.OtherDesignFactor = "Block2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp11) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP53()
        {
            string testName = "NP53";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp11";
            model.Treatment = "Treat13";
            model.OtherDesignFactor = "Block3";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp11) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP54()
        {
            string testName = "NP54";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp11";
            model.Treatment = "Treat14";
            model.OtherDesignFactor = "Block4";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp11) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP55()
        {
            string testName = "NP55";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp11";
            model.Treatment = "Treat15";
            model.OtherDesignFactor = "Block5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP56()
        {
            string testName = "NP56";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp11";
            model.Treatment = "Treat15";
            model.OtherDesignFactor = "Block5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP57()
        {
            string testName = "NP57";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp11";
            model.Treatment = "Treat15";
            model.OtherDesignFactor = "Block5";
            model.Control = "a";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP58()
        {
            string testName = "NP58";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp11";
            model.Treatment = "Treat16";
            model.OtherDesignFactor = "Block6";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP59()
        {
            string testName = "NP59";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp11";
            model.Treatment = "Treat16";
            model.OtherDesignFactor = "Block6";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP60()
        {
            string testName = "NP60";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp11";
            model.Treatment = "Treat16";
            model.OtherDesignFactor = "Block6";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP61()
        {
            string testName = "NP61";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp12";
            model.Treatment = "Treat17";
            model.OtherDesignFactor = "Block7";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP62()
        {
            string testName = "NP62";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp12";
            model.Treatment = "Treat17";
            model.OtherDesignFactor = "Block7";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP63()
        {
            string testName = "NP63";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp12";
            model.Treatment = "Treat17";
            model.OtherDesignFactor = "Block7";
            model.Control = "b";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP64()
        {
            string testName = "NP64";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp12";
            model.Treatment = "Treat18";
            model.OtherDesignFactor = "Block8";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP65()
        {
            string testName = "NP65";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp12";
            model.Treatment = "Treat18";
            model.OtherDesignFactor = "Block8";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP66()
        {
            string testName = "NP66";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp12";
            model.Treatment = "Treat18";
            model.OtherDesignFactor = "Block8";
            model.Control = "2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP67()
        {
            string testName = "NP67";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat11";
            model.OtherDesignFactor = "Block1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP68()
        {
            string testName = "NP68";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat11";
            model.OtherDesignFactor = "Block1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP69()
        {
            string testName = "NP69";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat11";
            model.OtherDesignFactor = "Block1";
            model.Control = "a";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP70()
        {
            string testName = "NP70";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp9";
            model.Treatment = "Treat11";
            model.OtherDesignFactor = "Block1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP71()
        {
            string testName = "NP71";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp9";
            model.Treatment = "Treat11";
            model.OtherDesignFactor = "Block1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP72()
        {
            string testName = "NP72";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp9";
            model.Treatment = "Treat11";
            model.OtherDesignFactor = "Block1";
            model.Control = "a";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP73()
        {
            string testName = "NP73";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat12";
            model.OtherDesignFactor = "Block2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP74()
        {
            string testName = "NP74";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat12";
            model.OtherDesignFactor = "Block2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP75()
        {
            string testName = "NP75";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat12";
            model.OtherDesignFactor = "Block2";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP76()
        {
            string testName = "NP76";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp9";
            model.Treatment = "Treat12";
            model.OtherDesignFactor = "Block2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP77()
        {
            string testName = "NP77";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp9";
            model.Treatment = "Treat12";
            model.OtherDesignFactor = "Block2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP78()
        {
            string testName = "NP78";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp9";
            model.Treatment = "Treat12";
            model.OtherDesignFactor = "Block2";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP79()
        {
            string testName = "NP79";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat13";
            model.OtherDesignFactor = "Block3";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP80()
        {
            string testName = "NP80";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat13";
            model.OtherDesignFactor = "Block3";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP81()
        {
            string testName = "NP81";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat13";
            model.OtherDesignFactor = "Block3";
            model.Control = "q 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP82()
        {
            string testName = "NP82";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp10";
            model.Treatment = "Treat13";
            model.OtherDesignFactor = "Block3";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP83()
        {
            string testName = "NP83";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp10";
            model.Treatment = "Treat13";
            model.OtherDesignFactor = "Block3";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP84()
        {
            string testName = "NP84";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp10";
            model.Treatment = "Treat13";
            model.OtherDesignFactor = "Block3";
            model.Control = "q 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP85()
        {
            string testName = "NP85";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat14";
            model.OtherDesignFactor = "Block4";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP86()
        {
            string testName = "NP86";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat14";
            model.OtherDesignFactor = "Block4";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP87()
        {
            string testName = "NP87";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp8";
            model.Treatment = "Treat14";
            model.OtherDesignFactor = "Block4";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP88()
        {
            string testName = "NP88";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp10";
            model.Treatment = "Treat14";
            model.OtherDesignFactor = "Block4";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP89()
        {
            string testName = "NP89";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp10";
            model.Treatment = "Treat14";
            model.OtherDesignFactor = "Block4";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP90()
        {
            string testName = "NP90";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp10";
            model.Treatment = "Treat14";
            model.OtherDesignFactor = "Block4";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP91()
        {
            string testName = "NP91";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp13";
            model.Treatment = "Treat19";
            model.OtherDesignFactor = "Block9";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP92()
        {
            string testName = "NP92";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp14";
            model.Treatment = "Treat19";
            model.OtherDesignFactor = "Block9";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP93()
        {
            string testName = "NP93";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp15";
            model.Treatment = "Treat20";
            model.OtherDesignFactor = "Block10";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP94()
        {
            string testName = "NP94";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp16";
            model.Treatment = "Treat20";
            model.OtherDesignFactor = "Block10";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP95()
        {
            string testName = "NP95";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp17";
            model.Treatment = "Treat21";
            model.OtherDesignFactor = "Block11";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP96()
        {
            string testName = "NP96";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp18";
            model.Treatment = "Treat21";
            model.OtherDesignFactor = "Block11";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP97()
        {
            string testName = "NP97";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp19";
            model.Treatment = "Treat22";
            model.OtherDesignFactor = "Block12";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP98()
        {
            string testName = "NP98";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp19";
            model.Treatment = "Treat22";
            model.OtherDesignFactor = "Block12";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP99()
        {
            string testName = "NP99";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp20";
            model.Treatment = "Treat22";
            model.OtherDesignFactor = "Block12";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP100()
        {
            string testName = "NP100";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp20";
            model.Treatment = "Treat22";
            model.OtherDesignFactor = "Block12";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }
    }

    public class NonParametricTests_2 : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public NonParametricTests_2(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task NP101()
        {
            string testName = "NP101";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp21";
            model.Treatment = "Treat23";
            model.OtherDesignFactor = "Block13";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP102()
        {
            string testName = "NP102";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp21";
            model.Treatment = "Treat23";
            model.OtherDesignFactor = "Block13";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP103()
        {
            string testName = "NP103";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp22";
            model.Treatment = "Treat23";
            model.OtherDesignFactor = "Block13";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP104()
        {
            string testName = "NP104";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp22";
            model.Treatment = "Treat23";
            model.OtherDesignFactor = "Block13";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP105()
        {
            string testName = "NP105";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp19";
            model.Treatment = "Treat22";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP106()
        {
            string testName = "NP106";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp22";
            model.Treatment = "Treat23";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP107()
        {
            string testName = "NP107";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp19";
            model.Treatment = "Treat22";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP108()
        {
            string testName = "NP108";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp20";
            model.Treatment = "Treat22";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP109()
        {
            string testName = "NP109";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp21";
            model.Treatment = "Treat23";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat23) has only two levels so a Mann-Whitney test will be presented.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP110()
        {
            string testName = "NP110";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp22";
            model.Treatment = "Treat23";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat23) has only two levels so a Mann-Whitney test will be presented.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP111()
        {
            string testName = "NP111";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp19";
            model.Treatment = "Treat22";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP112()
        {
            string testName = "NP112";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp20";
            model.Treatment = "Treat22";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP113()
        {
            string testName = "NP113";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp21";
            model.Treatment = "Treat23";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat23) has only two levels so a Mann-Whitney test will be presented.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP114()
        {
            string testName = "NP114";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp20";
            model.Treatment = "Treat22";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP115()
        {
            string testName = "NP115";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp21";
            model.Treatment = "Treat23";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat23) has only two levels so a Mann-Whitney test will be presented.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task NP116()
        {
            string testName = "NP116";

            //Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Non-parametric").Key;
            model.Response = "Resp22";
            model.Treatment = "Treat23";
            model.Control = "1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat23) has only two levels so a Mann-Whitney test will be presented.", warnings);
            Helpers.SaveOutput("NonParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("NonParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }
    }
}