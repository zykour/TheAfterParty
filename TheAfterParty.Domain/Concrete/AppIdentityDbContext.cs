using System.Data.Entity;
using TheAfterParty.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace TheAfterParty.Domain.Concrete
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AuctionBid> AuctionBids { get; set; }
        public DbSet<BalanceEntry> BalanceEntries { get; set; }
        public DbSet<BoostedObjective> BoostedObjectives { get; set; }
        public DbSet<ClaimedProductKey> ClaimedProductKeys { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<DiscountedListing> DiscountedListings { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<Giveaway> Giveaways { get; set; }
        public DbSet<GiveawayEntry> GiveawayEntries { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Mail> Mail { get; set; }
        public DbSet<MappedListing> MappedListings { get; set; }
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<ObjectiveGameMapping> ObjectiveGameMappings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OwnedGame> OwnedGames { get; set; }
        public DbSet<Prize> Prizes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ListingComment> ListingComments { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ProductKey> ProductKeys { get; set; }
        public DbSet<ProductOrderEntry> ProductOrderEntries { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ShoppingCartEntry> ShoppingCartEntries { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<WishlistEntry> WishlistEntries { get; set; }
        public DbSet<WonPrize> WonPrizes { get; set; }

        public AppIdentityDbContext() : base("AppIdentityDbContext")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        static AppIdentityDbContext()
        {
            Database.SetInitializer<AppIdentityDbContext>(new EFDbInit());
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
                
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AppUser ---
            modelBuilder.Entity<Gift>().HasRequired<AppUser>(g => g.AppUserReceiver).WithMany(au => au.ReceivedGifts).HasForeignKey(g => g.ReceiverID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Gift>().HasRequired<AppUser>(g => g.AppUserSender).WithMany(au => au.SentGifts).HasForeignKey(g => g.SenderID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Mail>().HasRequired<AppUser>(m => m.AppUserReceiver).WithMany(au => au.ReceivedMail).HasForeignKey(m => m.ReceiverUserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Mail>().HasRequired<AppUser>(m => m.AppUserSender).WithMany(au => au.SentMail).HasForeignKey(m => m.SenderUserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Giveaway>().HasRequired<AppUser>(g => g.AppUserCreator).WithMany(au => au.CreatedGiveaways).HasForeignKey(g => g.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<AuctionBid>().HasRequired<AppUser>(ab => ab.AppUser).WithMany(au => au.AuctionBids).HasForeignKey(ab => ab.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<GiveawayEntry>().HasRequired<AppUser>(ge => ge.AppUser).WithMany(au => au.GiveawayEntries).HasForeignKey(ge => ge.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Order>().HasRequired<AppUser>(o => o.AppUser).WithMany(au => au.Orders).HasForeignKey(o => o.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<ClaimedProductKey>().HasRequired<AppUser>(cpk => cpk.AppUser).WithMany(au => au.ClaimedProductKeys).HasForeignKey(cpk => cpk.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<BalanceEntry>().HasRequired<AppUser>(be => be.AppUser).WithMany(au => au.BalanceEntries).HasForeignKey(be => be.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Tag>().HasRequired<AppUser>(t => t.AppUser).WithMany(au => au.Tags).HasForeignKey(t => t.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<WishlistEntry>().HasRequired<AppUser>(we => we.AppUser).WithMany(au => au.WishlistEntries).HasForeignKey(we => we.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<ListingComment>().HasRequired<AppUser>(lc => lc.AppUser).WithMany(au => au.ListingComments).HasForeignKey(lc => lc.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<UserNotification>().HasRequired<AppUser>(un => un.AppUser).WithMany(au => au.UserNotifications).HasForeignKey(un => un.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<UserCoupon>().HasRequired<AppUser>(uc => uc.AppUser).WithMany(au => au.UserCoupons).HasForeignKey(uc => uc.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<WonPrize>().HasRequired<AppUser>(wp => wp.AppUser).WithMany(au => au.WonPrizes).HasForeignKey(wp => wp.UserID).WillCascadeOnDelete(false);
            modelBuilder.Entity<OwnedGame>().HasRequired<AppUser>(og => og.AppUser).WithMany(au => au.OwnedGames).HasForeignKey(og => og.UserID).WillCascadeOnDelete(true);
            modelBuilder.Entity<ShoppingCartEntry>().HasRequired<AppUser>(sce => sce.AppUser).WithMany(au => au.ShoppingCartEntries).HasForeignKey(sce => sce.UserID).WillCascadeOnDelete(true);

            // Auction ---
            modelBuilder.Entity<AuctionBid>().HasRequired<Auction>(ab => ab.Auction).WithMany(a => a.AuctionBids).HasForeignKey(ab => ab.AuctionID).WillCascadeOnDelete(true);

            // ClaimedProductKey ---
            modelBuilder.Entity<Gift>().HasRequired<ClaimedProductKey>(g => g.ClaimedProductKey).WithOptional(cpk => cpk.Gift).WillCascadeOnDelete(false);
            modelBuilder.Entity<ProductOrderEntry>().HasRequired<ClaimedProductKey>(poe => poe.ClaimedProductKey).WithOptional(cpk => cpk.ProductOrderEntry).WillCascadeOnDelete(false);

            // Giveaway ---
            modelBuilder.Entity<GiveawayEntry>().HasRequired<Giveaway>(ge => ge.Giveaway).WithMany(g => g.GiveawayEntries).HasForeignKey(ge => ge.GiveawayID).WillCascadeOnDelete(true);

            // Objective ---
            modelBuilder.Entity<ObjectiveGameMapping>().HasRequired<Objective>(ogm => ogm.Objective).WithMany(o => o.ObjectiveGameMappings).HasForeignKey(ogm => ogm.ObjectiveID).WillCascadeOnDelete(true);

            // Order ---
            modelBuilder.Entity<ProductOrderEntry>().HasRequired<Order>(poe => poe.Order).WithMany(o => o.ProductOrderEntries).HasForeignKey(poe => poe.TransactionID).WillCascadeOnDelete(true);

            // Prize ---
            modelBuilder.Entity<WonPrize>().HasRequired<Prize>(wp => wp.Prize).WithMany(p => p.WonPrizes).HasForeignKey(wp => wp.PrizeID).WillCascadeOnDelete(false);

            // Listing ---
            modelBuilder.Entity<ShoppingCartEntry>().HasRequired<Listing>(sce => sce.Listing).WithMany(l => l.ShoppingCartEntries).HasForeignKey(sce => sce.ListingID).WillCascadeOnDelete(true);
            modelBuilder.Entity<Auction>().HasRequired<Listing>(a => a.Listing).WithMany(l => l.Auctions).HasForeignKey(a => a.ListingID).WillCascadeOnDelete(false);
            modelBuilder.Entity<ClaimedProductKey>().HasRequired<Listing>(cpk => cpk.Listing).WithMany(l => l.ClaimedProductKeys).HasForeignKey(cpk => cpk.ListingID).WillCascadeOnDelete(false);
            modelBuilder.Entity<DiscountedListing>().HasRequired<Listing>(dl => dl.Listing).WithOptional(dl => dl.DiscountedListing).WillCascadeOnDelete(true);
            modelBuilder.Entity<Giveaway>().HasRequired<Listing>(g => g.Listing).WithMany(l => l.Giveaways).HasForeignKey(g => g.ListingID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Prize>().HasRequired<Listing>(p => p.Listing).WithOptional(l => l.Prize).WillCascadeOnDelete(true);
            modelBuilder.Entity<ListingComment>().HasRequired<Listing>(lc => lc.Listing).WithMany(l => l.ListingComments).HasForeignKey(lc => lc.ListingID).WillCascadeOnDelete(true);
            modelBuilder.Entity<ProductKey>().HasRequired<Listing>(pk => pk.Listing).WithMany(l => l.ProductKeys).HasForeignKey(pk => pk.ListingID).WillCascadeOnDelete(false);
            modelBuilder.Entity<WishlistEntry>().HasRequired<Listing>(we => we.Listing).WithMany(l => l.WishlistEntries).HasForeignKey(we => we.ListingID).WillCascadeOnDelete(false);
            modelBuilder.Entity<MappedListing>().HasRequired<Listing>(ml => ml.ChildListing).WithMany(l => l.ChildListings).HasForeignKey(ml => ml.ChildListingID).WillCascadeOnDelete(false);
            modelBuilder.Entity<MappedListing>().HasRequired<Listing>(ml => ml.ParentListing).WithMany(l => l.ParentListings).HasForeignKey(ml => ml.ParentListingID).WillCascadeOnDelete(true);
            modelBuilder.Entity<Product>().HasRequired<Listing>(p => p.Listing).WithOptional(l => l.Product).WillCascadeOnDelete(false);

            // Product ---
            modelBuilder.Entity<ProductReview>().HasRequired<Product>(pr => pr.Product).WithMany(p => p.ProductReviews).HasForeignKey(pr => pr.ProductID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Tag>().HasRequired<Product>(t => t.Product).WithMany(p => p.Tags).HasForeignKey(t => t.ProductID).WillCascadeOnDelete(true);
            modelBuilder.Entity<ObjectiveGameMapping>().HasRequired<Product>(ogm => ogm.Product).WithMany(p => p.ObjectiveGameMappings).HasForeignKey(ogm => ogm.ProductID).WillCascadeOnDelete(false);
            modelBuilder.Entity<ProductDetail>().HasMany<Product>(pd => pd.Products).WithOptional(p => p.ProductDetail).WillCascadeOnDelete(false);
        }
    }
}
