using SilveR.StatsModels;
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
        public async Task PVA1()
        {
            string testName = "PVA1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentDatasetBasedInputsModel model = new PValueAdjustmentDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "P-value Adjustment Dataset").Key;
            model.PValues = "p-value - PVA1";
            model.DatasetLabels = "Comparison - PVA1";
            model.SelectedTest = "Hochberg";
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PVA2()
        {
            string testName = "PVA2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentDatasetBasedInputsModel model = new PValueAdjustmentDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "P-value Adjustment Dataset").Key;
            model.PValues = "p-value - PVA2";
            model.DatasetLabels = "Comparison - PVA2";
            model.SelectedTest = "Hochberg";
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PVA3()
        {
            string testName = "PVA3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PValueAdjustmentDatasetBasedInputsModel model = new PValueAdjustmentDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "P-value Adjustment Dataset").Key;
            model.PValues = "p-value - PVA3";
            model.DatasetLabels = "Comparison - PVA2";
            model.SelectedTest = "Hochberg";
            model.Significance = "0.05";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PValueAdjustmentDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PValueAdjustmentDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PValueAdjustmentDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }
    }
}