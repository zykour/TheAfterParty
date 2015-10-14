using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data on current inventory only

    public class StockedProduct
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Required, ForeignKey("Product")]
        public int StoreID { get; set; }

        [Required, Display(Name = "Price")]
        public int ItemPrice { get; set; }

        [Required, Display(Name = "Quantity")]
        public int ItemQuantity { get; set; }

        [Required]
        public int Platform { get; set; }

        public virtual IEnumerable<ProductKey> ProductKeys { get; set; }

        public virtual Product Product { get; set; }

        public virtual DiscountedProduct DiscountedProduct { get; set; }
    }
}