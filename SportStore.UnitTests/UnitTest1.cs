using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportStore.Domain.Abstract;
using SportStore.Domain.Entities;
using SportStore.WebUI.Controllers;
using SportStore.WebUI.HtmlHelpers;
using SportStore.WebUI.Models;

namespace SportStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //Arrange
            var mock = this.GetMockProducts();
            ProductController controller = new ProductController(mock.Object) { PageSize = 3 };

            //Act
            ProductsListViewModel result =
            (ProductsListViewModel)controller.List(null, 2).Model;

            //Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");

        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //Arrange - Define an Html helper 
            HtmlHelper myHelper = null;

            //Arrange - Create PageInfo data

            PageInfo pageInfo = new PageInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            //Arrange - Set up the delegate
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //Act
            MvcHtmlString result = myHelper.PageLinks(pageInfo, pageUrlDelegate);

            //Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>",
                result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            var mock = this.GetMockProducts();
            ProductController controller = new ProductController(mock.Object) { PageSize = 3 };
           
            //Act
            ProductsListViewModel result = (ProductsListViewModel) controller.List(null, 2).Model;
            
            //Assert
            PageInfo pageInfo = result.PageInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);

        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            //Arrange
            var mock = this.GetMockProducts();
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Action 
            Product[] result = ((ProductsListViewModel) controller.List("Cat2", 1).Model).Products.ToArray();


            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");

        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //Arrange
            var mock = this.GetMockProducts();
            NavController target = new NavController(mock.Object);

            //Act 
            string[] results = ((IEnumerable<string>) target.Menu().Model).ToArray();

            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Cat1");
            Assert.AreEqual(results[1], "Cat2");
            Assert.AreEqual(results[2], "Cat3");

        }

        [TestMethod]
        public void Can_SelectCategory()
        {
            //Arrange
            var mock = this.GetMockProducts();
            NavController target = new NavController(mock.Object);
            string categoryToSelect = "Cat1";

            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            // Assert
            Assert.AreEqual(categoryToSelect, result);

        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            // Arrange
            var mock = this.GetMockProducts();
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            // Action
            int res1 = ((ProductsListViewModel) target.List("Cat1").Model).PageInfo.TotalItems;
            int res2 = ((ProductsListViewModel) target.List("Cat2").Model).PageInfo.TotalItems;
            int res3 = ((ProductsListViewModel) target.List("Cat3").Model).PageInfo.TotalItems;

            int resAll = ((ProductsListViewModel) target.List(null).Model).PageInfo.TotalItems;

            // Assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }


        private Mock<IProductRepository> GetMockProducts()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"},
            });

            return mock;
        }
    }
}
