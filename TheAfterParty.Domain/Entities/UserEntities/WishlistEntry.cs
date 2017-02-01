using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class WishlistEntry
    {
        public WishlistEntry() { }
        public WishlistEntry(AppUser user, int appId)
        {
            AppUser = user;
            AppID = appId;
            DateAdded = DateTime.Now;
        }

        [Key]
        public int WishlistEntryID { get; set; }

        public string UserID { get; set; }

        public int? ListingID { get; set; }

        public int AppID { get; set; }

        public virtual Listing Listing { get; set; }

        // the user who wants this item
        public virtual AppUser AppUser { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
