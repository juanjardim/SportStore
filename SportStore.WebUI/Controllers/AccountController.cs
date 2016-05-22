using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportStore.WebUI.Infrastructure.Abstract;
using SportStore.WebUI.Models;

namespace SportStore.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthProvider _auth;
        public AccountController(IAuthProvider auth)
        {
            _auth = auth;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_auth.Authenticate(model.UserName, model.Password))
                {
                    return Redirect(returnUrl ?? Url.Action("Index", "Admin"));
                }
                ModelState.AddModelError("", "Incorrect username or password");
            }
            return View();
        }
    }
}