using ControlledForms.IntegrationTests;
using ExcelDataReader;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SilveR.Controllers;
using SilveR.Helpers;
using SilveR.Models;
using SilveR.Services;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.IntegrationTests
{
    public class SummaryStatisticsTests : IClassFixture<SilveRTestWebApplicationFactory<Startup>>
    {
        private readonly SilveRTestWebApplicationFactory<Startup> _factory;

        private readonly Dictionary<int, string> sheetNames = new Dictionary<int, string>();

        public SummaryStatisticsTests(SilveRTestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;

            LoadDatasets();
        }

        private void LoadDatasets()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open("_test dataset.xlsx", FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    int counter = 1;
                    foreach (DataTable dataTable in result.Tables)
                    {
                        sheetNames.Add(counter, dataTable.TableName);
                        counter++;
                    }
                }
            }

            var client = CreateClient();
            foreach (string sheetName in sheetNames.Values)
            {
                HttpContent dataUploadContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("sheetSelection", sheetName),
                    new KeyValuePair<string, string>("fileName", "_test dataset.xlsx")
                });

                client.PostAsync("Data/SheetSelector", dataUploadContent);
            }
        }

        private HttpClient CreateClient()
        {
            HttpClient client = _factory.CreateClient();
            return client;
        }


        [Fact]
        public async Task SS1()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp1" };

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("The response variable selected (Resp1) contains non-numerical data. Please amend the dataset prior to running the analysis.", errors.Single());
        }

        [Fact]
        public async Task SS2()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat1";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("The categorisation factor selected (Cat1) contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.", errors.Single());
        }

        [Fact]
        public async Task SS3()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat2";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("There is no replication in one or more of the levels of the Response (Cat2). Please select another factor.", errors.Single());
        }

        [Fact]
        public async Task SS4()
        {
            //Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat3";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("There are no observations recorded on the levels of the Response (Cat3). Please amend the dataset prior to running the analysis.", errors.Single());
        }

        [Fact]
        public async Task SS5()
        {
            //Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp3" };
            model.Transformation = "Log10";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("You have Log10 transformed the Resp3 variable. Unfortunately some of the Resp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", errors.Single());
        }

        [Fact]
        public async Task SS6()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp3" };
            model.Transformation = "Square Root";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("You have Square Root transformed the Resp3 variable. Unfortunately some of the Resp3 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings.Single());
        }

        [Fact]
        public async Task SS7()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp3" };
            model.Transformation = "Loge";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("You have Loge transformed the Resp3 variable. Unfortunately some of the Resp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings.Single());
        }

        [Fact]
        public async Task SS8()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp3" };
            model.Transformation = "ArcSine";
            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("You have ArcSine transformed the Resp3 variable. Unfortunately some of the Resp3 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings.Single());
        }

        [Fact]
        public async Task SS9()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp11" };
            model.Transformation = "ArcSine";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("You have ArcSine transformed the Resp11 variable. Unfortunately some of the Resp11 values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.", warnings.Single());
        }

        [Fact]
        public async Task SS10()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp1", "Resp 2" };

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("The response variable selected (Resp1) contains non-numerical data. Please amend the dataset prior to running the analysis.", errors.Single());
        }

        [Fact]
        public async Task SS11()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp 2", "Resp 5" };
            model.FirstCatFactor = "Cat1";
            model.SecondCatFactor = "Cat4";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("The categorisation factor selected (Cat1) contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.", errors.Single());
        }

        [Fact]
        public async Task SS12()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat2";
            model.SecondCatFactor = "Cat4";
            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("There is no replication in one or more of the levels of the Response (Cat2). Please select another factor.", errors.Single());
        }

        [Fact]
        public async Task SS13()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp 2" };
            model.FirstCatFactor = "Cat3";
            model.SecondCatFactor = "Cat4";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("There are no observations recorded on the levels of the Response (Cat3). Please amend the dataset prior to running the analysis.", errors.Single());
        }

        [Fact]
        public async Task SS14()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp 2" };
            model.Significance = 0.95m;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("You have selected a confidence limit that is less than 1. Note that this value should be entered as a percentage and not a fraction.", errors.Single());
        }

        [Fact]
        public async Task SS15()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp3", "Resp 2" };
            model.Transformation = "Log10";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("You have Log10 transformed the Resp3 variable. Unfortunately some of the Resp3 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings.Single());
        }

        [Fact]
        public async Task SS16()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp3", "Resp 2" };
            model.Transformation = "Square Root";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("You have Square Root transformed the Resp3 variable. Unfortunately some of the Resp3 values are negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings.Single());
        }

        [Fact]
        public async Task SS17()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp4", "Resp 2" };
            model.Transformation = "Loge";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> warnings = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("You have Loge transformed the Resp4 variable. Unfortunately some of the Resp4 values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.", warnings.Single());
        }

        [Fact]
        public async Task SS18()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp6", "Resp7" };
            model.FirstCatFactor = "Cat4";

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("There are no observations recorded on the levels of the Response (Cat4). Please amend the dataset prior to running the analysis.", errors.Single());
        }

        [Fact]
        public async Task SS19()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp1" };
            model.Mean = false;
            model.N = false;
            model.StandardDeviation = false;
            model.Variance = false;
            model.StandardErrorOfMean = false;
            model.MinAndMax = false;
            model.MedianAndQuartiles = false;
            model.CoefficientOfVariation = false;
            model.NormalProbabilityPlot = false;
            model.CoefficientOfVariation = false;
            model.ByCategoriesAndOverall = false;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("You have not selected anything to output!", errors.Single());
        }

        [Fact]
        public async Task SS20()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp6", "Resp7" };
            model.FirstCatFactor = "Cat4";
            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
            Assert.Equal("There are no observations recorded on the levels of the Response (Cat4). Please amend the dataset prior to running the analysis.", errors.Single());
        }

        //SS21 - Not required

        [Fact]
        public async Task SS22()
        {
            // Arrange
            HttpClient client = CreateClient();

            SummaryStatisticsModel model = new SummaryStatisticsModel();
            model.DatasetID = sheetNames.Single(x => x.Value == "summary").Key;
            model.Responses = new string[] { "Resp 2" };
            model.Transformation = "Loge";
            model.Mean = true;
            model.N = true;
            model.StandardDeviation = true;
            model.Variance = true;
            model.StandardErrorOfMean = true;
            model.MinAndMax = true;
            model.MedianAndQuartiles = true;
            model.CoefficientOfVariation = true;
            model.NormalProbabilityPlot = true;
            model.CoefficientOfVariation = true;
            model.ByCategoriesAndOverall = true;

            var content = new FormUrlEncodedContent(model.ToKeyValue());

            //Act
            HttpResponseMessage response = await client.PostAsync("Analyses/SummaryStatistics", content);
            IEnumerable<string> errors = await Helpers.ExtractErrors(response);

            //Assert
        }
    }
}