using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class WishlistEntry
    {
        [Key, Column(Order=1)]
        public string UserID { get; set; }

        [Key, Column(Order=2)]
        public int ListingID { get; set; }

        public virtual Listing Listing { get; set; }

        // the user who wants this item
        public virtual AppUser AppUser { get; set; }
    }
}
