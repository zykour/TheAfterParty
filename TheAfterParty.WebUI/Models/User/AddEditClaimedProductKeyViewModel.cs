﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.User
{
    public class AddEditClaimedProductKeyViewModel : NavModel
    {
        public ClaimedProductKey ClaimedProductKey { get; set; }
        public string UserID { get; set; }
        public string UserNickName { get; set; }
    }
}