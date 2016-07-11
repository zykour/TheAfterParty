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
            
            if (model.RequestedUser == null)
            {
                return RedirectToAction("Index");
            }

            if (model.RequestedUser.LargeAvatar == null)
            {
                userService.BuildUser(model.RequestedUser, System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]);
            }

            model.ActivityFeedList = userService.GetPublicActivityFeedItems(model.RequestedUser);

            if (userService.IsInRole(model.RequestedUser, "Admin"))
            {
                model.HighestRole = "Admin";
            }
            else if(userService.IsInRole(model.RequestedUser, "Moderator"))
            {
                model.HighestRole = "Mod";
            }
            else if (userService.IsInRole(model.RequestedUser, "Member"))
            {
                model.HighestRole = "Member";
            }
            else 
            {
                model.HighestRole = "Guest";
            }

            return View(model);
        }

        #region Admin Actions

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

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminAppUsers()
        {
            AdminAppUserViewModel model = new AdminAppUserViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            model.AppUsers = userService.GetAllUsers();

            return View(model);
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddAppUser()
        {
            AddEditAppUserViewModel model = new AddEditAppUserViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddAppUser(AddEditAppUserViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            await userService.CreateAppUser(model.AppUser, model.RoleToAdd, System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]);

            return RedirectToAction("AdminAppUsers");
        }

        [HttpGet, Authorize(Roles = "Admin")]
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

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditAppUser(AddEditAppUserViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            await userService.EditAppUser(model.AppUser, model.RoleToAdd, model.RoleToRemove);

            return RedirectToAction("AdminAppUsers");
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminBalanceEntries()
        {
            AdminBalanceEntryViewModel model = new AdminBalanceEntryViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            model.BalanceEntries = userService.GetBalanceEntries();

            return View(model);
        }
        
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddBalanceEntry(string id = "")
        {
            AddEditBalanceEntryViewModel model = new AddEditBalanceEntryViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            model.UserID = id;

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddBalanceEntry(AddEditBalanceEntryViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            if (String.IsNullOrEmpty(model.UserID))
            {
                await userService.CreateBalanceEntry(model.BalanceEntry, model.ObjectiveID, model.UserNickName);
            }
            else
            {
                model.BalanceEntry.AppUser = await userService.GetUserByID(model.UserID);
                await userService.CreateBalanceEntry(model.BalanceEntry, model.ObjectiveID);
            }

            return RedirectToAction("AdminBalanceEntries");
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditBalanceEntry(int id)
        {
            AddEditBalanceEntryViewModel model = new AddEditBalanceEntryViewModel();

            model.BalanceEntry = userService.GetBalanceEntryByID(id);

            model.UserID = model.BalanceEntry.AppUser.Id;

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditBalanceEntry(AddEditBalanceEntryViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            await userService.EditBalanceEntry(model.BalanceEntry, model.ObjectiveID);

            return RedirectToAction("AdminBalanceEntries");
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteBalanceEntry(int id)
        {
            await userService.DeleteBalanceEntry(id);

            return RedirectToAction("AdminBalanceEntries");
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminClaimedProductKeys()
        {
            AdminClaimedProductKeyViewModel model = new AdminClaimedProductKeyViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            model.ClaimedProductKeys = userService.GetClaimedProductKeys();

            return View(model);
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddClaimedProductKey(string id = null)
        {
            AddEditClaimedProductKeyViewModel model = new AddEditClaimedProductKeyViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            model.UserID = id;

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddClaimedProductKey(AddEditClaimedProductKeyViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            if (String.IsNullOrEmpty(model.UserID))
            {
                userService.CreateClaimedProductKey(model.ClaimedProductKey, model.UserNickName);
            }
            else
            {
                model.ClaimedProductKey.AppUser = await userService.GetUserByID(model.UserID);
                userService.CreateClaimedProductKey(model.ClaimedProductKey);
            }

            return RedirectToAction("AdminClaimedProductKeys");
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditClaimedProductKey(int id)
        {
            AddEditClaimedProductKeyViewModel model = new AddEditClaimedProductKeyViewModel();

            model.ClaimedProductKey = userService.GetClaimedProductKeyByID(id);

            model.UserID = model.ClaimedProductKey.AppUser.Id;

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditClaimedProductKey(AddEditClaimedProductKeyViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            userService.EditClaimedProductKey(model.ClaimedProductKey);

            return RedirectToAction("AdminClaimedProductKeys");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteClaimedProductKey(int id)
        {
            userService.DeleteClaimedProductKey(id);

            return RedirectToAction("AdminClaimedProductKeys");
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminOrders()
        {
            AdminOrderViewModel model = new AdminOrderViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            model.Orders = userService.GetOrders();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminOrder(int id)
        {
            AdminSingleOrderViewModel model = new AdminSingleOrderViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            model.Order = userService.GetOrderByID(id);

            return View(model);
        }
        
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddOrder(string id = null)
        {
            AddEditOrderViewModel model = new AddEditOrderViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            model.Order = new Order();

            model.UserID = id;

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddOrder(AddEditOrderViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            
            model.Order.AppUser = (String.IsNullOrEmpty(model.UserID)) ? userService.GetUserByNickname(model.UserNickName) : await userService.GetUserByID(model.UserID);
            model.ClaimedProductKey.AppUser = model.Order.AppUser;
            model.ClaimedProductKey.Date = DateTime.Now;

            if (!model.UseDBKey)
            {
                model.ProductOrderEntry.AddClaimedProductKey(model.ClaimedProductKey);
            }

            model.Order.ProductOrderEntries = new List<ProductOrderEntry>() { model.ProductOrderEntry };

            await userService.CreateOrder(model.Order, model.AlreadyCharged, model.UseDBKey);

            return RedirectToAction("AdminOrders");
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditOrder(int id)
        {
            AddEditOrderViewModel model = new AddEditOrderViewModel();

            model.Order = userService.GetOrderByID(id);

            model.UserID = model.Order.AppUser.Id;

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditOrder(AddEditOrderViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            userService.EditOrder(model.Order);

            return RedirectToAction("AdminOrders");
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            await userService.DeleteOrder(id);

            return RedirectToAction("AdminOrders");
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditProductOrderEntry(int id)
        {
            AddEditProductOrderEntryViewModel model = new AddEditProductOrderEntryViewModel();

            model.ProductOrderEntry = userService.GetProductOrderEntryByID(id);
            
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditProductOrderEntry(AddEditProductOrderEntryViewModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateUserControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            userService.EditProductOrderEntry(model.ProductOrderEntry);

            return RedirectToAction("AdminOrders");
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProductOrderEntry(int id)
        {
            ProductOrderEntry entry = userService.GetProductOrderEntryByID(id);

            await userService.DeleteProductOrderEntry(id);

            return RedirectToAction("AdminOrder", new { id = entry.Order.OrderID });
        }

        #endregion
        
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
            navItem.Destination = "/User/AdminAppUsers/";
            navItem.DestinationName = "View Users";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/User/AddAppUser/";
            navItem.DestinationName = "Add User";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/User/AdminBalanceEntries/";
            navItem.DestinationName = "View Balance Entries";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/User/AddBalances/";
            navItem.DestinationName = "Add Balances";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/User/AddBalance/";
            navItem.DestinationName = "Add Balance Entry";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/User/AdminClaimedProductKeys/";
            navItem.DestinationName = "View User Keys";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/User/AddClaimedProductKey/";
            navItem.DestinationName = "Add Key for User";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/User/AdminOrders/";
            navItem.DestinationName = "View Orders";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/User/AddOrder/";
            navItem.DestinationName = "Add Order";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();

            navList.Add(navGrouping);

            return navList;
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