using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data on sold items

    public class Order
    {
        public Order(int userId, DateTime date)
        {
            this.UserID = userId;
            SaleDate = date;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; set; }

        // the ID of the user this order is associated with
        public int UserID { get; set; }

        // the appuser object associated to the user who made this order
        public virtual AppUser AppUser { get; set; }

        // the products in this bundle
        public virtual ICollection<ProductOrderEntry> ProductOrderEntries { get; set; }
        
        // date this order was made
        public DateTime? SaleDate { get; set; }
    }
}