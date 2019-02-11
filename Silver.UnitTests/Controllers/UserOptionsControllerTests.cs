using Microsoft.AspNetCore.Mvc;
using Moq;
using SilveR.Controllers;
using SilveR.Models;
using System.Threading.Tasks;
using Xunit;

namespace Silver.UnitTests.Controllers
{

    public class UserOptionsControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAnActionResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            mock.Setup(x => x.GetUserOptions()).ReturnsAsync(new UserOption());

            UserOptionsController sut = new UserOptionsController(mock.Object);

            //Act
            IActionResult result = await sut.Index();

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            UserOption userOption = (UserOption)viewResult.Model;

            Assert.IsType<UserOption>(userOption);
        }

        [Fact]
        public async Task UpdateUserOptionsPost_ReturnsARedirectResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            mock.Setup(x => x.GetUserOptions()).ReturnsAsync(It.IsAny<UserOption>());

            UserOptionsController sut = new UserOptionsController(mock.Object);

            //Act
            IActionResult result = await sut.UpdateUserOptions(new UserOption(), "save");

            //Assert
            RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public async Task UpdateUserOptionsPost_ResetUserOptions_ReturnsARedirectResult()
        {
            //Arrange
            Mock<ISilveRRepository> mock = new Mock<ISilveRRepository>();
            mock.Setup(x => x.GetUserOptions()).ReturnsAsync(It.IsAny<UserOption>());

            UserOptionsController sut = new UserOptionsController(mock.Object);

            //Act
            IActionResult result = await sut.UpdateUserOptions(new UserOption(), "reset");

            //Assert
            RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(result);
            //Assert.Equal("UserOptions", viewResult.ControllerName);?
            Assert.Equal("Index", viewResult.ActionName);
        }
    }
}