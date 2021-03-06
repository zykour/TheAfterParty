﻿using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{
    public class BalanceEntry
    {
        public BalanceEntry(AppUser user, string notes, int pointsAdjusted, DateTime date)
        {
            Notes = notes;
            PointsAdjusted = pointsAdjusted;
            Date = date;

            UserID = user.Id;
            AppUser = user;
        }
        public BalanceEntry(){}

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BalanceEntryID { get; set; }

        // the steam id of the user who had a balance change
        public string UserID { get; set; }

        // the appuser object associated with this balance change
        public virtual AppUser AppUser { get; set; }

        // notes related to the balance change (such as reasons for it)
        public string Notes { get; set; }

        // how many points were adjusted
        public int PointsAdjusted { get; set; }

       // public int ObjectiveID { get; set; }
       // public Objective CompletedObjective { get; set; }

        // when the balance change was made
        public DateTime Date { get; set; }
        
        public virtual Objective Objective { get; set; }
        public void AddObjective(Objective objective)
        {
            Objective = objective;
            
            if (objective.BalanceEntries == null)
            {
                objective.BalanceEntries = new List<BalanceEntry>();
                objective.BalanceEntries.Add(this);
            }
        }
    }
}
