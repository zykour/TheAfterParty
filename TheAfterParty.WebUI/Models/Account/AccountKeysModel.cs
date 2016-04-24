using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Account
{
    public class AccountKeysModel : NavModel
    {
        public List<ClaimedProductKey> Keys { get; set; }
    }
}