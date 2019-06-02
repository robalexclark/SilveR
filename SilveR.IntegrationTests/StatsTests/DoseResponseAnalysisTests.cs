//using ControlledForms.IntegrationTests;
//using SilveR.StatsModels;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Xunit;

//namespace SilveR.IntegrationTests
//{
//    public class DoseResponseAnalysisTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
//    {
//        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

//        public DoseResponseAnalysisTests(SilveRTestWebApplicationFactory<Startup> factory)
//        {
//            _factory = factory;
//        }

//        //[Fact]
//        //public async Task DR1()
//        //{
//        //    string testName = "DR1";

//        //    //Arrange
//        //    HttpClient client = _factory.CreateClient();

//        //    DoseResponseAndNonLinearRegesssionAnalysisModel model = new DoseResponseAndNonLinearRegesssionAnalysisModel();
//        //    model.DatasetID = _factory.SheetNames.Single(x => x.Value == "Dose Response").Key;
//        //    model.Response =  "Resp1";
//        //    model.ResponseTransformation = "None";

//        //    //Act
//        //    HttpResponseMessage response = await client.PostAsync("Analyses/DoseResponseAndNonLinearRegesssionAnalysis", new FormUrlEncodedContent(model.ToKeyValue()));
//        //    IEnumerable<string> errors = await Helpers.ExtractErrors(response);

//        //    //Assert
//        //    Assert.Contains("The response selected (Resp 5) contains non-numerical data. Please amend the dataset prior to running the analysis.", errors);
//        //    Helpers.SaveOutput("DoseResponseAndNonLinearRegesssionAnalysis", testName, errors);
//        //}
//    }
//}