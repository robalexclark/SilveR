using ControlledForms.IntegrationTests;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class PValueAdjustmentUserBasedInputsTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public PValueAdjustmentUserBasedInputsTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PVA1()
        {
            string testName = "PVA1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.01d";
            model.SelectedTest = "Holm";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("P values contains non-numeric values detected or values are not comma separated.", errors);
            Helpers.SaveOutput("PValueAdjustmentUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PVA2()
        {
            string testName = "PVA2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.01 0.02 1.2";
            model.SelectedTest = "Holm";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("P values contains non-numeric values detected or values are not comma separated.", errors);
            Helpers.SaveOutput("PValueAdjustmentUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PVA3()
        {
            string testName = "PVA3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "<0.001,0.02";
            model.SelectedTest = "Holm";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have entered unadjusted p-value(s) of the form <0.001. For the purposes of the numerical calculations this value has been replaced with 0.00099 and hence the adjusted p-values may be unduly conservative.", warnings);
            Helpers.SaveOutput("PValueAdjustmentUserBasedInputs", testName, warnings);
        }

        [Fact]
        public async Task PVA4()
        {
            string testName = "PVA4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "<0.0001,0.02";
            model.SelectedTest = "Holm";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have entered unadjusted p-value(s) of the form <0.0001. For the purposes of the numerical calculations this value has been replaced with 0.000099 and hence the adjusted p-values may be unduly conservative.", warnings);
            Helpers.SaveOutput("PValueAdjustmentUserBasedInputs", testName, warnings);
        }

        [Fact]
        public async Task PVA5()
        {
            string testName = "PVA5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "<0.001, <0.0001,0.02";
            model.SelectedTest = "Holm";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have entered unadjusted p-value(s) of the form <0.0001. For the purposes of the numerical calculations this value has been replaced with 0.000099 and hence the adjusted p-values may be unduly conservative.", warnings);
            Helpers.SaveOutput("PValueAdjustmentUserBasedInputs", testName, warnings);
        }

        [Fact]
        public async Task PVA6()
        {
            string testName = "PVA6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.03631, 0.96873,<0.0001,0.03318,<0.0001,0.0001";
            model.SelectedTest = "Benjamini-Hochberg";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have entered unadjusted p-value(s) of the form <0.0001. For the purposes of the numerical calculations this value has been replaced with 0.000099 and hence the adjusted p-values may be unduly conservative.", warnings);
            Helpers.SaveOutput("PValueAdjustmentUserBasedInputs", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA7()
        {
            string testName = "PVA7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.2";
            model.SelectedTest = "Holm";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA8()
        {
            string testName = "PVA8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.06,0.1,0.2";
            model.SelectedTest = "Holm";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA9()
        {
            string testName = "PVA9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.02,0.4";
            model.SelectedTest = "Bonferroni";
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA10()
        {
            string testName = "PVA10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.02,0.4";
            model.SelectedTest = "Bonferroni";
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA11()
        {
            string testName = "PVA11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.015";
            model.SelectedTest = "Bonferroni";
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA12()
        {
            string testName = "PVA12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.001,0.2";
            model.SelectedTest = "Bonferroni";
            model.Significance = "0.01";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA13()
        {
            string testName = "PVA13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.01,0.2,0.4";
            model.SelectedTest = "Bonferroni";
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA14()
        {
            string testName = "PVA14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.01,0.003,0.4";
            model.SelectedTest = "Hochberg";
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA15()
        {
            string testName = "PVA15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.001,0.003,0.004,0.4";
            model.SelectedTest = "Hochberg";
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA16()
        {
            string testName = "PVA16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.001,0.002,0.003,0.004,0.4";
            model.SelectedTest = "Hochberg";
            model.Significance = "0.10";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA17()
        {
            string testName = "PVA17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "<0.001,0.002,0.003,0.004,0.4";
            model.SelectedTest = "Hochberg";
            model.Significance = "0.10";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have entered unadjusted p-value(s) of the form <0.001. For the purposes of the numerical calculations this value has been replaced with 0.00099 and hence the adjusted p-values may be unduly conservative.", warnings);
            Helpers.SaveOutput("PValueAdjustmentUserBasedInputs", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA18()
        {
            string testName = "PVA18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "<0.001,0.4";
            model.SelectedTest = "Hochberg";
            model.Significance = "0.10";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have entered unadjusted p-value(s) of the form <0.001. For the purposes of the numerical calculations this value has been replaced with 0.00099 and hence the adjusted p-values may be unduly conservative.", warnings);
            Helpers.SaveOutput("PValueAdjustmentUserBasedInputs", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PVA19()
        {
            string testName = "PVA19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentUserBasedInputsModel model = new PValueAdjustmentUserBasedInputsModel();
            model.PValues = "0.00001,0.4";
            model.SelectedTest = "Hochberg";
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentUserBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }
    }
}