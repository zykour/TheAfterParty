using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class Auction
    {
        public Auction() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuctionID { get; set; }

        // the id of the product being auctioned
        public int ListingID { get; set; }

        // the product being auctioned
        public virtual Listing Listing { get; set; }

        // if the prize is not a Product in the db (custom prize)
        public string AlternativePrize { get; set; }

        // when the auction ends
        public DateTime EndTime { get; set; }

        // a list of bids on the item
        public virtual ICollection<AuctionBid> AuctionBids { get; set; }

        // the winner of the auction (if it's over)
        public int WinnerID { get; set; }

        // the appuser object of the winner
        public virtual AppUser Winner { get; set; }

        // is the auction a silent auction
        public bool IsSilent { get; set; }

        // minimum bid amount (or starting bid) if any
        public int? MinimumBid { get; set; }

        public string CreatorID { get; set; }

        public AppUser Creator { get; set; }
    }
}
