using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data on sold items

    public class Order
    {
        public Order(AppUser user, DateTime date)
        {
            this.UserID = user.Id;
            SaleDate = date;

            ProductOrderEntries = new HashSet<ProductOrderEntry>();

            this.AppUser = user;
        }
        public Order() { }

        // Get the total price paid for this entire order
        public int TotalSalePrice()
        {
            int total = 0;

            foreach (ProductOrderEntry orderEntry in ProductOrderEntries)
            {
                total += orderEntry.SalePrice;
            }

            return total;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; set; }

        // the ID of the user this order is associated with
        public string UserID { get; set; }

        // the appuser object associated to the user who made this order
        public virtual AppUser AppUser { get; set; }

        // the products in this bundle
        public virtual ICollection<ProductOrderEntry> ProductOrderEntries { get; set; }
        
        // date this order was made
        public DateTime? SaleDate { get; set; }

        public int BalanceEntryID { get; set; }
    }
}