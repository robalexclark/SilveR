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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Resp 1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp 1) has been selected in more than one input category, please change your input options.", errors);
            Assert.Contains("Treatment factor (Resp 1) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA2()
        {
            string testName = "SMA2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Tre at4" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Tre at4) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA5()
        {
            string testName = "SMA5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp4";
            model.Treatments = new string[] { "Treat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA6()
        {
            string testName = "SMA6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov4" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Covariate (Cov4) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task SMA7()
        {
            string testName = "SMA7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp5";
            model.Treatments = new string[] { "Treat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp5) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA8()
        {
            string testName = "SMA8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov5" };
            model.PrimaryFactor = "Treat1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Covariate (Cov5) contains missing data. Any response that does not have a corresponding covariate will be excluded from the analysis.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task SMA9()
        {
            string testName = "SMA9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp8";
            model.Treatments = new string[] { "Treat12", "Treat13" };
            model.SelectedEffect = "Treat12";
            model.Significance = "90";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA22()
        {
            string testName = "SMA22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp11";
            model.Treatments = new string[] { "Treat14" };
            model.SelectedEffect = "Treat14";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA23()
        {
            string testName = "SMA23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA24()
        {
            string testName = "SMA24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.SelectedEffect = "Treat1";
            model.AllPairwise = "Tukey";
            model.ComparisonsBackToControl = "Dunnett";
            model.ControlGroup = "D3";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA25()
        {
            string testName = "SMA25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.SelectedEffect = "Treat2";
            model.AllPairwise = "Holm";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "F";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA26()
        {
            string testName = "SMA26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.SelectedEffect = "Treat2";
            model.AllPairwise = "Hochberg";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "F";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA27()
        {
            string testName = "SMA27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.SelectedEffect = "Treat2";
            model.AllPairwise = "Hommel";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "F";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA28()
        {
            string testName = "SMA28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov 1" };
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.SelectedEffect = "Treat1";
            model.AllPairwise = "Bonferroni";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA29()
        {
            string testName = "SMA29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Log10";
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.SelectedEffect = "Treat1";
            model.AllPairwise = "Benjamini-Hochberg";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D10";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA30()
        {
            string testName = "SMA30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA31()
        {
            string testName = "SMA31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "None";
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Tukey";
            model.ComparisonsBackToControl = "Dunnett";
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA32()
        {
            string testName = "SMA32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Holm";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA33()
        {
            string testName = "SMA33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hochberg";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA34()
        {
            string testName = "SMA34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hommel";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA35()
        {
            string testName = "SMA35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Bonferroni";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA36()
        {
            string testName = "SMA36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Benjamini-Hochberg";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA37()
        {
            string testName = "SMA37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "F";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA38()
        {
            string testName = "SMA38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.Covariates = new string[] { "Cov 1" };
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Tukey";
            model.ComparisonsBackToControl = "Dunnett";
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA39()
        {
            string testName = "SMA39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Holm";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA40()
        {
            string testName = "SMA40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hochberg";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA41()
        {
            string testName = "SMA41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "None";
            model.PrimaryFactor = "Treat2";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hommel";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA42()
        {
            string testName = "SMA42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
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
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Bonferroni";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA43()
        {
            string testName = "SMA43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Benjamini-Hochberg";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA44()
        {
            string testName = "SMA44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1 * Treat2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA45()
        {
            string testName = "SMA45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Blo ck1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1 * Treat2 * Treat3";
            model.LSMeansSelected = true;
            model.AllPairwise = "Tukey";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA46()
        {
            string testName = "SMA46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Holm";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "M";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA47()
        {
            string testName = "SMA47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1 * Treat2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hochberg";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA48()
        {
            string testName = "SMA48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.Covariates = new string[] { "Cov 1" };
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1 * Treat2 * Treat3";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hommel";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA49()
        {
            string testName = "SMA49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Log10";
            model.PrimaryFactor = "Treat2";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Bonferroni";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA50()
        {
            string testName = "SMA50";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat3";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1 * Treat2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Benjamini-Hochberg";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA51()
        {
            string testName = "SMA51";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "None";
            model.PrimaryFactor = "Treat3";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA52()
        {
            string testName = "SMA52";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat3";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Tukey";
            model.ComparisonsBackToControl = "Dunnett";
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA53()
        {
            string testName = "SMA53";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.Significance = "0.1";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Holm";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA54()
        {
            string testName = "SMA54";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.1";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hochberg";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA55()
        {
            string testName = "SMA55";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat8" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat8";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hommel";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "A";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA56()
        {
            string testName = "SMA56";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "None";
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.1";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1 * Treat2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA57()
        {
            string testName = "SMA57";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Blo ck1", "Blo ck2" };
            model.Covariates = new string[] { "Cov 1" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat3";
            model.Significance = "0.1";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat2 * Treat3";
            model.LSMeansSelected = true;
            model.AllPairwise = "Tukey";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA58()
        {
            string testName = "SMA58";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.AllPairwise = "Holm";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp7) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA59()
        {
            string testName = "SMA59";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9", "Treat10" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hochberg";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp7) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA60()
        {
            string testName = "SMA60";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9", "Treat10", "Treat11" };
            model.OtherDesignFactors = new string[] { "Block3" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hommel";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp7) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA61()
        {
            string testName = "SMA61";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.Covariates = new string[] { "Cov6" };
            model.PrimaryFactor = "Treat9";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.AllPairwise = "Bonferroni";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp7) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA62()
        {
            string testName = "SMA62";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat10", "Treat9" };
            model.Covariates = new string[] { "Cov6" };
            model.PrimaryFactor = "Treat9";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat10";
            model.LSMeansSelected = true;
            model.AllPairwise = "Benjamini-Hochberg";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp7) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA63()
        {
            string testName = "SMA63";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat10", "Treat9", "Treat11" };
            model.OtherDesignFactors = new string[] { "Block3", "Block4" };
            model.Covariates = new string[] { "Cov6" };
            model.PrimaryFactor = "Treat9";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat10";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp7) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA64()
        {
            string testName = "SMA64";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat10", "Treat9", "Treat11" };
            model.OtherDesignFactors = new string[] { "Block3", "Block4" };
            model.Covariates = new string[] { "Cov6" };
            model.PrimaryFactor = "Treat9";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat10";
            model.LSMeansSelected = true;
            model.AllPairwise = "Tukey";
            model.ComparisonsBackToControl = "Dunnett";
            model.ControlGroup = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp7) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA65()
        {
            string testName = "SMA65";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat10", "Treat9", "Treat11" };
            model.OtherDesignFactors = new string[] { "Block3", "Block4" };
            model.Covariates = new string[] { "Cov3" };
            model.CovariateTransformation = "ArcSine";
            model.PrimaryFactor = "Treat9";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat10";
            model.LSMeansSelected = true;
            model.AllPairwise = "Holm";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp7) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA66()
        {
            string testName = "SMA66";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp1 2";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 15", "Treat16" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat 15 * Treat16";
            model.LSMeansSelected = true;
            model.AllPairwise = "Tukey";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA67()
        {
            string testName = "SMA67";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 13";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 17", "Treat 18", "Treat 19" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat 17 * Treat 18 * Treat 19";
            model.LSMeansSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA68()
        {
            string testName = "SMA68";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 13";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 17", "Treat 18", "Treat 19", "Treat 20" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat 17 * Treat 18 * Treat 19 * Treat 20";
            model.LSMeansSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA69()
        {
            string testName = "SMA69";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp 13";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 17", "Treat 18", "Treat 19", "Treat 20", "Treat 21" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat 17 * Treat 18 * Treat 19 * Treat 20 * Treat 21";
            model.LSMeansSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA70()
        {
            string testName = "SMA70";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp14";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat22", "Treat23" };
            model.Covariates = new string[] { "Cov7" };
            model.PrimaryFactor = "Treat22";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat22 * Treat23";
            model.LSMeansSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA71()
        {
            string testName = "SMA71";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp15";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat22", "Treat23" };
            model.Covariates = new string[] { "Cov7" };
            model.PrimaryFactor = "Treat22";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat22 * Treat23";
            model.LSMeansSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp15) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA72()
        {
            string testName = "SMA72";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "PVTestResponse1a";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "PVTestGroup1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "PVTestGroup1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA73()
        {
            string testName = "SMA73";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "PVTestResponse1b";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "PVTestGroup1" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "PVTestGroup1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Tukey";
            model.ComparisonsBackToControl = "Dunnett";
            model.ControlGroup = "1";


            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA74()
        {
            string testName = "SMA74";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "PVTestResponse2";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "PVTEstGroup2" };
            model.Covariates = new string[] { "PVTEstCovariate2a" };
            model.PrimaryFactor = "PVTEstGroup2";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "PVTEstGroup2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Holm";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA75()
        {
            string testName = "SMA75";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "PVTestResponse2";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "PVTEstGroup2" };
            model.Covariates = new string[] { "PVTEstCovariate2b" };
            model.PrimaryFactor = "PVTEstGroup2";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "PVTEstGroup2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hommel";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA76()
        {
            string testName = "SMA76";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "CVResp";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "CVTreat1", "CVTreat2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "CVTreat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "B";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA77()
        {
            string testName = "SMA77";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "CVResp";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "CVTreat3", "CVTreat4" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "CVTreat3 * CVTreat4";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA78() //INCOMPLETE FACTORIAL
        {
            string testName = "SMA78";

            //Arrange
            HttpClient client = _factory.CreateClient();

            IncompleteFactorialParametricAnalysisModel model = new IncompleteFactorialParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "IFResp";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "IFTreat1", "IFTreat2" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "IFTreat1 * IFTreat2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "IncompleteFactorialParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA79() //INCOMPLETE FACTORIAL
        {
            string testName = "SMA79";

            //Arrange
            HttpClient client = _factory.CreateClient();

            IncompleteFactorialParametricAnalysisModel model = new IncompleteFactorialParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "IFResp";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "IFTreat3", "IFTreat4" };
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "IFTreat3 * IFTreat4";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "IncompleteFactorialParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA80()
        {
            string testName = "SMA80";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Resp 1", "Cov 1" };
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Unadjusted (LSD)";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA81()
        {
            string testName = "SMA81";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.Covariates = new string[] { "Resp 1", "Cov 1" };
            model.PrimaryFactor = "Treat2";
            model.CovariateTransformation = "Square Root";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1 * Treat2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hochberg";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA82()
        {
            string testName = "SMA82";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.Covariates = new string[] { "Resp 1", "Cov 1" };
            model.CovariateTransformation = "Loge";
            model.PrimaryFactor = "Treat2";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1 * Treat2 * Treat3";
            model.LSMeansSelected = true;
            model.AllPairwise = "Benjamini-Hochberg";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA83()
        {
            string testName = "SMA83";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.OtherDesignFactors = new string[] { "Blo ck1" };
            model.Covariates = new string[] { "Resp 1", "Cov 1" };
            model.CovariateTransformation = "Log10";
            model.PrimaryFactor = "Treat1";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Holm";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA84()
        {
            string testName = "SMA84";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1", "Treat2" };
            model.OtherDesignFactors = new string[] { "Blo ck1" };
            model.Covariates = new string[] { "Cov 1", "Cov1a" };
            model.CovariateTransformation = "ArcSine";
            model.PrimaryFactor = "Treat2";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.AllPairwise = "Tukey";
            model.ComparisonsBackToControl = "Dunnett";
            model.ControlGroup = "D0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA85()
        {
            string testName = "SMA85";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat1", "Treat2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Blo ck1" };
            model.Covariates = new string[] { "Cov 1", "Resp 1" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat2";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat1 * Treat2";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hommel";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA86()
        {
            string testName = "SMA86";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9", "Treat10" };
            model.Covariates = new string[] { "Cov 1", "Resp 1" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat10";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hommel";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp7) contains missing data.", warnings);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("SingleMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA87()
        {
            string testName = "SMA87";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresParametricAnalysisModel model = new SingleMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Single Measures Parametric").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat6" };
            model.OtherDesignFactors = new string[] { "T reat5" };
            model.PrimaryFactor = "Treat6";
            model.Significance = "0.05";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.AllPairwise = "Hommel";
            model.ComparisonsBackToControl = model.AllPairwise;
            model.ControlGroup = "2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the factors (T reat5) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("SingleMeasuresParametricAnalysis", testName, errors);
        }
    }
}