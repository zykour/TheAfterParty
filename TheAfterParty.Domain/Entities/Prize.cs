using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{

    // add prize win table, to track past winners?

    public class Prize
    {
        public Prize() { }

        [Key]
        public int PrizeID { get; set; }

        // what tier a prize is in (if any)
        public int? Tier { get; set; }

        // what chance there is to win this prize
        public int? ChancePerThousand { get; set; }

        public bool IsAvailable { get; set; }
        
        [Required]
        public virtual Listing Listing { get; set; }

        public string Description { get; set; }

        public virtual ICollection<WonPrize> WonPrizes { get; set; }
    }
}
