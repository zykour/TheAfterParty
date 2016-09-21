using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Api
{
    public class ApiObjectiveModel
    {
        public ApiObjectiveModel(Objective objective)
        {
            ObjeciveID = objective.ObjectiveID;
            ObjectiveTitle = objective.Title;
            ObjectiveName = objective.ObjectiveName;
            ObjectiveDescription = objective.Description;
            AdjustedReward = objective.FixedReward();
            IsBoosted = objective.HasBoostedObjective();
            IsActive = objective.IsActive;
            BoostedExpiry = objective.GetBoostedExpiryOrNull();
        }
        public ApiObjectiveModel()
        {
            BoostedExpiry = null;
            ObjectiveTitle = String.Empty;
            ObjectiveName = String.Empty;
            ObjectiveDescription = String.Empty;
        }

        public int ObjeciveID { get; set; }
        public string ObjectiveTitle { get; set; }
        public string ObjectiveName { get; set; }
        public string ObjectiveDescription { get; set; }
        public int AdjustedReward { get; set; }
        public bool IsBoosted { get; set; }
        public bool IsActive { get; set; }
        public DateTime? BoostedExpiry { get; set; }
    }
}