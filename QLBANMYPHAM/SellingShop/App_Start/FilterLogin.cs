using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SellingShop.Models
{
    public class FilterLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpCookie cookie = context.HttpContext.Request.Cookies.Get(StaticObject.cookieName);
            if(cookie == null)
            {
                context.Result = new RedirectResult("/Account/Login");
            }
            else
            {
                try {
                    var base64 = cookie.Value;
                    var dec = StaticObject.FromBase64(base64);
                    if (StaticObject.qlbmp.accounts.FirstOrDefault(m => m.username.Equals(dec)) == null)
                    {
                        context.Result = new RedirectResult("/Account/Login");
                    }
                    StaticObject.username = dec;
                }
                catch
                {
                    context.Result = new RedirectResult("/Account/Login");
                }
                
            }
            base.OnActionExecuting(context);
        }
    }
}