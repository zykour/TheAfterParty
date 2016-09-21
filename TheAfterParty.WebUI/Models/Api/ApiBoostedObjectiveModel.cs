using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Api
{
    public class ApiBoostedObjectiveModel
    {
        public ApiBoostedObjectiveModel() { }
        public ApiBoostedObjectiveModel(BoostedObjective boostedObjective)
        {
            BoostedObjectiveID = boostedObjective.BoostedObjectiveID;
            EndDate = boostedObjective.EndDate;
        }

        public int BoostedObjectiveID { get; set; }
        public DateTime EndDate { get; set; }
    }
}