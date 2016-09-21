using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Store
{
    public class AddNonSteamProductsViewModel : NavModel
    {
        public AddNonSteamProductsViewModel()
        {
            AddedProducts = new List<string>();
        }
        public string Input { get; set; }
        public List<string> AddedProducts { get; set; }
    }
}