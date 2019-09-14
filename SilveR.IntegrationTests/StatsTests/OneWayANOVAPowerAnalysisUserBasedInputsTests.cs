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
        public async Task PSS2()
        {
            string testName = "PSS2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "2,2,x";
            model.VariabilityEstimate = VariabilityEstimate.Variance;
            model.Variance = 2;

            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Means has non-numeric values or the values are not comma separated.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS3()
        {
            string testName = "PSS3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "2,2,,-3";
            model.VariabilityEstimate = VariabilityEstimate.Variance;
            model.Variance = -2;

            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Variance must be > 0.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS4()
        {
            string testName = "PSS4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "2,2,3";
            model.VariabilityEstimate = VariabilityEstimate.Variance;
            model.Variance = 0;

            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Variance must be > 0.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS6()
        {
            string testName = "PSS6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "2,2,3";
            model.VariabilityEstimate = VariabilityEstimate.StandardDeviation;
            model.StandardDeviation = -2;

            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Standard deviation must be > 0.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS7()
        {
            string testName = "PSS7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "2,2,3";
            model.VariabilityEstimate = VariabilityEstimate.StandardDeviation;
            model.StandardDeviation = 0;

            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Standard deviation must be > 0.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS10()
        {
            string testName = "PSS10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "2,2,3";
            model.VariabilityEstimate = VariabilityEstimate.StandardDeviation;
            model.StandardDeviation = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = -50;
            model.PowerTo = 80;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Power from must be > 0.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS13()
        {
            string testName = "PSS13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "2,2,3";
            model.VariabilityEstimate = VariabilityEstimate.StandardDeviation;
            model.StandardDeviation = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = -10;
            model.SampleSizeTo = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Sample size from must be > 0.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS14()
        {
            string testName = "PSS14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "2,2,3";
            model.VariabilityEstimate = VariabilityEstimate.StandardDeviation;
            model.StandardDeviation = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 80;
            model.PowerTo = 50;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Power To value must be greater than the From value.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS15()
        {
            string testName = "PSS15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "2,2,3";
            model.VariabilityEstimate = VariabilityEstimate.StandardDeviation;
            model.StandardDeviation = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 80;
            model.SampleSizeTo = 50;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Sample Size To value must be greater than the From value.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS16()
        {
            string testName = "PSS16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "3.5,3.75,5.5,8";
            model.VariabilityEstimate = VariabilityEstimate.Variance;
            model.Variance = 3;
            model.Significance = "0.05";
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 6;
            model.SampleSizeTo = 15;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS17()
        {
            string testName = "PSS17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "3.5,3.75,5.5,8";
            model.VariabilityEstimate = VariabilityEstimate.StandardDeviation;
            model.StandardDeviation = 3;
            model.Significance = "0.01";
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 6;
            model.SampleSizeTo = 15;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS18()
        {
            string testName = "PSS18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "3.5,3.75,5.5,8";
            model.VariabilityEstimate = VariabilityEstimate.Variance;
            model.Variance = 8;
            model.Significance = "0.001";
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 2;
            model.SampleSizeTo = 20;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS19()
        {
            string testName = "PSS19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "3.5,3.75,5.5,8";
            model.VariabilityEstimate = VariabilityEstimate.StandardDeviation;
            model.StandardDeviation = 3;
            model.Significance = "0.01";
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 2;
            model.SampleSizeTo = 20;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS20()
        {
            string testName = "PSS20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "3.5,3.75,5.5,8";
            model.VariabilityEstimate = VariabilityEstimate.Variance;
            model.Variance = 8;
            model.Significance = "0.05";
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS21()
        {
            string testName = "PSS21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "3.5,3.75,5.5,8";
            model.VariabilityEstimate = VariabilityEstimate.StandardDeviation;
            model.StandardDeviation = 3;
            model.Significance = "0.05";
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 70;
            model.PowerTo = 90;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS22()
        {
            string testName = "PSS22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "3.5,3.75,5.5,8";
            model.VariabilityEstimate = VariabilityEstimate.Variance;
            model.Variance = 8;
            model.Significance = "0.05";
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 70;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS23()
        {
            string testName = "PSS23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisUserBasedInputsModel model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
            model.Means = "3.5,3.75,5.5,8";
            model.VariabilityEstimate = VariabilityEstimate.StandardDeviation;
            model.StandardDeviation = 3;
            model.Significance = "0.05";
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 70;
            model.PowerTo = 90;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }
    }
}