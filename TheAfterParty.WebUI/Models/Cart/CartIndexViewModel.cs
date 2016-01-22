using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Cart
{
    public class CartIndexViewModel
    {
        public AppUser LoggedInUser { get; set; }
        public IEnumerable<ShoppingCartEntry> CartEntries { get; set; }
        public string ReturnUrl { get; set; }
    }
}