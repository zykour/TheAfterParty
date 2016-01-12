using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class BoostedObjective
    {
        // specified the objective that will be boosted
        [Key, Required]
        public int ObjectiveID { get; set; }

        // the associated objective that is being boosted
        [Required]
        public virtual Objective Objective { get; set; }

        // how much the reward is being boosted by (numeric)
        public double BoostAmount { get; set; }

        // when the boost ends
        public DateTime EndDate { get; set; }
    }
}
