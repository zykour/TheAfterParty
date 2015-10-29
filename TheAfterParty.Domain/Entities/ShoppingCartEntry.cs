using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class ShoppingCartEntry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShoppingID { get; set; }
        
        public int UserID { get; set; }
        
        public virtual AppUser AppUser { get; set; }

        public int ListingID { get; set; }

        public virtual Listing Listing { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
