using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Account
{
    public class AccountKeysModel
    {
        public List<ClaimedProductKey> Keys { get; set; }
    }
}