using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin.Security.Providers.Steam;    
using Owin;
using TheAfterParty.Domain.Concrete;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using System;
using TheAfterParty.WebUI;
using Hangfire;
using Hangfire.Dashboard;
using System.Collections.Generic;
using TheAfterParty.WebUI.Infrastructure;
using TheAfterParty.WebUI.App_Start;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;

namespace TheAfterParty
{
    public class IdentityConfig
    {
        //public void ConfigureServices(IServiceCollection services)
        //{
         //   services.AddMemoryCache();
         //   //services.AddMvc();
        //}

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.CreatePerOwinContext<AppIdentityDbContext>(AppIdentityDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/tap/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1.1),
                Provider = new SimpleAuthorizationServerProvider(AppIdentityDbContext.Create)
            };

            // Token Generation

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                Provider = new OAuthBearerAuthenticationProvider()
            });

            WebApiConfig.Register(config);
            
            app.UseWebApi(config);

            // MVC Authentication

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/account/login"),
                ExpireTimeSpan = TimeSpan.FromDays(30),
                SlidingExpiration = true
            });
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseSteamAuthentication(System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]);

            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("AppIdentityDbContext");
            var auth = new Hangfire.Dashboard.AuthorizationFilter() { Users = "Monukai", Roles = "Admin" };
            //var options = new BackgroundJobServerOptions { WorkerCount = 1 };
            app.UseHangfireDashboard("/hangfire", new DashboardOptions {
                Authorization = new[] { new HangfireAuthorizationFilter() { Roles = new List<String> { "Admin" } } }
            });

            app.UseHangfireServer();

            TaskRegister.RegisterAuctions();
            TaskRegister.RegisterDailyTasks();
        }        
    }
}