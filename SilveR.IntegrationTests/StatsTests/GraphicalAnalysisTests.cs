using ControlledForms.IntegrationTests;
using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class GraphicalAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public GraphicalAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GRA1()
        {
            string testName = "GRA1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp3";
            model.XAxisTransformation = "Log10";
            model.Response = "Resp 1";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp3 variable. Unfortunately some of the Resp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA2()
        {
            string testName = "GRA2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp3";
            model.XAxisTransformation = "Square Root";
            model.Response = "Resp 1";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp3 variable. Unfortunately some of the Resp3 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA3()
        {
            string testName = "GRA3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp4";
            model.XAxisTransformation = "Loge";
            model.Response = "Resp 1";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp4 variable. Unfortunately some of the Resp4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA4()
        {
            string testName = "GRA4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp3";
            model.XAxisTransformation = "Log10";
            model.Response = "Resp 1";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp3 variable. Unfortunately some of the Resp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA5()
        {
            string testName = "GRA5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp3";
            model.XAxisTransformation = "Square Root";
            model.Response = "Resp 1";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp3 variable. Unfortunately some of the Resp3 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA6()
        {
            string testName = "GRA6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp4";
            model.XAxisTransformation = "Log10";
            model.Response = "Resp 1";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp4 variable. Unfortunately some of the Resp4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA7()
        {
            string testName = "GRA7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "Loge";
            model.Response = "Resp 1";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the x-axis variable (Treat 1). Unfortunately the x-axis variable is non-numerical and hence cannot be transformed. The transformation has been ignored.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA8()
        {
            string testName = "GRA8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "Square Root";
            model.Response = "Resp 1";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the x-axis variable (Treat 1). Unfortunately the x-axis variable is non-numerical and hence cannot be transformed. The transformation has been ignored.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA9()
        {
            string testName = "GRA9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp 1";
            model.Response = "Treat 1";
            model.ResponseTransformation = "Log10";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the response variable (Treat 1). Unfortunately the response variable is non-numerical and hence cannot be transformed. The transformation has been ignored.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA10()
        {
            string testName = "GRA10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp 1";
            model.Response = "Treat 1";
            model.ResponseTransformation = "Square Root";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the response variable (Treat 1). Unfortunately the response variable is non-numerical and hence cannot be transformed. The transformation has been ignored.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA11()
        {
            string testName = "GRA11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp 1";
            model.Response = "Resp8";
            model.ResponseTransformation = "ArcSine";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA12()
        {
            string testName = "GRA12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp8";
            model.XAxisTransformation = "ArcSine";
            model.Response = "Resp 1";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The x-axis variable selected (Resp8) contains missing values whereas the response variable (Resp 1) contains data. To generate the Histogram (which does not require an x-axis variable) deselect the x-axis variable prior to analysis.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA13()
        {
            string testName = "GRA13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp 1";
            model.Response = "Resp9";
            model.ResponseTransformation = "ArcSine";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp9 variable. Unfortunately some of the Resp9 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA14()
        {
            string testName = "GRA14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp9";
            model.XAxisTransformation = "ArcSine";
            model.Response = "Resp 1";
            model.ScatterplotSelected = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The x-axis variable selected (Resp9) contains missing values whereas the response variable (Resp 1) contains data. To generate the Histogram (which does not require an x-axis variable) deselect the x-axis variable prior to analysis.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

    }
}