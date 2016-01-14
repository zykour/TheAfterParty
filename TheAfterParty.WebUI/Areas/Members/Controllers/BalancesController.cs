using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.Domain.Abstract;
using Microsoft.AspNet.Identity.Owin;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty.WebUI.Areas.Members.Controllers
{
    public class BalancesController : Controller
    {
        // GET: CoopShop/Balances
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