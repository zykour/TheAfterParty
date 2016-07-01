using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Services
{
    public class ActivityFeedContainer
    {
        private DateTime siteEpoch = new DateTime(2013, 9, 1);

        public DateTime ItemDate { get; set; }
        private BalanceEntry BalanceEntry { get; set; }
        private Order Order { get; set; }
        private GiveawayEntry WonGiveaway { get; set; }
        private Giveaway CreatedGiveaway { get; set; }
        // consider whether to omit ended lost auctions
        private AuctionBid AuctionBid { get; set; }
        private Auction CreatedAuction { get; set; }
        // gifts?
        private WonPrize WonPrize { get; set; }
        // listing comment?
        private ProductReview ProductReview { get; set; }
        // wishlist entry
        // user tag

        protected ActivityFeedContainer() { }
        public ActivityFeedContainer(BalanceEntry entry) : this()
        {            
            ItemDate = (entry.Date == null) ? siteEpoch : entry.Date;
            BalanceEntry = entry;
        }
        public ActivityFeedContainer(Order order) : this()
        {
            ItemDate = order.SaleDate ?? siteEpoch;
            Order = order;
        }
        public ActivityFeedContainer(GiveawayEntry entry) : this()
        {
            ItemDate = entry.Giveaway.EndDate;
            WonGiveaway = entry;
        }
        public ActivityFeedContainer(Giveaway giveaway, DateTime date) : this()
        {
            ItemDate = date;
            CreatedGiveaway = giveaway;
        }
        public ActivityFeedContainer(AuctionBid entry) : this()
        {
            ItemDate = entry.Auction.EndTime;
            AuctionBid = entry;
        }
        public ActivityFeedContainer(Auction auction, DateTime date) : this()
        {
            ItemDate = date;
            CreatedAuction = auction;
        }
        public ActivityFeedContainer(WonPrize prize) : this()
        {
            ItemDate = prize.TimeWon;
            WonPrize = prize;
        }
        public ActivityFeedContainer(ProductReview productReview)
        {
            ItemDate = productReview.PostDate;
            ProductReview = productReview;
        }

        public Object GetActivityFeedItem()
        {
            if (BalanceEntry != null)
            {
                return BalanceEntry;
            }
            else if (Order != null)
            {
                return Order;
            }
            else if (WonGiveaway != null)
            {
                return WonGiveaway;
            }
            else if (CreatedGiveaway != null)
            {
                return CreatedGiveaway;
            }
            else if (AuctionBid != null)
            {
                return AuctionBid;
            }
            else if (CreatedAuction != null)
            {
                return CreatedAuction;
            }
            else if (WonPrize != null)
            {
                return WonPrize;
            }
            else if (ProductReview != null)
            {
                return ProductReview;
            }

            return null;
        }
        
    }
}
