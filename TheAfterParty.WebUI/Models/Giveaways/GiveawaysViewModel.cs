using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Giveaways
{
    public class GiveawaysViewModel : NavModel
    {
        public GiveawaysViewModel()
        {
            Giveaways = new List<Giveaway>();
            Title = "";
            SearchText = "";
        }

        public List<Giveaway> Giveaways { get; set; }
        public string Title { get; set; }

        public string SearchText { get; set; }
        
        public string ActionName { get; set; }
    }
}