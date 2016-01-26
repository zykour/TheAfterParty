using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data on currently running sales only

    public class DiscountedListing
    {
        public DiscountedListing() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscountedListingID { get; set; }

        public int ListingID { get; set; }
        
        // the discount percentage (i.e. 10% off)
        public int ItemDiscountPercent { get; set; }

        // when the discount ends for this product
        public DateTime ItemSaleExpiry { get; set; }

        // the associated product that's on sale
        [Required]
        public virtual Listing Listing { get; set; }

        public bool DailyDeal { get; set; }
        public bool WeeklyDeal { get; set; }
    }
}