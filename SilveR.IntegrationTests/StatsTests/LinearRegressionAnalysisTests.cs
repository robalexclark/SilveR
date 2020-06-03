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
    public class LinearRegressionAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public LinearRegressionAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task LRA1()
        {
            string testName = "LRA1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Response1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Response1) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA2()
        {
            string testName = "LRA2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response2";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Unfortunately as there are less than two valid responses in the dataset no analysis has been performed.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA3()
        {
            string testName = "LRA3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat4" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Categorical factor (Cat4). Please select another factor.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA4()
        {
            string testName = "LRA4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat5" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Categorical factor (Cat5) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA5()
        {
            string testName = "LRA5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp4";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA6()
        {
            string testName = "LRA6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };
            model.Covariates = new string[] { "Resp4" };
            model.PrimaryFactor = "Cat6";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Covariate (Resp3) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA7()
        {
            string testName = "LRA7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Resp3" };
            model.Covariates = new string[] { "Covariate" };
            model.PrimaryFactor = "Resp3";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp3) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA8()
        {
            string testName = "LRA8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp5";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp5) contains missing data.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA9()
        {
            string testName = "LRA9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };
            model.Covariates = new string[] { "Resp5" };
            model.PrimaryFactor = "Cat6";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Covariate (Resp3) contains missing data.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA10()
        {
            string testName = "LRA10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6", "Cat7" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the factors (Cat7) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA11()
        {
            string testName = "LRA11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "Log10";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp6 variable. Unfortunately some of the Resp6 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA12()
        {
            string testName = "LRA12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "Loge";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp7 variable. Unfortunately some of the Resp7 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA13()
        {
            string testName = "LRA13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "Square Root";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp6 variable. Unfortunately some of the Resp6 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA14()
        {
            string testName = "LRA14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ResponseTransformation = "Log10";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };
            model.Covariates = new string[] { "Resp6" };
            model.PrimaryFactor = "Cat6";
            model.CovariateTransformation = "Log10";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp6 variable. Unfortunately some of the Resp6 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA15()
        {
            string testName = "LRA15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ResponseTransformation = "Log10";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };
            model.Covariates = new string[] { "Resp7" };
            model.PrimaryFactor = "Cat6";
            model.CovariateTransformation = "Log10";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp7 variable. Unfortunately some of the Resp7 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA16()
        {
            string testName = "LRA16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };
            model.Covariates = new string[] { "Resp6" };
            model.PrimaryFactor = "Cat6";
            model.CovariateTransformation = "Square Root";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp6 variable. Unfortunately some of the Resp6 values are negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA17()
        {
            string testName = "LRA17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "ArcSine";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp6 variable. Unfortunately some of the Resp6 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA18()
        {
            string testName = "LRA18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "ArcSine";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA19()
        {
            string testName = "LRA19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ResponseTransformation = "ArcSine";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };
            model.Covariates = new string[] { "Resp6" };
            model.PrimaryFactor = "Cat6";
            model.CovariateTransformation = "ArcSine";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp6 variable. Unfortunately some of the Resp6 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA20()
        {
            string testName = "LRA20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ResponseTransformation = "ArcSine";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6" };
            model.Covariates = new string[] { "Resp8" };
            model.PrimaryFactor = "Cat6";
            model.CovariateTransformation = "ArcSine";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA21()
        {
            string testName = "LRA21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat6", "Cat8" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA22()
        {
            string testName = "LRA22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp10";
            model.ContinuousFactors = new string[] { "Cont5" };
            model.CategoricalFactors = new string[] { "Cat11", "Cat12" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA23()
        {
            string testName = "LRA23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ContinuousFactors = new string[] { "Resp4", "Resp6", "Resp7" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The continuous variable (Resp4) contains non-numeric data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA24()
        {
            string testName = "LRA24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA25()
        {
            string testName = "LRA25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA26()
        {
            string testName = "LRA26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont 3" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA27()
        {
            string testName = "LRA27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat1" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA28()
        {
            string testName = "LRA28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.CategoricalFactors = new string[] { "Cat1" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA29()
        {
            string testName = "LRA29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Square Root";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont 3" };
            model.CategoricalFactors = new string[] { "Cat1" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA30()
        {
            string testName = "LRA30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "None";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA31()
        {
            string testName = "LRA31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA32()
        {
            string testName = "LRA32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont 3" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA33()
        {
            string testName = "LRA33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Log10";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA34()
        {
            string testName = "LRA34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA35()
        {
            string testName = "LRA35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont 3" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA36()
        {
            string testName = "LRA36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Log10";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA37()
        {
            string testName = "LRA37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Loge";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.OtherDesignFactors = new string[] { "Block1", "Block£2" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA38()
        {
            string testName = "LRA38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Square Root";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont 3" };
            model.OtherDesignFactors = new string[] { "Block1", "Block£2", "Block3" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA39()
        {
            string testName = "LRA39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA40()
        {
            string testName = "LRA40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2" };
            model.OtherDesignFactors = new string[] { "Block1", "Block£2" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA41()
        {
            string testName = "LRA41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont 3" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2" };
            model.OtherDesignFactors = new string[] { "Block1", "Block£2", "Block3" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA42()
        {
            string testName = "LRA42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Log10";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Covariates = new string[] { "Covariate" };
            model.CovariateTransformation = "Log10";
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA43()
        {
            string testName = "LRA43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Loge";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Covariates = new string[] { "Covariate" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA44()
        {
            string testName = "LRA44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Square Root";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont 3" };
            model.Covariates = new string[] { "Covariate" };
            model.CovariateTransformation = "Square Root";
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA45()
        {
            string testName = "LRA45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Log10";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Covariates = new string[] { "Covariate" };
            model.PrimaryFactor = "Cat1";
            model.CovariateTransformation = "Log10";
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA46()
        {
            string testName = "LRA46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Loge";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Covariates = new string[] { "Covariate" };
            model.PrimaryFactor = "Cat1";
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA47()
        {
            string testName = "LRA47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Response1";
            model.ResponseTransformation = "Square Root";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont 3" };
            model.CategoricalFactors = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Covariates = new string[] { "Covariate" };
            model.PrimaryFactor = "Cat&2";
            model.CovariateTransformation = "Square Root";
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA48()
        {
            string testName = "LRA48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.ContinuousFactors = new string[] { "Cont5" };
            model.CategoricalFactors = new string[] { "Cat9", "Cat10" };
            model.Covariates = new string[] { "Covariate2" };
            model.PrimaryFactor = "Cat10";
            model.CovariateTransformation = "None";
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA49()
        {
            string testName = "LRA49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp11";
            model.ResponseTransformation = "None";
            model.ContinuousFactors = new string[] { "Cont5" };
            model.CategoricalFactors = new string[] { "Cat9", "Cat10" };
            model.Covariates = new string[] { "Covariate2" };
            model.PrimaryFactor = "Cat9";
            model.CovariateTransformation = "None";
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp11) contains missing data.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA50()
        {
            string testName = "LRA50";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ResponseTransformation = "None";
            model.ContinuousFactors = new string[] { "Resp10" };
            model.CategoricalFactors = new string[] { "Cat8" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Continuous factor (Resp10) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA51()
        {
            string testName = "LRA51";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "None";
            model.ContinuousFactors = new string[] { "Resp3" };
            model.CategoricalFactors = new string[] { "Cat6" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp5) contains missing data.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA52()
        {
            string testName = "LRA52";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp12";
            model.ResponseTransformation = "None";
            model.ContinuousFactors = new string[] { "Cont6" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA53()
        {
            string testName = "LRA53";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ResponseTransformation = "None";
            model.ContinuousFactors = new string[] { "Resp8" };
            model.CategoricalFactors = new string[] { "Cat8" };
            model.Covariates = new string[] { "Resp10" };
            model.PrimaryFactor = "Cat8";
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Covariate (Resp3) contains missing data.", warnings);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LinearRegressionAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("LinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA54()
        {
            string testName = "LRA54";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp3";
            model.ResponseTransformation = "None";
            model.ContinuousFactors = new string[] { "Resp8" };
            model.CategoricalFactors = new string[] { "Cat9" };
            model.Covariates = new string[] { "Cat8" };
            model.PrimaryFactor = "Cat9";
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Categorical factor (Cat9) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);            
        }

        [Fact]
        public async Task LRA55()
        {
            string testName = "LRA55";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LinearRegressionAnalysisModel model = new LinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Linear Regression").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.ContinuousFactors = new string[] { "Resp7" };
            model.OtherDesignFactors = new string[] { "Cat7" };
            model.ANOVASelected = true;
            model.Coefficients = true;
            model.AdjustedRSquared = true;
            model.ResidualsVsPredictedPlot = true;
            model.NormalProbabilityPlot = true;
            model.CooksDistancePlot = true;
            model.LeveragePlot = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the factors (Cat7) has only one level present in the dataset. Please select another factor.", errors);

            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }
    }
}