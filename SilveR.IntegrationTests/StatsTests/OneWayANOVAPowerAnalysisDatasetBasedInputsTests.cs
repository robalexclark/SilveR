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
    public class OneWayANOVAPowerAnalysisDatasetBasedInputsTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public OneWayANOVAPowerAnalysisDatasetBasedInputsTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SMA1()
        {
            string testName = "SMA1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp1";
            model.Treatment = "Resp1";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp1) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task SMA2()
        {
            string testName = "SMA2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp2";
            model.Treatment = "Treat1";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment (Treat1). Please select another factor.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task SMA3()
        {
            string testName = "SMA3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp3";
            model.Treatment = "Treat2";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the Treatment (Treat2). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task SMA4()
        {
            string testName = "SMA4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat3";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat3) contains missing data where there are observations present in the Response. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task SMA5()
        {
            string testName = "SMA5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp4";
            model.Treatment = "Treat1";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains non-numerical data that cannot be processed. Please check raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, errors);
        }
        
        [Fact]
        public async Task SMA6()
        {
            string testName = "SMA6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp5";
            model.Treatment = "Treat1";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp5) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, warnings);
        }

        [Fact]
        public async Task SMA7()
        {
            string testName = "SMA7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat4";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Treat4) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task SMA8()
        {
            string testName = "SMA8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp5";
            model.Treatment = "Treat5";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment (Treat5). Please select another factor.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task SMA9()
        {
            string testName = "SMA9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "Loge";
            model.Treatment = "Treat1";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp6 variable. Unfortunately some of the Resp6 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, warnings);
        }

        [Fact]
        public async Task SMA10()
        {
            string testName = "SMA10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "Log10";
            model.Treatment = "Treat1";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp7 variable. Unfortunately some of the Resp7 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, warnings);
        }

        [Fact]
        public async Task SMA11()
        {
            string testName = "SMA11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "Square Root";
            model.Treatment = "Treat1";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp6 variable. Unfortunately some of the Resp6 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, warnings);
        }
        
        [Fact]
        public async Task SMA12()
        {
            string testName = "SMA12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "ArcSine";
            model.Treatment = "Treat1";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp6 variable. Unfortunately some of the Resp6 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, warnings);
        }

        [Fact]
        public async Task SMA13()
        {
            string testName = "SMA13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp1";
            model.ResponseTransformation = "ArcSine";
            model.Treatment = "Treat1";
            model.Significance = "0.05";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp1 variable. Unfortunately some of the Resp1 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, warnings);
        }



        [Fact]
        public async Task SMA20()
        {
            string testName = "SMA20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat1";
            model.Significance = "0.05";
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = -10;
            model.SampleSizeTo = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Sample size from must be > 0", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task SMA21()
        {
            string testName = "SMA21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat1";
            model.Significance = "0.05";
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 80;
            model.PowerTo = 50;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Power To value must be greater than the From value.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task SMA22()
        {
            string testName = "SMA22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat1";
            model.Significance = "0.05";
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 80;
            model.SampleSizeTo = 50;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Sample Size To value must be greater than the From value.", errors);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, errors);
        }

        [Fact]
        public async Task SMA23()
        {
            string testName = "SMA23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp1";
            model.Treatment = "Treat1";
            model.Significance = "0.05";
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA24()
        {
            string testName = "SMA24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp1";
            model.ResponseTransformation = "Log10";
            model.Treatment = "Treat2";
            model.Significance = "0.01";
            model.PlottingRangeType = PlottingRangeTypeOption.SampleSize;
            model.SampleSizeFrom = 2;
            model.SampleSizeTo = 20;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA25()
        {
            string testName = "SMA25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp1";
            model.ResponseTransformation = "Square Root";
            model.Treatment = "Treat1";
            model.Significance = "0.1";
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 70;
            model.PowerTo = 90;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMA26()
        {
            string testName = "SMA26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            OneWayANOVAPowerAnalysisDatasetBasedInputsModel model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Power - One-way ANOVA").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "ArcSine";
            model.Treatment = "Treat2";
            model.Significance = "0.05";
            model.PlottingRangeType = PlottingRangeTypeOption.Power;
            model.PowerFrom = 50;
            model.PowerTo = 70;

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp5) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", testName, warnings);

            //Act2 - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "OneWayANOVAPowerAnalysisDatasetBasedInputs", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("OneWayANOVAPowerAnalysisDatasetBasedInputs", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "OneWayANOVAPowerAnalysisDatasetBasedInputs", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }
    }
}