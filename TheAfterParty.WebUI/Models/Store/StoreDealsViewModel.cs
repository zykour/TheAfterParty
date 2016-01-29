using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Store
{
    public class StoreDealsViewModel
    {
        public string DealType { get; set; }
        public IEnumerable<Listing> ListingsWithDeals { get; set; }
        public bool IsWeekly { get; set; }
        public bool IsDaily { get; set; }
    }
}