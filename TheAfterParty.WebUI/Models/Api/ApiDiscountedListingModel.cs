using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Api
{
    public class ApiDiscountedListingModel
    {
        public ApiDiscountedListingModel() { }
        public ApiDiscountedListingModel(DiscountedListing discountedListing)
        {
            DiscountedListingID = discountedListing.DiscountedListingID;
            EndDate = discountedListing.ItemSaleExpiry;
        }

        public int DiscountedListingID { get; set; }
        public DateTime EndDate { get; set; }
    }
}