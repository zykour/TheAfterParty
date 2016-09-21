using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class BoostedObjective
    {
        public BoostedObjective()
        {
            IsDaily = false;
        }
        
        public int BoostedObjectiveID { get; set; }
        
        // the associated objective that is being boosted
        public virtual Objective Objective { get; set; }

        // how much the reward is being boosted by (numeric)
        public double BoostAmount { get; set; }

        // when the boost ends
        public DateTime EndDate { get; set; }
        public bool IsLive()
        {
            return EndDate.CompareTo(DateTime.Now) > 0;
        }

        // is a daily boosted objective
        public bool IsDaily { get; set; }
    }
}
