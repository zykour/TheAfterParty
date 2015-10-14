using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data on game keys. Not all current inventory items necessarily have game keys in the database (may require custom retrieval)

    public class ProductKey
    {
        [Key, Column(Order = 1), Required, ForeignKey("StockedProduct")]
        public int StoreID { get; set; }

        [Key, Column(Order = 2), Required, StringLength(100), Display(Name = "Key")]
        public string ItemKey { get; set; }

        public bool IsSold { get; set; }

        [Required]
        public int Platform { get; set; }
    }
}