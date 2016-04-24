﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Giveaways
{
    public class CreatedGiveawaysModel : NavModel
    {
        public List<Giveaway> CreatedGiveaways { get; set; }
    }
}