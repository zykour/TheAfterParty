using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Objectives
{
    public class AddEditObjectivesViewModel : NavModel
    {
        public Objective Objective { get; set; }
        public int ProductID { get; set; }
    }
}