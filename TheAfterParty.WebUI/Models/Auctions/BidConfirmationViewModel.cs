using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Auctions
{
    public class BidConfirmationViewModel : NavModel
    {
        public BidConfirmationViewModel()
        {
            AuctionBid = new AuctionBid();
            IsConfirmed = false;
        }

        public bool IsConfirmed { get; set; }
        public Auction Auction { get; set; }
        public AuctionBid AuctionBid { get; set; }
    }
}