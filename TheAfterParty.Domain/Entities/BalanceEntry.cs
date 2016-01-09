using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class BalanceEntry
    {
        public BalanceEntry(int userId, string notes, int pointsAdjusted, DateTime date)
        {
            UserID = userId;
            Notes = notes;
            PointsAdjusted = pointsAdjusted;
            Date = date;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BalanceID { get; set; }

        // the steam id of the user who had a balance change
        public int UserID { get; set; }

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
