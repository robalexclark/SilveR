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

        //[Fact]
        //public async Task LORA1()
        //{
        //    string testName = "LRA1";

        //    //Arrange
        //    HttpClient client = _factory.CreateClient();

        //    LogisticRegressionAnalysisModel model = new LogisticRegressionAnalysisModel();
        //    model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Logistic Regression").Key;
        //    model.Response = "Response1";
        //    model.ContinuousFactors = new string[] { "Cont1" };
        //    //model.CategoricalFactors = new string[] { "Response1" };

        //    //Act
        //    HttpResponseMessage response = await client.PostAsync("Analyses/LogisticRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
        //    IEnumerable<string> errors = await Helpers.ExtractErrors(response);

        //    //Assert
        //    Assert.Contains("Response (Response1) has been selected in more than one input category, please change your input options.", errors);
        //    Helpers.SaveOutput("LinearRegressionAnalysis", testName, errors);
        //}

    }
}