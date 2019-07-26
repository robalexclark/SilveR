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
    public class PValueAdjustmentDatasetBasedInputsTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public PValueAdjustmentDatasetBasedInputsTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task XXX1()
        {
            string testName = "PSS1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentDatasetBasedInputsModel model = new PValueAdjustmentDatasetBasedInputsModel();
            //model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            //model.Response = "Resp 1";
            //model.Treatment = "Resp 1";
            //model.ControlGroup = "0.98";
            //model.PercentChange = "10";
            //model.ChangeType = ChangeTypeOption.Percent;
            //model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            ////Act
            //HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            //IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            ////Assert
            //Assert.Contains("Response (Resp 1) has been selected in more than one input category, please change your input options.", errors);
            //Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }


    }
}