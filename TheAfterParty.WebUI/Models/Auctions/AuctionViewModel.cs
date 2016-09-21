using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Auctions
{
    public class AuctionViewModel : NavModel
    {
        public AuctionViewModel()
        {
            Auction = new Auction();
            AuctionBid = new AuctionBid();
        }

        public Auction Auction { get; set; }
        public AuctionBid AuctionBid { get; set; }
    }
}