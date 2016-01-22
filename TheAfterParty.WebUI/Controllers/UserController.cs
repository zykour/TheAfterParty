using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheAfterParty.WebUI.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index(string userName)
        {


            return View();
        }
    }
}