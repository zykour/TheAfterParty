using System;
using Owin;
using Hangfire;
using Hangfire.Dashboard;
using System.Collections.Generic;
using Hangfire.SqlServer;
using Microsoft.Owin;

namespace TheAfterParty.WebUI.Infrastructure
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public List<String> Roles { get; set; }

        public bool Authorize(DashboardContext context)
        {
            var owinContext = new OwinContext(context.GetOwinEnvironment());

            foreach (String role in Roles)
            {
                if (owinContext.Authentication.User.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }
    }
}