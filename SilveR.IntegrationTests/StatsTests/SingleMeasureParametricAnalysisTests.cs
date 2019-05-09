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
    [Collection("Sequential")]
    public class SingleMeasureParametricAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public SingleMeasureParametricAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task SMA1()
        {
            string testName = "SMA1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp1";
            model.Treatments = new string[] { "Resp1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Treatments has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA2()
        {
            string testName = "SMA2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp6";
            model.Treatments = new string[] { "Treat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the Treatment factor (Treat1). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA3()
        {
            string testName = "SMA3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat7" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the Treatment factor (Treat7). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA4()
        {
            string testName = "SMA4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Tre at4" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor selected (Tre at4) contains missing data where there are observations present in the Response variable. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA5()
        {
            string testName = "SMA5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp4";
            model.Treatments = new string[] { "Treat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response selected (Resp4) contain non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA6()
        {
            string testName = "SMA6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov4" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Covariate selected (Cov4) contain non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA7()
        {
            string testName = "SMA7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp5";
            model.Treatments = new string[] { "Treat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response selected (Resp5) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA8()
        {
            string testName = "SMA8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov5" };
            model.PrimaryFactor = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Covariate selected (Cov5) contains missing data. Any response that does not have a corresponding covariate will be excluded from the analysis.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA9()
        {
            string testName = "SMA9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "T reat5" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the factors (T reat5) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA10()
        {
            string testName = "SMA10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat6" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat6). Please select another factor.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA11()
        {
            string testName = "SMA11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 2";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp 2 variable. Unfortunately some of the Resp 2 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA12()
        {
            string testName = "SMA12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp3";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp3 variable. Unfortunately some of the Resp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA13()
        {
            string testName = "SMA13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 2";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp 2 variable. Unfortunately some of the Resp 2 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA14()
        {
            string testName = "SMA14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov2" };
            model.CovariateTransformation = "Loge";
            model.PrimaryFactor = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Cov2 variable. Unfortunately some of the Cov2 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA15()
        {
            string testName = "SMA15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov3" };
            model.CovariateTransformation = "Loge";
            model.PrimaryFactor = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Cov3 variable. Unfortunately some of the Cov3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA16()
        {
            string testName = "SMA16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov2" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Cov2 variable. Unfortunately some of the Cov2 values are negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA17()
        {
            string testName = "SMA17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp9 variable. Unfortunately some of the Resp9 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA18()
        {
            string testName = "SMA18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA19()
        {
            string testName = "SMA19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov2" };
            model.CovariateTransformation = "ArcSine";
            model.PrimaryFactor = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Cov2 variable. Unfortunately some of the Cov2 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);

            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA20()
        {
            string testName = "SMA20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Resp8" };
            model.CovariateTransformation = "ArcSine";
            model.PrimaryFactor = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA21()
        {
            string testName = "SMA21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp8";
            model.Treatments = new string[] { "Treat12", "Treat13" };

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Fact]
        public async Task SMA22()
        {
            string testName = "SMA22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp11";
            model.Treatments = new string[] { "Treat14" };

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA23(string testType, string testTypeValue)
        {
            string testName = "SMA23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA24(string testType, string testTypeValue)
        {
            string testName = "SMA24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D3";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA25(string testType, string testTypeValue)
        {
            string testName = "SMA25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat2";
            model.LSMeansSelected = true;
            if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "F";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA26(string testType, string testTypeValue)
        {
            string testName = "SMA26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat2";
            model.LSMeansSelected = true;
            if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "F";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA27(string testType, string testTypeValue)
        {
            string testName = "SMA27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat2";
            model.LSMeansSelected = true;
            if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "F";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA28(string testType, string testTypeValue)
        {
            string testName = "SMA28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov 1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D1";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA29(string testType, string testTypeValue)
        {
            string testName = "SMA29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Log10";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D10";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA30(string testType, string testTypeValue)
        {
            string testName = "SMA30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Square Root";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA31(string testType, string testTypeValue)
        {
            string testName = "SMA31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "None";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA32(string testType, string testTypeValue)
        {
            string testName = "SMA32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Square Root";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("SelectedEffect", "Treat 1")]
        [InlineData("SelectedEffect", "Treat1 * Treat2")]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA33(string testType, string testTypeValue)
        {
            string testName = "SMA33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;

            if (testType == "SelectedEffect")
            {
                model.SelectedEffect = testTypeValue;
                model.LSMeansSelected = true;
            }
            else if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("SelectedEffect", "Treat 1")]
        [InlineData("SelectedEffect", "Treat1 * Treat2")]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA34(string testType, string testTypeValue)
        {
            string testName = "SMA34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;

            if (testType == "SelectedEffect")
            {
                model.SelectedEffect = testTypeValue;
                model.LSMeansSelected = true;
            }
            else if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("SelectedEffect", "Treat 2")]
        [InlineData("SelectedEffect", "Treat1 * Treat2")]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA35(string testType, string testTypeValue)
        {
            string testName = "SMA35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;

            if (testType == "SelectedEffect")
            {
                model.SelectedEffect = testTypeValue;
                model.LSMeansSelected = true;
            }
            else if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("SelectedEffect", "Treat 2")]
        [InlineData("SelectedEffect", "Treat1 * Treat2")]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA36(string testType, string testTypeValue)
        {
            string testName = "SMA36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;

            if (testType == "SelectedEffect")
            {
                model.SelectedEffect = testTypeValue;
                model.LSMeansSelected = true;
            }
            else if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("SelectedEffect", "Treat 2")]
        [InlineData("SelectedEffect", "Treat1 * Treat2")]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA37(string testType, string testTypeValue)
        {
            string testName = "SMA37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;

            if (testType == "SelectedEffect")
            {
                model.SelectedEffect = testTypeValue;
                model.LSMeansSelected = true;
            }
            else if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("SelectedEffect", "Treat 1")]
        [InlineData("SelectedEffect", "Treat1 * Treat2")]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA38(string testType, string testTypeValue)
        {
            string testName = "SMA38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.Covariates = new string[] { "Cov 1" };
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;

            if (testType == "SelectedEffect")
            {
                model.SelectedEffect = testTypeValue;
                model.LSMeansSelected = true;
            }
            else if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("SelectedEffect", "Treat 1")]
        [InlineData("SelectedEffect", "Treat1 * Treat2")]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA39(string testType, string testTypeValue)
        {
            string testName = "SMA39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Log10";
            model.PrimaryFactor = "Treat2";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;

            if (testType == "SelectedEffect")
            {
                model.SelectedEffect = testTypeValue;
                model.LSMeansSelected = true;
            }
            else if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }

        [Theory]
        [InlineData("SelectedEffect", "Treat 1")]
        [InlineData("SelectedEffect", "Treat1 * Treat2")]
        [InlineData("AllPairwise", "Unadjusted(LSD)")]
        [InlineData("AllPairwise", "Tukey")]
        [InlineData("AllPairwise", "Holm")]
        [InlineData("AllPairwise", "Hochberg")]
        [InlineData("AllPairwise", "Hommel")]
        [InlineData("AllPairwise", "Bonferroni")]
        [InlineData("AllPairwise", "Benjamini-Hochberg")]
        [InlineData("ComparisonsToControl", "Unadjusted(LSD)")]
        [InlineData("ComparisonsToControl", "Dunnett")]
        [InlineData("ComparisonsToControl", "Holm")]
        [InlineData("ComparisonsToControl", "Hochberg")]
        [InlineData("ComparisonsToControl", "Hommel")]
        [InlineData("ComparisonsToControl", "Bonferroni")]
        [InlineData("ComparisonsToControl", "Benjamini-Hochberg")]
        public async Task SMA40(string testType, string testTypeValue)
        {
            string testName = "SMA40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "singlemeasures").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;

            if (testType == "SelectedEffect")
            {
                model.SelectedEffect = testTypeValue;
                model.LSMeansSelected = true;
            }
            else if (testType == "AllPairwise")
            {
                model.AllPairwise = testTypeValue;
            }
            else if (testType == "ComparisonsToControl")
            {
                model.ComparisonsBackToControl = testTypeValue;
                model.ControlGroup = "D0";
            }

            //Act
            string htmlResults = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveHtmlOutput("SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue, htmlResults);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + "-" + testType + "-" + testTypeValue + ".html"));
            Assert.Equal(expectedHtml, htmlResults);
        }
    }
}