﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace TheAfterParty.Domain.Entities
{
    public class ProductOrderEntry
    {
        public ProductOrderEntry(Order order, ShoppingCartEntry cartEntry, ClaimedProductKey claimedKey)
        {
            ListingID = cartEntry.ListingID;
            SalePrice = cartEntry.Listing.SaleOrDefaultPrice();

            ClaimedProductKey = claimedKey;
            OrderID = claimedKey.ClaimedProductKeyID;
            claimedKey.ProductOrderEntry = this;

            OrderID = order.OrderID;
            this.Order = order;
        }
        protected ProductOrderEntry(){}
        
        [Key]
        public int ProductOrderEntryID { get; set; }

        public int ListingID { get; set; }

        public virtual Listing Listing { get; set; }

        public int SalePrice { get; set; }
        
        [Required]
        public virtual ClaimedProductKey ClaimedProductKey { get; set; }

        public int OrderID { get; set; }

        public virtual Order Order { get; set; }
    }
}