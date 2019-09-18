using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SilveR.Controllers;
using SilveR.Models;
using SilveR.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SilveR.UnitTests.Controllers
{
    
    public class DataControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            mock.Setup(x => x.HasDatasets()).ReturnsAsync(It.IsAny<bool>());

            var tempDataMock = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            DataController sut = new DataController(mock.Object);
            sut.TempData = tempDataMock;

            //Act
            IActionResult result = await sut.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task GetDatasets_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            mock.Setup(x => x.GetDatasetViewModels()).ReturnsAsync(GetDatasets());
            DataController sut = new DataController(mock.Object);

            //Act
            IActionResult result = await sut.GetDatasets();

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);

            IEnumerable<DatasetViewModel> datasetViewModel = (IEnumerable<DatasetViewModel>)jsonResult.Value;
            DatasetViewModel dataset = datasetViewModel.First();
            Assert.Equal("_test dataset.xlsx [doseresponse] v1", dataset.DatasetNameVersion);
        }

        [Fact]
        public void DataUploader_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            DataController sut = new DataController(mock.Object);

            //Act
            IActionResult result = sut.DataUploader();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task DataUploaderPost_NoFiles_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            DataController sut = new DataController(mock.Object);

            //Act
            IActionResult result = await sut.DataUploader(new List<IFormFile>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("File failed to load, please try again", viewResult.ViewData["ErrorMessage"]);
        }

        [Fact]
        public async Task DataUploaderPost_EmptyFile_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            DataController sut = new DataController(mock.Object);

            //Arrange
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("");
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns("Test.txt");
            fileMock.Setup(_ => _.Length).Returns(0);

            //Act
            IActionResult result = await sut.DataUploader(new List<IFormFile> { fileMock.Object });

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("File failed to load, please try again", viewResult.ViewData["ErrorMessage"]);
        }

        [Fact]
        public void DataUploaderPost_ReturnsAnActionResult()
        {
            //todo - refactor dataupload process to allow mocked file?
        }

        [Fact]
        public void SheetSelector_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();

            var tempDataMock = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            tempDataMock.Add("TableNames", new List<string> { "SheetName1", "SheetName 2", "SheetName 3" });

            DataController sut = new DataController(mock.Object);
            sut.TempData = tempDataMock;

            //Act
            IActionResult result = sut.SheetSelector("TheFile.csv");

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("TheFile.csv", viewResult.ViewData["FileName"]);
            Assert.Equal(new List<string> { "SheetName1", "SheetName 2", "SheetName 3" }, viewResult.ViewData["SheetList"]);
        }

        [Fact]
        public void SheetSelector_Post_ReturnsAnActionResult()
        {
            //todo - refactor dataupload process to allow mocked file?
        }

        [Fact]
        public async Task Destroy_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            mock.Setup(x => x.DeleteDataset(It.IsAny<int>())).Returns(Task.CompletedTask);

            DataController sut = new DataController(mock.Object);

            //Act
            IActionResult result = await sut.Destroy(It.IsAny<int>());

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            Assert.True((bool)jsonResult.Value);
        }

        [Fact]
        public async Task ViewDataTable_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            mock.Setup(x => x.GetDatasetByID(It.IsAny<int>())).ReturnsAsync(GetDataset());

            DataController sut = new DataController(mock.Object);

            //Act
            IActionResult result = await sut.ViewDataTable(It.IsAny<int>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Sheet sheet = (Sheet)viewResult.Model;
            Assert.Equal("6", sheet.Name);
            Assert.Equal(13, sheet.Rows.Count);
        }

        [Fact]
        public async Task UpdateDataset_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            mock.Setup(x => x.UpdateDataset(It.IsAny<Dataset>())).Returns(Task.CompletedTask);

            DataController sut = new DataController(mock.Object);

            //Act
            IActionResult result = await sut.UpdateDataset(GetSheets());

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            Assert.True((bool)jsonResult.Value);
        }



        private Dataset GetDataset()
        {
            var dataset = new Dataset
            {
                DatasetID = 6,
                DatasetName = "_test dataset.xlsx [unpairedttest]",
                DateUpdated = new DateTime(2018, 11, 16, 9, 14, 35),
                TheData = "SilveRSelected,Resp 1,Resp2,Resp 3,Resp4,Resp 5,Resp 6,Resp 7,Resp8,Resp:9,Resp-10,Resp^11,Treat1,Treat2,Treat3,Treat4,Treat(5,Treat£6,Treat:7,Treat}8,PVTestresponse1,PVTestresponse2,PVTestgroup\r\nTrue,65,65,65,x,,-2,0,-2,65,65,0.1,A,A,1,A,1,A,A,A,1,1,1\r\nTrue,32,,32,32,32,32,32,0.1,32,32,0.1,A,A,1,A,1,A,A,A,2,2,1\r\nTrue,543,,543,543,543,543,543,0.2,543,543,0.2,A,A,1,A,1,A,A,A,3,3,1\r\nTrue,675,,675,675,675,675,675,0.1,675,675,0.1,A,A,1,B,1,A,A,A,4,4,1\r\nTrue,876,,876,876,876,876,876,0.2,876,876,0.2,A,A,1,B,1,A,A,A,11,10,2\r\nTrue,54,,54,54,54,54,54,0.3,54,54,0.3,A,A,1,B,1,A,A,A,12,11,2\r\nTrue,432,,,432,432,432,432,0.45,432,432,0.45,B,B,2,C,2,B,B,B,13,12,2\r\nTrue,564,,,564,564,564,564,0.2,564,564,0.2,B,B,2,C,2,B,B,,14,13,2\r\nTrue,76,,,76,76,76,76,0.14,76,76,0.14,B,B,2,C,2,B,B,,,,\r\nTrue,54,,,54,54,54,54,0.2,54,54,0.2,B,B,2,D,3,B,B,,,,\r\nTrue,32,,,32,32,32,32,0.1,32,32,0.1,B,B,2,D,3,B,B,,,,\r\nTrue,234,,,234,234,234,234,0.4,234,234,0.4,B,,2,D,3,B,B,,,,",
                VersionNo = 1
            };

            return dataset;
        }

        private IList<DatasetViewModel> GetDatasets()
        {
            var datasets = new List<SilveR.ViewModels.DatasetViewModel>
            {
                new SilveR.ViewModels.DatasetViewModel
                {
                    DatasetID = 1,
                    DatasetName = "_test dataset.xlsx [doseresponse]",
                    DateUpdated = new DateTime(2018, 11, 11, 10, 58, 48),
                    VersionNo = 1
                },
                new SilveR.ViewModels.DatasetViewModel
                {
                    DatasetID = 2,
                    DatasetName = "_test dataset.xlsx [nonpara]",
                    DateUpdated = new DateTime(2018, 11, 11, 11, 12, 16),
                    VersionNo = 1
                },
                new SilveR.ViewModels.DatasetViewModel
                {
                    DatasetID = 3,
                    DatasetName = "_test dataset.xlsx [singlemeasures]",
                    DateUpdated = new DateTime(2018, 11, 11, 11, 14, 2),
                    VersionNo = 1
                },
                new SilveR.ViewModels.DatasetViewModel
                {
                    DatasetID = 4,
                    DatasetName = "_test dataset.xlsx [regression]",
                    DateUpdated = new DateTime(2018, 11, 12, 11, 53, 6),
                    VersionNo = 1
                }
            };

            return datasets;
        }

        private Sheets GetSheets()
        {
            var sheets = new SilveR.ViewModels.Sheets
            {
            };

            sheets.sheets.Add(new SilveR.ViewModels.Sheet
            {
                Name = "6"
            });

            return sheets;
        }
    }
}