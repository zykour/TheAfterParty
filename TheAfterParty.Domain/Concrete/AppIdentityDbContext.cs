using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Concrete
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext() : base("DefaultConnection") { }

        static AppIdentityDbContext()
        {
            Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }

    
    public class IdentityDbInit
    : DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
    {
        protected override void Seed(AppIdentityDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }
        public void PerformInitialSetup(AppIdentityDbContext context)
        {
            AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));

            /*
            //AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));
            //string roleName = "Administrators";
            string userName = "Monukai";
            string password = "password12345";
            string email = "admin@tap.com";
            int UserID = "76561198030277114";

            //if (!roleMgr.RoleExists(roleName))
            //{
             //   roleMgr.Create(new AppRole(roleName));
            //}

            AppUser user = userMgr.FindByName(userName);

            if (user == null)
            {
                userMgr.Create(new AppUser { UserName = userName, Email = email, UserID = UserID, Balance = 0 },
                password);
            }*/

            userMgr.Create(new AppUser { UserName = "Monukai", Email = "monu.kai@example.com", SteamID = "76561198030277114", Balance = 0, IsPrivateWishlist = true }, "password");
            userMgr.Create(new AppUser { UserName = "Lucky", Email = "luckyboy@example.com", SteamID = "76561198019064906", Balance = 75, IsPrivateWishlist = false }, "lucky777");
            userMgr.Create(new AppUser { UserName = "Don_Vino", SteamID = "76561197962202166", Balance = 50, IsPrivateWishlist = false }, "KHAAAAAAN");
            userMgr.Create(new AppUser { UserName = "Lina", SteamID = "76561198040771781", Balance = 420, IsPrivateWishlist = false}, "linaface");
            userMgr.Create(new AppUser { UserName = "Wesley", Email = "wbarton@example.com", SteamID = "76561198038935514", Balance = 100, IsPrivateWishlist = false }, "cryptic");

            context.SaveChanges();
        }
    }
}