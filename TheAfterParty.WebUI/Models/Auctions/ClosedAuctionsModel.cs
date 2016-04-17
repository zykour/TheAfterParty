using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Auctions
{
    public class ClosedAuctionsModel
    {
        public List<Auction> ClosedAuctions { get; set; }
    }
}