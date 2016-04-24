using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Giveaways
{
    public class GiveawaysModel : NavModel
    {
        public List<Giveaway> OpenGiveaways { get; set; }
    }
}