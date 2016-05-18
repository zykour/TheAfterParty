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
    }
}