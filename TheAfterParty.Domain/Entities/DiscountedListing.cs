using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data on currently running sales only

    public class DiscountedListing
    {
        // there is at most only one discount per product in the store, thus the FK can also be the key
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Required]
        public int ListingID { get; set; }

        // the discounted price of the product
        public int ItemDiscountedPrice { get; set; }

        // the discount percentage (i.e. 10% off)
        public int ItemDiscountPercent { get; set; }

        // when the discount ends for this product
        public DateTime ItemSaleExpiry { get; set; }

        // the associated product that's on sale
        public virtual Listing Listing { get; set; }
    }
}