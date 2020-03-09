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
    public class ComparisonOfMeansDatasetBasedInputsTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public ComparisonOfMeansDatasetBasedInputsTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PSS1()
        {
            string testName = "PSS1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp 1";
            model.Treatment = "Resp 1";
            model.ControlGroup = "0.998758912";
            model.PercentChange = "10";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp 1) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
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
            model.AbsoluteChange = "0.25,0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains only 1 value. Please select another factor.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS3()
        {
            string testName = "PSS3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat3";
            model.ControlGroup = "A";
            model.AbsoluteChange = "0.25,0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment (Treat3). Please select another factor.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS4()
        {
            string testName = "PSS4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat4";
            model.ControlGroup = "A";
            model.AbsoluteChange = "0.25,0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat4) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS5()
        {
            string testName = "PSS5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp3";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.AbsoluteChange = "0.25,0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains non-numerical data that cannot be processed. Please check input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS8()
        {
            string testName = "PSS8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.AbsoluteChange = "0.25,0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = -50;
            model.PowerTo = 80;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Power from must be > 0.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS11()
        {
            string testName = "PSS11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.AbsoluteChange = "0.25,0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = -10;
            model.SampleSizeTo = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Sample size from must be > 0.", errors);
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
            model.AbsoluteChange = "0.25,0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 80;
            model.PowerTo = 50;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Power To value must be greater than the From value.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS13()
        {
            string testName = "PSS13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.AbsoluteChange = "0.25,0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 80;
            model.SampleSizeTo = 50;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Sample Size To value must be greater than the From value.", errors);
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


        [Fact]
        public async Task PSS15()
        {
            string testName = "PSS15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.PercentChange = "40, 60, 80,100";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS16()
        {
            string testName = "PSS16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.1";
            model.PercentChange = "40, 60, 80, 100";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 4;
            model.SampleSizeTo = 8;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS17()
        {
            string testName = "PSS17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.01";
            model.PercentChange = "40, 60, 80, 100";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS18()
        {
            string testName = "PSS18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.PercentChange = "40, 60 ,80, 100";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }


        [Fact]
        public async Task PSS19()
        {
            string testName = "PSS19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS20()
        {
            string testName = "PSS20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.10";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 4;
            model.SampleSizeTo = 8;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS21()
        {
            string testName = "PSS21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.10";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS22()
        {
            string testName = "PSS22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS23()
        {
            string testName = "PSS23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = null;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS24()
        {
            string testName = "PSS24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = null;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 4;
            model.SampleSizeTo = 8;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS25()
        {
            string testName = "PSS25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = null;
            model.Significance = "0.01";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS26()
        {
            string testName = "PSS26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = null;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS27()
        {
            string testName = "PSS27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = null;
            model.ControlGroup = null;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS28()
        {
            string testName = "PSS28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = null;
            model.ControlGroup = null;
            model.Significance = "0.05";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 4;
            model.SampleSizeTo = 8;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS29()
        {
            string testName = "PSS29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = null;
            model.ControlGroup = null;
            model.Significance = "0.01";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS30()
        {
            string testName = "PSS30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = null;
            model.ControlGroup = null;
            model.Significance = "0.01";
            model.AbsoluteChange = "0.25, 0.5, 0.75, 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(expectedHtml, statsOutput.HtmlResults);
        }

        [Fact]
        public async Task PSS31()
        {
            string testName = "PSS31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = null;
            model.ControlGroup = null;
            model.Significance = "0.01";
            model.AbsoluteChange = "0.25 0.5 0.75 1";
            model.ChangeType = ChangeTypeOption.Absolute;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Absolute changes has non-numeric values or the values are not comma separated.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task PSS32()
        {
            string testName = "PSS32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Comp").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.01";
            model.PercentChange = "0.25 0.5 0.75 1";
            model.ChangeType = ChangeTypeOption.Percent;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 80;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ComparisonOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Percent changes has non-numeric values or the values are not comma separated.", errors);
            Helpers.SaveOutput("ComparisonOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }
    }
}