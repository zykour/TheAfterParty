using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace TheAfterParty.Domain.Entities
{
    public class ProductOrderEntry
    {
        public int TransactionID { get; set; }

        [Key]
        public int OrderID { get; set; }

        public int ListingID { get; set; }

        public virtual Listing Listing { get; set; }

        public int SalePrice { get; set; }

        public int KeyID { get; set; }
        
        public virtual ClaimedProductKey ClaimedProductKey { get; set; }

        public virtual Order Order { get; set; }
    }
}