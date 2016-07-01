using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class Auction
    {
        public Auction()
        {
            CreatedTime = DateTime.Now;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuctionID { get; set; }

        // the id of the product being auctioned
        public int ListingID { get; set; }

        // the product being auctioned
        public virtual Listing Listing { get; set; }

        // if the prize is not a Product in the db (custom prize)
        public string AlternativePrize { get; set; }

        public DateTime CreatedTime { get; set; }
        // when the auction ends
        public DateTime EndTime { get; set; }
        public bool IsOpen()
        {
            return EndTime > DateTime.Now;
        }

        // a list of bids on the item
        public virtual ICollection<AuctionBid> AuctionBids { get; set; }
        public void AddAuctionBid(AuctionBid auctionBid)
        {
            if (AuctionBids == null)
            {
                AuctionBids = new HashSet<AuctionBid>();
            }

            AuctionBids.Add(auctionBid);

            if (!IsSilent)
            {
                MinimumBid = (int)Math.Max(MinimumBid, auctionBid.BidAmount + Increment);
            }
        }
        public int WinningBid()
        {
            if (AuctionBids == null)
            {
                return 0;
            }

            int max = 0;

            foreach (AuctionBid bid in AuctionBids)
            {
                max = (max < bid.BidAmount) ? bid.BidAmount : max;
            }

            return max;
        }

        public int Increment { get; set; }

        // the winner of the auction (if it's over)
        public int WinnerID { get; set; }

        // the appuser object of the winner
        public virtual AppUser Winner { get; set; }
        public bool IsWinner(AppUser user)
        {
            return (Winner == null) ? false : Object.Equals(user.Id, Winner.Id);
        }

        // is the auction a silent auction
        public bool IsSilent { get; set; }

        // minimum bid amount (or starting bid) if any
        public int MinimumBid { get; set; }

        public string CreatorID { get; set; }

        public AppUser Creator { get; set; }
        public bool IsCreator(AppUser user)
        {
            return (Creator == null) ? false : Object.Equals(user.Id, Creator.Id);
        }

        public int Copies { get; set; }
    }
}
