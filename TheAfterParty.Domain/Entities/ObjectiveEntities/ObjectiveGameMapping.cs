using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class ObjectiveGameMapping
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int ObjectiveGameMappingID { get; set; }

        public int ObjectiveID { get; set; }

        public virtual Objective Objective { get; set; }

        public int ProductID { get; set; }

        public virtual Product Product { get; set; }
    }
}
