using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using SteamKit2;
using System.Linq;
using System.Text;

namespace TheAfterParty.Domain.Entities
{ 
    public class AppUser : IdentityUser
    {
        public AppUser(string name, int balance, bool privateWishlist, Int64 steamId)
        {
            UserSteamID = steamId;
            UserName = name;
            Balance = balance;
            IsPrivateWishlist = privateWishlist;
            MemberSince = DateTime.Now;
            LastLogon = DateTime.Now;
        }
        public AppUser() {  }

        // the 64bit UserID representing the users on this site
        [Required]
        public Int64 UserSteamID { get; set; }
        public SteamID GetAsSteamID()
        {
            return new SteamKit2.SteamID((ulong)UserSteamID);
        }

        public DateTime MemberSince { get; set; }
        public DateTime LastLogon { get; set; }

        // for my use as admin, the goal of nicknames is to make it easier for me to write up parseable input for admin commands rather than writing out full usernames or IDs
        public string Nickname { get; set; }

        // denotes the users balance (if any)
        public int Balance { get; set; }
        public int ReservedBalance()
        {
            return GetPublicAuctionReservedBalance() + GetSilentAuctionReservedBalance();
        }

        // is there wishlist private?
        public bool IsPrivateWishlist { get; set; }

        public byte[] AvatarData { get; set; }
        public string AvatarMimeType { get; set; }
        public string SmallAvatar { get; set; }
        public string MediumAvatar { get; set; }
        public string LargeAvatar { get; set; }

        // the list of auctions the user has participated in
        public virtual ICollection<AuctionBid> AuctionBids { get; set; }
        public void AddAuctionBid(AuctionBid auctionBid)
        {
            auctionBid.AppUser = this;
            AuctionBids.Add(auctionBid);
        }
        public AuctionBid RemoveAuctionBid(AuctionBid auctionBid)
        {
            if (AuctionBids.Contains(auctionBid))
            {
                AuctionBids.Remove(auctionBid);
                return auctionBid;
            }

            return null;
        }
        public int GetSilentAuctionReservedBalance()
        {
            if (AuctionBids == null)
            {
                return 0;
            }

            ICollection<AuctionBid> openBids = AuctionBids.Where(a => a.Auction.IsSilent == true && a.Auction.IsOpen()).ToList();

            if (openBids.Count == 0)
            {
                return 0;
            }

            return openBids.Sum(a => a.BidAmount);
        }
        public int GetPublicAuctionReservedBalance()
        {
            if (AuctionBids == null)
            {
                return 0;
            }

            ICollection<AuctionBid> openBids = AuctionBids.Where(a => a.Auction.IsSilent == false && a.Auction.IsOpen() && a.BidAmount == a.Auction.WinningBid()).ToList();

            if (openBids.Count == 0)
            {
                return 0;
            }

            return openBids.Sum(a => a.BidAmount);
        }

        // the list of giveaways the user has entered
        public virtual ICollection<GiveawayEntry> GiveawayEntries { get; set; }

        public virtual ICollection<Giveaway> CreatedGiveaways { get; set; }

        // the list of orders the user has successfully made (cancel orders are temporary only, and appear in the shopping cart)
        public virtual ICollection<Order> Orders { get; set; }
        public void AddOrder(Order order)
        {
            if (Orders == null)
            {
                Orders = new HashSet<Order>();
            }

            Orders.Add(order);
        }
        public bool AssertValidOrder()
        {
            if (AssertBalanceExceedsCost() && AssertQuantityOfCart())
                return true;
            else
                return false;
        }
        public String OrderSummary(int transactionId)
        {
            Order order = Orders.Where(o => o.OrderID == transactionId).Single();

            StringBuilder summary = new StringBuilder("Order #" + order.OrderID);

            summary.Append("\n\nSale date: " + order.SaleDate);
            summary.Append("\n\nTotal points paid: " + order.TotalSalePrice());

            return summary.ToString();
        }
        public List<String> OrderEntriesSummary(int transactionId)
        {
            Order order = Orders.Where(o => o.OrderID == transactionId).Single();
            List<String> entrySummaries = new List<String>();

            foreach (ProductOrderEntry orderEntries in order.ProductOrderEntries)
            {
                entrySummaries.Add(orderEntries.SalePrice + "\t\t" + orderEntries.Listing.ListingName);
            }

            entrySummaries.Sort();

            return entrySummaries;
        }

        public virtual ICollection<Auction> Auctions { get; set; }

        // the keys this user has gained on the site (by any means)
        public virtual ICollection<ClaimedProductKey> ClaimedProductKeys { get; set; }
        public void AddClaimedProductKey(ClaimedProductKey key)
        {
            if (ClaimedProductKeys == null)
            {
                ClaimedProductKeys = new HashSet<ClaimedProductKey>();
            }

            ClaimedProductKeys.Add(key);
        }

        // the gifts this user has received from others
        public virtual ICollection<Gift> ReceivedGifts { get; set; }

        // the gifts this user has sent from others
        public virtual ICollection<Gift> SentGifts { get; set; }

        // the entries for changes in user balance
        public virtual ICollection<BalanceEntry> BalanceEntries { get; set; }
        public void CreateBalanceEntry(string notes, int pointsAdjusted, DateTime date)
        {
            BalanceEntry entry = new BalanceEntry(this, notes, pointsAdjusted, date);

            if (BalanceEntries == null)
            {
                BalanceEntries = new HashSet<BalanceEntry>();
            }

            BalanceEntries.Add(entry);

            Balance = Balance + pointsAdjusted;
        }
        public void AddBalanceEntry(BalanceEntry entry)
        {
            if (BalanceEntries == null)
            {
                BalanceEntries = new HashSet<BalanceEntry>();
            }

            BalanceEntries.Add(entry);
        }

        // the items this user has wishlisted
        public virtual ICollection<WishlistEntry> WishlistEntries { get; set; }

        // the tags this user has created
        //public virtual ICollection<Tag> Tags { get; set; }

        // the reviews this user has posted (for products)
        public virtual ICollection<ProductReview> ProductReviews { get; set; }

        // the comments this user has posted (on product pages?)
        public virtual ICollection<ListingComment> ListingComments { get; set; }

        // notifications for this user
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
        public void CreateUserNotification(Mail mail)
        {
            new UserNotification(mail.AppUserReceiver, mail.DateSent, "New mail from " + mail.AppUserReceiver.UserName);
        }

        // mail received by the user
        public virtual ICollection<Mail> ReceivedMail { get; set; }
        public Mail CreateMail(AppUser sender, string heading, string body)
        {
            Mail mail = new Mail(this, sender, heading, body, DateTime.Now);
            CreateUserNotification(mail);

            return mail;
        }

        // mail sent by the user
        public virtual ICollection<Mail> SentMail { get; set; }

        // the coupons that this user has available
        public virtual ICollection<UserCoupon> UserCoupons { get; set; }

        public virtual ICollection<WonPrize> WonPrizes { get; set; }

        public virtual ICollection<OwnedGame> OwnedGames { get; set; }
        public bool OwnsListing(Listing listing)
        {
            if (OwnedGames == null) return false;
            return (OwnedGames.Where(o => o.AppID == listing.Product.AppID).Count() >= 1) ? true : false;
        }
        public bool OwnsProduct(Product product)
        {
            if (OwnedGames == null) return false;
            return (OwnedGames.Where(o => o.AppID == product.AppID).Count() >= 1) ? true : false;
        }
        public void AddOwnedGame(OwnedGame ownedGame)
        {
            if (OwnedGames == null)
            {
                OwnedGames = new HashSet<OwnedGame>();
            }

            OwnedGames.Add(ownedGame);
            ownedGame.AppUser = this;
        }

        public virtual ICollection<ShoppingCartEntry> ShoppingCartEntries { get; set; }
        public ShoppingCartEntry AddShoppingCartEntry(Listing listing, int quantity = 1)
        {
            ShoppingCartEntry entry = ShoppingCartEntries.Where(e => e.ListingID == listing.ListingID).SingleOrDefault(); // && Object.Equals(e.UserID, Id)).SingleOrDefault();

            if (entry == null)
            {
                entry = new ShoppingCartEntry(this, listing, quantity);
                ShoppingCartEntries.Add(entry);
            }
            else
            {
                entry.Quantity++;
            }

            return entry;
        }
        public void ReduceEntry(int listingId, int quantityDeduction)
        {
            ShoppingCartEntry cartEntry = ShoppingCartEntries.Where(entry => Object.Equals(entry.UserID, this.Id) && entry.ListingID == listingId).Single();
            cartEntry.Quantity -= quantityDeduction;

            ShoppingCartEntries.Remove(cartEntry);

            // removing all the quantity is essentially a delete, only re-add if there is some quantity left
            if (cartEntry.Quantity > 0)
            {
                ShoppingCartEntries.Add(cartEntry);
            }
        }
        public void RemoveEntry(int listingId)
        {
            ShoppingCartEntry cartEntry = ShoppingCartEntries.Where(entry => Object.Equals(entry.UserID, this.Id) && entry.ListingID == listingId).Single();
            ShoppingCartEntries.Remove(cartEntry);
        }
        public void RemoveAllCartEntries()
        {
            // Get entries associated with this UserID
            ICollection<ShoppingCartEntry> myEntries = ShoppingCartEntries.Where(entry => Object.Equals(entry.UserID, this.Id)).ToList();

            foreach (ShoppingCartEntry entry in myEntries)
            {
                ShoppingCartEntries.Remove(entry);
            }
        }
        public bool AssertBalanceExceedsCost()
        {
            if (GetCartTotal() > (Balance - ReservedBalance()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool AssertQuantityOfCart()
        {
            // Get entries associated with this UserID
            ICollection<ShoppingCartEntry> myEntries = ShoppingCartEntries.Where(entry => Object.Equals(entry.UserID, this.Id)).ToList();

            foreach (ShoppingCartEntry entry in myEntries)
            {
                if (entry.Quantity > entry.Listing.Quantity)
                {
                    return false;
                }
            }

            Dictionary<Listing, int> quantityDict = new Dictionary<Listing, int>();

            foreach (ShoppingCartEntry entry in myEntries.Where(e => e.Listing.IsComplex()).ToList())
            {
                if (entry.Quantity > entry.Listing.ListingKeysQuantity())
                {
                    foreach (Listing listing in entry.Listing.ChildListings)
                    {
                        quantityDict[listing] = (entry.Quantity - entry.Listing.ListingKeysQuantity());
                    }
                }
            }
            foreach (ShoppingCartEntry entry in myEntries.Where(e => !e.Listing.IsComplex()).ToList())
            {
                if (quantityDict.ContainsKey(entry.Listing))
                {
                    quantityDict[entry.Listing] += entry.Quantity;
                }
                else
                {
                    quantityDict[entry.Listing] = entry.Quantity;
                }
            }
            foreach (Listing listing in quantityDict.Keys)
            {
                if (listing.Quantity < quantityDict[listing])
                {
                    return false;
                }
            }

                return true;
        }
        public int GetCartTotal()
        {
            // Get entries associated with this UserID
            ICollection<ShoppingCartEntry> myEntries = ShoppingCartEntries.Where(entry => Object.Equals(entry.UserID, this.Id)).ToList();

            // Totals are always integer values
            int total = 0;

            foreach (ShoppingCartEntry entry in myEntries)
            {
                // SaleOrDefaultPrice returns the price, if it's on sale it's the discounted price, otherwise the base price
                total += entry.Listing.SaleOrDefaultPrice() * entry.Quantity;
            }

            return total;
        }
        public int GetCartQuantity()
        {
            int total = 0;

            // Get entries associated with this UserID
            ICollection<ShoppingCartEntry> myEntries = ShoppingCartEntries.Where(entry => Object.Equals(entry.UserID, this.Id)).ToList();
            
            foreach (ShoppingCartEntry entry in myEntries)
            {
                total += entry.Quantity;
            }

            return total;
        }

        public virtual ICollection<UserTag> UserTags { get; set; }

        public virtual ICollection<Listing> BlacklistedListings { get; set; }
        public void AddListingBlacklistEntry(Listing listing)
        {
            if (BlacklistedListings == null)
            {
                BlacklistedListings = new HashSet<Listing>();
            }

            if (listing.UsersBlacklist == null)
            {
                listing.UsersBlacklist = new HashSet<AppUser>();
            }

            BlacklistedListings.Add(listing);
            listing.UsersBlacklist.Add(this);
        }
    }
}