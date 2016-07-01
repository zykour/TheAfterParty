using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Cart
{
    public class PurchaseViewModel : NavModel
    {
        public Order Order { get; set; }
        // is this model being used to represent a purchase just made (true), or just showing a past order (false)
        public bool IsSuccessfulPurchase { get; set; }
    }
}