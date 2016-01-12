using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.Domain.Concrete;
using Microsoft.AspNet.Identity.Owin;

namespace TheAfterParty.WebUI.Areas.CoopShop.Controllers
{
    public class AccountController : Controller
    {
        // GET: CoopShop/Account
        public ActionResult Index()
        {
            return View(UserManager.Users);
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
    }
}