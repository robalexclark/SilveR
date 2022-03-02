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
    public class LogisticRegressionAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public LogisticRegressionAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task LRA1()
        {
            string testName = "LRA1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Response1" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Response1) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA2()
        {
            string testName = "LRA2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response2";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat1" };
            model.PositiveResult = "2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Unfortunately as there are less than two valid responses in the dataset no analysis has been performed.", errors);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA3()
        {
            string testName = "LRA3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response4";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat4" };
            model.PositiveResult = "2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the Treatment factor (Cat4). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA4()
        {
            string testName = "LRA4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat5" };
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Cat5) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA5()
        {
            string testName = "LRA5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response3";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response must have 2 distinct values.", errors);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA6()
        {
            string testName = "LRA6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Covariates = new string[] { "Covariate3" };
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Covariate (Covariate3) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA7()
        {
            string testName = "LRA7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response4";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Response4) contains missing data.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA8()
        {
            string testName = "LRA8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Covariates = new string[] { "Covariate4" };
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Covariate (Covariate4) contains missing data. Any response that does not have a corresponding covariate will be excluded from the analysis.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA9()
        {
            string testName = "LRA9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat4" };
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Cat4). Please select another factor.", errors);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA10()
        {
            string testName = "LRA10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Covariates = new string[] { "Covariate5" };
            model.CovariateTransformation = "Log10";
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Covariate5 variable. Unfortunately some of the Covariate5 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA11()
        {
            string testName = "LRA11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Covariates = new string[] { "Covariate6" };
            model.CovariateTransformation = "Log10";
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Covariate6 variable. Unfortunately some of the Covariate6 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA12()
        {
            string testName = "LRA12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Covariates = new string[] { "Covariate5" };
            model.CovariateTransformation = "Square Root";
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Covariate5 variable. Unfortunately some of the Covariate5 values are negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA13()
        {
            string testName = "LRA13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Covariates = new string[] { "Covariate7" };
            model.CovariateTransformation = "ArcSine";
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Covariate7 variable. Unfortunately some of the Covariate7 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA14()
        {
            string testName = "LRA14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont4" };
            model.ContinuousFactorsTransformation = "Log10";
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Cont4 variable. Unfortunately some of the Cont4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA15()
        {
            string testName = "LRA15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont5" };
            model.ContinuousFactorsTransformation = "Loge";
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Cont5 variable. Unfortunately some of the Cont5 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA16()
        {
            string testName = "LRA16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont4" };
            model.ContinuousFactorsTransformation = "Square Root";
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Cont4 variable. Unfortunately some of the Cont4 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA17()
        {
            string testName = "LRA17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont6" };
            model.ContinuousFactorsTransformation = "ArcSine";
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Cont6 variable. Unfortunately some of the Cont6 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task LRA18()
        {
            string testName = "LRA18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.Treatments = new string[] { "Cat6", "Cat7" };
            model.ContinuousFactors = new string[] { "Cont1" };
            model.PositiveResult = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA19()
        {
            string testName = "LRA19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response5";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Response5) contains missing data.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA20()
        {
            string testName = "LRA20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Covariate3" };
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Continuous factor (Covariate3) contains non-numeric data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA21()
        {
            string testName = "LRA21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Covariate4" };
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Continuous factor (Covariate4) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task LRA22()
        {
            string testName = "LRA22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response 6";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.PositiveResult = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Response 6) contains missing data.", warnings);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA23()
        {
            string testName = "LRA23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Block 1" };
            model.PositiveResult = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA24()
        {
            string testName = "LRA24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.PositiveResult = "0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA25()
        {
            string testName = "LRA25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.PositiveResult = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA26()
        {
            string testName = "LRA26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.PositiveResult = "0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task LRA27()
        {
            string testName = "LRA27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat1" };
            model.PositiveResult = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA28()
        {
            string testName = "LRA28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.ContinuousFactorsTransformation = "None";
            model.Treatments = new string[] { "Cat1" };
            model.PositiveResult = "-1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA29()
        {
            string testName = "LRA29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.ContinuousFactorsTransformation = "Square Root";
            model.Treatments = new string[] { "Cat1" };
            model.PositiveResult = "4";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA30()
        {
            string testName = "LRA30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat1","Cat&2" };
            model.PositiveResult = "-1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA31()
        {
            string testName = "LRA31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Treatments = new string[] { "Cat1", "Cat&2" };
            model.PositiveResult = "4";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA32()
        {
            string testName = "LRA32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.Treatments = new string[] { "Cat1", "Cat&2" };
            model.PositiveResult = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task LRA33()
        {
            string testName = "LRA33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.ContinuousFactorsTransformation = "Log10";
            model.Treatments = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.PositiveResult = "0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA34()
        {
            string testName = "LRA34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Treatments = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.PositiveResult = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA35()
        {
            string testName = "LRA35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.Treatments = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.PositiveResult = "0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA36()
        {
            string testName = "LRA36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.ContinuousFactorsTransformation = "Log10";
            model.OtherDesignFactors = new string[] { "Block 1" };
            model.PositiveResult = "-1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA37()
        {
            string testName = "LRA37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.ContinuousFactorsTransformation = "Loge";
            model.OtherDesignFactors = new string[] { "Block 1", "Block£2" };
            model.PositiveResult = "4";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task LRA38()
        {
            string testName = "LRA38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.ContinuousFactorsTransformation = "Square Root";
            model.OtherDesignFactors = new string[] { "Block 1", "Block£2", "Block 3" };
            model.PositiveResult = "-1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA39()
        {
            string testName = "LRA39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1"};
            model.Treatments = new string[] { "Cat1", "Cat&2" };
            model.OtherDesignFactors = new string[] { "Block 1" };
            model.PositiveResult = "4";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA40()
        {
            string testName = "LRA40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Treatments = new string[] { "Cat1", "Cat&2" };
            model.OtherDesignFactors = new string[] { "Block 1", "Block£2" };
            model.PositiveResult = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA41()
        {
            string testName = "LRA41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.Treatments = new string[] { "Cat1", "Cat&2" };
            model.OtherDesignFactors = new string[] { "Block 1", "Block£2", "Block 3" };
            model.PositiveResult = "0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA42()
        {
            string testName = "LRA42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.ContinuousFactorsTransformation = "Log10";
            model.Covariates = new string[] { "Covariate1" };
            model.CovariateTransformation = "Log10";
            model.PositiveResult = "1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA43()
        {
            string testName = "LRA43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.ContinuousFactorsTransformation = "Loge";
            model.Covariates = new string[] { "Covariate1" };
            model.PositiveResult = "0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA44()
        {
            string testName = "LRA44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.Covariates = new string[] { "Covariate1" };
            model.CovariateTransformation = "Square Root";
            model.PositiveResult = "-1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA45()
        {
            string testName = "LRA45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] {"Cat1", "Cat&2", "Cat$3" };
            model.Covariates = new string[] { "Covariate1" };
            model.CovariateTransformation = "Log10";
            model.PositiveResult = "0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA46()
        {
            string testName = "LRA46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Treatments = new string[] {"Cat1", "Cat&2", "Cat$3" };
            model.Covariates = new string[] { "Covariate1", "Covariate2" };
            model.PositiveResult = "0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA47()
        {
            string testName = "LRA47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.Treatments = new string[] {"Cat1", "Cat&2", "Cat$3" };
            model.Covariates = new string[] { "Covariate1", "Covariate2" };
            model.CovariateTransformation = "Square Root";
            model.PositiveResult = "0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA48()
        {
            string testName = "LRA48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA49()
        {
            string testName = "LRA49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }
        
        [Fact]
        public async Task LRA50()
        {
            string testName = "LRA50";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }
        
        [Fact]
        public async Task LRA51()
        {
            string testName = "LRA51";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] {"Cat1"};
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA52()
        {
            string testName = "LRA52";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Treatments = new string[] {"Cat1"};
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA53()
        {
            string testName = "LRA53";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.ContinuousFactorsTransformation = "Square Root";
            model.Treatments = new string[] {"Cat1"};
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA54()
        {
            string testName = "LRA54";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] {"Cat1", "Cat&2"};
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA55()
        {
            string testName = "LRA55";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Treatments = new string[] {"Cat1", "Cat&2"};
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA56()
        {
            string testName = "LRA56";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.Treatments = new string[] {"Cat1", "Cat&2"};
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA57()
        {
            string testName = "LRA57";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] {"Cat1", "Cat&2", "Cat$3" };
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA58()
        {
            string testName = "LRA58";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Treatments = new string[] {"Cat1", "Cat&2", "Cat$3" };
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA59()
        {
            string testName = "LRA59";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.Treatments = new string[] {"Cat1", "Cat&2", "Cat$3" };
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA60()
        {
            string testName = "LRA60";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.OtherDesignFactors = new string[] { "Block 1" };
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA61()
        {
            string testName = "LRA61";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.OtherDesignFactors = new string[] { "Block 1", "Block£2" };
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA62()
        {
            string testName = "LRA62";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.OtherDesignFactors = new string[] { "Block 1", "Block£2", "Block 3" };
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA63()
        {
            string testName = "LRA63";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat1", "Cat&2" };
            model.OtherDesignFactors = new string[] { "Block 1" };
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA64()
        {
            string testName = "LRA64";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Treatments = new string[] { "Cat1", "Cat&2" };
            model.OtherDesignFactors = new string[] { "Block 1", "Block£2" };
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]        
        public async Task LRA65()
        {
            string testName = "LRA65";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.Treatments = new string[] { "Cat1", "Cat&2" };
            model.OtherDesignFactors = new string[] { "Block 1", "Block£2", "Block 3" };
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA66()
        {
            string testName = "LRA66";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Covariates = new string[] { "Covariate1" };
            model.CovariateTransformation = "Log10";
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA67()
        {
            string testName = "LRA67";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Covariates = new string[] { "Covariate1" };
            model.CovariateTransformation = "None";
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA68()
        {
            string testName = "LRA68";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.Covariates = new string[] { "Covariate1" };
            model.CovariateTransformation = "Square Root";
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA69()
        {
            string testName = "LRA69";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.OtherDesignFactors = new string[] { "Block 1" };
            model.Covariates = new string[] { "Covariate1" };
            model.CovariateTransformation = "Log10";
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA70()
        {
            string testName = "LRA70";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2" };
            model.Treatments = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.OtherDesignFactors = new string[] { "Block 1" };
            model.Covariates = new string[] { "Covariate1", "Covariate2" };
            model.CovariateTransformation = "None";
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA71()
        {
            string testName = "LRA71";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1", "Cont2", "Cont3" };
            model.Treatments = new string[] { "Cat1", "Cat&2", "Cat$3" };
            model.OtherDesignFactors = new string[] { "Block 1" };
            model.Covariates = new string[] { "Covariate1", "Covariate2" };
            model.CovariateTransformation = "Square Root";
            model.PositiveResult = "N";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA72()
        {
            string testName = "LRA72";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat1" };
            model.PositiveResult = "0";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA73()
        {
            string testName = "LRA73";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat1" };
            model.PositiveResult = "-1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA74()
        {
            string testName = "LRA74";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1a";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat1" };
            model.PositiveResult = "4";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task LRA75()
        {
            string testName = "LRA75";

            //Arrange
            HttpClient client = _factory.CreateClient();

            LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
            model.Response = "Response1b";
            model.ContinuousFactors = new string[] { "Cont1" };
            model.Treatments = new string[] { "Cat1" };
            model.PositiveResult = "Y";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("LogisticRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "LogisticRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }
    }
}