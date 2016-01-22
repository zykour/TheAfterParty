using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class GiveawayEntry
    {
        public GiveawayEntry() { }

        // specifies which Giveaway this entry is for
        [Key, Column(Order = 1)]
        public int GiveawayID { get; set; }

        // the user who has entered the giveaway specified by GiveawayID
        [Key, Column(Order = 2), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EntryNumber { get; set; }
            
        public string UserID { get; set; }
        
        // the associated Giveaway object for this entry
        public virtual Giveaway Giveaway { get; set; }

        // the associated appuser object for this entry
        public virtual AppUser AppUser { get; set; }

        // the datetime of when this giveaway was entered
        public DateTime EntryDate { get; set; }

        // whether this user decided to donate additional points to the giveaway creator
        public bool HasDonated { get; set; }
    }
}
