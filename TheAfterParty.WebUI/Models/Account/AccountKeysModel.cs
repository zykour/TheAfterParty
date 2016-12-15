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
        public AccountKeysModel()
        {
            SearchText = "";
        }

        public IEnumerable<ClaimedProductKey> Keys { get; set; }

        public bool PreviousFilterRevealed { get; set; }
        public bool FilterRevealed { get; set; }
        
        public bool PreviousFilterUsed { get; set; }
        public bool FilterUsed { get; set; }

        public bool PreviousFilterUnrevealed { get; set; }
        public bool FilterUnrevealed { get; set; }

        public bool PreviousFilterUnused { get; set; }
        public bool FilterUnused { get; set; }

        public string SearchText { get; set; }
    }
}