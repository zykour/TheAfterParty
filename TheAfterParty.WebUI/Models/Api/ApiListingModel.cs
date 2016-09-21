using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Api
{
    public class ApiListingModel
    {
        public ApiListingModel(Listing listing)
        {
            ListingName = listing.ListingName;
            ListingPrice = listing.ListingPrice;
            ListingID = listing.ListingID;
            Quantity = listing.Quantity;
            DailyDeal = listing.HasDailyDeal();
            WeeklyDeal = listing.HasWeeklyDeal();
            OtherDeal = listing.HasOtherDeal();
            SalePercent = listing.GetSalePercent();
            SalePrice = listing.SaleOrDefaultPrice();
            EarliestExpiry = listing.GetEarliestExpiry(false, false);
        }
        public ApiListingModel()
        {
            ListingName = String.Empty;
            EarliestExpiry = null;
        }

        public string ListingName { get; set; }
        public int ListingPrice { get; set; }
        public int ListingID { get; set; }
        public int Quantity { get; set; }
        public bool DailyDeal { get; set; }
        public bool WeeklyDeal { get; set; }
        public bool OtherDeal { get; set; }
        public double SalePercent { get; set; }
        public int SalePrice { get; set; }
        public DateTime? EarliestExpiry { get; set; }
    }
}