using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Services;
using System.Web.Routing;
using TheAfterParty.WebUI.Models.Auctions;

namespace TheAfterParty.WebUI.Controllers
{
    public class AuctionController : Controller
    {
        private IUserService userService;
        
        public AuctionController(IUserService userService)
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

        // GET: Auction
        public ActionResult Index()
        {
            AuctionsModel view = new AuctionsModel();

            view.OpenAuctions = userService.GetAuctions().Where(a => a.EndTime > DateTime.Now).ToList();

            return View(view);
        }

        public ActionResult Closed()
        {
            AuctionsModel view = new AuctionsModel();

            view.OpenAuctions = userService.GetAuctions().Where(a => a.EndTime <= DateTime.Now).ToList();

            return View(view);
        }

        public async Task<ActionResult> Created()
        {
            CreatedAuctionsModel view = new CreatedAuctionsModel();

            view.CreatedAuctions = (await userService.GetCurrentUser()).Auctions.ToList();

            return View(view);
        }
    }
}