using System.Data.Entity;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;

public class DbInit : DropCreateDatabaseAlways<AppIdentityDbContext>
{
    protected override void Seed(AppIdentityDbContext context)
    {
        PerformInitialSetup(context);
        base.Seed(context);
    }

    public void PerformInitialSetup(AppIdentityDbContext context)
    {
        IUnitOfWork unitOfWork = new UnitOfWork(context);
        IAuctionRepository auctionRepository = new AuctionRepository(unitOfWork);
        ICouponRepository couponRepository = new CouponRepository(unitOfWork);
        IGiveawayRepository giveawayRepository = new GiveawayRepository(unitOfWork);
        IListingRepository listingRepository = new ListingRepository(unitOfWork);
        IObjectiveRepository objectiveRepository = new ObjectiveRepository(unitOfWork);
        IPrizeRepository prizeRepository = new PrizeRepository(unitOfWork);
        IUserRepository userRepository = new UserRepository(unitOfWork);

        AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
        IdentityRole role = new IdentityRole();
        role.Name = "Member";
        IdentityRole admin = new IdentityRole();
        admin.Name = "Admin";

        context.Roles.Add(role);
        context.Roles.Add(admin);
        unitOfWork.Save();

        AppUser monu = new AppUser { UserSteamID = 76561198030277114, UserName = "Monukai", Email = "monu.kai@example.com", Balance = 152, IsPrivateWishlist = true };
        monu.AddOwnedGame(new OwnedGame(45000));
        monu.AddOwnedGame(new OwnedGame(620));
        monu.Nickname = "MONU";

        userMgr.Create(monu);
        userMgr.Create(new AppUser { UserSteamID = 76561198019064906, UserName = "Lucky", Email = "luckyboy@example.com", Balance = 75, IsPrivateWishlist = false, Nickname = "LUCKY" });
        userMgr.Create(new AppUser { UserSteamID = 76561197962202166, UserName = "Don_Vino", Balance = 50, IsPrivateWishlist = false, Nickname = "VINO" });
        userMgr.Create(new AppUser("Lina", 420, false, 76561198040771781) { Nickname = "LINA" });
        userMgr.Create(new AppUser { UserSteamID = 76561198038935514, UserName = "Wesley", Email = "wbarton@example.com", Balance = 100, IsPrivateWishlist = false, Nickname = "WES" });

        unitOfWork.Save();

        userMgr.AddToRole(monu.Id, "Admin");

        DateTime dateAdded = DateTime.Now;

        Tag rpg = new Tag("RPG");
        Tag puzzle = new Tag("Puzzle");
        Tag action = new Tag("Action");
        Tag racing = new Tag("Racing");
        Tag td = new Tag("Tower Defense");
        Tag sim = new Tag("Simulation");

        ProductCategory sp = new ProductCategory("Singleplayer");
        ProductCategory mp = new ProductCategory("Multiplayer");
        ProductCategory cp = new ProductCategory("Co-op");

        Platform steam = new Platform("Steam", "http://store.steampowered.com");
        steam.HasAppID = true;

        Listing l = new Listing("Sol Survivor", 5, dateAdded);
        Product p = new Product(45000, "Sol Survivor");
        ProductDetail pd = new ProductDetail() { };
        p.AddProductDetail(pd);
        l.AddProduct(p);
        l.AddPlatform(steam);
        ProductKey pk = new ProductKey("ASD09-SDF7A-9D70S");
        l.AddProductKey(pk);
        pk = new ProductKey("FSD07-SDFSD-ZB9S9");
        l.AddProductKey(pk);
        l.Product.AddTag(td);
        l.Product.AddProductCategory(sp);
        l.Product.AddProductCategory(cp);

        listingRepository.InsertListing(l);
        unitOfWork.Save();

        l = new Listing("Zero Gear", 3, dateAdded);
        p = new Product(18820, "Zero Gear");
        pd = new ProductDetail() {};
        p.AddProductDetail(pd);
        l.AddPlatform(steam);
        l.AddProduct(p);
        pk = new ProductKey("4A07B-SDS8N-MANSB");
        l.AddProductKey(pk);
        l.Product.AddTag(racing);
        l.Product.AddTag(action);
        l.Product.AddProductCategory(sp);
        l.Product.AddProductCategory(mp);

        listingRepository.InsertListing(l);
        unitOfWork.Save();

        l = new Listing("Rocket League", 40, dateAdded);
        p = new Product(252950, "Rocket League");
        pd = new ProductDetail() { };
        p.AddProductDetail(pd);
        l.AddPlatform(steam);
        l.AddProduct(p);
        pk = new ProductKey("42B00-SD9Z8-BSBWN");
        l.AddProductKey(pk);
        pk = new ProductKey("NMSMW-AS9B8-SJZJW");
        l.AddProductKey(pk);
        l.Product.AddTag(racing);
        l.Product.AddTag(action);
        l.Product.AddProductCategory(sp);
        l.Product.AddProductCategory(mp);

        listingRepository.InsertListing(l);
        unitOfWork.Save();

        monu.AddListingBlacklistEntry(l);
        userMgr.Update(monu);
        unitOfWork.Save();

        DiscountedListing dl = new DiscountedListing() { DailyDeal = true, ItemDiscountPercent = 25, ItemSaleExpiry = DateTime.Today, WeeklyDeal = false };

        l = new Listing("Tabletop Simulator", 25, dateAdded);
        p = new Product(286160, "Tabletop Simulator");
        pd = new ProductDetail() { };
        p.AddProductDetail(pd);
        l.AddPlatform(steam);
        l.AddProduct(p);
        pk = new ProductKey("W0B0Z-SHDFM-AW88W");
        l.AddProductKey(pk);
        pk = new ProductKey("N0SHZ-S9D9F-BHZHZ");
        l.AddProductKey(pk);
        l.AddDiscountedListing(dl);
        l.Product.AddTag(sim);
        l.Product.AddProductCategory(sp);
        l.Product.AddProductCategory(mp);
        l.Product.AddProductCategory(cp);

        listingRepository.InsertListing(l);
        unitOfWork.Save();

        DiscountedListing dl2 = new DiscountedListing() { DailyDeal = true, ItemDiscountPercent = 25, ItemSaleExpiry = DateTime.Today, WeeklyDeal = false };
        l = new Listing("Warhammer: The End Times - Vermintide", 60, dateAdded);
        p = new Product(235540, "Warhammer: The End Times - Vermintide");
        pd = new ProductDetail() { };
        p.AddProductDetail(pd);
        l.AddPlatform(steam);
        l.AddProduct(p);
        pk = new ProductKey("BAH0Z-29GGU-AQW2M");
        l.AddProductKey(pk);
        pk = new ProductKey("YXUB0-SYWBZM-S8CYB");
        l.AddProductKey(pk);
        l.AddDiscountedListing(dl2);
        DiscountedListing dl3 = new DiscountedListing() { DailyDeal = false, ItemDiscountPercent = 50, ItemSaleExpiry = DateTime.Today, WeeklyDeal = true };
        l.AddDiscountedListing(dl3);
        l.Product.AddTag(rpg);
        l.Product.AddTag(action);
        l.Product.AddProductCategory(sp);
        l.Product.AddProductCategory(cp);

        listingRepository.InsertListing(l);
        unitOfWork.Save();

        l = new Listing("Portal 2", 15, dateAdded);
        p = new Product(620, "Portal 2");
        pd = new ProductDetail() {  };
        p.AddProductDetail(pd);
        l.AddPlatform(steam);
        l.AddProduct(p);
        pk = new ProductKey("N0WHE-ATL3Y-ISBST");
        l.AddProductKey(pk);
        pk = new ProductKey("GL4D0-S1S7H-EB3ST");
        l.AddProductKey(pk);
        l.Product.AddTag(puzzle);
        l.Product.AddTag(action);
        l.Product.AddProductCategory(sp);
        l.Product.AddProductCategory(cp);

        listingRepository.InsertListing(l);
        unitOfWork.Save();
        
        Platform gog = new Platform("GOG Galaxy", "http://www.gog.com/");

        l = new Listing("The Witcher 3", 75, dateAdded);
        p = new Product("The Witcher 3");
        pd = new ProductDetail();
        p.AddProductDetail(pd);
        l.AddPlatform(gog);
        l.AddProduct(p);
        pk = new ProductKey(true, "");
        l.AddProductKey(pk);
        pk = new ProductKey("WITCH-ER3KE-Y1234");
        l.AddProductKey(pk);
        pk = new ProductKey("WITCH-ER3KE-Y5678");
        l.AddProductKey(pk);
        l.Product.AddTag(action);
        l.Product.AddTag(rpg);
        l.Product.AddProductCategory(sp);
        
        listingRepository.InsertListing(l);
        unitOfWork.Save();
    }
}