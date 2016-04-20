using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data on game keys. Not all current inventory items necessarily have game keys in the database (may require custom retrieval)

    public class ProductKey
    {
        public ProductKey() { }

        public ProductKey(int listingId, string itemKey, Platform platform)
        {
            this.ListingID = listingId;
            this.ItemKey = itemKey;
        }
        public ProductKey(int listingId, bool isGift, Platform platform)
        {
            this.ListingID = listingId;
            this.IsGift = isGift;
        }
        public ProductKey(bool isGift, string itemKey, Platform platform)
        {
            this.IsGift = isGift;
            this.ItemKey = itemKey;
        }
        public ProductKey(string itemKey, Platform platform)
        {
            this.ItemKey = itemKey;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ProductKeyID { get; set; }

        //public int PlatformID { get; set; }
        public virtual Platform Platform { get; set; }

        // the id of the product this key redeems
        public int ListingID { get; set; }

        public virtual Listing Listing { get; set; }

        public bool IsGift { get; set; }
        
        // the key to be redeemed
        public string ItemKey { get; set; }
    }
}