﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Store
{
    public class AddGamesViewModel
    {
        public Platform Platform { get; set; }
        public string Input { get; set; }
    }
}