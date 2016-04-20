
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Auctions
{
    public class CreatedAuctionsModel
    {
        public List<Auction> CreatedAuctions { get; set; }
    }
}