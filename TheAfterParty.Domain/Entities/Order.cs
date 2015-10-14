using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data on sold items

    public class Order
    {
        [Key, Required]
        public int TransactionID { get; set; }

        public bool IsActive { get; set; }

        public virtual AppUser AppUser { get; set; }

        public virtual IEnumerable<OrderProduct> OrderProducts { get; set; }

        [Required]
        public DateTime? SaleDate { get; set; }
    }
}