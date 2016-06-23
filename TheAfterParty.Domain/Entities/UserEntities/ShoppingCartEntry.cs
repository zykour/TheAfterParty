using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class ShoppingCartEntry
    {
        public ShoppingCartEntry(AppUser user, Listing listing, int quantity)
        {
            DateAdded = DateTime.Now;
            this.Quantity = quantity;

            this.ListingID = listing.ListingID;
            this.Listing = listing;

            this.UserID = user.Id;
            this.AppUser = user;
        }
        protected ShoppingCartEntry() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShoppingCartEntryID { get; set; }
        
        public string UserID { get; set; }
        
        public virtual AppUser AppUser { get; set; }

        public int Quantity { get; set; }
        public bool QuantityExceedsAvailability()
        {
            return Listing.Quantity < Quantity;
        }

        public int ListingID { get; set; }

        public virtual Listing Listing { get; set; }

        public DateTime DateAdded { get; set; }

    }
}
