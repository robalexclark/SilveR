using ControlledForms.IntegrationTests;
using HtmlAgilityPack;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    [Collection("Sequential")]
    public class NonParametricTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public NonParametricTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task NP1()
        {
            string testName = "NP1";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat3";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor selected (Treat3) contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP2()
        {
            string testName = "NP2";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat4";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor selected (Treat4) contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP3()
        {
            string testName = "NP3";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat4";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor selected (Treat4) contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP4()
        {
            string testName = "NP4";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat4";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor selected (Treat4) contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP5()
        {
            string testName = "NP5";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The treatment factor (Treat5) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP6()
        {
            string testName = "NP6";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The treatment factor (Treat5) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP7()
        {
            string testName = "NP7";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat5";
            model.Control = "x";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The treatment factor (Treat5) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP8()
        {
            string testName = "NP8";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat6";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat6). Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP9()
        {
            string testName = "NP9";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat7";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat7). Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP10()
        {
            string testName = "NP10";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat7";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat7). Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP11()
        {
            string testName = "NP11";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat7";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat7). Please select another factor.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP12()
        {
            string testName = "NP12";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp3";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The response selected (Resp3) contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP13()
        {
            string testName = "NP13";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp3";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The response selected (Resp3) contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP14()
        {
            string testName = "NP14";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp3";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.AllComparisons;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The response selected (Resp3) contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP15()
        {
            string testName = "NP15";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp3";
            model.Treatment = "Treat 2";
            model.Control = "A";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The response selected (Resp3) contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP16()
        {
            string testName = "NP16";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp4";
            model.Treatment = "Resp4";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Treatment has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP17()
        {
            string testName = "NP17";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp5";
            model.Treatment = "Resp5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Treatment has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP18()
        {
            string testName = "NP18";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp5";
            model.Treatment = "Resp5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Treatment has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP19()
        {
            string testName = "NP19";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp5";
            model.Treatment = "Resp5";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Treatment has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP20()
        {
            string testName = "NP20";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.CompareToControl;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/NonParametricAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Control level is a required variable when comparing to control.", errors);
            Helpers.SaveOutput("NonParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task NP21()
        {
            string testName = "NP21";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.01";

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("NonParametricAnalysis", testName, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Fact]
        public async Task NP22()
        {
            string testName = "NP22";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("NonParametricAnalysis", testName, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Fact]
        public async Task NP23()
        {
            string testName = "NP23";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 1";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.1";

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("NonParametricAnalysis", testName, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Fact]
        public async Task NP24()
        {
            string testName = "NP24";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.01";

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("NonParametricAnalysis", testName, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Fact]
        public async Task NP25()
        {
            string testName = "NP25";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.05";

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("NonParametricAnalysis", testName, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Fact]
        public async Task NP26()
        {
            string testName = "NP26";

            // Arrange
            HttpClient client = _factory.CreateClient();

            NonParametricAnalysisModel model = new NonParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "nonpara").Key;
            model.Response = "Resp 1";
            model.Treatment = "Treat 2";
            model.AnalysisType = NonParametricAnalysisModel.AnalysisOption.MannWhitney;
            model.Significance = "0.1";

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "NonParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("NonParametricAnalysis", testName, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "NonParametricAnalysis", testName + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }
    }
}