using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace TheAfterParty.Domain.Entities
{
    public class WonPrize
    {
        public WonPrize() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WonPrizeID { get; set; }

        public int PrizeID { get; set; }
        
        public DateTime TimeWon { get; set; }

        [Required]
        public virtual Prize Prize { get; set; }

        // what the user did when they won
        public string WinningAction { get; set; }

        public string UserID { get; set; }

        public virtual AppUser AppUser { get; set; }
    }
}