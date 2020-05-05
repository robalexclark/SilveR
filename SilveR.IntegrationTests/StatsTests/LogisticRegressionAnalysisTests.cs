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
            Assert.Contains("The Covariate (Covariate3) contain non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
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
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Response4) contain non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
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

        //[Fact]
        //public async Task LRA11()
        //{
        //    string testName = "LRA11";

        //    //Arrange
        //    HttpClient client = _factory.CreateClient();

        //    LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
        //    model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
        //    model.Response = "Response1b";
        //    model.ContinuousFactors = new string[] { "Cont1" };
        //    model.Covariates = new string[] { "Covariate6" };
        //    model.CovariateTransformation = "Log10";
        //    model.PositiveResult = "1";

        //    //Act
        //    HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
        //    IEnumerable<string> errors = await Helpers.ExtractErrors(response);

        //    //Assert
        //    Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Cat4). Please select another factor.", errors);
        //    Helpers.SaveOutput("LogisticRegressionAnalysis", testName, errors);
        //}
    }
}