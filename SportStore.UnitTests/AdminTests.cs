using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using SportStore.WebUI.Controllers;

namespace SportStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
            });

            AdminController target = new AdminController(mock.Object);

            //Action
            Product[] result = ((IEnumerable<Product>) target.Index().ViewData.Model).ToArray();

            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
            });

            AdminController target = new AdminController(mock.Object);

            Product result1 = target.Edit(1).ViewData.Model as Product;
            Product result2 = target.Edit(2).ViewData.Model as Product;
            Product result3 = target.Edit(3).ViewData.Model as Product;

            Assert.AreEqual(1, result1.ProductID);
            Assert.AreEqual(2, result2.ProductID);
            Assert.AreEqual(3, result3.ProductID);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product {Name = "Test"};

            //Action
            ActionResult result = target.Edit(product);

            //Assert
            mock.Verify(m => m.SaveProduct(product));
            //Check the method result type
            Assert.IsNotInstanceOfType(result, typeof (ViewResult));

        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product {Name = "Test"};
            target.ModelState.AddModelError("error", "error");

            //Act
            ActionResult result = target.Edit(product);

            //Assert
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            Assert.IsInstanceOfType(result, typeof (ViewResult));
        }

        public void Can_Create_Product()
        {
            //Arrange 
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);

            //Act
            var result = target.Create().ViewData.Model;

            //Assert
            Assert.IsInstanceOfType(result, typeof (Product));
        }

        public void Can_Delete_Product()
        {
            //Arrange
            Product prod = new Product {ProductID = 2, Name = "P2"};
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                prod,
                new Product {ProductID = 3, Name = "P3"},
            });

            AdminController target = new AdminController(mock.Object);

            //Act
            target.Delete(prod.ProductID);

            //Assert
            mock.Verify(m => m.DeleteProduct(prod.ProductID));

        }
    }
}
