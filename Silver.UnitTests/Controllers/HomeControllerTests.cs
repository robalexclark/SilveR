using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SilveR.Controllers;
using Xunit;

namespace Silver.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_SimpleGet_ReturnsAnActionResult()
        {
            //Arrange
            Mock<IHostingEnvironment> mock = new Mock<IHostingEnvironment>();
            HomeController sut = new HomeController(mock.Object);

            //Act
            IActionResult result = sut.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}