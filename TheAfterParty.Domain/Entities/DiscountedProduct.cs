using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data on currently running sales only

    public class DiscountedProduct
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Required, ForeignKey("StockedProduct")]
        public int StoreID { get; set; }

        [Required, Display(Name = "Discount Price")]
        public int ItemDiscountedPrice { get; set; }

        [Required, Display(Name = "Discount Percent")]
        public int ItemDiscountPercent { get; set; }

        [Required, Display(Name = "Ending Date")]
        public DateTime? ItemSaleExpiry { get; set; }

        public virtual StockedProduct StockedProduct { get; set; }
    }
}