using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SellingShop.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NotAllowed()
        {
            return View("Error403");
        }

        public ActionResult NotFound()
        {
            return View("Error404");
        }
    }
}