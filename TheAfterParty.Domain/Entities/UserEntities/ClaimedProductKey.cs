using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace TheAfterParty.Domain.Entities
{
    public class ClaimedProductKey
    {
        public ClaimedProductKey(ProductKey productKey, AppUser user, DateTime? dateAdded, string note)
        {
            ListingID = productKey.ListingID;
            Key = productKey.ItemKey;
            Date = dateAdded ?? DateTime.Now;
            IsGift = false;
            IsUsed = false;
            IsRevealed = false;
            AcquisitionTitle = note;

            UserID = user.Id;
            AppUser = user;
        }
        public ClaimedProductKey() { }

        // need a key value that can be safely used in hidden forms
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClaimedProductKeyID { get; set; }
        
        public int ListingID { get; set; }

        // the user this key is claimed by
        public string UserID { get; set; }

        public bool IsGift { get; set; }
        
        public virtual AppUser AppUser { get; set; }

        // associate a Listing entry with the key to correctly label the key (the game name) and get platform information
        public virtual Listing Listing { get; set; }

        // has the user clicked to reveal the product key
        public bool IsRevealed { get; set; }

        // has the user marked the key as used
        public bool IsUsed { get; set; }

        // the product key that the purchaser receives
        public string Key { get; set; }

        // the title of where this key came from (i.e. "Shop Order" "Sol Survivor Giveaway", etc.)
        public string AcquisitionTitle { get; set; }

        // the date this key was added
        public DateTime Date { get; set; }

        public virtual Gift Gift { get; set; }

        public virtual ProductOrderEntry ProductOrderEntry { get; set; }
    }
}
