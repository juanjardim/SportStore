using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportStore.Domain.Abstract;
using SportStore.WebUI.Models;

namespace SportStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _repo;
        public int PageSize = 4;

        public ProductController(IProductRepository productRepository)
        {
            _repo = productRepository;
        }
        // GET: Product
        public ViewResult List(int page = 1)
        {
            ProductsListViewModel model = new ProductsListViewModel
            {
                 Products = _repo.Products
                .OrderBy(p => p.ProductID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                 PageInfo = new PageInfo
                 {
                     CurrentPage = page,
                     ItemsPerPage = PageSize,
                     TotalItems = _repo.Products.Count()
                 }
            };

            return View(model);
        }

    }
}