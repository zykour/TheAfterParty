using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Store
{
    public class AddEditDiscountedListingViewModel : NavModel
    {
        public AddEditDiscountedListingViewModel()
        {
            DiscountedListing = new DiscountedListing();
        }

        public DiscountedListing DiscountedListing { get; set; }
        public int DaysDealLast { get; set; }
    }
}