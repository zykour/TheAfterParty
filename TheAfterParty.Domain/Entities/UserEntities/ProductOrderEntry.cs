using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TheAfterParty.Domain.Entities
{
    public class ProductOrderEntry
    {
        public ProductOrderEntry(Order order, ShoppingCartEntry cartEntry, ClaimedProductKey claimedKey)
        {
            ListingID = cartEntry.ListingID;
            Listing = cartEntry.Listing;
            SalePrice = cartEntry.Listing.SaleOrDefaultPrice();

            ClaimedProductKeys = new HashSet<ClaimedProductKey>() { claimedKey };
            claimedKey.ProductOrderEntry = this;

            OrderID = order.OrderID;
            this.Order = order;

            //ProductOrderEntryID = claimedKey.ClaimedProductKeyID;
        }
        public ProductOrderEntry(Order order, ShoppingCartEntry cartEntry)
        {
            ListingID = cartEntry.ListingID;
            Listing = cartEntry.Listing;
            SalePrice = cartEntry.Listing.SaleOrDefaultPrice();

            ClaimedProductKeys = new HashSet<ClaimedProductKey>();

            OrderID = order.OrderID;
            this.Order = order;
        }
        public ProductOrderEntry(){}
        
        [Key]//, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductOrderEntryID { get; set; }

        public int ListingID { get; set; }

        public virtual Listing Listing { get; set; }

        public int SalePrice { get; set; }
        
        public virtual ICollection<ClaimedProductKey> ClaimedProductKeys { get; set; }
        public void AddClaimedProductKey(ClaimedProductKey key)
        {
            if (ClaimedProductKeys == null)
            {
                ClaimedProductKeys = new HashSet<ClaimedProductKey>();
            }

            ClaimedProductKeys.Add(key);
            key.ProductOrderEntry = this;
        }

        public int OrderID { get; set; }

        public virtual Order Order { get; set; }
    }
}