using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SportStore.Domain.Entities;

namespace SportStore.WebUI.Infrastructure.Binders
{
    public class CartModelBinder : IModelBinder
    {
        private const string SessionKey = "Cart";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //Get the cart from the session

            Cart cart = null;
            if (controllerContext.HttpContext.Session != null)
            {
                cart = (Cart) controllerContext.HttpContext.Session[SessionKey];
            }
            if (cart == null)
            {
                cart = new Cart();
                if (controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[SessionKey] = cart;
                }
            }
            return cart;
        }
    }
}
