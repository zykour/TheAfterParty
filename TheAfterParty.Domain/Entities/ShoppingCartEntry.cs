using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class ShoppingCartEntry
    {
        public ShoppingCartEntry(int userId, int listingId, DateTime dateAdded, int quantity)
        {
            this.UserID = userId;
            this.ListingID = listingId;
            this.DateAdded = dateAdded;
            this.Quantity = quantity;
        }
        public ShoppingCartEntry(int userId, int listingId, int quantit)
        {
            this.UserID = userId;
            this.ListingID = listingId;
            this.DateAdded = DateTime.Now;
            this.Quantity = quantity;
        }
        protected ShoppingCartEntry() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShoppingID { get; set; }
        
        public int UserID { get; set; }
        
        public virtual AppUser AppUser { get; set; }

        public int Quantity { get; set; }

        public int ListingID { get; set; }

        public virtual Listing Listing { get; set; }

        public DateTime DateAdded { get; set; }

    }
}
