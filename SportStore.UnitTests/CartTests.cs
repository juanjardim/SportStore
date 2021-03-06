﻿using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using SportStore.WebUI.Controllers;
using SportStore.WebUI.Models;

namespace SportStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //Arrange
            var p1 = new Product {ProductID = 1, Name = "P1"};
            var p2 = new Product {ProductID = 2, Name = "P2"};

            var target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            //Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //Arrange
            var p1 = new Product { ProductID = 1, Name = "P1" };
            var p2 = new Product { ProductID = 2, Name = "P2" };
            var target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);

            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);

        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            // Arrange
            var p1 = new Product { ProductID = 1, Name = "P1" };
            var p2 = new Product { ProductID = 2, Name = "P2" };
            var p3 = new Product { ProductID = 3, Name = "P3" };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            //Act 
            target.RemoveLine(p2);

            //Assert
            Assert.AreEqual(target.Lines.Count(c => c.Product == p2), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Can_Clean_Contents()
        {
           // Arrange
            Product p1 = new Product {ProductID = 1, Name = "P1", Price = 100M};
            Product p2 = new Product {ProductID = 2, Name = "P2", Price = 50M};

            var target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            //Act
            target.Clear();

            //Assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //Arrange
            Mock<IOrderProcessor> orderProcessorMock = new Mock<IOrderProcessor>();

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"}
            }.AsQueryable());


            Cart cart = new Cart();

            CartController target = new CartController(mock.Object, orderProcessorMock.Object);

            //Act
            target.AddToCart(cart, 1, null);

            //Assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            //Arrange
            Mock<IOrderProcessor> orderProcessorMock = new Mock<IOrderProcessor>();
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"}
            }.AsQueryable());

            Cart cart = new Cart();
            CartController target = new CartController(mock.Object, orderProcessorMock.Object);

            //Act
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            //Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            // Arrange - Create a Cart
            Mock<IOrderProcessor> orderProcessorMock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            CartController target = new CartController(null, orderProcessorMock.Object);
            //Act
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;
            //Assert
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_ShippingDetails()
        {
            //Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();
            CartController target = new CartController(null, mock.Object);

            //Act
            ViewResult result = target.Checkout(cart, shippingDetails);

            //Assert
            //Check that the order has never been passed to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            //Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController target = new CartController(null, mock.Object);
            target.ModelState.AddModelError("error", "error");

            //Act
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //Assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);

        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            //Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            CartController target = new CartController(null, mock.Object);

            //Act
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //Assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
