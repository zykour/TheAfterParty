using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheAfterParty.Domain.Entities
{
    public class ProductDetail
    {

        //https: //wiki.teamfortress.com/wiki/User:RJackson/StorefrontAPI#appdetails
        //http: //store.steampowered.com/api/appdetails?appids=205630
        [Key]
        public int ProductDetailID { get; set; }

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
}
