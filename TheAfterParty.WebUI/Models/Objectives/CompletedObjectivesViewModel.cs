using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Objectives
{
    public class CompletedObjectivesViewModel : NavModel
    {
        public IEnumerable<BalanceEntry> BalanceEntries { get; set; }
    }
}