using SilveR.StatsModels;
using System.Collections.Generic;
using System.IO;
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
        public async Task DBTOST1()
        {
            string testName = "DBTOST1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Resp1";
            model.ControlGroup = "0.998758912";
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp1) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST2()
        {
            string testName = "DBTOST2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp2";
            model.Treatment = null;
            model.ControlGroup = null;
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The response selected (Resp2) contains only 1 value. Please select another factor.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }


        [Fact]
        public async Task DBTOST3()
        {
            string testName = "DBTOST3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat3";
            model.ControlGroup = "A";
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment (Treat3). Please select another factor.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST4()
        {
            string testName = "DBTOST4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat4";
            model.ControlGroup = "A";
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat4) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST5()
        {
            string testName = "DBTOST5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp3";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -10;
            model.UpperBoundAbsolute = 10;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains non-numerical data that cannot be processed. Please check input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }



        [Fact]
        public async Task DBTOST8()
        {
            string testName = "DBTOST8";

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
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = -50;
            model.PowerTo = 80;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Power from must be > 0.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST11()
        {
            string testName = "DBTOST11";

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
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = -10;
            model.SampleSizeTo = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Sample size from must be > 0.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST12()
        {
            string testName = "DBTOST12";

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
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 80;
            model.PowerTo = 50;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Power To value must be greater than the From value.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }


        [Fact]
        public async Task DBTOST13()
        {
            string testName = "DBTOST13";

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
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 80;
            model.SampleSizeTo = 50;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Sample Size To value must be greater than the From value.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST14()
        {
            string testName = "DBTOST14";

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
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("You have selected % change from control as the acceptance bound, but as you have not defined the control group it is not possible to calculate the % change.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST15()
        {
            string testName = "DBTOST15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = null;
            model.ControlGroup = null;
            model.Significance = "0.05";
            model.TrueDifference = "0 1 2 3";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("True difference contains non-numeric values detected or values are not comma separated.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST16()
        {
            string testName = "DBTOST16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = null;
            model.ControlGroup = null;
            model.Significance = "0.05";
            model.TrueDifference = "c";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 10;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("True difference contains non-numeric values detected or values are not comma separated.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST17()
        {
            string testName = "DBTOST17";

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
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The lower bound selected is higher than the upper bound, please check the bounds as the lower bound should be less than the upper bound.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST18()
        {
            string testName = "DBTOST18";

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
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 1;
            model.SampleSizeTo = 30;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The sample size selected must be greater than 1.", errors);
            Helpers.SaveOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task DBTOST19()
        {
            string testName = "DBTOST19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0, 0.02, 0.04, 0.06, 0.08";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.4m;
            model.UpperBoundAbsolute = 0.4m;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DBTOST20()
        {
            string testName = "DBTOST20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0, 0.02, 0.04, 0.06, 0.08";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.4m;
            model.UpperBoundAbsolute = 0.4m;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 2;
            model.SampleSizeTo = 30;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DBTOST21()
        {
            string testName = "DBTOST21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0, 0.02, 0.04, 0.06, 0.08";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 60;
            model.UpperBoundPercentageChange = 70;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DBTOST22()
        {
            string testName = "DBTOST22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0, 0.02, 0.04, 0.06, 0.08";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 60;
            model.UpperBoundPercentageChange = 70;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 2;
            model.SampleSizeTo = 30;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DBTOST23()
        {
            string testName = "DBTOST23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0, 0.02, 0.04, 0.06, 0.08";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 80;
            model.UpperBoundPercentageChange = 70;
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 2;
            model.SampleSizeTo = 30;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DBTOST24()
        {
            string testName = "DBTOST24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0, 0.02, 0.04, 0.06, 0.08";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute =-0.4m;
            model.UpperBoundAbsolute = 0.4m;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DBTOST25()
        {
            string testName = "DBTOST25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0, 0.02, 0.04, 0.06, 0.08";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.4m;
            model.UpperBoundAbsolute = 0.4m;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 99;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DBTOST26()
        {
            string testName = "DBTOST26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0, 0.02, 0.04, 0.06, 0.08";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 60;
            model.UpperBoundPercentageChange = 70;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DBTOST27()
        {
            string testName = "DBTOST27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0, 0.02, 0.04, 0.06, 0.08";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 60;
            model.UpperBoundPercentageChange = 70;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 99;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DBTOST28()
        {
            string testName = "DBTOST28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - Means Equivalence").Key;
            model.Response = "Resp1";
            model.Treatment = "Trea t1";
            model.ControlGroup = "x";
            model.Significance = "0.05";
            model.TrueDifference = "0, 0.02, 0.04, 0.06, 0.08";
            model.EquivalenceBoundsType = EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 80;
            model.UpperBoundPercentageChange = 70;
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 99;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.SanitizeHtml(expectedHtml), Helpers.SanitizeHtml(statsOutput.HtmlResults));
        }
    }
}