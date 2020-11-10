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
    public class AreaUnderCurveDataTransformationTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public AreaUnderCurveDataTransformationTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task AUC1()
        {
            string testName = "AUC1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp3";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC2()
        {
            string testName = "AUC2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time3";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The TimeFactor (Time3) contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC3()
        {
            string testName = "AUC3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = null;
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response is required.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC4()
        {
            string testName = "AUC4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = null;
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Subject Factor is required.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC5()
        {
            string testName = "AUC5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = null;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Time Factor is required.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC6()
        {
            string testName = "AUC6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp4";
            model.SubjectFactor = "Subject3";
            model.TimeFactor = "Time4";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC7()
        {
            string testName = "AUC7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp4";
            model.SubjectFactor = "Subject3";
            model.TimeFactor = "Time4";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC8()
        {
            string testName = "AUC8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp5";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp5) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC9()
        {
            string testName = "AUC9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject2";
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Subject factor (Subject2) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC10()
        {
            string testName = "AUC10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time2";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Time factor (Time2) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC11()
        {
            string testName = "AUC11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp6" };
            model.NumericalTimePoints = "3,4,8,12";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp6) contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC12()
        {
            string testName = "AUC12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8,12a";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the numerical timepoints is not numeric.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC13()
        {
            string testName = "AUC13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The number of numerical timepoints has to equal the number of response variables selected.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC14()
        {
            string testName = "AUC14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1" };
            model.NumericalTimePoints = "3";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Only one response found.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC15()
        {
            string testName = "AUC15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1" };
            model.NumericalTimePoints = "3";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Only one response found.", errors);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, errors);
        }

        [Fact]
        public async Task AUC16()
        {
            string testName = "AUC16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp5" };
            model.NumericalTimePoints = "1,5";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC17()
        {
            string testName = "AUC17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp5" };
            model.NumericalTimePoints = "3,5";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC18()
        {
            string testName = "AUC18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC19()
        {
            string testName = "AUC19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = true;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC20()
        {
            string testName = "AUC20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new List<string> { "Var1", "Var2", "Var3", "Var4" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC21()
        {
            string testName = "AUC21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC22()
        {
            string testName = "AUC22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = true;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC23()
        {
            string testName = "AUC23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new List<string> { "Var1", "Var2", "Var3", "Var4" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC24()
        {
            string testName = "AUC24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC25()
        {
            string testName = "AUC25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = true;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC26()
        {
            string testName = "AUC26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp1";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new List<string> { "Var1", "Var2", "Var3", "Var4" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC27()
        {
            string testName = "AUC27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp2";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC28()
        {
            string testName = "AUC28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp2";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = true;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC29()
        {
            string testName = "AUC29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp2";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new List<string> { "Var1", "Var2", "Var3", "Var4" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC30()
        {
            string testName = "AUC30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp2";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC31()
        {
            string testName = "AUC31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp2";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = true;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC32()
        {
            string testName = "AUC32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp2";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new List<string> { "Var1", "Var2", "Var3", "Var4" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC33()
        {
            string testName = "AUC33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp2";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC34()
        {
            string testName = "AUC34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp2";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = true;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC35()
        {
            string testName = "AUC35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - RM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.RepeatedMeasuresFormat;
            model.Response = "Resp2";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time1";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new List<string> { "Var1", "Var2", "Var3", "Var4" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("AreaUnderCurveDataTransformation", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task AUC36()
        {
            string testName = "AUC36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8,12";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC37()
        {
            string testName = "AUC37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8,12";
            model.IncludeAllVariables = true;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC38()
        {
            string testName = "AUC38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8,12";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new List<string> { "Treat1", "Treat2", "Treat3" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC39()
        {
            string testName = "AUC39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8,12";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC40()
        {
            string testName = "AUC40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8,12";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC41()
        {
            string testName = "AUC41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8,12";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new List<string> { "Treat1", "Treat2", "Treat3" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC42()
        {
            string testName = "AUC42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8,12";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC43()
        {
            string testName = "AUC43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8,12";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC44()
        {
            string testName = "AUC44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp4" };
            model.NumericalTimePoints = "3,4,8,12";
            model.SelectedVariables = new List<string> { "Treat1", "Treat2", "Treat3" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC45()
        {
            string testName = "AUC45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp5" };
            model.NumericalTimePoints = "3,4,8,12";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC46()
        {
            string testName = "AUC46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp5" };
            model.NumericalTimePoints = "3,4,8,12";
            model.IncludeAllVariables = true;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC47()
        {
            string testName = "AUC47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp5" };
            model.NumericalTimePoints = "3,4,8,12";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new List<string> { "Treat1", "Treat2", "Treat4" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC48()
        {
            string testName = "AUC48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp5" };
            model.NumericalTimePoints = "3,4,8,12";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC49()
        {
            string testName = "AUC49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp5" };
            model.NumericalTimePoints = "3,4,8,12";
            model.IncludeAllVariables = true;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC50()
        {
            string testName = "AUC50";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp5" };
            model.NumericalTimePoints = "3,4,8,12";
            model.SelectedVariables = new List<string> { "Treat1", "Treat2", "Treat4" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromInitialTimepoint;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC51()
        {
            string testName = "AUC51";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp5" };
            model.NumericalTimePoints = "3,4,8,12";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC52()
        {
            string testName = "AUC52";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp5" };
            model.NumericalTimePoints = "3,4,8,12";
            model.IncludeAllVariables = true;
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task AUC53()
        {
            string testName = "AUC53";

            //Arrange
            HttpClient client = _factory.CreateClient();

            AreaUnderCurveDataTransformationModel model = new AreaUnderCurveDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "AUC - SM data").Key;
            model.SelectedInputFormat = AreaUnderCurveDataTransformationModel.InputFormatType.SingleMeasuresFormat;
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp5" };
            model.NumericalTimePoints = "3,4,8,12";
            model.SelectedVariables = new List<string> { "Treat1", "Treat2", "Treat4" };
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCForChangeFromBaseline;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }
    }
}