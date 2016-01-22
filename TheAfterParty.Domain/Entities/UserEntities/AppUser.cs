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
        public AppUser(string name, int balance, bool privateWishlist, Int64 steamId) : this()
        {
            UserSteamID = steamId;
            UserName = name;
            Balance = balance;
            IsPrivateWishlist = privateWishlist;
        }
        public AppUser()
        {
            AuctionBids = new HashSet<AuctionBid>();
            BalanceEntries = new HashSet<BalanceEntry>();
            GiveawayEntries = new HashSet<GiveawayEntry>();
            CreatedGiveaways = new HashSet<Giveaway>();
            Orders = new HashSet<Order>();
            ClaimedProductKeys = new HashSet<ClaimedProductKey>();
            ReceivedGifts = new HashSet<Gift>();
            SentGifts = new HashSet<Gift>();
            WishlistEntries = new HashSet<WishlistEntry>();
            UserNotifications = new HashSet<UserNotification>();
            ReceivedMail = new HashSet<Mail>();
            SentMail = new HashSet<Mail>();
            ListingComments = new HashSet<ListingComment>();
            ProductReviews = new HashSet<ProductReview>();
            Tags = new HashSet<Tag>();
            ShoppingCartEntries = new HashSet<ShoppingCartEntry>();
            WonPrizes = new HashSet<WonPrize>();
            UserCoupons = new HashSet<UserCoupon>();
            OwnedGames = new HashSet<OwnedGame>();
        }
        
        // the 64bit UserID representing the users on this site
        [Required]
        public Int64 UserSteamID { get; set; }
        public SteamID GetAsSteamID()
        {
            return new SteamKit2.SteamID((ulong)UserSteamID);
        }

        // denotes the users balance (if any)
        public int Balance { get; set; }
        public int ReservedBalance()
        {
            int reservedPoints = 0;

            if (AuctionBids != null)
            {
                foreach (AuctionBid bid in AuctionBids)
                {
                    if (bid.Auction.EndTime.CompareTo(DateTime.Now) > 0)
                    {
                        reservedPoints += bid.BidAmount;
                    }
                }
            }

            return reservedPoints;
        }

        // is there wishlist private?
        public bool IsPrivateWishlist { get; set; }

        public byte[] AvatarData { get; set; }

        public string AvatarMimeType { get; set; }

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

        // the list of giveaways the user has entered
        public virtual ICollection<GiveawayEntry> GiveawayEntries { get; set; }

        public virtual ICollection<Giveaway> CreatedGiveaways { get; set; }

        // the list of orders the user has successfully made (cancel orders are temporary only, and appear in the shopping cart)
        public virtual ICollection<Order> Orders { get; set; }
        public bool AssertValidOrder()
        {
            if (AssertBalanceExceedsCost() && AssertQuantityOfCart())
                return true;
            else
                return false;
        }
        public String OrderSummary(int transactionId)
        {
            Order order = Orders.Where(o => o.TransactionID == transactionId).Single();

            StringBuilder summary = new StringBuilder("Order #" + order.TransactionID);

            summary.Append("\n\nSale date: " + order.SaleDate);
            summary.Append("\n\nTotal points paid: " + order.TotalSalePrice());

            return summary.ToString();
        }
        public List<String> OrderEntriesSummary(int transactionId)
        {
            Order order = Orders.Where(o => o.TransactionID == transactionId).Single();
            List<String> entrySummaries = new List<String>();

            foreach (ProductOrderEntry orderEntries in order.ProductOrderEntries)
            {
                entrySummaries.Add(orderEntries.SalePrice + "\t\t" + orderEntries.Listing.ListingName);
            }

            entrySummaries.Sort();

            return entrySummaries;
        }

        // the keys this user has gained on the site (by any means)
        public virtual ICollection<ClaimedProductKey> ClaimedProductKeys { get; set; }
        /*public ClaimedProductKey ClaimKey(Listing listing, DateTime dateClaimed, string note)
        {
            return new ClaimedProductKey(listing.RemoveProductKey(listing.ListingID), this, dateClaimed, note);
        }*/
        //---- Add a method for  adding other keys not from the productkey list (giveawa, auctions, gifts)

        // the gifts this user has received from others
        public virtual ICollection<Gift> ReceivedGifts { get; set; }

        // the gifts this user has sent from others
        public virtual ICollection<Gift> SentGifts { get; set; }

        // the entries for changes in user balance
        public virtual ICollection<BalanceEntry> BalanceEntries { get; set; }
        public void CreateBalanceEntry(string notes, int pointsAdjusted, DateTime date)
        {
            new BalanceEntry(this, notes, pointsAdjusted, date);
            Balance = Balance - pointsAdjusted;
        }

        // the items this user has wishlisted
        public virtual ICollection<WishlistEntry> WishlistEntries { get; set; }

        // the tags this user has created
        public virtual ICollection<Tag> Tags { get; set; }

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
    }
}