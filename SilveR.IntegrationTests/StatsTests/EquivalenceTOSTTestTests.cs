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
    public class EquivalenceTOSTTestTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public EquivalenceTOSTTestTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task ETT1()
        {
            string testName = "ETT1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Resp 1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp 1) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }

        [Fact]
        public async Task ETT2()
        {
            string testName = "ETT2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp6";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the Treatment factor (Treat1). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }

        [Fact]
        public async Task ETT3()
        {
            string testName = "ETT3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat7" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the Treatment factor (Treat7). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }


        [Fact]
        public async Task ETT4()
        {
            string testName = "ETT4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Tre at4" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment factor (Tre at4) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }

        [Fact]
        public async Task ETT5()
        {
            string testName = "ETT5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp4";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }

        [Fact]
        public async Task ETT6()
        {
            string testName = "ETT6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov4" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Covariate (Cov4) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }

        [Fact]
        public async Task ETT7()
        {
            string testName = "ETT7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp5";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp5) contains missing data.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT8()
        {
            string testName = "ETT8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov5" };
            model.PrimaryFactor = "Treat1";
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Covariate (Cov5) contains missing data. Any response that does not have a corresponding covariate will be excluded from the analysis.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT9()
        {
            string testName = "ETT9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "T reat5" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the factors (T reat5) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }

        [Fact]
        public async Task ETT10()
        {
            string testName = "ETT10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat6" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment factor (Treat6). Please select another factor.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }

        [Fact]
        public async Task ETT11()
        {
            string testName = "ETT11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 2";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp 2 variable. Unfortunately some of the Resp 2 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT12()
        {
            string testName = "ETT12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp3";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10;
            model.UpperBoundPercentageChange = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp3 variable. Unfortunately some of the Resp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT13()
        {
            string testName = "ETT13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 2";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp 2 variable. Unfortunately some of the Resp 2 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT14()
        {
            string testName = "ETT14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov2" };
            model.CovariateTransformation = "Log10";
            model.PrimaryFactor = "Treat1";
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Cov2 variable. Unfortunately some of the Cov2 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT15()
        {
            string testName = "ETT15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov3" };
            model.CovariateTransformation = "Loge";
            model.PrimaryFactor = "Treat1";
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Cov3 variable. Unfortunately some of the Cov3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT16()
        {
            string testName = "ETT16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov2" };
            model.CovariateTransformation = "Square Root";
            model.PrimaryFactor = "Treat1";
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Cov2 variable. Unfortunately some of the Cov2 values are negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT17()
        {
            string testName = "ETT17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp9 variable. Unfortunately some of the Resp9 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT18()
        {
            string testName = "ETT18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT19()
        {
            string testName = "ETT19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Cov2" };
            model.CovariateTransformation = "ArcSine";
            model.PrimaryFactor = "Treat1";
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Cov2 variable. Unfortunately some of the Cov2 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT20()
        {
            string testName = "ETT20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.Covariates = new string[] { "Resp8" };
            model.CovariateTransformation = "ArcSine";
            model.PrimaryFactor = "Treat1";
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = 20;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }


        [Fact]
        public async Task ETT21()
        {
            string testName = "ETT21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp8";
            model.Treatments = new string[] { "Treat12", "Treat13" };
            model.SelectedEffect = "Treat12";
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = 20;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT22()
        {
            string testName = "ETT22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp11";
            model.Treatments = new string[] { "Treat14" };
            model.SelectedEffect = "Treat14";
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 10;
            model.UpperBoundAbsolute = 20;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT23()
        {
            string testName = "ETT23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 0.5m;
            model.UpperBoundAbsolute = 0.1m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The lower bound selected is higher than the upper bound, please check the bounds as the lower bound should be less than the upper bound.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }

        [Fact]
        public async Task ETT25()
        {
            string testName = "ETT25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = -1m;
            model.UpperBoundPercentageChange = 0.1m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("User cannot select a negative Percentage change.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }

        [Fact]
        public async Task ETT26()
        {
            string testName = "ETT26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = -2m;
            model.UpperBoundPercentageChange = -1m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("User cannot select a negative Percentage change.", errors);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, errors);
        }

        [Fact]
        public async Task ETT27()
        {
            string testName = "ETT27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 1m;
            model.UpperBoundAbsolute = 2m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("As you have chosen to log transformed the response, the comparisons between the predicted means will be in the form of ratios. This implies that the difference between the means will be a % change and hence you should select % boundaries rather than absolute boundaries.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT28()
        {
            string testName = "ETT28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = 1m;
            model.UpperBoundAbsolute = 2m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("As you have chosen to log transformed the response, the comparisons between the predicted means will be in the form of ratios. This implies that the difference between the means will be a % change and hence you should select % boundaries rather than absolute boundaries.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT29()
        {
            string testName = "ETT29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 1m;
            model.UpperBoundPercentageChange = 2m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("As you have selected to [sqrt, arcsine, rank] transform the responses you should choose absolute boundaries rather than % boundaries. The absolute boundaries should be defined on the transformed scale rather than the original scale.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT30()
        {
            string testName = "ETT30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 1m;
            model.UpperBoundPercentageChange = 2m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("As you have selected to [sqrt, arcsine, rank] transform the responses you should choose absolute boundaries rather than % boundaries. The absolute boundaries should be defined on the transformed scale rather than the original scale.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT31()
        {
            string testName = "ETT31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Rank";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 1m;
            model.UpperBoundPercentageChange = 2m;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("As you have selected to [sqrt, arcsine, rank] transform the responses you should choose absolute boundaries rather than % boundaries. The absolute boundaries should be defined on the transformed scale rather than the original scale.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);
        }

        [Fact]
        public async Task ETT32()
        {
            string testName = "ETT32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.2m;
            model.UpperBoundAbsolute = 0.2m;
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT33()
        {
            string testName = "ETT33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.2m;
            model.UpperBoundAbsolute = 0.2m;
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT34()
        {
            string testName = "ETT34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.15m;
            model.UpperBoundAbsolute = 0.15m;
            model.Significance = "0.001";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT35()
        {
            string testName = "ETT35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.15m;
            model.UpperBoundAbsolute = 0.15m;
            model.Significance = "0.001";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT36()
        {
            string testName = "ETT36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.3m;
            model.UpperBoundAbsolute = 0.3m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT37()
        {
            string testName = "ETT37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.3m;
            model.UpperBoundAbsolute = 0.3m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT38()
        {
            string testName = "ETT38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the overall mean of the response.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT39()
        {
            string testName = "ETT39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the selected control group.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT40()
        {
            string testName = "ETT40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT41()
        {
            string testName = "ETT41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT42()
        {
            string testName = "ETT42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT43()
        {
            string testName = "ETT43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT44()
        {
            string testName = "ETT44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.2m;
            //model.UpperBoundAbsolute = 0m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT45()
        {
            string testName = "ETT45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.2m;
            //model.UpperBoundAbsolute = 0m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT46()
        {
            string testName = "ETT46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.15m;
            //model.UpperBoundAbsolute = 0m;
            model.Significance = "0.001";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT47()
        {
            string testName = "ETT47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.3m;
            //model.UpperBoundAbsolute = 0m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT48()
        {
            string testName = "ETT48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            //model.UpperBoundAbsolute = 0m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the overall mean of the response.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT49()
        {
            string testName = "ETT49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            //model.UpperBoundAbsolute = 0m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT50()
        {
            string testName = "ETT50";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            //model.UpperBoundAbsolute = 0m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT51()
        {
            string testName = "ETT51";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            //model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            //model.LowerBoundAbsolute = 0.2m;
            model.UpperBoundAbsolute = 0.2m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT52()
        {
            string testName = "ETT52";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            //model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            //model.LowerBoundAbsolute = 0.2m;
            model.UpperBoundAbsolute = 0.2m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT53()
        {
            string testName = "ETT53";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            //model.LowerBoundAbsolute = 0.2m;
            model.UpperBoundAbsolute = 0.15m;
            model.Significance = "0.001";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT54()
        {
            string testName = "ETT54";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            //model.LowerBoundAbsolute = 0.2m;
            model.UpperBoundAbsolute = 0.3m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT55()
        {
            string testName = "ETT55";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            //model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            //model.LowerBoundAbsolute = 0.2m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the overall mean of the response.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT56()
        {
            string testName = "ETT56";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            //model.ResponseTransformation = "ArcSine";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            //model.LowerBoundAbsolute = 0.2m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the selected control group.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT57()
        {
            string testName = "ETT57";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            //model.LowerBoundAbsolute = 0.2m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "D1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT58()
        {
            string testName = "ETT58";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Treatments = new string[] { "Treat1" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            //model.LowerBoundAbsolute = 0.2m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat1";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT59()
        {
            string testName = "ETT59";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.2m;
            model.UpperBoundAbsolute = 0.2m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT60()
        {
            string testName = "ETT60";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.2m;
            model.UpperBoundAbsolute = 0.2m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "2";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT61()
        {
            string testName = "ETT61";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the overall mean of the response.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT62()
        {
            string testName = "ETT62";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the selected control group.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT63()
        {
            string testName = "ETT63";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.2m;
            //model.UpperoundAbsolute = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT64()
        {
            string testName = "ETT64";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            model.LowerBoundAbsolute = -0.2m;
            //model.UpperoundAbsolute = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "2";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT65()
        {
            string testName = "ETT65";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            //model.UpperoundAbsolute = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the overall mean of the response.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT66()
        {
            string testName = "ETT66";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            model.LowerBoundPercentageChange = 10m;
            //model.UpperoundAbsolute = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the selected control group.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT67()
        {
            string testName = "ETT67";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            //model.LowerBoundPercentageChange = 10m;
            model.UpperBoundAbsolute = -0.2m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT68()
        {
            string testName = "ETT68";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Absolute;
            //model.LowerBoundPercentageChange = 10m;
            model.UpperBoundAbsolute = -0.2m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "2";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT69()
        {
            string testName = "ETT69";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            //model.LowerBoundPercentageChange = 10m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.AllPairwise;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the overall mean of the response.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task ETT70()
        {
            string testName = "ETT70";

            //Arrange
            HttpClient client = _factory.CreateClient();

            EquivalenceTOSTTestModel model = new EquivalenceTOSTTestModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Equivalence TOST").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Treatments = new string[] { "Treat9" };
            model.EquivalenceBoundsType = EquivalenceTOSTTestModel.EquivalenceBoundsOption.Percentage;
            //model.LowerBoundPercentageChange = 10m;
            model.UpperBoundPercentageChange = 10m;
            model.Significance = "0.05";
            model.PRPlotSelected = true;
            model.SelectedEffect = "Treat9";
            model.LSMeansSelected = true;
            model.ComparisonType = EquivalenceTOSTTestModel.ComparisonOption.ComparisonsToControl;
            model.ControlGroup = "1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/EquivalenceTOSTTest", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("As you have chosen the % equivalence bounds, the actual boundaries has been calculated as a % of the selected control group.", warnings);
            Helpers.SaveOutput("EquivalenceTOSTTest", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "EquivalenceTOSTTest", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("EquivalenceTOSTTest", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "EquivalenceTOSTTest", testName + ".html"));
            Assert.Equal(Helpers.FixForUnixOSs(expectedHtml), Helpers.FixForUnixOSs(statsOutput.HtmlResults));
        }
    }
}