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
            Assert.Contains("Response has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        }


    }
}