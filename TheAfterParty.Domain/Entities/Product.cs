using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data for all items ever sold in the store, may include items that have never been sold on the store too.

    // This table is a superset of the CurrentSaleItem table

    public class Product
    {
        // id of an item in the store's database, many other entities rely on this key
        [Key]
        public int ProductID { get; set; }
        
        // a numeric int serving as an identifier for the platform (i.e. Steam = 1, Origin = 2)
        [Required]
        public int Platform { get; set; }

        // if the corresponding platform has a publicly displayed application/game ID, this is stored here
        public int AppID { get; set; }

        public string ProductName { get; set; }

        public virtual Listing Listing { get; set; }
        
        public virtual ICollection<ProductReview> ProductReviews { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<ObjectiveGameMapping> ObjectiveGameMappings { get; set; }
        
        public virtual ProductDetail ProductDetail { get; set; }

        // bundle will be via custom join
        // add steam data, banner, price, genres?, FEATURES, etc.
    }

    public class ProductDetail
    {
        [Key]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public bool IsCoop { get; set; }
        public bool IsLocalCoop { get; set; }
        public bool IsMultiplayer { get; set; }
        public bool IsSinglePlayer { get; set; }
        public bool HasAchievements { get; set; }
        public bool HasTradingCards { get; set; }
        public bool HasControllerSupport { get; set; }
    }
}