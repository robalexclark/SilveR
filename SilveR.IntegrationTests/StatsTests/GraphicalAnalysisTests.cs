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

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
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
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The x-axis variable selected (Resp9) contains missing values whereas the response variable (Resp 1) contains data. To generate the Histogram (which does not require an x-axis variable) deselect the x-axis variable prior to analysis.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
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
            Assert.Contains("The 1st categorisation factor selected is numerical. Each numerical value present will consitute a category.", warnings);
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
            Assert.Contains("The 2nd categorisation factor selected is numerical. Each numerical value present will consitute a category.", warnings);
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
            Assert.Contains("The 1st categorisation factor selected has only one distinct level. Please review your selection.", errors);
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
            Assert.Contains("The 2nd categorisation factor selected has only one distinct level. Please review your selection.", errors);
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
            Assert.Contains("The 1st categorisation factor selected (Cat4) contains missing values whereas the response variable (Resp 1) contains data.", errors);
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
            Assert.Contains("The 2nd categorisation factor selected (Cat4) contains missing values whereas the response variable (Resp 1) contains data.", errors);
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
            model.Response = "Resp2";
            model.XAxis = "Resp 1";
            model.FirstCatFactor = "Cat4";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The 1st categorisation factor selected (Cat4) contains missing values whereas the response variable (Resp2) contains data.", errors);
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
            model.Response = "Resp2";
            model.XAxis = "Resp 1";
            model.SecondCatFactor = "Cat4";
            model.ScatterplotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The 2nd categorisation factor selected (Cat4) contains missing values whereas the response variable (Resp2) contains data.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
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
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The x-axis variable selected (Resp5) contains missing values whereas the response variable (Resp2) contains data. To generate the Histogram (which does not require an x-axis variable) deselect the x-axis variable prior to analysis.", errors);
            Helpers.SaveOutput("GraphicalAnalysis", testName, errors);
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
            Assert.Contains("The response selected (Resp5) contains missing values whereas the x-axis variable (Resp2) contains data. The corresponding x-axis variable values have been excluded from the analysis.", warnings);
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
            Assert.Contains("As one of the axes is non-numeric, the best fit line is not included on the scatterplot.", warnings);
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
            Assert.Contains("As one of the axes is non-numeric, the best fit line is not included on the scatterplot.", warnings);
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
            model.SEMPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Both the x-axis and the response variables are not numeric.", errors);
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
            model.SEMPlotSelected = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/GraphicalAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The selected x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
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
            model.SEMPlotSelected = true;

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
            Assert.Contains("Both the x-axis and the response variables are not numeric.", errors);
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
            Assert.Contains("The selected x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.", warnings);
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
            model.Response = "Resp1";
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
    }


}