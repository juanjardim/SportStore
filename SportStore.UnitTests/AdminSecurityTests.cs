using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportStore.WebUI.Controllers;
using SportStore.WebUI.Infrastructure.Abstract;
using SportStore.WebUI.Models;

namespace SportStore.UnitTests
{
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void Can_Login_With_Valid_Credentials()
        {
            //Arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "secret")).Returns(true);

            LoginViewModel model = new LoginViewModel
            {
                UserName = "admin",
                Password = "secret"
            };

            AccountController target = new AccountController(mock.Object);

            //Act
            ActionResult result = target.Login(model, "/MyURL");

            //Assert
            Assert.IsInstanceOfType(result, typeof (RedirectResult));
            Assert.AreEqual("/MyURL", ((RedirectResult) result).Url);
        }

        [TestMethod]
        public void Cannot_Login_With_Invalid_Credentials()
        {
            //Arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("Admin", "Secret")).Returns(true);
            LoginViewModel model = new LoginViewModel
            {
                UserName = "He",
                Password = "123"
            };

            AccountController target = new AccountController(mock.Object);

            //Act
            ActionResult result = target.Login(model, "/MyURL");

            //Assert
            Assert.IsInstanceOfType(result, typeof (ViewResult));
            Assert.IsFalse(((ViewResult) result).ViewData.ModelState.IsValid);
        }
    }
}
