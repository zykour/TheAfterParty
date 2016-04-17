using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Giveaways
{
    public class ClosedGiveawaysModel
    {
        public List<Giveaway> ClosedGiveaways { get; set; }
    }
}