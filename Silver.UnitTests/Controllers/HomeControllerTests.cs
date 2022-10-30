using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SilveR.Controllers;
using System.Globalization;
using Xunit;

namespace SilveR.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_SimpleGet_ReturnsAnActionResult()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var appSettingsOptions = Options.Create(new AppSettings() { CustomRScriptLocation = "InvalidLocation" });

            HomeController sut = new HomeController(appSettingsOptions);

            //Act
            IActionResult result = sut.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        //[Fact]
        //public void Index_CheckRInstalled_ReturnsNoErrorMessage()
        //{
        //    //Arrange
        //    System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        //    var appSettingsOptions = Options.Create(new AppSettings() { CustomRScriptLocation = null });

        //    HomeController sut = new HomeController(appSettingsOptions);

        //    //Act
        //    IActionResult actionResult = sut.Index();
        //    string result = sut.ViewBag.ErrorMessage;

        //    //Assert
        //    Assert.Null(result);
        //}

        [Fact]
        public void Index_CheckRInstalled_ReturnsErrorMessage()
        {
            //Arrange
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var appSettingsOptions = Options.Create(new AppSettings() { CustomRScriptLocation = "InvalidLocation" });

            HomeController sut = new HomeController(appSettingsOptions);

            //Act
            IActionResult actionResult = sut.Index();
            string result = sut.ViewBag.ErrorMessage;

            //Assert
            Assert.NotNull(result);
        }

    }
}