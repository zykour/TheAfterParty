using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Objectives
{
    public class AdminObjectivesViewModel : NavModel
    {
        public IEnumerable<Objective> Objectives { get; set; }
        public IEnumerable<BoostedObjective> BoostedObjectives { get; set; }
    }
}