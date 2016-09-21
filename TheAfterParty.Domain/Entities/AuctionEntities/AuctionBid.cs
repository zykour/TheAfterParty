using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class AuctionBid
    {
        public AuctionBid() { }

        [Key]
        public int AuctionBidID { get; set; }

        public int AuctionID { get; set; }

        // the bid number of this bid
        public int BidNumber { get; set; }

        // the auction info for the auction this bid is associated with
        public virtual Auction Auction { get; set; }
        public void AddAuction(Auction auction)
        {
            Auction = auction;

            if (Auction.AuctionBids == null)
            {
                Auction.AuctionBids = new HashSet<AuctionBid>();
            }

            Auction.AuctionBids.Add(this);
        }

        // the id of the bidder
        public string UserID { get; set; }

        // the appuser who made this bid
        public virtual AppUser AppUser { get; set; }
        public void AddAppUser(AppUser user)
        {
            if (user.AuctionBids == null)
            {
                user.AuctionBids = new HashSet<AuctionBid>();
            }

            user.AuctionBids.Add(this);
            this.AppUser = user;
        }

        // the date of this bid
        public DateTime BidDate { get; set; }

        // the amount of this bid
        public int BidAmount { get; set; }
    }
}
