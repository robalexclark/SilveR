using Microsoft.AspNetCore.Mvc;
using Moq;
using SilveR.Controllers;
using SilveR.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Silver.UnitTests.Controllers
{
    public class ValuesControllerTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetLevels_ReturnsAnActionResult(bool includeNull)
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            mock.Setup(x => x.GetDatasetByID(It.IsAny<int>())).ReturnsAsync(GetDataset());

            ValuesController sut = new ValuesController(mock.Object);

            //Act
            IActionResult result = await sut.GetLevels("Treat1", 6, includeNull);

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);

            if (includeNull)
                Assert.Equal(new List<string> { String.Empty, "A", "B" }, jsonResult.Value);
            else
                Assert.Equal(new List<string> { "A", "B" }, jsonResult.Value);
        }

        [Fact]
        public void GetSMPAInteractions_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            ValuesController sut = new ValuesController(mock.Object);

            //Act
            IActionResult result = sut.GetSMPAInteractions(new List<string> { "Treat1", "Treat2", "Treat3" });

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);

            Assert.Equal(new List<string> { "Treat1 * Treat2", "Treat1 * Treat3", "Treat2 * Treat3", "Treat1 * Treat2 * Treat3" }, jsonResult.Value);
        }


        [Fact]
        public void GetSMPASelectedEffectsLists_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            ValuesController sut = new ValuesController(mock.Object);

            //Act
            IActionResult result = sut.GetSMPASelectedEffectsList(new List<string> { "Treat1", "Treat2", "Treat3" });

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);

            Assert.Equal(new List<string> { "Treat1","Treat2","Treat3", "Treat1 * Treat2", "Treat1 * Treat3", "Treat2 * Treat3", "Treat1 * Treat2 * Treat3" }, jsonResult.Value);
        }

        [Fact]
        public void GetRMPAInteractions_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            ValuesController sut = new ValuesController(mock.Object);

            //Act
            IActionResult result = sut.GetRMPAInteractions(new List<string> { "Treat1", "Treat2", "Treat3" });

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);

            Assert.Equal(new List<string> { "Treat1 * Treat2", "Treat1 * Treat3", "Treat2 * Treat3", "Treat1 * Treat2 * Treat3" }, jsonResult.Value);
        }


        [Fact]
        public void GetRMPASelectedEffectsLists_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            ValuesController sut = new ValuesController(mock.Object);

            //Act
            IActionResult result = sut.GetRMPASelectedEffectsList(new List<string> { "Treat1", "Treat2", "Treat3" }, "Day1");

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);

            Assert.Equal(new List<string> { "Treat1 * Day1", "Treat2 * Day1", "Treat3 * Day1", "Treat1 * Treat2 * Day1", "Treat1 * Treat3 * Day1", "Treat2 * Treat3 * Day1", "Treat1 * Treat2 * Treat3 * Day1" }, jsonResult.Value);
        }

        [Fact]
        public void GetIncompleteFactorialInteractions_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            ValuesController sut = new ValuesController(mock.Object);

            //Act
            IActionResult result = sut.GetIncompleteFactorialInteractions(new List<string> { "Treat1", "Treat2", "Treat3" });

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);

            Assert.Equal(new List<string> { "Treat1 * Treat2", "Treat1 * Treat3", "Treat2 * Treat3", "Treat1 * Treat2 * Treat3" }, jsonResult.Value);
        }


        [Fact]
        public void GetIncompleteFactorialSelectedEffectsLists_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            ValuesController sut = new ValuesController(mock.Object);

            //Act
            IActionResult result = sut.GetIncompleteFactorialSelectedEffectsList(new List<string> { "Treat1", "Treat2", "Treat3" });

            //Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);

            Assert.Equal(new List<string> { "Treat1 * Treat2 * Treat3" }, jsonResult.Value);
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
    }
}