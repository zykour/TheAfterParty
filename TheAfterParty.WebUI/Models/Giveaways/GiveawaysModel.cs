using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Giveaways
{
    public class GiveawaysModel
    {
        public List<Giveaway> OpenGiveaways { get; set; }
    }
}