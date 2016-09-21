using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    /// <summary>
    /// An entity that describes a discount for a particular listing
    /// </summary>
    public class DiscountedListing
    {
        /// <summary> Creates a new discounted listing entity </summary>
        public DiscountedListing()
        {
            ItemDiscountPercent = 0;
            ItemSaleExpiry = DateTime.Now;
            DailyDeal = false;
            WeeklyDeal = false;
        }
        ///  <summary> Creates a new discounted listing </summary>
        ///  <param name="ItemDiscountPercent"> The discount amount (1-99) </param>
        ///  <param name="ItemSaleExpiry"> When the deal ends </param>
        public DiscountedListing(int ItemDiscountPercent, DateTime ItemSaleExpiry)
        {
            this.ItemDiscountPercent = ItemDiscountPercent;
            this.ItemSaleExpiry = ItemSaleExpiry;
        }

        /// <summary> The entity framework identity for this discounted listing. </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscountedListingID { get; set; }

        /// <summary> Indicates whether or not this discount listing represents a daily deal. </summary>
        public bool DailyDeal { get; set; }
        
        /// <summary> The integer representation of the discount percent (e.g. 33 = 33% off). </summary>
        public int ItemDiscountPercent { get; set; }

        /// <summary> The expiration date of this discount. </summary>
        public DateTime ItemSaleExpiry { get; set; }
        #region ItemSaleExpiry

        /// <summary> Determines if this discount is active. </summary>
        /// <returns> Returns true if this discount is active, otherwise returns false. </returns> 
        public bool IsLive()
        {
            return ItemSaleExpiry.CompareTo(DateTime.Now) > 0;
        }

        #endregion

        /// <summary> The navigation property pointing to the associated listing entity. </summary>
        public virtual Listing Listing { get; set; }

        /// <summary> The entity framework identity for the associated listing. </summary>
        [Required]
        public int ListingID { get; set; }

        /// <summary> Indicates whether or not this discount listing represents a weekly deal. </summary>
        public bool WeeklyDeal { get; set; }
    }
}