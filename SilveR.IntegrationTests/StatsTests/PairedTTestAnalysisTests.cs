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
    public class PairedTTestAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public PairedTTestAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PTT1()
        {
            string testName = "PTT1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Treatment = "Day1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Subject field is required.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT2()
        {
            string testName = "PTT2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment field is required.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT3()
        {
            string testName = "PTT3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp2";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Treatment (Day1). Please select another factor.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT4()
        {
            string testName = "PTT4";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp7";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("There are no observations recorded on the levels of the Treatment (Day1). Please amend the dataset prior to running the analysis.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT5()
        {
            string testName = "PTT5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";
            model.Treatment = "Day2";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Treatment (Day2) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT6()
        {
            string testName = "PTT6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal2";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Subject (Animal2) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT7()
        {
            string testName = "PTT7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.OtherDesignFactors = new string[] { "Block3" };
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Other design (block) factor (Block3) contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT8()
        {
            string testName = "PTT8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp3";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT9()
        {
            string testName = "PTT9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";
            model.Covariates = new string[] { "Cov2" };

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Covariate (Cov2) contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT10()
        {
            string testName = "PTT10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp4";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT11()
        {
            string testName = "PTT11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariates = new string[] { "Cov3" };
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Covariate (Cov3) contains missing data. Any response that does not have a corresponding covariate will be excluded from the analysis.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT12()
        {
            string testName = "PTT12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp9";
            model.Subject = "Animal3";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("There is no replication in one or more of the levels of the Subject factor (Animal3). This can lead to unreliable results so you may want to remove any subjects from the dataset with only one replicate.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT13()
        {
            string testName = "PTT13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp5 variable. Unfortunately some of the Resp5 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT14()
        {
            string testName = "PTT14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "Loge";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp6 variable. Unfortunately some of the Resp6 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT15()
        {
            string testName = "PTT15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp5 variable. Unfortunately some of the Resp5 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT16()
        {
            string testName = "PTT16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariates = new string[] { "Cov4" };
            model.CovariateTransformation = "Loge";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Cov4 variable. Unfortunately some of the Cov4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT17()
        {
            string testName = "PTT17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariates = new string[] { "Cov5" };
            model.CovariateTransformation = "Log10";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Cov5 variable. Unfortunately some of the Cov5 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT18()
        {
            string testName = "PTT18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "ArcSine";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp5 variable. Unfortunately some of the Resp5 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT19()
        {
            string testName = "PTT19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "ArcSine";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp8 variable. Unfortunately some of the Resp8 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT20()
        {
            string testName = "PTT20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariates = new string[] { "Cov4" };
            model.CovariateTransformation = "ArcSine";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Cov4 variable. Unfortunately some of the Cov4 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT21()
        {
            string testName = "PTT21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariates = new string[] { "Cov6" };
            model.CovariateTransformation = "ArcSine";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Cov6 variable. Unfortunately some of the Cov6 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT22()
        {
            string testName = "PTT22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariates = new string[] { "Cov4" };
            model.CovariateTransformation = "Square Root";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Cov4 variable. Unfortunately some of the Cov4 values are negative. These values have been ignored in the analysis as it is not possible to transform them. Any response where the covariate has been removed will also be excluded from the analysis.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);
        }

        [Fact]
        public async Task PTT23()
        {
            string testName = "PTT23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.OtherDesignFactors = new string[] { "Block2" };
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("According to the dataset at least one subject is associated with more than one level of one of the blocking factors. Please review this, as each subject must be associated with only one level of each blocking factor.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT24()
        {
            string testName = "PTT24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";
            model.Treatment = "Day3";
            model.Covariance = "Compound Symmetric";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("At least one of the subjects has more than one observation recorded on one of the treatments. Please make sure the data was entered correctly as each subject can only be measured once at each level of the Treatment.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }

        [Fact]
        public async Task PTT25()
        {
            string testName = "PTT25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "4";
            model.Covariance = "Compound Symmetric";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT26()
        {
            string testName = "PTT26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.Covariates = new string[] { "Cov1" };
            model.ControlGroup = "2";
            model.Covariance = "Compound Symmetric";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;
            
            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT27()
        {
            string testName = "PTT27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ControlGroup = "2";
            model.Covariance = "Compound Symmetric";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;
            
            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT28()
        {
            string testName = "PTT28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "6";
            model.Covariance = "Autoregressive(1)";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT29()
        {
            string testName = "PTT29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "1";
            model.Covariance = "Autoregressive(1)";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT30()
        {
            string testName = "PTT30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "4";
            model.Covariance = "Autoregressive(1)";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT31()
        {
            string testName = "PTT31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "6";
            model.Covariance = "Unstructured";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT32()
        {
            string testName = "PTT32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "1";
            model.Covariance = "Unstructured";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT33()
        {
            string testName = "PTT33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "4";
            model.Covariance = "Unstructured";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT34()
        {
            string testName = "PTT34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp4";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "2";
            model.Covariance = "Compound Symmetric";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT35()
        {
            string testName = "PTT35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp4";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "4";
            model.Covariance = "Compound Symmetric";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT36()
        {
            string testName = "PTT36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp4";
            model.ResponseTransformation = "Loge";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "1";
            model.Covariance = "Compound Symmetric";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT37()
        {
            string testName = "PTT37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp4";
            model.ResponseTransformation = "None";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "6";
            model.Covariance = "Autoregressive(1)";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT38()
        {
            string testName = "PTT38";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp4";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "2";
            model.Covariance = "Autoregressive(1)";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT39()
        {
            string testName = "PTT39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp4";
            model.ResponseTransformation = "Loge";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "4";
            model.Covariance = "Autoregressive(1)";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT40()
        {
            string testName = "PTT40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp4";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "1";
            model.Covariance = "Unstructured";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT41()
        {
            string testName = "PTT41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp4";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "6";
            model.Covariance = "Unstructured";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT42()
        {
            string testName = "PTT42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp4";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "1";
            model.Covariance = "Unstructured";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data.", warnings);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, warnings);

            //Act - ignore warnings
            var modelIgnoreWarnings = model.ToKeyValue();
            modelIgnoreWarnings.Add("ignoreWarnings", "true");
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(modelIgnoreWarnings));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT43()
        {
            string testName = "PTT43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Subject = "Animal4";
            model.Treatment = "Day4";
            model.ControlGroup = "4";
            model.Covariance = "Compound Symmetric";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT44()
        {
            string testName = "PTT44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal4";
            model.Treatment = "Day4";
            model.ControlGroup = "6";
            model.Covariance = "Compound Symmetric";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT45()
        {
            string testName = "PTT45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Subject = "Animal4";
            model.Treatment = "Day4";
            model.ControlGroup = "1";
            model.Covariance = "Compound Symmetric";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT46()
        {
            string testName = "PTT46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Subject = "Animal4";
            model.Treatment = "Day4";
            model.ControlGroup = "2";
            model.Covariance = "Autoregressive(1)";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT47()
        {
            string testName = "PTT47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal4";
            model.Treatment = "Day4";
            model.ControlGroup = "4";
            model.Covariance = "Autoregressive(1)";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT48()
        {
            string testName = "PTT48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Subject = "Animal4";
            model.Treatment = "Day4";
            model.ControlGroup = "6";
            model.Covariance = "Autoregressive(1)";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT49()
        {
            string testName = "PTT49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal4";
            model.Treatment = "Day4";
            model.ControlGroup = "6";
            model.Covariance = "Unstructured";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT50()
        {
            string testName = "PTT50";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal4";
            model.Treatment = "Day4";
            model.ControlGroup = "1";
            model.Covariance = "Unstructured";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT51()
        {
            string testName = "PTT51";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal4";
            model.Treatment = "Day4";
            model.ControlGroup = "2";
            model.Covariance = "Unstructured";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT52()
        {
            string testName = "PTT52";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Subject = "Animal 5";
            model.Treatment = "Day6";
            model.ControlGroup = "1";
            model.Covariance = "Compound Symmetric";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT53()
        {
            string testName = "PTT53";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal 5";
            model.Treatment = "Day6";
            model.ControlGroup = "2";
            model.Covariance = "Compound Symmetric";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT54()
        {
            string testName = "PTT54";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Subject = "Animal 5";
            model.Treatment = "Day6";
            model.ControlGroup = "2";
            model.Covariance = "Compound Symmetric";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT55()
        {
            string testName = "PTT55";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Subject = "Animal 5";
            model.Treatment = "Day6";
            model.ControlGroup = "1";
            model.Covariance = "Autoregressive(1)";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT56()
        {
            string testName = "PTT56";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal 5";
            model.Treatment = "Day6";
            model.ControlGroup = "1";
            model.Covariance = "Autoregressive(1)";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT57()
        {
            string testName = "PTT57";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Subject = "Animal 5";
            model.Treatment = "Day6";
            model.ControlGroup = "2";
            model.Covariance = "Autoregressive(1)";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT58()
        {
            string testName = "PTT58";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal 5";
            model.Treatment = "Day6";
            model.ControlGroup = "2";
            model.Covariance = "Unstructured";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT59()
        {
            string testName = "PTT59";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal 5";
            model.Treatment = "Day6";
            model.ControlGroup = "2";
            model.Covariance = "Unstructured";
            model.Covariates = new string[] { "Cov1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT60()
        {
            string testName = "PTT60";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal 5";
            model.Treatment = "Day6";
            model.ControlGroup = "1";
            model.Covariance = "Unstructured";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT61()
        {
            string testName = "PTT61";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp10";
            model.ResponseTransformation = "None";
            model.Subject = "Animal6";
            model.Treatment = "Day7";
            model.ControlGroup = "2";
            model.Covariance = "Compound Symmetric";
            model.Covariates = new string[] { "Cov9" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT62()
        {
            string testName = "PTT62";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "PVtestResp1";
            model.ResponseTransformation = "None";
            model.Subject = "PVTestAnimal1";
            model.Treatment = "PVTestDay1";
            model.ControlGroup = "4";
            model.Covariance = "Compound Symmetric";
            model.Covariates = new string[] { "PVTestCOV1a" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT63()
        {
            string testName = "PTT63";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "PVtestResp1";
            model.ResponseTransformation = "None";
            model.Subject = "PVTestAnimal1";
            model.Treatment = "PVTestDay1";
            model.ControlGroup = "4";
            model.Covariance = "Compound Symmetric";
            model.Covariates = new string[] { "PVTestCOV1b" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT64()
        {
            string testName = "PTT64";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "CVResp";
            model.ResponseTransformation = "None";
            model.Subject = "CVAnimal1";
            model.Treatment = "CVTime1";
            model.ControlGroup = "4";
            model.Covariance = "Compound Symmetric";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT65()
        {
            string testName = "PTT65";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "CVResp";
            model.ResponseTransformation = "None";
            model.Subject = "CVAnimal2";
            model.Treatment = "CVTime2";
            model.ControlGroup = "4";
            model.Covariance = "Compound Symmetric";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT66()
        {
            string testName = "PTT66";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "1";
            model.Covariance = "Compound Symmetric";
            model.Covariates = new string[] { "Cov1", "Cov7", "Cov8" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT67()
        {
            string testName = "PTT67";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "1";
            model.Covariance = "Compound Symmetric";
            model.Covariates = new string[] { "Cov1", "Cov7" };
            model.CovariateTransformation = "Log10";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT68()
        {
            string testName = "PTT68";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "1";
            model.Covariance = "Unstructured";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.Covariates = new string[] { "Cov1", "Cov7", "Cov8" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT69()
        {
            string testName = "PTT69";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "1";
            model.Covariance = "Unstructured";
            model.Covariates = new string[] { "Cov1", "Cov7" };
            model.CovariateTransformation = "Square Root";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT70()
        {
            string testName = "PTT70";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.ControlGroup = "1";
            model.Covariance = "Autoregressive(1)";
            model.Covariates = new string[] { "Cov1", "Cov7", "Cov8" };
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT71()
        {
            string testName = "PTT71";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Subject = "Animal1";
            model.Treatment = "Day1";
            model.OtherDesignFactors = new string[] { "Block1" };
            model.ControlGroup = "1";
            model.Covariance = "Autoregressive(1)";
            model.Covariates = new string[] { "Cov1", "Cov7" };
            model.CovariateTransformation = "Square Root";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT72()
        {
            string testName = "PTT72";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Subject = "Animal 5";
            model.Treatment = "Day6";
            model.ControlGroup = "2";
            model.Covariance = "Unstructured";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("PairedTTestAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "PairedTTestAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task PTT73()
        {
            string testName = "PTT73";

            //Arrange
            HttpClient client = _factory.CreateClient();

            PairedTTestAnalysisModel model = new PairedTTestAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Paired t-test").Key;
            model.Response = "CVResp";
            model.Subject = "CVAnimal1";
            model.Treatment = "CVTime1";
            model.OtherDesignFactors = new string[] { "Block4" };
            model.ControlGroup = "2";
            model.Covariance = "Unstructured";
            model.ANOVASelected = true;
            model.PRPlotSelected = true;
            model.NormalPlotSelected = true;
            model.LSMeansSelected = true;
            model.AllPairwiseComparisons = true;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/PairedTTestAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("One or more of the factors (Block4) has only one level present in the dataset. Please select another factor.", errors);
            Helpers.SaveOutput("PairedTTestAnalysis", testName, errors);
        }
    }
}