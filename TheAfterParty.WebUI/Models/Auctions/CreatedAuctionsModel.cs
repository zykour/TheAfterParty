using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Auctions
{
    public class CreatedAuctionsModel : NavModel
    {
        public List<Auction> CreatedAuctions { get; set; }
    }
}