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
    public class DoseResponseAndNonLinearRegressionAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        public DoseResponseAndNonLinearRegressionAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task DR1()
        {
            string testName = "DR1";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp1";
            model.Dose = "Resp1";
            model.ResponseTransformation = "None";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp1) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR2()
        {
            string testName = "DR2";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp1";
            model.ResponseTransformation = "None";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Dose is a required variable.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR3()
        {
            string testName = "DR3";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Dose = "Resp1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response is a required variable.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        //[Fact]
        //public async Task DR4()
        //{
        //    string testName = "DR4";

        //    //Arrange
        //    HttpClient client = _factory.CreateClient();

        //    DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
        //    model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
        //    model.Response = "Resp1";
        //    model.Dose = "Dose1";
        //    //model.Offset = "n";

        //    //Act
        //    //Assert
        //    Assert.True(true);// test will always pass because numeric value is forced at UI
        //}

        [Fact]
        public async Task DR5()
        {
            string testName = "DR5";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp1";
            model.Dose = "Dose1";
            model.QCResponse = "Resp1";
            model.QCDose = "QC Resp1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("Response (Resp1) has been selected in more than one input category, please change your input options.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR6()
        {
            string testName = "DR6";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp2";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp2) contains only one value, please choose another response.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR7()
        {
            string testName = "DR7";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.Dose = "Dose2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Dose (Dose2) contains missing data where there are observations present in the Response. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR8()
        {
            string testName = "DR8";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.Dose = "Dose1";
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose3";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The QC Dose (QCDose3) contains missing data where there are observations present in the Response. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR9()
        {
            string testName = "DR9";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp3";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Response (Resp3) contains non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR10()
        {
            string testName = "DR10";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.Dose = "Dose3";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Dose (Dose3) contains non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR11()
        {
            string testName = "DR11";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.Dose = "Dose1";
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The QC Dose (QCDose4) contains non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR12()
        {
            string testName = "DR12";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.Dose = "Dose1";
            model.QCResponse = "QCResp2";
            model.QCDose = "QC Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The QC Response (QCResp2) contains non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR13()
        {
            string testName = "DR13";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.Dose = "Dose1";
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample2";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The Sample (Sample2) contains non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR14()
        {
            string testName = "DR14";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp4";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp4) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR15()
        {
            string testName = "DR15";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.Dose = "Dose4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("As you only have measured responses at four doses you cannot fit the selected dose response curve. You will need to fix at least one of the parameters.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR16()
        {
            string testName = "DR16";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.Dose = "Dose5";
            model.MinStartValue = 0;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("As you only have measured responses at three doses you cannot fit the selected dose response curve. You will need to fix at least two of the parameters.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR17()
        {
            string testName = "DR17";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.Dose = "Dose6";
            model.MinStartValue = 0;
            model.MaxStartValue = 100;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("As you only have measured responses at two doses you cannot fit the selected dose response curve. You will need to fix at least three of the parameters.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR18()
        {
            string testName = "DR18";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.Dose = "Dose7";
            model.MinStartValue = 0;
            model.MaxStartValue = 100;
            model.SlopeStartValue = 1;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("As you only have measured responses at one dose you cannot fit the selected dose response curve.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        [Fact]
        public async Task DR19()
        {
            string testName = "DR19";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "Log10";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp5 variable. Unfortunately some of the Resp5 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR20()
        {
            string testName = "DR20";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "Loge";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp5 variable. Unfortunately some of the Resp5 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR21()
        {
            string testName = "DR21";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "Log10";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Resp6 variable. Unfortunately some of the Resp6 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR22()
        {
            string testName = "DR22";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp6";
            model.ResponseTransformation = "Loge";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Resp6 variable. Unfortunately some of the Resp6 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR23()
        {
            string testName = "DR23";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Dose = "Dose1";
            model.QCResponse = "QCResp3";
            model.QCDose = "QCDose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the QCResp3 variable. Unfortunately some of the QCResp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR24()
        {
            string testName = "DR24";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Dose = "Dose1";
            model.QCResponse = "QCResp4";
            model.QCDose = "QCDose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the QCResp4 variable. Unfortunately some of the QCResp4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR25()
        {
            string testName = "DR25";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Dose = "Dose1";
            model.SamplesResponse = "Sample3";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Loge transformed the Sample3 variable. Unfortunately some of the Sample3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR26()
        {
            string testName = "DR26";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Dose = "Dose1";
            model.SamplesResponse = "Sample4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Log10 transformed the Sample4 variable. Unfortunately some of the Sample4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR27()
        {
            string testName = "DR27";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "Square Root";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Resp5 variable. Unfortunately some of the Resp5 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR28()
        {
            string testName = "DR28";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Dose = "Dose1";
            model.QCResponse = "QCResp3";
            model.QCDose = "QCDose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the QCResp3 variable. Unfortunately some of the QCResp3 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR29()
        {
            string testName = "DR29";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Dose = "Dose1";
            model.SamplesResponse = "Sample4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have Square Root transformed the Sample4 variable. Unfortunately some of the Sample4 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR30()
        {
            string testName = "DR30";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp5";
            model.ResponseTransformation = "ArcSine";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp5 variable. Unfortunately some of the Resp5 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }


        [Fact]
        public async Task DR31()
        {
            string testName = "DR31";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp7 variable. Unfortunately some of the Resp7 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR32()
        {
            string testName = "DR32";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Dose = "Dose1";
            model.QCResponse = "QCResp3";
            model.QCDose = "QCDose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp7 variable. Unfortunately some of the Resp7 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR33()
        {
            string testName = "DR33";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Dose = "Dose1";
            model.QCResponse = "QCResp5";
            model.QCDose = "QCDose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the QCResp5 variable. Unfortunately some of the QCResp5 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR34()
        {
            string testName = "DR34";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Dose = "Dose1";
            model.SamplesResponse = "Sample4";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp7 variable. Unfortunately some of the Resp7 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR35()
        {
            string testName = "DR35";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp7";
            model.ResponseTransformation = "ArcSine";
            model.Dose = "Dose1";
            model.SamplesResponse = "Sample5";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have ArcSine transformed the Resp7 variable. Unfortunately some of the Resp7 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR36()
        {
            string testName = "DR36";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp8";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("The Response (Resp8) contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR37()
        {
            string testName = "DR37";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.MinStartValue = 10;
            model.MaxStartValue = 1;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The max start value is greater than the min start value.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }

        //[Fact]
        //public async Task DR38()
        //{
        //    string testName = "DR38";

        //    //Arrange
        //    HttpClient client = _factory.CreateClient();

        //    DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
        //    model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
        //    model.Response = "Resp 1";
        //    model.ResponseTransformation = "None";
        //    model.Dose = "Dose1";

        //    //Act
        //    //Assert
        //    Assert.True(true);// test will always pass because numeric value is forced at UI
        //}

        [Fact]
        public async Task DR39()
        {
            string testName = "DR39";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.MinCoeff = 1;
            model.MinStartValue = 1;

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> warnings = await Helpers.ExtractWarnings(response);

            //Assert
            Assert.Contains("You have defined a start value for a parameter that has been fixed. The start value is ignored in the analysis.", warnings);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, warnings);
        }

        [Fact]
        public async Task DR40()
        {
            string testName = "DR40";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR41()
        {
            string testName = "DR41";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.Offset = 0.000001m;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";


            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR42()
        {
            string testName = "DR42";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR43()
        {
            string testName = "DR43";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.Offset = 0.0001m;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR44()
        {
            string testName = "DR44";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR45()
        {
            string testName = "DR45";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Log10";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR46()
        {
            string testName = "DR46";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR47()
        {
            string testName = "DR47";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Loge";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR48()
        {
            string testName = "DR48";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR49()
        {
            string testName = "DR49";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "Square Root";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR50()
        {
            string testName = "DR50";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR51()
        {
            string testName = "DR51";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "ArcSine";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR52()
        {
            string testName = "DR52";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";
            model.MinCoeff = 0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR53()
        {
            string testName = "DR53";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";
            model.MaxCoeff = 1;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR54()
        {
            string testName = "DR54";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";
            model.SlopeCoeff = 1;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR55()
        {
            string testName = "DR55";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";
            model.EDICCoeff = 0.03m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR56()
        {
            string testName = "DR56";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";
            model.MinStartValue = 10;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR57()
        {
            string testName = "DR57";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";
            model.MaxStartValue = 1;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR58()
        {
            string testName = "DR58";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";
            model.SlopeStartValue = 1;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR59()
        {
            string testName = "DR59";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp 1";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose1";
            model.SamplesResponse = "Sample 1";
            model.EDICStartValue = 0.03m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR60()
        {
            string testName = "DR60";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR61()
        {
            string testName = "DR61";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.Offset = 0.000001m;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR62()
        {
            string testName = "DR62";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR63()
        {
            string testName = "DR63";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.Offset = 0.0001m;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR64()
        {
            string testName = "DR64";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "Log10";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR65()
        {
            string testName = "DR65";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "Log10";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR66()
        {
            string testName = "DR66";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "Loge";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR67()
        {
            string testName = "DR67";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "Loge";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR68()
        {
            string testName = "DR68";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "Square Root";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR69()
        {
            string testName = "DR69";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "Square Root";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR70()
        {
            string testName = "DR70";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "ArcSine";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR71()
        {
            string testName = "DR71";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "ArcSine";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR72()
        {
            string testName = "DR72";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";
            model.MinCoeff = 0;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR73()
        {
            string testName = "DR73";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";
            model.MaxCoeff = 1;
            model.SlopeStartValue = -1;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR74()
        {
            string testName = "DR74";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";
            model.SlopeCoeff = 1;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR75()
        {
            string testName = "DR75";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";
            model.EDICCoeff = 0.03m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR76()
        {
            string testName = "DR76";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";
            model.MinStartValue = 10;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR77()
        {
            string testName = "DR77";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";
            model.MaxStartValue = 1;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR78()
        {
            string testName = "DR78";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Loge;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";
            model.SlopeStartValue = -1;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR79()
        {
            string testName = "DR79";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.Response = "Resp9";
            model.ResponseTransformation = "None";
            model.Dose = "Dose1";
            model.DoseScale = DoseResponseAndNonLinearRegressionAnalysisModel.DoseScaleOption.Log10;
            model.QCResponse = "QC Resp1";
            model.QCDose = "QCDose2";
            model.SamplesResponse = "Sample 1";
            model.EDICStartValue = 0.03m;

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR80()
        {
            string testName = "DR80";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.AnalysisType = DoseResponseAndNonLinearRegressionAnalysisModel.AnalysisOption.Equation;
            model.Equation = "A+B*x+C*x*x";
            model.StartValues = "A=1,B=1,C=1";
            model.EquationYAxis = "Resp10";
            model.EquationXAxis = "Dose8";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR81()
        {
            string testName = "DR81";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.AnalysisType = DoseResponseAndNonLinearRegressionAnalysisModel.AnalysisOption.Equation;
            model.Equation = "A+B*x+C*x*x";
            model.StartValues = "A=1,B=1, C=1";
            model.EquationYAxis = "Resp10";
            model.EquationXAxis = "Dose8";

            //Act
            StatsOutput statsOutput = await Helpers.SubmitAnalysis(client, "DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            Helpers.SaveTestOutput("DoseResponseAndNonLinearRegressionAnalysis", model, testName, statsOutput);

            //Assert
            string expectedHtml = File.ReadAllText(Path.Combine("ExpectedResults", "DoseResponseAndNonLinearRegressionAnalysis", testName + ".html"));
            Assert.Equal(Helpers.RemoveAllImageNodes(expectedHtml), Helpers.RemoveAllImageNodes(statsOutput.HtmlResults));
        }

        [Fact]
        public async Task DR82()
        {
            string testName = "DR82";

            //Arrange
            HttpClient client = _factory.CreateClient();

            DoseResponseAndNonLinearRegressionAnalysisModel model = new DoseResponseAndNonLinearRegressionAnalysisModel();
            model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
            model.AnalysisType = DoseResponseAndNonLinearRegressionAnalysisModel.AnalysisOption.Equation;
            model.Equation = "A+B*X+C*X*X";
            model.StartValues = "A=1,B=1,C=1";
            model.EquationYAxis = "Resp10";
            model.EquationXAxis = "Dose8";

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegressionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Contains("The formula should be of the form f=f(x) with x lower case.", errors);
            Helpers.SaveOutput("DoseResponseAndNonLinearRegressionAnalysis", testName, errors);
        }
    }
}