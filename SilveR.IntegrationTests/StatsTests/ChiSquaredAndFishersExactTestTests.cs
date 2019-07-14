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
    public class ChiSquaredAndFishersExactTestTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public ChiSquaredAndFishersExactTestTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CHI1()
        {
            string testName = "CHI1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "Count_1";
            model.GroupingFactor = "TreatmentG";
            model.ResponseCategories = "TreatmentH";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The response selected contains non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, errors);
        }

        [Fact]
        public async Task CHI2()
        {
            string testName = "CHI2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "TreatmentH";
            model.GroupingFactor = "Treatment x";
            model.ResponseCategories = "Treatmenty";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Grouping factor selected (Treatment x) contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, errors);
        }

        [Fact]
        public async Task CHI3()
        {
            string testName = "CHI3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "TreatmentH";
            model.GroupingFactor = "Treatmenty";
            model.ResponseCategories = "Treatment x";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response categories selected (Treatment x) contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, errors);
        }

        [Fact]
        public async Task CHI4()
        {
            string testName = "CHI4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "Count_2";
            model.GroupingFactor = "Treatmenty";
            model.ResponseCategories = "TreatmentG";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The response selected (Count_2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);
        }

        [Fact]
        public async Task CHI5()
        {
            string testName = "CHI5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountAB";
            model.GroupingFactor = "TreatmentA";
            model.ResponseCategories = "TreatmentB";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI6()
        {
            string testName = "CHI6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountAB";
            model.GroupingFactor = "TreatmentA";
            model.ResponseCategories = "TreatmentB";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Less-than";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI7()
        {
            string testName = "CHI7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountAB";
            model.GroupingFactor = "TreatmentA";
            model.ResponseCategories = "TreatmentB";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Greater-than";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI8()
        {
            string testName = "CHI8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountAB";
            model.GroupingFactor = "Treat A";
            model.ResponseCategories = "Treat B";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI9()
        {
            string testName = "CHI9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountAB";
            model.GroupingFactor = "Treat A";
            model.ResponseCategories = "Treat B";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Less-than";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI10()
        {
            string testName = "CHI10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountAB";
            model.GroupingFactor = "Treat A";
            model.ResponseCategories = "Treat B";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Greater-than";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI11()
        {
            string testName = "CHI11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountAB";
            model.GroupingFactor = "T_A";
            model.ResponseCategories = "T_B";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI12()
        {
            string testName = "CHI12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountAB";
            model.GroupingFactor = "T_A";
            model.ResponseCategories = "T_B";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Less-than";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI13()
        {
            string testName = "CHI13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountAB";
            model.GroupingFactor = "T_A";
            model.ResponseCategories = "T_B";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Greater-than";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI14()
        {
            string testName = "CHI14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountCD";
            model.GroupingFactor = "TreatmentC";
            model.ResponseCategories = "TreatmentD";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI15()
        {
            string testName = "CHI15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountCD";
            model.GroupingFactor = "TreatmentC";
            model.ResponseCategories = "TreatmentD";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Less-than";

            //Act1
            HttpResponseMessage response = await client.PostAsync("Analyses/ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Grouping factor or the Response categories have more than two levels. Barnard&#x27;s test can only be performed when there are two levels of the Grouping factor and two Response categories.", warnings);
            Helpers.SaveOutput("ChiSquaredAndFishersExactTest", testName, warnings);

            //Act2 - ignore warnings
            model.BarnardsTest = false; //Deselecting Barnards test!
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI16()
        {
            string testName = "CHI16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountCD";
            model.GroupingFactor = "BinC";
            model.ResponseCategories = "BinD";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI17()
        {
            string testName = "CHI17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountCD";
            model.GroupingFactor = "BinC";
            model.ResponseCategories = "BinD";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Less-than";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI18()
        {
            string testName = "CHI18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountCD";
            model.GroupingFactor = "BinC";
            model.ResponseCategories = "BinD";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Greater-than";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI19()
        {
            string testName = "CHI19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountEF";
            model.GroupingFactor = "BinE";
            model.ResponseCategories = "BinF";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI20()
        {
            string testName = "CHI20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountEF";
            model.GroupingFactor = "BinE";
            model.ResponseCategories = "BinF";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Less-than";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI21()
        {
            string testName = "CHI21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountEF";
            model.GroupingFactor = "BinE";
            model.ResponseCategories = "BinF";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Greater-than";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task CHI22()
        {
            string testName = "CHI22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountEF";
            model.GroupingFactor = "Bin(E";
            model.ResponseCategories = "Bin(F";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI23()
        {
            string testName = "CHI23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountEF";
            model.GroupingFactor = "Bin(E";
            model.ResponseCategories = "Bin(F";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Less-than";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI24()
        {
            string testName = "CHI24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountEF";
            model.GroupingFactor = "Bin(E";
            model.ResponseCategories = "Bin(F";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Greater-than";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI25()
        {
            string testName = "CHI25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountGH";
            model.GroupingFactor = "TreatmentG";
            model.ResponseCategories = "TreatmentH";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = false;
            model.Hypothesis = "Two-sided";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI26()
        {
            string testName = "CHI26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountGH";
            model.GroupingFactor = "TreatmentG";
            model.ResponseCategories = "TreatmentH";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = false;
            model.Hypothesis = "Less-than";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI27()
        {
            string testName = "CHI27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountGH";
            model.GroupingFactor = "TreatmentG";
            model.ResponseCategories = "TreatmentH";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = false;
            model.Hypothesis = "Greater-than";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task CHI28()
        {
            string testName = "CHI28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            ChiSquaredAndFishersExactTestModel model = new ChiSquaredAndFishersExactTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Chi-sq and Fishers Exact").Key;
            model.Response = "CountCD";
            model.GroupingFactor = "BinC";
            model.ResponseCategories = "BinD2";
            model.ChiSquaredTest = true;
            model.FishersExactTest = true;
            model.BarnardsTest = true;
            model.Hypothesis = "Two-sided";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "ChiSquaredAndFishersExactTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("ChiSquaredAndFishersExactTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "ChiSquaredAndFishersExactTest", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }
    }
}