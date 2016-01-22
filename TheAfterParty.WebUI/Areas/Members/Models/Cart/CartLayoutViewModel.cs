using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Areas.Members.Models.Cart
{
    public class CartLayoutViewModel
    {
        public AppUser LoggedInUser { get; set; }
    }
}