using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data for all items ever sold in the store, may include items that have never been sold on the store too.

    // This table is a superset of the CurrentSaleItem table

    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int StoreID { get; set; }
        
        [Required]
        public int Platform { get; set; }

        public int AppID { get; set; }

        [Required, StringLength(50)]//, Display(Name = "Name")]
        public string ItemName { get; set; }

        public virtual StockedProduct StockedProduct { get; set; }
    }
}