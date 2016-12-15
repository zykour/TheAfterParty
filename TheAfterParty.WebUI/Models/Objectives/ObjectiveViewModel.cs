using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;
using CodeKicker.BBCode;

namespace TheAfterParty.WebUI.Models.Objectives
{
    public class ObjectiveViewModel : NavModel
    {
        public Objective Objective { get; set; }
        public IEnumerable<BalanceEntry> BalanceEntries { get; set; }
        public BBCodeParser Parser { get; set; }
    }
}