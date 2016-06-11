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
using Microsoft.AspNet.Identity.Owin;
using TheAfterParty.Domain.Concrete;

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
            model.Title = "Users";
            model.FullNavList = CreateUserControllerNavList();

            return View(model);
        }

        public async Task<ActionResult> Admins()
        {
            UserIndexModel model = new UserIndexModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.Title = "Users > Admins";
            model.FullNavList = CreateUserControllerNavList();
            model.Users = userService.GetAdmins();

            return View("Index", model);
        }

        [HttpGet]
        public async Task<ActionResult> AddAppUser()
        {
            AddEditAppUserViewModel model = new AddEditAppUserViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddAppUser(AddEditAppUserViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            userService.BuildUser(model.AppUser, System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]);

            await HttpContext.GetOwinContext().GetUserManager<AppUserManager>().CreateAsync(model.AppUser);

            if (String.IsNullOrEmpty(model.RoleToAdd) == false)
            {
                await HttpContext.GetOwinContext().GetUserManager<AppUserManager>().AddToRoleAsync(model.AppUser.Id, model.RoleToAdd);
            }

            return View("EditAppUser", model);
        }

        [HttpGet]
        public async Task<ActionResult> EditAppUser(string id)
        {
            AddEditAppUserViewModel model = new AddEditAppUserViewModel();

            model.AppUser = await userService.GetUserByID(id);

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (model.AppUser != null)
            {
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditAppUser(AddEditAppUserViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            await HttpContext.GetOwinContext().GetUserManager<AppUserManager>().UpdateAsync(model.AppUser);

            if (String.IsNullOrEmpty(model.RoleToAdd) == false)
            {
                await HttpContext.GetOwinContext().GetUserManager<AppUserManager>().AddToRoleAsync(model.AppUser.Id, model.RoleToAdd);
            }

            if (String.IsNullOrEmpty(model.RoleToRemove) == false)
            {
                await HttpContext.GetOwinContext().GetUserManager<AppUserManager>().RemoveFromRoleAsync(model.AppUser.Id, model.RoleToAdd);
            }

            return View("AdminAppUsers");
        }

        [HttpGet]
        public async Task<ActionResult> AddBalanceEntry()
        {
            AddEditBalanceEntryViewModel model = new AddEditBalanceEntryViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddBalanceEntry(AddEditBalanceEntryViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            userService.CreateBalanceEntry(model.BalanceEntry);

            return View("EditBalanceEntry", model);
        }

        [HttpGet]
        public async Task<ActionResult> EditBalanceEntry(int id)
        {
            AddEditBalanceEntryViewModel model = new AddEditBalanceEntryViewModel();

            model.BalanceEntry = userService.GetBalanceEntryByID(id);

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditBalanceEntry(AddEditBalanceEntryViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            userService.EditBalanceEntry(model.BalanceEntry);

            return View("AdminBalanceEntries");
        }

        public ActionResult DeleteBalanceEntry(int id)
        {
            userService.DeleteBalanceEntry(id);

            return View("AdminBalanceEntries");
        }

        [HttpGet]
        public async Task<ActionResult> AddClaimedProductKey()
        {
            AddEditClaimedProductKeyViewModel model = new AddEditClaimedProductKeyViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddClaimedProductKey(AddEditClaimedProductKeyViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> EditClaimedProductKey()
        {
            AddEditClaimedProductKeyViewModel model = new AddEditClaimedProductKeyViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditClaimedProductKey(AddEditClaimedProductKeyViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            userService.EditClaimedProductKey(model.ClaimedProductKey);

            return View("AdminClaimedProductKeys");
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

            navGrouping.NavItems = new List<NavItem>();

            NavItem navItem = new NavItem();
            navItem.Destination = "/User/AddBalances/";
            navItem.DestinationName = "Add Balances";
            navGrouping.NavItems.Add(navItem);
            navItem.Destination = "/User/AddAppUser/";
            navItem.DestinationName = "Add User";
            navGrouping.NavItems.Add(navItem);

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