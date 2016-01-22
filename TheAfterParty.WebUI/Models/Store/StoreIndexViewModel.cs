using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Store
{
    public class StoreIndexViewModel
    {
        public IEnumerable<Listing> StoreListings { get; set; }
    }
}