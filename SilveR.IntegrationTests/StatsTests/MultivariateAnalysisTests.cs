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
            model.Responses = new string[] { "Sepal Length" };
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
            model.Responses = new string[] { "Sepal Length" };
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
            model.Responses = new string[] { "Sepal Length" };
            model.AnalysisType = MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/MultivariateAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Responses requires at least two entries.", errors);
            Helpers.SaveOutput("MultivariateAnalysis", testName, errors);
        }



    }
}