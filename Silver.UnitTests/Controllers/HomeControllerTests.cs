using Microsoft.AspNetCore.Mvc;
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
            HomeController sut = new HomeController();

            //Act
            IActionResult result = sut.Index();

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
        }
    }
}