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
    public class OneWayANOVAPowerAnalysisUserBasedInputsTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public OneWayANOVAPowerAnalysisUserBasedInputsTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task XXX1()
        {
            string testName = "PSS1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
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

        [Fact]
        public async Task PSS2()
        {
            string testName = "PSS2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp2";
            model.Treatment = null;
            model.ControlGroup = null;
            model.AbsoluteChange = "0.25 0.5 0.75 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains only 1 value. Please select another factor.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }






        [Fact]
        public async Task PSS12()
        {
            string testName = "PSS12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.AbsoluteChange = "0.25 0.5 0.75 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Absolute changes has non-numeric values or the values are not comma separated.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }




        [Fact]
        public async Task PSS14()
        {
            string testName = "PSS14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = null;
            model.ControlGroup = null;
            model.PercentChange = "40 60 80 100";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("You have selected % change as expected changes from control, but as you have not defined the control group it is not possible to calculate the % change.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }


    }
}