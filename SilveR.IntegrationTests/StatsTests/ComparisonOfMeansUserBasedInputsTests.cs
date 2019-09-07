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
        public async Task PSS4()
        {
            string testName = "PSS4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.5,1,2,,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The list of absolute changes contains missing values, please remove any blank entries between the comma separated values.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS5()
        {
            string testName = "PSS5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.PercentChange = "0.5,1,2,,4";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The list of percent changes contains missing values, please remove any blank entries between the comma separated values.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS6()
        {
            string testName = "PSS6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.5,1,2,,4,x";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The list of absolute changes contains missing values, please remove any blank entries between the comma separated values.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS7()
        {
            string testName = "PSS7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.PercentChange = "0.5,1,2,4,x (%)";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Percent changes has non-numeric values or the values are not comma separated.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS8()
        {
            string testName = "PSS8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.AbsoluteChange = "-0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Absolute changes has values less than zero.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS9()
        {
            string testName = "PSS9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.PercentChange = "-0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Percent changes has values less than zero.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS10()
        {
            string testName = "PSS10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = -10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 2;
            model.Significance = "0.05";
            model.AbsoluteChange = "-0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Group mean must be >= 0.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS11()
        {
            string testName = "PSS11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = -2;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Standard deviation must be > 0.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS12()
        {
            string testName = "PSS12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = -2;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Variance must be > 0.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS13()
        {
            string testName = "PSS13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.1";
            model.PercentChange = "10, 20, 30, 40 ";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS14()
        {
            string testName = "PSS14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.PercentChange = "10, 20, 30, 40 ";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 4;
            model.SampleSizeTo = 8;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS15()
        {
            string testName = "PSS15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.01";
            model.PercentChange = "10, 20, 30, 40 ";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task PSS16()
        {
            string testName = "PSS16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.01";
            model.PercentChange = "10, 20, 30, 40 ";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS17()
        {
            string testName = "PSS17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS18()
        {
            string testName = "PSS18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.01";
            model.AbsoluteChange = "0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 4;
            model.SampleSizeTo = 8;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS19()
        {
            string testName = "PSS19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.1";
            model.AbsoluteChange = "0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS20()
        {
            string testName = "PSS20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS21()
        {
            string testName = "PSS21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.1";
            model.PercentChange = "10,20,30,40";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS22()
        {
            string testName = "PSS22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.PercentChange = "10,20,30,40";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 4;
            model.SampleSizeTo = 8;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS23()
        {
            string testName = "PSS23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.01";
            model.PercentChange = "10,20,30,40";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS24()
        {
            string testName = "PSS24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.01";
            model.PercentChange = "10,20,30,40";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS25()
        {
            string testName = "PSS25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS26()
        {
            string testName = "PSS26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.01";
            model.AbsoluteChange = "0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 4;
            model.SampleSizeTo = 8;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS27()
        {
            string testName = "PSS27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.1";
            model.AbsoluteChange = "0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS28()
        {
            string testName = "PSS28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.5,1,2,4";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PSS29()
        {
            string testName = "PSS29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.PercentChange = "10 20 30 40";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Percent changes has non-numeric values or the values are not comma separated.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS30()
        {
            string testName = "PSS30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisUserBasedInputsModel model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
            model.GroupMean = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.AbsoluteChange = "10 20 30 40";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Absolute changes has non-numeric values or the values are not comma separated.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }
    }
}