using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class GiveawayEntry
    {
        public GiveawayEntry() { }

        [Key]
        public int GiveawayEntryID { get; set; }

        // specifies which Giveaway this entry is for
        public int GiveawayID { get; set; }
                    
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
