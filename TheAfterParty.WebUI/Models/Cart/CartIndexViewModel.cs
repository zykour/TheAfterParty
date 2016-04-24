using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Cart
{
    public class CartIndexViewModel : NavModel
    {
        public AppUser LoggedInUser { get; set; }
        public IEnumerable<ShoppingCartEntry> CartEntries { get; set; }
        public string ReturnUrl { get; set; }
    }
}