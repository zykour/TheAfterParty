using System.Data.Entity;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Entities;

public class EFDbInit : DropCreateDatabaseAlways<EFDbContext>
{
    protected override void Seed(EFDbContext context)
    {
        PerformInitialSetup(context);
        base.Seed(context);
    }
    public void PerformInitialSetup(EFDbContext context)
    {
        Listing l = new Listing() { ListingName = "Sol Survivor", ListingPrice = 5 };
        Product p = new Product() { AppID = 45000, ProductName = "Sol_Survivor", Platform = 1 };
        ProductDetail pd = new ProductDetail() { HasAchievements = true, HasControllerSupport = false, HasTradingCards = true, IsCoop = true, IsLocalCoop = false, IsMultiplayer = true, IsSinglePlayer = true };
        p.ProductDetail = pd;
        pd.Product = p;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        l = new Listing() { ListingName = "Zero_Gear", ListingPrice = 3 };
        p = new Product() { AppID = 18820, ProductName = "Zero_Gear", Platform = 1 };
        pd = new ProductDetail() { HasAchievements = true, HasControllerSupport = true, HasTradingCards = true, IsCoop = true, IsLocalCoop = true, IsMultiplayer = true, IsSinglePlayer = true };
        p.ProductDetail = pd;
        pd.Product = p;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        l = new Listing() { ListingName = "Rocket_League", ListingPrice = 40 };
        p = new Product() { AppID = 252950, ProductName = "Rocket_League", Platform = 1 };
        pd = new ProductDetail() { HasAchievements = true, HasControllerSupport = true, HasTradingCards = true, IsCoop = true, IsLocalCoop = true, IsMultiplayer = true, IsSinglePlayer = true };
        p.ProductDetail = pd;
        pd.Product = p;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        l = new Listing() { ListingName = "Tabletop_Simulator", ListingPrice = 25 };
        p = new Product() { AppID = 286160, ProductName = "Tabletop_Simulator", Platform = 1 };
        pd = new ProductDetail() { HasAchievements = true, HasControllerSupport = true, HasTradingCards = true, IsCoop = true, IsLocalCoop = false, IsMultiplayer = true, IsSinglePlayer = true };
        p.ProductDetail = pd;
        pd.Product = p;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        l = new Listing() { ListingName = "Warhammer:_End_Times_-_Vermintide", ListingPrice = 60 };
        p = new Product() { AppID = 235540, ProductName = "Warhammer:_End_Times_-_Vermintide", Platform = 1 };
        pd = new ProductDetail() { HasAchievements = true, HasControllerSupport = false, HasTradingCards = true, IsCoop = true, IsLocalCoop = false, IsMultiplayer = false, IsSinglePlayer = true };
        p.ProductDetail = pd;
        pd.Product = p;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        l = new Listing() { ListingName = "Portal_2", ListingPrice = 15 };
        p = new Product() { AppID = 620, ProductName = "Portal_2", Platform = 1 };
        pd = new ProductDetail() { HasAchievements = true, HasControllerSupport = true, HasTradingCards = true, IsCoop = true, IsLocalCoop = true, IsMultiplayer = false, IsSinglePlayer = true };
        p.ProductDetail = pd;
        pd.Product = p;
        l.Product = p;
        p.Listing = l;
        context.Listings.Add(l);
        context.Products.Add(p);
        context.ProductDetails.Add(pd);

        context.SaveChanges();
    }
}