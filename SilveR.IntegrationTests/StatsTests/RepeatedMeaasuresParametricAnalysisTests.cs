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
    public class RepeatedMeasuresParametricAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public RepeatedMeasuresParametricAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task RMA1()
        {
            string testName = "RMA1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Subject factor field is required.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA2()
        {
            string testName = "RMA2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Repeated factor field is required.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA3()
        {
            string testName = "RMA3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 3";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat 1). Please select another factor.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA4()
        {
            string testName = "RMA4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp7";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the Treatment factor (Treat 1). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA5()
        {
            string testName = "RMA5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat8" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat8) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA6()
        {
            string testName = "RMA6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp4";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA7()
        {
            string testName = "RMA7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariates = new string[] { "Cov2" };
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Covariate (Cov2) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA8()
        {
            string testName = "RMA8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp2";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA9()
        {
            string testName = "RMA9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariates = new string[] { "Cov3" };
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Covariate (Cov3) contains missing data. Any response that does not have a corresponding covariate will be excluded from the analysis.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA10()
        {
            string testName = "RMA10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat9" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the factors (Treat9) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA11()
        {
            string testName = "RMA11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat10" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of one or more of the factors (Treat10). Please review your factor selection.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA12()
        {
            string testName = "RMA12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp5 variable. Unfortunately some of the Resp5 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA13()
        {
            string testName = "RMA13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp5 variable. Unfortunately some of the Resp5 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA14()
        {
            string testName = "RMA14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp5 variable. Unfortunately some of the Resp5 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA15()
        {
            string testName = "RMA15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Covariates = new string[] { "Cov4" };
            model.CovariateTransformation = "Loge";
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Cov4 variable. Unfortunately some of the Cov4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA16()
        {
            string testName = "RMA16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Covariates = new string[] { "Cov5" };
            model.CovariateTransformation = "Log10";
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Cov5 variable. Unfortunately some of the Cov5 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA17()
        {
            string testName = "RMA17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp5 variable. Unfortunately some of the Resp5 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA18()
        {
            string testName = "RMA18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 9";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp 9 variable. Unfortunately some of the Resp 9 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA19()
        {
            string testName = "RMA19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Covariates = new string[] { "Cov4" };
            model.CovariateTransformation = "ArcSine";
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Cov4 variable. Unfortunately some of the Cov4 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA20()
        {
            string testName = "RMA20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Covariates = new string[] { "Cov1" };
            model.CovariateTransformation = "ArcSine";
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Cov1 variable. Unfortunately some of the Cov1 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA21()
        {
            string testName = "RMA21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Covariates = new string[] { "Cov4" };
            model.CovariateTransformation = "Square Root";
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Cov4 variable. Unfortunately some of the Cov4 values are negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA22()
        {
            string testName = "RMA22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.OtherDesignFactors = new string[] { "Block3" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("According to the dataset at least one subject is associated with more than one level of one of the blocking factors. Please review this, as each subject must be associated with only one level of each blocking factor.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA23()
        {
            string testName = "RMA23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1", "Treat 2", "Treat14" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("According to the dataset at least one subject is associated with more than one level of the Treatment factor(s) or Treatment factor interactions. Please review this, in the repeated measures module each subject should be associated with only one level of each Treatment factor.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA24()
        {
            string testName = "RMA24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp11";
            model.Treatments = new string[] { "Treat 1", "Treat15" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One of the levels of the Treatment factor(s), or a combination of the levels of the Treatment factors, is not present at at least one of the timepoints. Please review this selection as all Treatment factors (and combinations thereof) must be present at each timepoint.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA25()
        {
            string testName = "RMA25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA26()
        {
            string testName = "RMA26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat 1" };
            model.Covariates = new string[] { "Cov1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA27()
        {
            string testName = "RMA27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat 1" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA28()
        {
            string testName = "RMA28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat 1" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA29()
        {
            string testName = "RMA29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA30()
        {
            string testName = "RMA30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA31()
        {
            string testName = "RMA31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA32()
        {
            string testName = "RMA32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA33()
        {
            string testName = "RMA33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA34()
        {
            string testName = "RMA34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA35()
        {
            string testName = "RMA35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat 1" };
            model.Covariates = new string[] { "Cov1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA36()
        {
            string testName = "RMA36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat 1" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA37()
        {
            string testName = "RMA37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat 1" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA38()
        {
            string testName = "RMA38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA39()
        {
            string testName = "RMA39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA40()
        {
            string testName = "RMA40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA41()
        {
            string testName = "RMA41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA42()
        {
            string testName = "RMA42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA43()
        {
            string testName = "RMA43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA44()
        {
            string testName = "RMA44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat 1" };
            model.Covariates = new string[] { "Cov1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA45()
        {
            string testName = "RMA45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat 1" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA46()
        {
            string testName = "RMA46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat 1" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA47()
        {
            string testName = "RMA47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA48()
        {
            string testName = "RMA48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA49()
        {
            string testName = "RMA49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA50()
        {
            string testName = "RMA50";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA51()
        {
            string testName = "RMA51";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Treat 2 * Treat3 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA52()
        {
            string testName = "RMA52";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat4" };
            model.Subject = "Animal2";
            model.RepeatedFactor = "Day2";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat4 * Day2";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA53()
        {
            string testName = "RMA53";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat4" };
            model.Covariates = new string[] { "Cov1" };
            model.Subject = "Animal2";
            model.RepeatedFactor = "Day2";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat4 * Day2";
            model.PrimaryFactor = "Treat 4";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA54()
        {
            string testName = "RMA54";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat4" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Subject = "Animal2";
            model.RepeatedFactor = "Day2";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat4 * Day2";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA55()
        {
            string testName = "RMA55";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat4" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal2";
            model.RepeatedFactor = "Day2";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat4 * Day2";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA56()
        {
            string testName = "RMA56";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat4", "Treat5" };
            model.Subject = "Animal2";
            model.RepeatedFactor = "Day2";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat4 * Treat5 * Day2";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA57()
        {
            string testName = "RMA57";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat4", "Treat5", "Treat6" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal2";
            model.RepeatedFactor = "Day2";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat4 * Treat5 * Day2";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA58()
        {
            string testName = "RMA58";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA59()
        {
            string testName = "RMA59";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA60()
        {
            string testName = "RMA60";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat 1", "Treat 2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Treat 2 * Treat3 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA61()
        {
            string testName = "RMA61";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA62()
        {
            string testName = "RMA62";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1" };
            model.Covariates = new string[] { "Cov1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA63()
        {
            string testName = "RMA63";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA64()
        {
            string testName = "RMA64";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA65()
        {
            string testName = "RMA65";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA66()
        {
            string testName = "RMA66";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat 1", "Treat 2", "Treat3" };
            model.OtherDesignFactors = new string[] { "Block1", "Block2" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA67()
        {
            string testName = "RMA67";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp12";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat17", "Treat18", "Treat19", "Treat20" };
            model.Subject = "Animal3";
            model.RepeatedFactor = "Day3";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat17 * Treat18 * Treat19 * Treat20 * Day3";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA68()
        {
            string testName = "RMA68";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp13";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat21", "Treat22" };
            model.Covariates = new string[] { "Cov6a" };
            model.Subject = "Animal4";
            model.RepeatedFactor = "Day4";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat21 * Treat22 * Day4";
            model.PrimaryFactor = "Treat21";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA69()
        {
            string testName = "RMA69";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "PVtestResp1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "PVTestTreat1" };
            model.Covariates = new string[] { "PVTestCOV1a" };
            model.Subject = "PVTestAnimal1";
            model.RepeatedFactor = "PVTestDay1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "PVTestTreat1 * PVTestDay1";
            model.PrimaryFactor = "PVTestTreat1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA70()
        {
            string testName = "RMA70";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "PVtestResp1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "PVTestTreat1" };
            model.Covariates = new string[] { "PVTestCOV1b" };
            model.Subject = "PVTestAnimal1";
            model.RepeatedFactor = "PVTestDay1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "PVTestTreat1 * PVTestDay1";
            model.PrimaryFactor = "PVTestTreat1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA71()
        {
            string testName = "RMA71";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "PVTestResp2";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "PVTestTreat2" };
            model.Covariates = new string[] { "PVTestCov2a" };
            model.Subject = "PVTestAnimal2";
            model.RepeatedFactor = "PVTestDay2";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "PVTestTreat2 * PVTestDay2";
            model.PrimaryFactor = "PVTestTreat2";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA72()
        {
            string testName = "RMA72";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "PVTestResp2";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "PVTestTreat2" };
            model.Covariates = new string[] { "PVTestCov2b" };
            model.Subject = "PVTestAnimal2";
            model.RepeatedFactor = "PVTestDay2";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "PVTestTreat2 * PVTestDay2";
            model.PrimaryFactor = "PVTestTreat2";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA73()
        {
            string testName = "RMA73";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "CVResp";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "CVTreat1" };
            model.Subject = "CVAnimal1";
            model.RepeatedFactor = "CVTime1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "CVTreat1 * CVTime1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA74()
        {
            string testName = "RMA74";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "CVResp";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "CVTreat2" };
            model.Subject = "CVAnimal2";
            model.RepeatedFactor = "CVTime2";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "CVTreat2 * CVTime2";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA75()
        {
            string testName = "RMA75";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1", "Cov6", "Cov7" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA76()
        {
            string testName = "RMA76";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1", "Cov6", "Cov7" };
            model.CovariateTransformation = "Square Root";
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Compound Symmetric";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA77()
        {
            string testName = "RMA77";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1", "Cov6", "Cov7" };
            model.CovariateTransformation = "None";
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA78()
        {
            string testName = "RMA78";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1", "Cov6", "Cov7" };
            model.CovariateTransformation = "Log10";
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA79()
        {
            string testName = "RMA79";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1", "Cov6", "Cov7" };
            model.CovariateTransformation = "None";
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Treat 2 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllComparisonsWithinSelected;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA80()
        {
            string testName = "RMA80";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1", "Treat 2" };
            model.Covariates = new string[] { "Cov1", "Cov6", "Cov7" };
            model.CovariateTransformation = "Log10";
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "Autoregressive(1)";
            model.SelectedEffect = "Treat 1 * Day 1";
            model.PrimaryFactor = "Treat 1";
            model.LSMeansSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;
            model.NormalPlotSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA81()
        {
            string testName = "RMA81";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "CVResp";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "CVTreat1" };
            model.Subject = "CVAnimal1";
            model.RepeatedFactor = "CVTime1";
            model.Covariance = "Unstructured";
            model.SelectedEffect = "CVTreat1 * CVTime1";
            model.PrimaryFactor = "CVTreat1";
            model.LSMeansSelected = true;
            model.NormalPlotSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task RMA82()
        {
            string testName = "RMA82";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "MDResp";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "MDTreat" };
            model.RepeatedFactor = "MDTime";
            model.Subject = "MDAnimal";
            model.Covariance = "CompoundSymmetric";
            model.SelectedEffect = "MDTreat * MDTime";
            model.PrimaryFactor = "MDTreat";
            model.LSMeansSelected = true;
            model.NormalPlotSelected = true;
            model.ComparisonType = RepeatedMeasuresParametricAnalysisModel.ComparisonOption.AllPairwiseComparisons;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Subject factor (MDAnimal). This can lead to unreliable results so you may want to remove any subjects from the dataset with only one replicate.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("RepeatedMeasuresParametricAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "RepeatedMeasuresParametricAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task RMA83()
        {
            string testName = "RMA83";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Repeated Measures Parametric").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.RepeatedFactor = "Day 1";
            model.Subject = "Animal1";
            model.OtherDesignFactors = new string[] { "Treat9" };
            model.Covariance = "CompoundSymmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the factors (Treat9) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }
    }
}