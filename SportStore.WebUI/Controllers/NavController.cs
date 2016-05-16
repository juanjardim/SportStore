using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportStore.Domain.Abstract;

namespace SportStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        private readonly IProductRepository _repo;

        public NavController(IProductRepository repo)
        {
            _repo = repo;
        }

        // GET: Menu
        public PartialViewResult Menu(string category = null)
        {
            ViewBag.SelectedCategory = category;

            IEnumerable<string> categories = _repo.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x);
            
            return PartialView("FlexMenu", categories);
        }
    }
}