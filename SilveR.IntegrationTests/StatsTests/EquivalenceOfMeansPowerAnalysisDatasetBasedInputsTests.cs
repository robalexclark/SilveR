using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class EquivalenceOfMeansPowerAnalysisDatasetBasedInputsTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public EquivalenceOfMeansPowerAnalysisDatasetBasedInputsTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task PSS1()
        {
            string testName = "PSS1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = null;
            model.ControlGroup = null;
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("You have selected % change from control as the acceptance bound, but as you have not defined the control group it is not possible to calculate the % change.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS2()
        {
            string testName = "PSS2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = null;
            model.ControlGroup = null;
            model.Significance = "0.05";
            model.TrueDifference = "0 1 2 3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("True difference contains non-numeric values detected or values are not comma separated.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }


        [Fact]
        public async Task PSS3()
        {
            string testName = "PSS3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = -10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The lower bound selected is higher than the upper bound, please check the bounds as the lower bound should be less than the upper bound.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS4()
        {
            string testName = "PSS4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;
            model.SampleSizeFrom = 1;
            model.SampleSizeTo = 30;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The sample size selected must be greater than 1.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }
    }
}