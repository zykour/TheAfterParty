using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using TheAfterParty.Domain.Services;
using System.Web.Routing;
using TheAfterParty.WebUI.Models.Account;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.WebUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IUserService userService;
        private const string indexActionDest = "Account Home";
        private const string ordersActionDest = "My Orders";
        private const string keysActionDest = "My Keys";
        private const string objsActionDest = "My Objectives";
        private const string settingsActionDest = "My Settings";

        public AccountController(IUserService userService) : base()
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

        #region Actions
        // GET: /Account
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            AccountIndexModel model = new AccountIndexModel();
            
            List<ActivityFeedContainer> list = await userService.GetActivityFeedItems();
            model.Initialize((await userService.GetCurrentUser()), CreateAccountControllerNavList(new List<string>() { indexActionDest }));
            model.ActivityFeedList = model.SkipAndTake<ActivityFeedContainer>(list).ToList();

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(AccountIndexModel model)
        {
            List<ActivityFeedContainer> list = await userService.GetActivityFeedItems();
            model.Initialize((await userService.GetCurrentUser()), CreateAccountControllerNavList(new List<string>() { indexActionDest }));
            model.ActivityFeedList = model.SkipAndTake<ActivityFeedContainer>(list).ToList();

            ModelState.Clear();

            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> MySettings()
        {
            MySettingsViewModel model = new MySettingsViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            List<String> destNames = new List<String>() { settingsActionDest };
            model.FullNavList = CreateAccountControllerNavList(destNames);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> MySettings(MySettingsViewModel model)
        {
            List<String> destNames = new List<String>() { settingsActionDest };

            if (String.IsNullOrEmpty(model.PaginationPreference) == false)
            {
                int pagination = 0;

                Int32.TryParse(model.PaginationPreference, out pagination);

                if (model.PaginationPreference.CompareTo("0") != 0 && pagination == 0)
                {
                    ModelState.AddModelError(String.Empty, "Invalid pagination value selected.");

                    model.LoggedInUser = await userService.GetCurrentUser();
                    model.FullNavList = CreateAccountControllerNavList(destNames);

                    return View(model);
                }
                else
                {
                    model.LoggedInUser.PaginationPreference = pagination;
                }
            }
            
            if (String.IsNullOrEmpty(model.AppIDs) == false)
            {
                model.SuccessfulItemsAdded = await userService.AddWishlistItems(model.AppIDs);
            }

            await userService.EditAppUserSettings(model.LoggedInUser);

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateAccountControllerNavList(destNames);

            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> Orders()
        {
            AccountOrdersModel model = new AccountOrdersModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            List<String> destNames = new List<String>() { ordersActionDest };
            model.FullNavList = CreateAccountControllerNavList(destNames);

            model.Orders = (await userService.GetUserOrders()).OrderBy(o => o.SaleDate).ToList();

            model.CurrentPage = 1;

            model.TotalItems = model.Orders.Count;

            if (model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;

                model.Orders = model.Orders.Take(model.UserPaginationPreference).ToList();
            }

            return View(model);
        }        
        [HttpPost]
        public async Task<ActionResult> Orders(AccountOrdersModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            List<String> destNames = new List<String>() { ordersActionDest };
            model.FullNavList = CreateAccountControllerNavList(destNames);

            model.Orders = (await userService.GetUserOrders()).OrderBy(o => o.SaleDate).ToList();

            model.CurrentPage = model.SelectedPage;

            if (model.CurrentPage < 1)
            {
                model.CurrentPage = 1;
            }

            model.TotalItems = model.Orders.Count;

            if (model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;

                model.Orders = model.Orders.Skip((model.CurrentPage - 1) * model.UserPaginationPreference).Take(model.UserPaginationPreference).ToList();
            }

            return View(model);
        }
        public async Task<ActionResult> Order(int id)
        {
            AccountOrderModel model = new AccountOrderModel();
            
            model.LoggedInUser = await userService.GetCurrentUser();

            model.Order = model.LoggedInUser.Orders.SingleOrDefault(o => o.OrderID == id);

            if (model.Order == null)
            {
                return RedirectToAction("Orders");
            }

            model.Order.ProductOrderEntries = model.Order.ProductOrderEntries.OrderBy(p => p.Listing.ListingName).ToList();
            List<String> destNames = new List<String>() { ordersActionDest };
            model.FullNavList = CreateAccountControllerNavList(destNames);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Keys()
        {
            AccountKeysModel model = new AccountKeysModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            List<String> destNames = new List<String>() { keysActionDest };
            model.FullNavList = CreateAccountControllerNavList(destNames);

            model.Keys = await userService.GetKeys();
            
            PopulateKeysModel(model);

            model.Keys = model.Keys.OrderBy(k => k.Listing.ListingName).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Keys(AccountKeysModel model)
        {
            model.LoggedInUser = await userService.GetCurrentUser();
            List<String> destNames = new List<String>() { keysActionDest };
            model.FullNavList = CreateAccountControllerNavList(destNames);

            model.Keys = await userService.GetKeys();

            PopulateKeysModel(model);

            model.Keys = model.Keys.OrderBy(k => k.Listing.ListingName).ToList();

            ModelState.Clear();

            return View(model);
        }

        #endregion

        #region Ajax Methods
        public string AjaxRevealKey(int productKeyId)
        {            
            return userService.RevealKey(productKeyId);
        }
        public bool AjaxMarkKeyUsed(int productKeyId)
        {
            return userService.MarkKeyUsed(productKeyId);
        }
        #endregion

        #region Login/out methods
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect(returnUrl ?? "/");
            }

            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();

            return RedirectToAction("Index", "Store");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SteamLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("SteamLoginCallback", new { returnUrl = returnUrl })
            };
            HttpContext.GetOwinContext().Authentication.Challenge(properties, "Steam");

            return new HttpUnauthorizedResult();
        }
        
        [AllowAnonymous]
        public async Task<ActionResult> SteamLoginCallback(string returnUrl)
        {
            // Get the external login info from Steam and find the corresponding user if it exists based on that info
            ExternalLoginInfo loginInfo = await AuthManager.GetExternalLoginInfoAsync();
            AppUser user = await userService.GetUserManager().FindAsync(loginInfo.Login);

            // regular expression logic for extracing a 64-bit steam id from the claimed id
            Regex SteamIDRegex = new Regex(@"^https?://steamcommunity\.com/openid/id/(7[0-9]{15,25})$");
            Match IDMatch = SteamIDRegex.Match(loginInfo.Login.ProviderKey);

            Int64 steamId = 0;

            IdentityResult result;

            if (IDMatch.Success)
            {
                steamId = Int64.Parse(IDMatch.Groups[1].Value);
            }

            if (user == null)
            {
                // attempt to figure out if this user exists but hasn't logged in via the Steam external login provider before
                user = userService.GetUserManager().Users.Where(u => u.UserSteamID == steamId).SingleOrDefault();
                
                // pre-authorized users only
                if (user == null)
                {
                    ErrorViewModel model = new ErrorViewModel();
                    model.Errors = new string[] { "Sorry but you are not a member of The After Party Steam group. If you are, and you receive this error, please contact Monu." };
                    return View("Error", model);
                }
                else
                {
                    Task task = new Task(new Action(() => userService.BuildUser(user, System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"])));
                    task.Start();
                }

                if (user.UserSteamID == 0)
                {
                    user.UserSteamID = steamId;
                }

                // add the login info to the existing or new user
                result = await userService.GetUserManager().AddLoginAsync(user.Id, loginInfo.Login);

                if (!result.Succeeded)
                {
                    ErrorViewModel model = new ErrorViewModel();
                    model.Errors = result.Errors.ToArray();
                    return View("Error", model);
                }
            }
            
            ClaimsIdentity ident = await userService.GetUserManager().CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            ident.AddClaims(loginInfo.ExternalIdentity.Claims);

            AuthManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = true
            }, ident);

            return Redirect(returnUrl ?? "/");
        }
        #endregion

        #region Getters
        private IAuthenticationManager AuthManager
        {   
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
        #endregion

        public void PopulateKeysModel(AccountKeysModel model)
        {
            if (model.FilterRevealed)
            {
                // assume only one Filter value can be submitted per form submit
                model.PreviousFilterUnrevealed = false;
                model.PreviousFilterRevealed = !model.PreviousFilterRevealed;
            }

            if (model.FilterUnrevealed)
            {
                model.PreviousFilterRevealed = false;
                model.PreviousFilterUnrevealed = !model.PreviousFilterUnrevealed;
            }

            if (model.FilterUsed)
            {
                model.PreviousFilterUnused = false;
                model.PreviousFilterUsed = !model.PreviousFilterUsed;
            }

            if (model.FilterUnused)
            {
                model.PreviousFilterUsed = false;
                model.PreviousFilterUnused = !model.PreviousFilterUnused;
            }

            if (String.IsNullOrEmpty(model.SearchText) == false)
            {
                model.Keys = model.Keys.Where(k => k.Listing.ListingName.ToLower().Contains(model.SearchText.ToLower())).ToList();
            }

            if (model.PreviousFilterRevealed)
            {
                model.Keys = model.Keys.Where(k => k.IsRevealed == true).ToList();
            }
            else if (model.PreviousFilterUnrevealed)
            {
                model.Keys = model.Keys.Where(k => k.IsRevealed == false).ToList();
            }

            if (model.PreviousFilterUsed)
            {
                model.Keys = model.Keys.Where(k => k.IsUsed == true).ToList();
            }
            else if (model.PreviousFilterUnused)
            {
                model.Keys = model.Keys.Where(k => k.IsUsed == false).ToList();
            }

            model.CurrentPage = model.SelectedPage;
            if (model.CurrentPage < 1)
            {
                model.CurrentPage = 1;
            }
            model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
            model.TotalItems = model.Keys.Count();

            if (model.UserPaginationPreference != 0)
            {
                model.Keys = model.Keys.OrderByDescending(k => k.Listing.ListingName).Skip((model.CurrentPage - 1) * model.UserPaginationPreference).Take(model.UserPaginationPreference).ToList();
            }
        }
        
        public List<NavGrouping> CreateAccountControllerNavList(List<String> destNames)
        {
            List<NavGrouping> navList = new List<NavGrouping>();

            NavGrouping navGrouping = new NavGrouping();
            navGrouping.GroupingHeader = "My Account";

            navGrouping.NavItems = new List<NavItem>();

            NavItem navItem = new NavItem();
            navItem.Destination = "/account/";
            navItem.DestinationName = indexActionDest;
            navItem.SetSelected(destNames);
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/account/orders/";
            navItem.DestinationName = ordersActionDest;
            navItem.SetSelected(destNames);
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/account/keys/";
            navItem.DestinationName = keysActionDest;
            navItem.SetSelected(destNames);
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/objectives/myobjectives/";
            navItem.DestinationName = objsActionDest;
            navItem.SetSelected(destNames);
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/account/mysettings/";
            navItem.DestinationName = settingsActionDest;
            navItem.SetSelected(destNames);
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/account/logout/";
            navItem.DestinationName = "Logout";
            navGrouping.NavItems.Add(navItem);

            navList.Add(navGrouping);

            return navList;
        }
    }
}