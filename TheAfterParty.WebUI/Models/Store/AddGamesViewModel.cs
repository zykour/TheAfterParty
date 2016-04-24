﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Store
{
    public class AddGamesViewModel : NavModel
    {
        public Platform Platform { get; set; }
        public string Input { get; set; }
    }
}