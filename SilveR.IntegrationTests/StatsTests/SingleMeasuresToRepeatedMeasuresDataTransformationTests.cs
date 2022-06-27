using SilveR.StatsModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class SingleMeasuresToRepeatedMeasuresDataTransformationTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public SingleMeasuresToRepeatedMeasuresDataTransformationTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task SMtoRM1()
        {
            string testName = "SMtoRM1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1" };
            model.SubjectFactor = null;
            model.IncludeAllVariables =false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Responses requires at least two entries.", errors);
            Helpers.SaveOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", testName, errors);
        }

        [Fact]
        public async Task SMtoRM2()
        {
            string testName = "SMtoRM2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2" };
            model.SubjectFactor = null;
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM3()
        {
            string testName = "SMtoRM3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!" };
            model.SubjectFactor = null;
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM4()
        {
            string testName = "SMtoRM4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM5()
        {
            string testName = "SMtoRM5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM6()
        {
            string testName = "SMtoRM6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2" };
            model.SubjectFactor = null;
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM7()
        {
            string testName = "SMtoRM7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!" };
            model.SubjectFactor = null;
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM8()
        {
            string testName = "SMtoRM8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM9()
        {
            string testName = "SMtoRM9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!" };
            model.SubjectFactor = null;
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM10()
        {
            string testName = "SMtoRM10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2", "Time miss" };
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM11()
        {
            string testName = "SMtoRM11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2", "Time miss" };
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));

            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM12()
        {
            string testName = "SMtoRM12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2", "Time miss" };
            model.ResponseName = "Resper";
            model.RepeatedFactorName = "Repeater";
            model.SubjectFactorName = "Subjecter";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM13()
        {
            string testName = "SMtoRM13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2", "Time miss" };
            model.ResponseName = "Resper";
            model.RepeatedFactorName = "Repeater";
            model.SubjectFactorName = "Subjecter";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM14()
        {
            string testName = "SMtoRM14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = null;
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM15()
        {
            string testName = "SMtoRM15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!", "Time miss" };
            model.SubjectFactor = null;
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM16()
        {
            string testName = "SMtoRM16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM17()
        {
            string testName = "SMtoRM17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!", "Time miss" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM18()
        {
            string testName = "SMtoRM18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = null;
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM19()
        {
            string testName = "SMtoRM19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = null;
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM20()
        {
            string testName = "SMtoRM20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM21()
        {
            string testName = "SMtoRM21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM22()
        {
            string testName = "SMtoRM22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" }; ;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM23()
        {
            string testName = "SMtoRM23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" }; ;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM24()
        {
            string testName = "SMtoRM24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" }; ;
            model.ResponseName = "Resper";
            model.RepeatedFactorName = "Repeater";
            model.SubjectFactorName = "Subjecter";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM25()
        {
            string testName = "SMtoRM25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = "Resper";
            model.RepeatedFactorName = "Repeater";
            model.SubjectFactorName = "Subjecter";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM26()
        {
            string testName = "SMtoRM26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual2";
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM27()
        {
            string testName = "SMtoRM27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!", "Time miss" };
            model.SubjectFactor = "Individual2";
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM28()
        {
            string testName = "SMtoRM28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual2";
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM29()
        {
            string testName = "SMtoRM29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual2";
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM30()
        {
            string testName = "SMtoRM30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual2";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM31()
        {
            string testName = "SMtoRM31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual2";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM32()
        {
            string testName = "SMtoRM32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual2";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = "Resper";
            model.RepeatedFactorName = "Repeater";
            model.SubjectFactorName = "Subjecter";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM33()
        {
            string testName = "SMtoRM33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual2";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = "Resper";
            model.RepeatedFactorName = "Repeater";
            model.SubjectFactorName = "Subjecter";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM34()
        {
            string testName = "SMtoRM34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual3";
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM35()
        {
            string testName = "SMtoRM35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!", "Time miss" };
            model.SubjectFactor = "Individual3";
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM36()
        {
            string testName = "SMtoRM36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual3";
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM37()
        {
            string testName = "SMtoRM37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual3";
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM38()
        {
            string testName = "SMtoRM38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual3";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM39()
        {
            string testName = "SMtoRM39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual3";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM40()
        {
            string testName = "SMtoRM40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual3";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = "Resper";
            model.RepeatedFactorName = "Repeater";
            model.SubjectFactorName = "Subjecter";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM41()
        {
            string testName = "SMtoRM41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual3";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = "Resper";
            model.RepeatedFactorName = "Repeater";
            model.SubjectFactorName = "Subjecter";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM42()
        {
            string testName = "SMtoRM42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual4";
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM43()
        {
            string testName = "SMtoRM43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time4!", "Time miss" };
            model.SubjectFactor = "Individual4";
            model.IncludeAllVariables = false;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM44()
        {
            string testName = "SMtoRM44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual4";
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM45()
        {
            string testName = "SMtoRM45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual4";
            model.IncludeAllVariables = true;
            model.SelectedVariables = null;
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM46()
        {
            string testName = "SMtoRM46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual4";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM47()
        {
            string testName = "SMtoRM47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual4";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = null;
            model.RepeatedFactorName = null;
            model.SubjectFactorName = null;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task SMtoRM48()
        {
            string testName = "SMtoRM48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time miss" };
            model.SubjectFactor = "Individual4";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = "Resper";
            model.RepeatedFactorName = "Repeater";
            model.SubjectFactorName = "Subjecter";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }
        [Fact]
        public async Task SMtoRM49()
        {
            string testName = "SMtoRM49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            SingleMeasuresToRepeatedMeasuresDataTransformationModel model = new SingleMeasuresToRepeatedMeasuresDataTransformationModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "RM short fat").Key;
            model.Responses = new string[] { "Time1", "Time2", "Time3£", "Time miss" };
            model.SubjectFactor = "Individual4";
            model.IncludeAllVariables = false;
            model.SelectedVariables = new string[] { "Treatment", "Treatment2" };
            model.ResponseName = "Resper";
            model.RepeatedFactorName = "Repeater";
            model.SubjectFactorName = "Subjecter";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "SingleMeasuresToRepeatedMeasuresDataTransformation", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("SingleMeasuresToRepeatedMeasuresDataTransformation", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "SingleMeasuresToRepeatedMeasuresDataTransformation", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }
    }
}