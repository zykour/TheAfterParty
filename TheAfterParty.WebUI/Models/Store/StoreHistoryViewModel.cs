using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Store
{
    public class StoreHistoryViewModel : NavModel
    {
        public IEnumerable<ProductOrderEntry> PurchaseEntries { get; set; }
    }
}