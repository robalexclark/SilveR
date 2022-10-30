using SilveR.StatsModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class MultivariateAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public MultivariateAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task MVA1()
        {
            string testName = "MVA1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length"};
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Responses requires at least two entries.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }

        [Fact]
        public async Task MVA2()
        {
            string testName = "MVA2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length" };
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.ClusterAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Responses requires at least two entries.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }

        [Fact]
        public async Task MVA3()
        {
            string testName = "MVA3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length" };
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Responses requires at least two entries.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }

        [Fact]
        public async Task MVA4()
        {
            string testName = "MVA4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.CategoricalPredictor = "Categorial factor";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("When performing a PCA analysis the categorical predictor you have selected will not be used. If you do need to use them in the analysis, then another analysis option may be more appropriate.", warnings);
            Helpers.SaveOutput("MultivariateAnalysis", testName, warnings);
        }

        [Fact]
        public async Task MVA5()
        {
            string testName = "MVA5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.CategoricalPredictor = "Categorial factor";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.ClusterAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("When performing a Cluster analysis the categorical predictor you have selected will not be used. If you do need to use them in the analysis, then another analysis option may be more appropriate.", warnings);
            Helpers.SaveOutput("MultivariateAnalysis", testName, warnings);
        }

        [Fact]
        public async Task MVA6()
        {
            string testName = "MVA6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width (reduced)", "Petal length" };
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the response variables contains missing data. Please delete any rows of the dataset that contain missing data prior to running the analysis.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }

        [Fact]
        public async Task MVA7()
        {
            string testName = "MVA7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width (reduced)", "Petal length" };
            model.CaseID = "Case ID";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the response variables contains missing data. Please delete any rows of the dataset that contain missing data prior to running the analysis.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }

        [Fact]
        public async Task MVA8()
        {
            string testName = "MVA8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width (reduced)", "Petal length" };
            model.CategoricalPredictor = "Categorial factor";
            model.CaseID = "Case ID";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the response variables contains missing data. Please delete any rows of the dataset that contain missing data prior to running the analysis.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }

        [Fact]
        public async Task MVA9()
        {
            string testName = "MVA9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.CaseID = "Case ID reduced";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Case ID (Case ID reduced) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }

        [Fact]
        public async Task MVA10()
        {
            string testName = "MVA10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.CaseID = "Case ID reduced";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.ClusterAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Case ID (Case ID reduced) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }

        [Fact]
        public async Task MVA11()
        {
            string testName = "MVA11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.CategoricalPredictor = "Categorial factor";
            model.CaseID = "Case ID reduced";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Case ID (Case ID reduced) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }

        [Fact]
        public async Task MVA12()
        {
            string testName = "MVA12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.CategoricalPredictor = "Categorial (reduced)";
            model.CaseID = "Case ID";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Categorical predictor (Categorial (reduced)) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }


        [Fact]
        public async Task MVA13()
        {
            string testName = "MVA13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };

            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task MVA14()
        {
            string testName = "MVA14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.ResponseTransformation = "Loge";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task MVA15()
        {
            string testName = "MVA15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.ResponseTransformation = "Log10";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task MVA16()
        {
            string testName = "MVA16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.ResponseTransformation = "Square Root";

            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task MVA17()
        {
            string testName = "MVA17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.CategoricalPredictor = "Categorial factor";

            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task MVA18()
        {
            string testName = "MVA18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.ResponseTransformation = "Loge";
            model.CategoricalPredictor = "Categorial factor";
            model.CaseID = "Case ID";

            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task MVA19()
        {
            string testName = "MVA19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.ResponseTransformation = "Log10";
            model.CategoricalPredictor = "Categorical continuous";

            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task MVA20()
        {
            string testName = "MVA20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.ResponseTransformation = "Square Root";
            model.CategoricalPredictor = "Categorial factor";
            model.CaseID = "Case ID";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task MVA21()
        {
            string testName = "MVA21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.CategoricalPredictor = "Categorical Binary 2";

            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task MVA22()
        {
            string testName = "MVA22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.ResponseTransformation = "Loge";
            model.CategoricalPredictor = "Categorical Binary 2";
            model.CaseID = "Case ID";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task MVA23()
        {
            string testName = "MVA23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.ResponseTransformation = "Log10";
            model.CategoricalPredictor = "Categorical Binary 1";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task MVA24()
        {
            string testName = "MVA24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            MultivariateAnalysisModel model = new MultivariateAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Multivariate").Key;
            model.Responses = new string[] { "Sepal length", "Sepal width", "Petal width", "Petal length" };
            model.ResponseTransformation = "Square Root";
            model.CategoricalPredictor = "Categorical Binary 2";
            model.CaseID = "Case ID";
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("MultivariateAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "MultivariateAnalysis", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }
    }
}