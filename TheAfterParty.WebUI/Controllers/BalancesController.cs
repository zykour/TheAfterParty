using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.Domain.Abstract;
using Microsoft.AspNet.Identity.Owin;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.WebUI.Models.Balances;
using System.Threading.Tasks;

namespace TheAfterParty.WebUI.Controllers
{
    public class BalancesController : Controller
    {
        // GET: CoopShop/Balances
        public async Task<ActionResult> Index()
        {
            BalancesIndexViewModel balanceViewModel = new BalancesIndexViewModel();

            if (User.Identity.IsAuthenticated)
            {
                balanceViewModel.LoggedInUser = await UserManager.FindByNameAsync(User.Identity.Name);
            }

            balanceViewModel.SiteUsers = UserManager.Users;

            return View(balanceViewModel);
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