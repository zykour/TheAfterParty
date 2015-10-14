using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace TheAfterParty.Domain.Entities
{
    public class OrderProduct
    {
        [Key, Column(Order=1), ForeignKey("Order")]
        public int TransactionID { get; set; }

        [Key, Column(Order=2), Required]
        public int OrderID { get; set; }

        [ScaffoldColumn(false), Required]
        public int StoreID { get; set; }

        [Required, Display(Name = "Price")]
        public int SalePrice { get; set; }

        // productKey retains the platform information on a per-key basis

        public virtual ProductKey ProductKey { get; set; }

        // support for bundled keys?

        [Required]
        public DateTime? DateAdded { get; set; }
    }
}