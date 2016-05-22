using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using SportStore.WebUI.Infrastructure.Abstract;
using static System.Web.Security.FormsAuthentication;

namespace SportStore.WebUI.Infrastructure.Concrete
{
    public class FormsAuthProvider : IAuthProvider
    {
        public bool Authenticate(string username, string password)
        {
            if (username == null || password == null)
            {
                return false;
            }
            var result = FormsAuthentication.Authenticate(username, password);
            if (result)
            {
                SetAuthCookie(username, false);
            }
            return result;
        }
    }
}