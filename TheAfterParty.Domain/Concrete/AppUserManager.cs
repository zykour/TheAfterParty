using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using TheAfterParty.Domain.Entities;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections;
using System;

namespace TheAfterParty.Domain.Concrete
{
    public class AppUserManager : UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store) : base(store)
        {

        }

        public override IQueryable<AppUser> Users
        {
            get
            {
                return base.Users;
            }
        }
        public Task<AppUser> FindByNameSimple(string userName)
        {
            return Users.SingleOrDefaultAsync(x => x.UserName == userName);
        }
        public Task<AppUser> FindByIdAsyncWithStoreFilters(string userId)
        {
            return Users.Include(x => x.BlacklistedListings).Include(x => x.ShoppingCartEntries.Select(c => c.Listing.DiscountedListings)).Include(x => x.AuctionBids.Select(ab => ab.Auction)).FirstOrDefaultAsync(u => u.Id == userId);
        }
        
        public Task<AppUser> FindByNameAsyncWithStoreFilters(string userName)
        {
            return Users.Include(x => x.BlacklistedListings)
                        .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.DiscountedListings))
                        .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.Product))
                        .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ProductKeys))
                        .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.Platforms))
                        .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(cl => cl.DiscountedListings)))
                        .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(cl => cl.Product)))
                        .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(cl => cl.Platforms)))
                        .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(cl => cl.ProductKeys)))
                        .Include(x => x.AuctionBids.Select(ab => ab.Auction.AuctionBids))
                        .Include(x => x.WishlistEntries)//.Select(we => we.Listing).Select(l => l.Product))
                        .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public AppUser FindByNameAsyncWithStoreFiltersSynch(string userName)
        {
            return Users.Include(x => x.BlacklistedListings)
                        .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.DiscountedListings))
                        .Include(x => x.AuctionBids.Select(ab => ab.Auction))
                        .Include(x => x.WishlistEntries)//.Select(we => we.Listing).Select(l => l.Product))
                        .FirstOrDefault(u => u.UserName == userName);
        }

        public Task<AppUser> FindByNameAsyncWithBlacklist(string userName)
        {
            return Users.Include(x => x.BlacklistedListings).FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public Task<AppUser> FindByNameAsyncWithAuctions(string userName)
        {
            return Users.Include(x => x.AuctionBids.Select(ab => ab.Auction)).FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public Task<AppUser> FindByNameAsyncWithActivityProperties(string userName)
        {
            return Users
                    .Include(x => x.GiveawayEntries)
                    .Include(x => x.CreatedGiveaways)
                    .Include(x => x.Auctions)
                    .Include(x => x.WonAuctions)
                    .Include(x => x.AuctionBids)
                    .Include(x => x.Orders.Select(o => o.ProductOrderEntries))
                    .Include(x => x.BalanceEntries)
                    //.Include(x => x.WonPrizes)
                    //.Include(x => x.ProductReviews)
                    .SingleOrDefaultAsync(x => x.UserName == userName);
        }

        public Task<AppUser> FindByNameAsyncWithCartAndOpenAuctionBids(string userName)
        {
            return Users
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.DiscountedListings))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ProductKeys.Select(pk => pk.Listing)))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.Platforms))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.Product.Listings.Select(d => d.ChildListings)))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(d => d.DiscountedListings)))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(d => d.ProductKeys)))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(d => d.Platforms)))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(d => d.Product.Listings.Select(f => f.ChildListings))))
                .Include(x => x.AuctionBids.Select(a => a.Auction.AuctionBids))
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
        public Task<AppUser> FindByNameAsyncWithCartOpenAuctionBidsAndOrders(string userName)
        {
            return Users
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.DiscountedListings))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ProductKeys.Select(pk => pk.Listing)))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.Platforms))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.Product.Listings.Select(d => d.ChildListings)))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(d => d.DiscountedListings)))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(d => d.ProductKeys)))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(d => d.Platforms)))
                .Include(x => x.ShoppingCartEntries.Select(c => c.Listing.ChildListings.Select(d => d.Product.Listings.Select(f => f.ChildListings))))
                .Include(x => x.AuctionBids.Select(a => a.Auction.AuctionBids))
                .Include(x => x.Orders.Select(c => c.ProductOrderEntries.Select(d => d.Listing.Product.Listings.Select(f => f.ChildListings))))
                .Include(x => x.Orders.Select(c => c.ProductOrderEntries.Select(d => d.Listing.ChildListings.Select(f => f.Product.Listings.Select(g => g.ChildListings)))))
                .Include(x => x.Orders.Select(c => c.ProductOrderEntries.Select(d => d.Listing.Platforms)))
                .Include(x => x.Orders.Select(c => c.ProductOrderEntries.Select(d => d.ClaimedProductKeys)))
                .Include(x => x.ClaimedProductKeys)
                .Include(x => x.BalanceEntries)
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            AppIdentityDbContext db = context.Get<AppIdentityDbContext>();
            AppUserManager manager = new AppUserManager(new UserStore<AppUser>(db));

            return manager;
        }
    }
}