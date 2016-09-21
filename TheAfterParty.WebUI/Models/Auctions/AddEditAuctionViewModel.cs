using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Auctions
{
    public class AddEditAuctionViewModel : NavModel
    {
        public AddEditAuctionViewModel()
        {
            Auction = new Auction();
        }
        
        public Auction Auction { get; set; }
    }
}