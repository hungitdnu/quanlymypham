using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SellingShop.App_Start
{
    public class FilterAuthorization : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie cookie = filterContext.HttpContext.Request.Cookies.Get(StaticObject.cookieName);
            if (cookie == null)
            {
                filterContext.Result = new RedirectResult("/Account/Login");
            }
            else
            {
                try
                {
                    var base64 = cookie.Value;
                    var dec = StaticObject.FromBase64(base64);
                    var account = StaticObject.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(dec));
                    if (account == null)
                    {
                        filterContext.Result = new RedirectResult("/Account/Login");
                    }
                    if(account.role.Equals("user",StringComparison.OrdinalIgnoreCase))
                    {
                        filterContext.Result = new RedirectResult("/Error/NotAllowed");
                    }
                }
                catch
                {
                    filterContext.Result = new RedirectResult("/Account/Login");
                }

            }
            base.OnActionExecuting(filterContext);
        }
    }
}