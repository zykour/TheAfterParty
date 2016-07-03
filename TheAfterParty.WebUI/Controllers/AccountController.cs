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

namespace TheAfterParty.WebUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IUserService userService;

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
        public async Task<ActionResult> Index()
        {
            AccountIndexModel model = new AccountIndexModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateAccountControllerNavList();

            model.ActivityFeedList = await userService.GetActivityFeedItems();

            return View(model);
        }
        public async Task<ActionResult> Orders()
        {
            AccountOrdersModel model = new AccountOrdersModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateAccountControllerNavList();

            model.Orders = await userService.GetUserOrders();

            model.Orders.OrderBy(o => o.SaleDate);

            return View(model);
        }        
        public async Task<ActionResult> Order(int id)
        {
            AccountOrderModel model = new AccountOrderModel();
            
            model.Order = userService.GetOrderByID(id);
            model.Order.ProductOrderEntries = model.Order.ProductOrderEntries.OrderBy(p => p.Listing.ListingName).ToList();
            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateAccountControllerNavList();

            return View(model);
        }
        public async Task<ActionResult> Keys()
        {
            AccountKeysModel model = new AccountKeysModel();

            model.LoggedInUser = await userService.GetCurrentUser();
            model.FullNavList = CreateAccountControllerNavList();

            model.Keys = await userService.GetKeys();
                
            model.Keys = model.Keys.OrderBy(k => k.Listing.ListingName).ToList();

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
                return View("Error", new string[] { "Access Denied" });
            }

            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();

            return RedirectToAction("Index", "Home");
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
            AppUser user = await UserManager.FindAsync(loginInfo.Login);

            // regular expression logic for extracing a 64-bit steam id from the claimed id
            Regex SteamIDRegex = new Regex(@"^http://steamcommunity\.com/openid/id/(7[0-9]{15,25})$");
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
                user = UserManager.Users.Where(u => u.UserSteamID == steamId).SingleOrDefault();
                
                // pre-authorized users only
                if (user == null)
                {
                    return View("Error", new string[] { "Sorry but you are not a member of The After Party Steam group. If you are, and you receive this error, please contact Monu." });
                }
                else
                {
                    userService.BuildUser(await userService.GetUserByID(user.Id), System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]);
                }

                if (user.UserSteamID == 0)
                {
                    user.UserSteamID = steamId;
                }
                
                // add the login info to the existing or new user
                result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);

                if (!result.Succeeded)
                {
                    return View("Error", result.Errors);
                }
            }

            ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            ident.AddClaims(loginInfo.ExternalIdentity.Claims);

            AuthManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
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

        public List<NavGrouping> CreateAccountControllerNavList()
        {
            List<NavGrouping> navList = new List<NavGrouping>();

            NavGrouping navGrouping = new NavGrouping();
            navGrouping.GroupingHeader = "My Account";

            navGrouping.NavItems = new List<NavItem>();

            NavItem navItem = new NavItem();
            navItem.Destination = "/Account/";
            navItem.DestinationName = "Account Home";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/Account/Orders/";
            navItem.DestinationName = "My Orders";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/Account/Keys/";
            navItem.DestinationName = "My Keys";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/Account/Logout/";
            navItem.DestinationName = "Logout";
            navGrouping.NavItems.Add(navItem);

            navList.Add(navGrouping);

            return navList;
        }
    }
}