using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin.Security.Providers.Steam;    
using Owin;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<AppIdentityDbContext>(AppIdentityDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            //app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/account/login")
            });
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseSteamAuthentication(System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]);
        }
    }
}