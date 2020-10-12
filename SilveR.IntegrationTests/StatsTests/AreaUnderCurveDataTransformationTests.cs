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
            model.Response = "Resp3";
            model.SubjectFactor = "Subject1";
            model.TimeFactor = "Time3";
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.", errors);
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
            model.Responses = new string[] { "Resp1", "Resp2", "Resp3", "Resp6"};
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
            model.AUCOutput = AreaUnderCurveDataTransformationModel.AUCOutputType.AUCFromTime0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "AreaUnderCurveDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("AreaUnderCurveDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "AreaUnderCurveDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }
    }
}