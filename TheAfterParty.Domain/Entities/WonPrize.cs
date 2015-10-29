using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TheAfterParty.Domain.Entities
{
    public class WonPrize
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WonPrizeID { get; set; }

        public int PrizeID { get; set; }

        public virtual Prize Prize { get; set; }

        // what the user did when they won
        public string WinningAction { get; set; }

        public int UserID { get; set; }

        public virtual AppUser AppUser { get; set; }
    }
}