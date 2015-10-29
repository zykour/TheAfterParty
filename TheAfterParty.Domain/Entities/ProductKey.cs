using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data on game keys. Not all current inventory items necessarily have game keys in the database (may require custom retrieval)

    public class ProductKey
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int KeyID { get; set; }

        // the id of the product this key redeems
        public int ListingID { get; set; }

        public virtual Listing Listing { get; set; }

        public bool IsGift { get; set; }
        
        // the key to be redeemed
        public string ItemKey { get; set; }
    }
}