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
    public class UnpairedTTestAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public UnpairedTTestAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task UPTT1()
        {
            string testName = "UPTT1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            UnpairedTTestAnalysisModel model = new UnpairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Unpaired t-test").Key;
            //model.Responses= new string[] { "Resp 5" };
            //model.TargetValue = 0.1m;

            ////Act
            //HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            //IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            ////Assert
            //Assert.Contains("There is no replication in the response variable (Resp 5). Please select another factor.", errors);
            //Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }


    }
}