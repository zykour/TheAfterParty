using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Api
{
    public class ApiAuctionModel
    {
        public ApiAuctionModel() { }
        public ApiAuctionModel(Auction auction)
        {
            AuctionID = auction.AuctionID;
            EndDate = auction.EndTime;
        }

        public int AuctionID { get; set; }
        public DateTime EndDate { get; set; }
    }
}