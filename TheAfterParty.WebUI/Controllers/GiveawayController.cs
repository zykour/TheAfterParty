using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models.Giveaways;
using System.Web.Routing;
using TheAfterParty.Domain.Services;
using System.Threading.Tasks;

namespace TheAfterParty.WebUI.Controllers
{
    public class GiveawayController : Controller
    {
        private IUserService userService;

        public GiveawayController(IUserService userService)
        {
            this.userService = userService;
        }


        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (User.Identity.IsAuthenticated)
            {
                userService.SetUserName(User.Identity.Name);
            }
        }

        // GET: Giveaway
        public ActionResult Index()
        {
            GiveawaysModel view = new GiveawaysModel();

            view.OpenGiveaways = userService.GetGiveaways().Where(g => g.EndDate > DateTime.Now).ToList();

            return View(view);
        }

        public ActionResult Closed()
        {
            GiveawaysModel view = new GiveawaysModel();

            view.OpenGiveaways = userService.GetGiveaways().Where(g => g.EndDate <= DateTime.Now).ToList();

            return View(view);
        }

        public async Task<ActionResult> Created()
        {
            CreatedGiveawaysModel view = new CreatedGiveawaysModel();

            view.CreatedGiveaways = (await userService.GetCurrentUser()).CreatedGiveaways.ToList();

            return View(view);
        }
    }
}