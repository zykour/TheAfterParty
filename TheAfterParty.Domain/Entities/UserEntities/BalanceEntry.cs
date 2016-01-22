using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int BalanceID { get; set; }

        // the steam id of the user who had a balance change
        public string UserID { get; set; }

        // the appuser object associated with this balance change
        public virtual AppUser AppUser { get; set; }

        // notes related to the balance change (such as reasons for it)
        public string Notes { get; set; }

        // how many points were adjusted
        public int PointsAdjusted { get; set; }

        // when the balance change was made
        public DateTime Date { get; set; }
    }
}
