using SilveR.StatsModels;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class EquivalenceOfMeansPowerAnalysisUserBasedInputsTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public EquivalenceOfMeansPowerAnalysisUserBasedInputsTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }



        [Fact]
        public async Task UBTOST4()
        {
            string testName = "UBTOST4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.TrueDifference = "0, 1, , 2";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("True difference contains non-numeric values detected or values are not comma separated.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST5()
        {
            string testName = "UBTOST5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.TrueDifference = "0, 1, x, 2";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("True difference contains non-numeric values detected or values are not comma separated.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }


        [Fact]
        public async Task UBTOST6()
        {
            string testName = "UBTOST6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = -10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.TrueDifference = "0, 1, 2";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UBTOST7()
        {
            string testName = "UBTOST7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = -2;
            model.Significance = "0.05";
            model.TrueDifference = "0, 1, 2";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Standard deviation must be > 0.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST8()
        {
            string testName = "UBTOST8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = -2;
            model.Significance = "0.05";
            model.TrueDifference = "0, 1, 2";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Variance must be > 0.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST11()
        {
            string testName = "UBTOST11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0, 1, 2";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = -50;
            model.PowerTo = 80;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Power from must be > 0.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST14()
        {
            string testName = "UBTOST14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0, 1, 2";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = -10;
            model.SampleSizeTo = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Sample size from must be > 0.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST15()
        {
            string testName = "UBTOST15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0, 1, 2";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 80;
            model.PowerTo = 50;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Power To value must be greater than the From value.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST16()
        {
            string testName = "UBTOST16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0, 1, 2";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 80;
            model.SampleSizeTo = 50;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Sample Size To value must be greater than the From value.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST17()
        {
            string testName = "UBTOST17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = null;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("You have selected percentage changes from observed difference, but as you have not defined the observed difference it is not possible to calculate percentage changes.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST18()
        {
            string testName = "UBTOST18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0 1 2 3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("True difference contains non-numeric values detected or values are not comma separated.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST19()
        {
            string testName = "UBTOST19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 2;
            model.Significance = "0.05";
            model.TrueDifference = "c";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("True difference contains non-numeric values detected or values are not comma separated.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST20()
        {
            string testName = "UBTOST20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = -10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The lower bound selected is higher than the upper bound, please check the bounds as the lower bound should be less than the upper bound.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST21()
        {
            string testName = "UBTOST21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 1;
            model.SampleSizeTo = 30;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The sample size selected must be greater than 1.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName, errors);
        }

        [Fact]
        public async Task UBTOST22()
        {
            string testName = "UBTOST22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0,1,2,3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -5;
            model.UpperBoundAbsolute = 5;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UBTOST23()
        {
            string testName = "UBTOST23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0,1,2,3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -5;
            model.UpperBoundAbsolute = 5;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 2;
            model.SampleSizeTo = 30;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UBTOST24()
        {
            string testName = "UBTOST24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0,1,2,3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 40;
            model.UpperBoundPercentageChange = 40;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UBTOST25()
        {
            string testName = "UBTOST25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0,1,2,3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 40;
            model.UpperBoundPercentageChange = 40;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 2;
            model.SampleSizeTo = 30;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UBTOST26()
        {
            string testName = "UBTOST26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0,1,2,3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -5;
            model.UpperBoundAbsolute = 5;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UBTOST27()
        {
            string testName = "UBTOST27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0,1,2,3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -5;
            model.UpperBoundAbsolute = 5;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 30;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UBTOST28()
        {
            string testName = "UBTOST28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0,1,2,3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 40;
            model.UpperBoundPercentageChange = 40;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UBTOST29()
        {
            string testName = "UBTOST29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.StandardDeviation;
            model.StandardDeviation = 2;
            model.Significance = "0.05";
            model.TrueDifference = "0,1,2,3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 40;
            model.UpperBoundPercentageChange = 40;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 30;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UBTOST30()
        {
            string testName = "UBTOST30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.TrueDifference = "0,1,2,3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -5;
            model.UpperBoundAbsolute = 5;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task UBTOST31()
        {
            string testName = "UBTOST31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisUserBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisUserBasedInputsModel();
            model.ObservedDifference = 10;
            model.DeviationType = DeviationType.Variance;
            model.Variance = 4;
            model.Significance = "0.05";
            model.TrueDifference = "0,1,2,3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 40;
            model.UpperBoundPercentageChange = 40;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 30;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisUserBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisUserBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }
    }
}