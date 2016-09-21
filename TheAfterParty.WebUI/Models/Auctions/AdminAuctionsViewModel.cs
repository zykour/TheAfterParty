using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Auctions
{
    public class AdminAuctionsViewModel : NavModel
    {
        public IEnumerable<Auction> Auctions { get; set; }
    }
}