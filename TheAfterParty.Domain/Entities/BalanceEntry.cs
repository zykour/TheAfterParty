using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class BalanceEntry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int BalanceID { get; set; }

        [Required, StringLength(20)]
        public string SteamID { get; set; }

        [StringLength(50)]
        public string Notes { get; set; }

        [Required]
        public int EarnedPoints { get; set; }

        [Required]
        public DateTime? UpdateDate { get; set; }
    }
}
