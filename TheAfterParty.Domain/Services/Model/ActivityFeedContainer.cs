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
        public ActivityFeedContainer(Order order)
        {
            ItemDate = Order.SaleDate ?? siteEpoch;
            Order = order;
        }
        
    }
}
