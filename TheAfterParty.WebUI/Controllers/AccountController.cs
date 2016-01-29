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

namespace TheAfterParty.WebUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // GET: CoopShop/Account
        public ActionResult Index()
        {
            return View(UserManager.Users);
        }










        // -----------------------
        // Login/logout and related methods

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

                // if this user doesn't exist in our database at all, create a new user
                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = loginInfo.DefaultUserName,
                        UserSteamID = steamId
                    };

                    result = await UserManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                else if (user.UserSteamID == 0)
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






        // -------------------------
        // Getter methods

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
    }
}