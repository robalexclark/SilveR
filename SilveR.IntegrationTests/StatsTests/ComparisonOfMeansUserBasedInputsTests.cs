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
    public class ComparisonOfMeansUserBasedInputsTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public ComparisonOfMeansUserBasedInputsTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PSS1()
        {
            string testName = "PSS1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            //model.GroupMean = "x";
            //model.StandardDeviation = "2";
            //model.ControlGroup = "0.98";
            //model.PercentChange = "10";
            //model.ChangeType = ChangeTypeOption.Percent;
            //model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            ////Act
            //HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            //IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            ////Assert
            //Assert.Contains("Response (Resp 1) has been selected in more than one input category, please change your input options.", errors);
            //Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }



    }
}