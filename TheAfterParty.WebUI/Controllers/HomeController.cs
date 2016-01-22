using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheAfterParty.WebUI.Controllers
{
    public class HomeController : Controller
    {
        // GET: CoopShop/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}