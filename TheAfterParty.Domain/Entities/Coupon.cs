using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class Coupon
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CouponID { get; set; }

        // when the coupon expires
        public DateTime Expiry { get; set; }

        // the name of the coupon
        public string CouponName { get; set; }
        
        // if the coupon is restricted to a listing, specify which listing
        public int? ListingID { get; set; }
        
        // how much the coupon discounts the price by (integer value between 1 and 100)
        public int DiscountPercent { get; set; }

        // does the coupon stack with other discounts?
        public bool IsStackable { get; set; }

        // can this coupon be used on the entire order
        public bool IsOrderWide { get; set; }
    }
}
