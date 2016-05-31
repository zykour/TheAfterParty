using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models;
using TheAfterParty.Domain.Services;
using TheAfterParty.Domain.Entities;
using System.Threading.Tasks;
using TheAfterParty.WebUI.Models._Nav;
using System.Web.Routing;

namespace TheAfterParty.WebUI.Controllers
{
    public class ObjectivesController : Controller
    { 
        private IStoreService storeService;

        public ObjectivesController(IStoreService storeService)
        {
            this.storeService = storeService;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                storeService.SetUserName(User.Identity.Name);
            }
        }
        // GET: CoopShop/Objectives
        public ActionResult Index()
        {
            return View();
        }
    }
}