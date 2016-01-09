using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data for all items ever sold in the store, may include items that have never been sold on the store too.

    // This table is a superset of the CurrentSaleItem table

    public class Product
    {
        public Product()
        {
            ProductReviews = new HashSet<ProductReview>();
            Tags = new HashSet<Tag>();
            ObjectiveGameMappings = new HashSet<ObjectiveGameMapping>();
        }
        public Product(int appId, int platform) : base() { }

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
        public void AddProductDetail(ProductDetail productDetail)
        {
            productDetail.Products.Add(this);
            ProductDetail = productDetail;
        }
    }

    public class ProductDetail
    {

        //https://wiki.teamfortress.com/wiki/User:RJackson/StorefrontAPI#appdetails
        //http://store.steampowered.com/api/appdetails?appids=205630
        [Key]
        public int ProductID { get; set; }
        public virtual ICollection<Product> Products { get; set; }

        // demo, movie, game, software
        public string ProductType { get; set; }
        // name of the product
        public string ProductName { get; set; }
        // appID
        public int AppID { get; set; }
        // age requirement (pegi/esrb)
        public int AgeRequirement { get; set; }
        // DLC
        public int[] DLCAppIDs { get; set; }
        public virtual ICollection<Product> DLCs { get; set; }
        // description, has HTML
        public string DetailedDescription { get; set; }
        // about the game, has HTML
        public string AboutTheGame { get; set; }
        // if dlc or movie, what is the base game?
        public int? BaseProductID { get; set; }
        public virtual Product BaseProduct { get; set; }
        // what is that base products name?
        public string BaseProductName { get; set; }
        //what languages this game supports, contains HTML
        public string SupportedLanguages { get; set; }
        //url to header image
        public string HeaderImageURL { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        // website
        public string ProductWebsite { get; set; }
        // requirements
        public string PCMinimumRequirements { get; set; }
        public string PCRecommendedRequirements { get; set; }
        public string MacMinimumRequirements { get; set; }
        public string MacRecommendedRequirements { get; set; }
        public string LinuxMinimumRequirements { get; set; }
        public string LinuxRecommendedRequirements { get; set; }
        // developers
        public string[] Developers { get; set; }
        // publishers
        public string[] Publishers { get; set; }
        // optional, demo id
        public int DemoAppID { get; set; }
        // optional, demo restrictions (how much can be played)
        public string DemoRestrictions { get; set; }
        // price overview, api doesn't return if F2P/Free
        public string CurrencyType { get; set; }
        // initial price (no sale), DDDDCC, one number
        public int InitialPrice { get; set; }
        // post-discount price
        public int FinalPrice { get; set; }
        // discount %
        public int DiscountPercent { get; set; }

        // package IDs and packages objects, many to many
        public int[] PackageIDs { get; set; }
        // omitting packages for now, will keep PackageIDs for URl construction 
        //public virtual ICollection<Package> Packages { get; set; }

        public bool AvailableOnPC { get; set; }
        public bool AvailableOnMac { get; set; }
        public bool AvailableOnLinux { get; set; }

        // metacritic data
        public int MetacriticScore { get; set; }
        public string MetacriticURL { get; set; }

        // steam categories, i.e. Co-op, Multiplayer, Steam Trading Cards, Partial Controller Support, Steam Cloud
        public string Categories { get; set; }
        public string Genres { get; set; }

        // probably not needed, total number of people who have recommended it
        public int TotalRecommendations { get; set; }

        public int NumAchievements { get; set; }
        //omitting featured achievement details, which is an array of achievement names/icons
        //would need another class to represent

        public bool Unreleased { get; set; }
        //convert to DateTime ?
        public string ReleaseDate { get; set; }

    }

    public class AppScreenshot
    {
        public int AppScreenshotID { get; set; }
        public string ThumbnailURL { get; set; }
        public string FullSizeURL { get; set; }
    }

    public class AppMovie
    {
        public int AppMovieID { get; set; }
        public string Name { get; set; }
        public string ThumbnailURL { get; set; }
        public string SmallMovieURL { get; set; }
        public string LargeMovieURL { get; set; }
        public bool Highlight { get; set; }
    }
}