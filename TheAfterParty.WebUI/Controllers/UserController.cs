using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using TheAfterParty.Domain.Services;
using TheAfterParty.WebUI.Models.User;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            userService.SetUserName(User.Identity.Name);
        }

        // GET: User
        public async Task<ActionResult> Index()
        {
            ModelState.Clear();

            UserIndexModel model = new UserIndexModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.Users = userService.GetAllUsers().OrderBy(u => u.UserName).ToList();
            model.Title = "All Users";
            model.FullNavList = CreateUserControllerNavList();

            return View(model);
        }

        public async Task<ActionResult> Admins()
        {
            UserIndexModel model = new UserIndexModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.Title = "Admins";
            model.FullNavList = CreateUserControllerNavList();
            model.Users = userService.GetAdmins();

            return View("Index", model);
        }

        private List<NavGrouping> CreateUserControllerNavList()
        {
            List<NavGrouping> navList;

            NavGrouping navGrouping = new NavGrouping();
            navGrouping.GroupingHeader = "Users";

            NavItem admin = new NavItem();
            admin.Destination = "/User/Admins/";
            admin.DestinationName = "Admins";

            NavItem users = new NavItem();
            users.Destination = "/User/";
            users.DestinationName = "All Users";

            navGrouping.NavItems = new List<NavItem>() { admin, users };
            navList = new List<NavGrouping>() { navGrouping };

            return navList;
        }

        private List<NavGrouping> CreateUserControllerAdminNavList()
        {
            List<NavGrouping> navList = CreateUserControllerNavList();

            NavGrouping navGrouping = new NavGrouping();
            navGrouping.GroupingHeader = "Admin Actions";

            NavItem balances = new NavItem();
            balances.Destination = "/User/AddBalances/";
            balances.DestinationName = "Add Balances";

            navGrouping.NavItems = new List<NavItem>() { balances };

            navList.Add(navGrouping);

            return navList;
        }

        // GET: User/Profile/name
        // base class Controller has a "Profile" method, thus need to rename this action and give it a custom route
        public async Task<ActionResult> UserProfile(string id = "")
        {
            
            ModelState.Clear();

            if (String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            UserProfileModel model = new UserProfileModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.RequestedUser = userService.GetRequestedUser(id);
            model.FullNavList = CreateUserControllerNavList();

            // handle this later with a generic error view
            if (model.RequestedUser == null)
            {
                return RedirectToAction("Index");
            }

            if (model.RequestedUser.LargeAvatar == null)
            {
                userService.BuildUser(model.RequestedUser, System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]);
            }

            return View(model);
        }

        public async Task<int> TotalReservedBalance()
        {
            return await userService.GetTotalReservedBalance();
        }

        public async Task<int> PublicAuctionReservedBalance()
        {
            return await userService.GetPublicAuctionReservedBalance();
        }

        public async Task<int> SilentAuctionReservedBalance()
        {
            return await userService.GetSilentAuctionReservedBalance();
        }

        public async Task<int> GetCartTotal()
        {
            return await userService.GetCartTotal();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult AddBalances()
        {
            UserAddBalancesViewModel model = new UserAddBalancesViewModel();
            model.Users = userService.GetAllUsers().OrderBy(u => u.UserName).ToList();
            model.FullNavList = CreateUserControllerNavList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddBalances(UserAddBalancesViewModel model)
        {
            if (String.IsNullOrEmpty(model.Input))
            {
                model.Users = userService.GetAllUsers().OrderBy(u => u.UserName).ToList();
                return View(model);
            }
            
            bool success = userService.AddBalances(model.Input);

            if (!success)
            {
                model.Users = userService.GetAllUsers().OrderBy(u => u.UserName).ToList();
                // add validation errors ?
                return View(model);
            }

            return RedirectToAction("Index");
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