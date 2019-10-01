using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using SilveR.Controllers;
using Xunit;

namespace SilveR.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_SimpleGet_ReturnsAnActionResult()
        {
            //Arrange
            var appSettingsOptions = Options.Create(new AppSettings() { CustomRScriptLocation = "InvalidLocation" });

            HomeController sut = new HomeController(appSettingsOptions);

            //Act
            IActionResult result = sut.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}