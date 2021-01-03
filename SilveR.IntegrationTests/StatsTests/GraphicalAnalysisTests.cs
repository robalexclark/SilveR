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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Response (Treat 1). Unfortunately the Response is non-numerical and hence cannot be transformed. The transformation has been ignored.", warnings);
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Response (Treat 1). Unfortunately the Response is non-numerical and hence cannot be transformed. The transformation has been ignored.", warnings);
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp9 variable. Unfortunately some of the Resp9 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA15()
        {
            string testName = "GRA15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Treat 1";
            model.Response = "Resp 1";
            model.FirstCatFactor = "Cat1";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA16()
        {
            string testName = "GRA16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Treat 1";
            model.Response = "Resp 1";
            model.SecondCatFactor = "Cat1";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA17()
        {
            string testName = "GRA17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Treat 1";
            model.Response = "Resp 1";
            model.FirstCatFactor = "Cat3";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The 1st categorisation factor has only one distinct level. Please review your selection.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA18()
        {
            string testName = "GRA18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Treat 1";
            model.Response = "Resp 1";
            model.SecondCatFactor = "Cat3";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor has only one distinct level. Please review your selection.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA19()
        {
            string testName = "GRA19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp2";
            model.Response = "Resp 1";
            model.FirstCatFactor = "Cat4";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The 1st categorisation factor (Cat4) contains missing values whereas the Response (Resp 1) contains data.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA20()
        {
            string testName = "GRA20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp2";
            model.Response = "Resp 1";
            model.SecondCatFactor = "Cat4";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor (Cat4) contains missing values whereas the Response (Resp 1) contains data.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA21()
        {
            string testName = "GRA21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2a";
            model.XAxis = "Resp 1";
            model.FirstCatFactor = "Cat4";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The 1st categorisation factor (Cat4) contains missing values whereas the x-axis variable (Resp 1) contains data.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA22()
        {
            string testName = "GRA22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2a";
            model.XAxis = "Resp 1";
            model.SecondCatFactor = "Cat4";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor (Cat4) contains missing values whereas the x-axis variable (Resp 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA23()
        {
            string testName = "GRA23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.XAxis = "Resp5";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable (Resp5) contains missing values whereas the Response (Resp2) contains data, these responses will therefore be excluded from all analyses. To generate the Histogram containing these excluded responses (which does not require an x-axis variable) deselect the x-axis variable prior to analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA24()
        {
            string testName = "GRA24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp5";
            model.XAxis = "Resp2";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp5) contains missing values whereas the x-axis variable (Resp2) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA25()
        {
            string testName = "GRA25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Cat2";
            model.XAxis = "Resp 1";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As one of the axes is non-numeric, the best fit linear line is not included on the scatterplot.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA26()
        {
            string testName = "GRA26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.XAxis = "Cat2";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As one of the axes is non-numeric, the best fit linear line is not included on the scatterplot.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA27()
        {
            string testName = "GRA27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp 1";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response field is required.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA28()
        {
            string testName = "GRA28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Cat2";
            model.XAxis = "Treat 1";
            model.ErrorBarPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The response variable is not numeric.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA29()
        {
            string testName = "GRA29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.XAxis = "Time 1";
            model.ErrorBarPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA30()
        {
            string testName = "GRA30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ErrorBarPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("X-axis variable required for all plots except histogram.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA31()
        {
            string testName = "GRA31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Cat2";
            model.XAxis = "Treat 1";
            model.BoxplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The response variable is not numeric.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA32()
        {
            string testName = "GRA32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.XAxis = "Time 1";
            model.BoxplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA33()
        {
            string testName = "GRA33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.BoxplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("X-axis variable required for all plots except histogram.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA34()
        {
            string testName = "GRA34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Cat2";
            model.HistogramSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("As the Response is non-numeric, no histogram or other output has been produced.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA35()
        {
            string testName = "GRA35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.XAxis = "Time 1";
            model.HistogramSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is ignored in the Histogram plot option. If you wish to include a categorisation factor in the plot, then select the categorisation factors.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA36()
        {
            string testName = "GRA36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.XAxis = "Resp 1";
            model.HistogramSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response field is required.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA37()
        {
            string testName = "GRA37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Treat 1";
            model.XAxis = "Time 1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("Only the scatterplot option accepts non-numeric response variables. As the response is non-numeric, no plot other than a scatterplot can be produced.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);
        }

        [Fact]
        public async Task GRA38()
        {
            string testName = "GRA38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("X-axis variable required for all plots except histogram.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
        }

        [Fact]
        public async Task GRA39()
        {
            string testName = "GRA39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.XAxis = "Resp 1";
            model.ScatterplotSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp2";
            model.LinearFitSelected = true;
            model.ReferenceLine = 0.75m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA40()
        {
            string testName = "GRA40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "Log10";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp2";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA41()
        {
            string testName = "GRA41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Resp 1";
            model.ScatterplotSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp2";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA42()
        {
            string testName = "GRA42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Treat 1";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "Loge";
            model.ScatterplotSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Treat 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA43()
        {
            string testName = "GRA43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Treat 1";
            model.ScatterplotSelected = true;
            model.JitterSelected = true;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 1";
            model.XAxisLevelsOrder = "T3,T2,T1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA44()
        {
            string testName = "GRA44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "Square Root";
            model.FirstCatFactor = "Cat1";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ReferenceLine = 1m;
            model.FirstCatFactorLevelsOrder = "2,1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA45()
        {
            string testName = "GRA45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "Square Root";
            model.SecondCatFactor = "Cat1";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.ReferenceLine = 1m;
            model.SecondCatFactorLevelsOrder = "2,1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA46()
        {
            string testName = "GRA46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = null;
            model.SecondCatFactor = "Cat1";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.SecondCatFactorLevelsOrder = "2,1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA47()
        {
            string testName = "GRA47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.JitterSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA48()
        {
            string testName = "GRA48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ScatterplotSelected = true;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.JitterSelected = true;
            model.FirstCatFactorLevelsOrder = "2,1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA49()
        {
            string testName = "GRA49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Treat 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "Square Root";
            model.FirstCatFactor = "Cat2";
            model.ScatterplotSelected = true;
            model.MainTitle = "Scatterplot test";
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Treat 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.JitterSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA50()
        {
            string testName = "GRA50";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Treat 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "Log10";
            model.FirstCatFactor = "Cat2";
            model.ScatterplotSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Treat 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.JitterSelected = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA51()
        {
            string testName = "GRA51";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA52()
        {
            string testName = "GRA52";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA53()
        {
            string testName = "GRA53";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "Square Root";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.ScatterplotSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.LinearFitSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA54()
        {
            string testName = "GRA54";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "Square Root";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA55()
        {
            string testName = "GRA55";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "ArcSine";
            model.XAxis = "Resp 1";
            model.XAxisTransformation = "ArcSine";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.ScatterplotSelected = true;
            model.LinearFitSelected = true;
            model.XAxisTitle = "Resp 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA56()
        {
            string testName = "GRA56";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.ScatterplotSelected = true;
            model.XAxisTitle = "Time2";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.JitterSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA57()
        {
            string testName = "GRA57";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.ScatterplotSelected = true;
            model.XAxisTitle = "Time2";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.JitterSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA58()
        {
            string testName = "GRA58";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Time2";
            model.ResponseTransformation = "None";
            model.XAxis = "Resp2";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Time 1";
            model.ScatterplotSelected = true;
            model.XAxisTitle = "Resp 2";
            model.YAxisTitle = "Time2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.JitterSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA59()
        {
            string testName = "GRA59";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Time2";
            model.ResponseTransformation = "None";
            model.XAxis = "Resp2";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Time 1";
            model.ScatterplotSelected = true;
            model.XAxisTitle = "Resp 2";
            model.YAxisTitle = "Time2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.JitterSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA60()
        {
            string testName = "GRA60";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.BoxplotSelected = true;
            model.MainTitle = "Boxplot test";
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp 2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA61()
        {
            string testName = "GRA61";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.BoxplotSelected = true;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 2";
            model.BoxplotOptions = "Outliers";
            model.ReferenceLine = 0m;
            model.XAxisLevelsOrder = "T3,T2,T1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA62()
        {
            string testName = "GRA62";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.BoxplotSelected = true;
            model.BoxplotOptions = "Include Data";
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA63()
        {
            string testName = "GRA63";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.BoxplotSelected = true;
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.BoxplotOptions = "Outliers";
            model.ReferenceLine = 1m;
            model.FirstCatFactorLevelsOrder = "2,1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA64()
        {
            string testName = "GRA64";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.BoxplotSelected = true;
            model.XAxisTitle = "Time 1 Cat 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.BoxplotOptions = "None";
            model.ReferenceLine = 1m;
            model.FirstCatFactorLevelsOrder = "2,1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA65()
        {
            string testName = "GRA65";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.SecondCatFactor = "Cat1";
            model.BoxplotSelected = true;
            model.BoxplotOptions = "Include Data";
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.SecondCatFactorLevelsOrder = "2,1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA66()
        {
            string testName = "GRA66";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.SecondCatFactor = "Cat1";
            model.BoxplotSelected = true;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.BoxplotOptions = "Outliers";
            model.SecondCatFactorLevelsOrder = "2,1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA67()
        {
            string testName = "GRA67";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat2";
            model.BoxplotSelected = true;
            model.BoxplotOptions = "Include Data";
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.BoxplotOptions = "Outliers";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA68()
        {
            string testName = "GRA68";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat2";
            model.BoxplotSelected = true;
            model.XAxisTitle = "Time 1 Cat 2";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.BoxplotOptions = "None";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA69()
        {
            string testName = "GRA69";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat2";
            model.BoxplotSelected = true;
            model.BoxplotOptions = "Include Data";
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisLevelsOrder = "T3,T2,T1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA70()
        {
            string testName = "GRA70";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat2";
            model.BoxplotSelected = true;
            model.XAxisTitle = "Treat 1 Cat 2";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.BoxplotOptions = "Outliers";
            model.XAxisLevelsOrder = "T3,T2,T1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA71()
        {
            string testName = "GRA71";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.BoxplotSelected = true;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.BoxplotOptions = "Outliers";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA72()
        {
            string testName = "GRA72";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.BoxplotSelected = true;
            model.XAxisTitle = "Time 1 Cat 1 Cat 2";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.BoxplotOptions = "None";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA73()
        {
            string testName = "GRA73";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Anima 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Time 1";
            model.BoxplotSelected = true;
            model.BoxplotOptions = "Include Data";
            model.XAxisTitle = "Anima 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA74()
        {
            string testName = "GRA74";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Anima 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Time 1";
            model.BoxplotSelected = true;
            model.XAxisTitle = "Anima 1 Time 1 Cat 1";
            model.YAxisTitle = "Resp 2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA75()
        {
            string testName = "GRA75";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.ErrorBarPlotSelected = true;
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp 2";
            model.ReferenceLine = 0.6m;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA76()
        {
            string testName = "GRA76";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.ErrorBarPlotSelected = true;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 2";
            model.XAxisLevelsOrder = "T3,T2,T1";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA77()
        {
            string testName = "GRA77";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp 2";
            model.ReferenceLine = 0.6m;
            model.FirstCatFactorLevelsOrder = "2,1";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA78()
        {
            string testName = "GRA78";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp 2";
            model.FirstCatFactorLevelsOrder = "2,1";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA79()
        {
            string testName = "GRA79";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Sqrt Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA80()
        {
            string testName = "GRA80";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.SecondCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "LogeResp2";
            model.ReferenceLine = -0.7m;
            model.XAxisLevelsOrder = "T3,T2,T1";
            model.SecondCatFactorLevelsOrder = "2,1";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA81()
        {
            string testName = "GRA81";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.SecondCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp2";
            model.SecondCatFactorLevelsOrder = "B,A";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA82()
        {
            string testName = "GRA82";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Time 1 Cat 1";
            model.YAxisTitle = "Log10Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA83()
        {
            string testName = "GRA83";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Square Root Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA84()
        {
            string testName = "GRA84";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Treat 1 Cat 2";
            model.YAxisTitle = "Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA85()
        {
            string testName = "GRA85";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA86()
        {
            string testName = "GRA86";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Time 1 Cat 1 Cat 2 ";
            model.YAxisTitle = "Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA87()
        {
            string testName = "GRA87";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Anima 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Time 1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Anima 1";
            model.YAxisTitle = "Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA88()
        {
            string testName = "GRA88";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Anima 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Time 1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Animal 1 Time 1 Cat 1";
            model.YAxisTitle = "Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA89()
        {
            string testName = "GRA89";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA90()
        {
            string testName = "GRA90";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat7";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisLevelsOrder = "D,E,A,B,C";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA91()
        {
            string testName = "GRA91";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat8";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA92()
        {
            string testName = "GRA92";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA93()
        {
            string testName = "GRA93";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat7";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA94()
        {
            string testName = "GRA94";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat8";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA95()
        {
            string testName = "GRA95";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA96()
        {
            string testName = "GRA96";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat7";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA97()
        {
            string testName = "GRA97";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat8";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task GRA98()
        {
            string testName = "GRA98";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA99()
        {
            string testName = "GRA99";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 100";
            model.ResponseTransformation = "ArcSine";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA100()
        {
            string testName = "GRA100";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat7";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA101()
        {
            string testName = "GRA101";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat8";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA102()
        {
            string testName = "GRA102";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisTitle = "Treat 1";
            model.MainTitle = "Title";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing values whereas the x-axis variable (Treat 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA103()
        {
            string testName = "GRA103";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 8";
            model.MainTitle = "Title";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing values whereas the x-axis variable (Treat 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA104()
        {
            string testName = "GRA104";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 8";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing values whereas the x-axis variable (Treat 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA105()
        {
            string testName = "GRA105";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 8";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing values whereas the x-axis variable (Treat 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA106()
        {
            string testName = "GRA106";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.SEM;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 8";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing values whereas the x-axis variable (Treat 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA107()
        {
            string testName = "GRA107";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.HistogramSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA108()
        {
            string testName = "GRA108";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.HistogramSelected = true;
            model.NormalDistSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA109()
        {
            string testName = "GRA109";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.HistogramSelected = true;
            model.NormalDistSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA110()
        {
            string testName = "GRA110";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Treat 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA111()
        {
            string testName = "GRA111";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Treat 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.NormalDistSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA112()
        {
            string testName = "GRA112";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Treat 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA113()
        {
            string testName = "GRA113";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Treat 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.NormalDistSelected = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA114()
        {
            string testName = "GRA114";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Time 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA115()
        {
            string testName = "GRA115";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Time 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.NormalDistSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA116()
        {
            string testName = "GRA116";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Time 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.NormalDistSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA117()
        {
            string testName = "GRA117";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Time 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA118()
        {
            string testName = "GRA118";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Time 1";
            model.SecondCatFactor = "Treat 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.NormalDistSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA119()
        {
            string testName = "GRA119";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Time 1";
            model.SecondCatFactor = "Treat 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.NormalDistSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA120()
        {
            string testName = "GRA120";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Time 1";
            model.SecondCatFactor = "Treat 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA121()
        {
            string testName = "GRA121";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.FirstCatFactor = "Time 1";
            model.SecondCatFactor = "Treat 1";
            model.HistogramSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA122()
        {
            string testName = "GRA122";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ReferenceLine = 1m;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA123()
        {
            string testName = "GRA123";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Time2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA124()
        {
            string testName = "GRA124";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Time 1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA125()
        {
            string testName = "GRA125";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA126()
        {
            string testName = "GRA126";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ReferenceLine = 1m;
            model.FirstCatFactorLevelsOrder = "2,1";
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA127()
        {
            string testName = "GRA127";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Time 1";
            model.SecondCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.SecondCatFactorLevelsOrder = "2,1";
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA128()
        {
            string testName = "GRA128";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA129()
        {
            string testName = "GRA129";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA130()
        {
            string testName = "GRA130";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.ReferenceLine = 1m;
            model.FirstCatFactorLevelsOrder = "2,1";
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA131()
        {
            string testName = "GRA131";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Time 1";
            model.SecondCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.SecondCatFactorLevelsOrder = "2,1";
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA132()
        {
            string testName = "GRA132";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA133()
        {
            string testName = "GRA133";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA134()
        {
            string testName = "GRA134";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA135()
        {
            string testName = "GRA135";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA136()
        {
            string testName = "GRA136";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA137()
        {
            string testName = "GRA137";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA138()
        {
            string testName = "GRA138";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA139()
        {
            string testName = "GRA139";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA140()
        {
            string testName = "GRA140";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA141()
        {
            string testName = "GRA141";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA142()
        {
            string testName = "GRA142";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.SecondCatFactor = "Treat 1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA143()
        {
            string testName = "GRA143";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat2";
            model.SecondCatFactor = "Treat 1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA144()
        {
            string testName = "GRA144";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA145()
        {
            string testName = "GRA145";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.SecondCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA146()
        {
            string testName = "GRA146";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA147()
        {
            string testName = "GRA147";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA148()
        {
            string testName = "GRA148";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA149()
        {
            string testName = "GRA149";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.SecondCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA150()
        {
            string testName = "GRA150";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = false;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA151()
        {
            string testName = "GRA151";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ReferenceLine = 1m;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA152()
        {
            string testName = "GRA152";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Time2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA153()
        {
            string testName = "GRA153";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Time 1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA154()
        {
            string testName = "GRA154";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA155()
        {
            string testName = "GRA155";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ReferenceLine = 1m;
            model.FirstCatFactorLevelsOrder = "2,1";
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA156()
        {
            string testName = "GRA156";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Time 1";
            model.SecondCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.SecondCatFactorLevelsOrder = "2,1";
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA157()
        {
            string testName = "GRA157";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA158()
        {
            string testName = "GRA158";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA159()
        {
            string testName = "GRA159";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.ReferenceLine = 1m;
            model.FirstCatFactorLevelsOrder = "2,1";
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA160()
        {
            string testName = "GRA160";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Time 1";
            model.SecondCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.SecondCatFactorLevelsOrder = "2,1";
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA161()
        {
            string testName = "GRA161";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA162()
        {
            string testName = "GRA162";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA163()
        {
            string testName = "GRA163";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA164()
        {
            string testName = "GRA164";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA165()
        {
            string testName = "GRA165";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA166()
        {
            string testName = "GRA166";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA167()
        {
            string testName = "GRA167";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA168()
        {
            string testName = "GRA168";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA169()
        {
            string testName = "GRA169";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA170()
        {
            string testName = "GRA170";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA171()
        {
            string testName = "GRA171";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.SecondCatFactor = "Treat 1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA172()
        {
            string testName = "GRA172";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat2";
            model.SecondCatFactor = "Treat 1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA173()
        {
            string testName = "GRA173";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA174()
        {
            string testName = "GRA174";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.SecondCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA175()
        {
            string testName = "GRA175";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA176()
        {
            string testName = "GRA176";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Treat 1";
            model.SecondCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Animal2";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA177()
        {
            string testName = "GRA177";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Time 1";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA178()
        {
            string testName = "GRA178";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat2";
            model.SecondCatFactor = "Cat1";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA179()
        {
            string testName = "GRA179";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.XAxis = "Time2";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.CaseProfilesPlotSelected = true;
            model.CaseIDFactor = "Anima 1";
            model.ShowCaseIDsInLegend = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.DisplayLegend = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA180()
        {
            string testName = "GRA180";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.ErrorBarPlotSelected = true;
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp 2";
            model.ReferenceLine = 0.6m;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA181()
        {
            string testName = "GRA181";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.ErrorBarPlotSelected = true;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 2";
            model.XAxisLevelsOrder = "T3,T2,T1";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA182()
        {
            string testName = "GRA182";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp 2";
            model.ReferenceLine = 0.6m;
            model.FirstCatFactorLevelsOrder = "2,1";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA183()
        {
            string testName = "GRA183";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp 2";
            model.FirstCatFactorLevelsOrder = "2,1";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA184()
        {
            string testName = "GRA184";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Sqrt Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA185()
        {
            string testName = "GRA185";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Loge";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.SecondCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "LogeResp2";
            model.ReferenceLine = -0.7m;
            model.XAxisLevelsOrder = "T3,T2,T1";
            model.SecondCatFactorLevelsOrder = "2,1";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA186()
        {
            string testName = "GRA186";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.SecondCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Time 1";
            model.YAxisTitle = "Resp2";
            model.SecondCatFactorLevelsOrder = "B,A";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA187()
        {
            string testName = "GRA187";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Log10";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Time 1 Cat 1";
            model.YAxisTitle = "Log10Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA188()
        {
            string testName = "GRA188";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "Square Root";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Square Root Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA189()
        {
            string testName = "GRA189";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Treat 1 Cat 2";
            model.YAxisTitle = "Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA190()
        {
            string testName = "GRA190";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA191()
        {
            string testName = "GRA191";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Time 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Time 1 Cat 1 Cat 2 ";
            model.YAxisTitle = "Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA192()
        {
            string testName = "GRA192";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Anima 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Time 1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "Anima 1";
            model.YAxisTitle = "Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA193()
        {
            string testName = "GRA193";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp2";
            model.ResponseTransformation = "None";
            model.XAxis = "Anima 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Time 1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "Animal 1 Time 1 Cat 1";
            model.YAxisTitle = "Resp2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Line;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The 1st categorisation factor is numerical. Each numerical value present will consitute a category.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA194()
        {
            string testName = "GRA194";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA195()
        {
            string testName = "GRA195";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat7";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisLevelsOrder = "D,E,A,B,C";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA196()
        {
            string testName = "GRA196";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat8";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA197()
        {
            string testName = "GRA197";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA198()
        {
            string testName = "GRA198";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat7";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA199()
        {
            string testName = "GRA199";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat8";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA200()
        {
            string testName = "GRA200";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA201()
        {
            string testName = "GRA201";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat7";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA202()
        {
            string testName = "GRA202";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat8";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.XAxisTitle = "";
            model.YAxisTitle = "Resp 2";
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }


        [Fact]
        public async Task GRA203()
        {
            string testName = "GRA203";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA204()
        {
            string testName = "GRA204";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp 100";
            model.ResponseTransformation = "ArcSine";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA205()
        {
            string testName = "GRA205";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat7";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA206()
        {
            string testName = "GRA206";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "None";
            model.XAxis = "Cat5";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat6";
            model.SecondCatFactor = "Cat8";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA207()
        {
            string testName = "GRA207";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisTitle = "Treat 1";
            model.MainTitle = "Title";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing values whereas the x-axis variable (Treat 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA208()
        {
            string testName = "GRA208";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 8";
            model.MainTitle = "Title";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing values whereas the x-axis variable (Treat 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA209()
        {
            string testName = "GRA209";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Separate;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 8";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing values whereas the x-axis variable (Treat 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA210()
        {
            string testName = "GRA210";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 8";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing values whereas the x-axis variable (Treat 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task GRA211()
        {
            string testName = "GRA211";

            //Arrange
            HttpClient client = _factory.CreateClient();

            GraphicalAnalysisModel model = new GraphicalAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Graphical").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.XAxis = "Treat 1";
            model.XAxisTransformation = "None";
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat2";
            model.ErrorBarPlotSelected = true;
            model.StyleType = GraphicalAnalysisModel.GraphStyleType.Overlaid;
            model.ErrorBarType = GraphicalAnalysisModel.ErrorBarPlotType.ConfidenceInterval;
            model.SEMType = GraphicalAnalysisModel.SEMPlotType.Column;
            model.XAxisTitle = "Treat 1";
            model.YAxisTitle = "Resp 8";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing values whereas the x-axis variable (Treat 1) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
            Helpers.SaveOutput("GraphicalAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "GraphicalAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("GraphicalAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "GraphicalAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }
    }
}