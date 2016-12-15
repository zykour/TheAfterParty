﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Store
{
    public class AddEditListingViewModel : NavModel
    {
        public Listing Listing { get; set; }
    }
}