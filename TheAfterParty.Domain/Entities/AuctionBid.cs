using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class AuctionBid
    {
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AuctionID { get; set; }

        // the bid number of this bid
        [Key, Column(Order = 2), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BidNumber { get; set; }

        // the auction info for the auction this bid is associated with
        public virtual Auction Auction { get; set; }

        // the id of the bidder
        public string UserID { get; set; }

        // the appuser who made this bid
        public virtual AppUser AppUser { get; set; }

        // the date of this bid
        public DateTime BidDate { get; set; }

        // the amount of this bid
        public int BidAmount { get; set; }
    }
}
