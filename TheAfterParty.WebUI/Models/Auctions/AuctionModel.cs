﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Auctions
{
    public class AuctionModel : NavModel
    {
        public Auction Auction { get; set; }
        // current winner if open, final winner if closed
        public AppUser AuctionWinner { get; set; }
    }
}