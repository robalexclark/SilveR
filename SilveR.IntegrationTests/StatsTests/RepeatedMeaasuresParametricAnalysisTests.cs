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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.RepeatedFactor = "Day1";
            model.Covariance = "CS";

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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.Covariance = "CS";

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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp 3";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "CS";

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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp7";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "CS";

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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat8" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "CS";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor selected (Treat8) contains missing data where there are observations present in the Response variable. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }

        [Fact]
        public async Task RMA6()
        {
            string testName = "RMA6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp4";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "CS";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response selected (Resp4) contain non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }
        
        [Fact]
        public async Task RMA7()
        {
            string testName = "RMA7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariates = new string[] { "Cov2" };
            model.Covariance = "CS";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Covariate selected (Cov2) contain non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, errors);
        }


        [Fact]
        public async Task RMA8()
        {
            string testName = "RMA8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp2";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "CS";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response selected (Resp2) contains missing data.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA9()
        {
            string testName = "RMA9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariates = new string[] { "Cov3" };
            model.Covariance = "CS";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Covariate selected (Cov3) contains missing data. Any response that does not have a corresponding covariate will be excluded from the analysis.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }

        [Fact]
        public async Task RMA10()
        {
            string testName = "RMA10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            RepeatedMeasuresParametricAnalysisModel model = new RepeatedMeasuresParametricAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat9" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "CS";

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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat10" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "CS";

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
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "repeatedmeasures").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat 1" };
            model.Subject = "Animal1";
            model.RepeatedFactor = "Day 1";
            model.Covariance = "CS";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/RepeatedMeasuresParametricAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp5 variable. Unfortunately some of the Resp5 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("RepeatedMeasuresParametricAnalysis", testName, warnings);
        }
    }
}