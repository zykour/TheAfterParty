using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{
    public class Platform
    {
        public Platform(string platformName, string platformUrl)
        {
            PlatformName = platformName;
            PlatformURL = platformUrl;
        }
        public Platform() { }

        [Key]
        public int PlatformID { get; set; }

        [Required]
        public string PlatformName { get; set; }

        public virtual ICollection<Listing> Listings { get; set; }

        // some platforms don't have a public facing ID for products, it's helpful to know which do and which don't
        public bool HasAppID { get; set; }

        //redemption URL
        public string PlatformURL { get; set; }

        public byte[] PlatformIcon { get; set; }

        public string PlatformIconMimeType { get; set; }

        /*public virtual ICollection<ProductKey> ProductKeys { get; set; }
        public void AddProductKey(ProductKey key)
        {
            if (ProductKeys == null)
            {
                ProductKeys = new HashSet<ProductKey>();
            }

            ProductKeys.Add(key);
            key.Platform = this;
        }
        public virtual ICollection<ClaimedProductKey> ClaimedProductKeys { get; set; }
        public void AddClaimedProductKey(ClaimedProductKey key)
        {
            if (ClaimedProductKeys == null)
            {
                ClaimedProductKeys = new HashSet<ClaimedProductKey>();
            }

            ClaimedProductKeys.Add(key);
            key.Platform = this;
        }*/
    }
}
