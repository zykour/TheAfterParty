using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web.Routing;
using TheAfterParty.Domain.Services;

namespace TheAfterParty.WebUI.Controllers
{
    public class UserController : Controller
    {
        private IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        // GET: User
        public ActionResult Index(string userName)
        {
            return View();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            userService.SetUserName(User.Identity.Name);
        }

        [Authorize]
        public async Task<ActionResult> AjaxAddToBlacklist(int listingId)
        {
            await userService.AddBlacklistEntry(listingId);

            return new PartialViewResult();
        }

        [Authorize]
        public async Task<ActionResult> AjaxTransferPoints(int points, string userId)
        {
            await userService.TransferPoints(points, userId);

            return new PartialViewResult();
        }
    }
}