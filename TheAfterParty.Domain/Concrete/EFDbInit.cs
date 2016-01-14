using System.Data.Entity;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

public class EFDbInit : DropCreateDatabaseAlways<AppIdentityDbContext>
{
    protected override void Seed(AppIdentityDbContext context)
    {
        PerformInitialSetup(context);
        base.Seed(context);
    }

    public void PerformInitialSetup(AppIdentityDbContext context)
    {
        AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));

        userMgr.Create(new AppUser { UserSteamID = 76561198030277114, UserName = "Monukai", Email = "monu.kai@example.com", Balance = 0, IsPrivateWishlist = true });
        userMgr.Create(new AppUser { UserSteamID = 76561198019064906, UserName = "Lucky", Email = "luckyboy@example.com", Balance = 75, IsPrivateWishlist = false });
        userMgr.Create(new AppUser { UserSteamID = 76561197962202166, UserName = "Don_Vino", Balance = 50, IsPrivateWishlist = false });
        userMgr.Create(new AppUser("Lina", 420, false, 76561198040771781));//{ SteamID = 76561198040771781UL, UserName = "Lina", Balance = 420, IsPrivateWishlist = false });
        userMgr.Create(new AppUser { UserSteamID = 76561198038935514, UserName = "Wesley", Email = "wbarton@example.com", Balance = 100, IsPrivateWishlist = false });

        Listing l = new Listing("Sol Survivor", 5);
        Product p = new Product() { AppID = 45000, ProductName = "Sol_Survivor", Platform = 1 };
        ProductDetail pd = new ProductDetail() { };
        p.ProductDetail = pd;
        //pd.Product = p;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        l = new Listing("Zero Gear", 3);
        p = new Product() { AppID = 18820, ProductName = "Zero_Gear", Platform = 1 };
        pd = new ProductDetail() {};
        p.ProductDetail = pd;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        l = new Listing("Rocket League", 40);
        p = new Product() { AppID = 252950, ProductName = "Rocket_League", Platform = 1 };
        pd = new ProductDetail() { };
        p.ProductDetail = pd;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        l = new Listing("Tabletop Simulator", 25);
        p = new Product() { AppID = 286160, ProductName = "Tabletop_Simulator", Platform = 1 };
        pd = new ProductDetail() { };
        p.ProductDetail = pd;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        l = new Listing("Warhammer: End Times - Vermintide", 60);
        p = new Product() { AppID = 235540, ProductName = "Warhammer:_End_Times_-_Vermintide", Platform = 1 };
        pd = new ProductDetail() { };
        p.ProductDetail = pd;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        l = new Listing("Portal_2", 15);
        p = new Product() { AppID = 620, ProductName = "Portal_2", Platform = 1 };
        pd = new ProductDetail() {  };
        p.ProductDetail = pd;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        context.SaveChanges();
    }
}