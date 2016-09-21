using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Entities;
using System;

public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
{
    private Func<AppIdentityDbContext> CreateCallback { get; set; }

    public SimpleAuthorizationServerProvider(Func<AppIdentityDbContext> createCallback) : base()
    {
        CreateCallback = createCallback;
    }

    public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
    {
        context.Validated();
    }

    public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
    {
        AppUserManager UserManager = new AppUserManager(new UserStore<AppUser>(CreateCallback.Invoke()));

        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

        if (String.IsNullOrEmpty(context.Password) == true || UserManager.Find(context.UserName, context.Password) == null)
        {
            context.SetError("invalid_grant", "The user name or password is incorrect.");
            return;
        }

        var identity = new ClaimsIdentity(context.Options.AuthenticationType);
        identity.AddClaim(new Claim("sub", context.UserName));
        identity.AddClaim(new Claim("role", "user"));

        context.Validated(identity);
    }
}