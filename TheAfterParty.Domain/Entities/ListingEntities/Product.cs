using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace TheAfterParty.Domain.Entities
{
    /// <summary>
    /// A platform-independent entity that describes a game or app.
    /// </summary>
    public class Product
    {
        #region Constructors 
        /// <summary> Creates a new blank product entity. </summary>
        public Product()
        {
            GroupEvents = new HashSet<GroupEvent>();
            Listings = new HashSet<Listing>();
            Objectives = new HashSet<Objective>();
            ProductCategories = new HashSet<ProductCategory>();
            ProductReviews = new HashSet<ProductReview>();
            Tags = new HashSet<Tag>();
            IsSteamAppID = true;
        }
        /// <summary> Creates a new product entity with the external identity. </summary>
        /// <param name="appId"> The specified external identity. </param>
        public Product(int appId) : this()
        {
            this.AppID = appId;
        }
        /// <summary> Creates a new product entity with the external identity. </summary>
        /// <param name="appId"> The specified external identity. </param>
        /// <param name="productName"> The sepcified product name. </param>
        public Product(int appId, string productName) : this()
        {
            this.AppID = appId;
            this.ProductName = productName;
        }
        /// <summary> Creates a new platform entity with the specified product name. </summary>
        /// <param name="productName"> The sepcified product name. </param>
        public Product(string productName) : this()
        {
            this.ProductName = productName;
        }
        /// <summary> Creates a new platform entity with the specified product name. </summary>
        /// <param name="productName"> The sepcified product name. </param>
        /// <param name="isSteamAppID"> Denotes whether this product has a Steam entry and uses Steam's app ID </param>
        public Product(string productName, bool isSteamAppID) : this()
        {
            this.ProductName = productName;
            this.IsSteamAppID = isSteamAppID;
        }

        #endregion

        /// <summary> The entity framework identity for this entity. </summary>
        [Key]
        public int ProductID { get; set; }
        
        /// <summary> The identity for this game or app for an external site. </summary>
        /// <remarks> A Steam identity is stored when availale. </remarks>
        public int AppID { get; set; }
                
        /// <summary>
        /// Tells the client whether or not the AppID is referrant to a Steam game's product,  or another platform
        /// </summary>
        /// <remarks> Most items in the database are assumed to be Steam, so set  true by default and set false otherwise</remarks>
        public bool IsSteamAppID { get; set; }

        /// <summary> A collection of associated group event entities. </summary>
        public virtual ICollection<GroupEvent> GroupEvents { get; set; }

        /// <summary> The url to the game or apps banner image. </summary>
        public string HeaderImageURL { get; set; }
        #region HeaderImageURL 

        /// <summary>
        /// Attempts to create a secured banner image URL
        /// </summary>
        /// <returns> Returns if able, a secured banner image url, or an empty string otherwise. </returns>
        public string GetHttpsSecuredHeaderImage()
        {
            if (HeaderImageURL != null && HeaderImageURL.ToLower().Contains("https"))
            {
                return HeaderImageURL;
            }
            else if (AppID != 0)
            {
                if (Listings != null)
                {
                    Listing listing = Listings.FirstOrDefault();

                    if (listing != null)
                    {
                        if (listing.IsComplex())
                        {
                            return "https://steamcdn-a.akamaihd.net/steam/subs/" + AppID + "/capsule_sm_120.jpg";
                        }
                    }
                }

                return "https://steamcdn-a.akamaihd.net/steam/apps/" + AppID + "/capsule_sm_120.jpg"; //"/header_292x136.jpg";
            }

            return string.Empty;
        }

        #endregion


        /// <summary> A collection of listings related to this entity. </summary>
        /// <remarks> Since listings are platform dependent, there can be more than one listing per product. </remarks>
        public virtual ICollection<Listing> Listings { get; set; }

        /// <summary> A collection of associated product entities. </summary>
        public virtual ICollection<Objective> Objectives { get; set; }
        
        /// <summary> A collection of product categories for this product (e.g. "Co-op", "Multiplayer", "Cloud Saves") </summary>
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
        #region ProductCategories

        /// <summary> Adds the specified category to this product. </summary>
        /// <param name="category"> </param>
        public void AddProductCategory(ProductCategory category)
        {
            if (ProductCategories == null)
            {
                ProductCategories = new HashSet<ProductCategory>();
            }

            ProductCategories.Add(category);

            if (category.Products == null)
            {
                category.Products = new HashSet<Product>();
            }

            category.Products.Add(this);
        }
        /// <summary> Determines if this product contains the specified category entity. </summary>
        /// <param name="category"> The specified category to lookup. </param>
        /// <returns> Returns true if this entity contains the category, or false otherwise. </returns>
        public bool HasProductCategory(ProductCategory category)
        {
            if (ProductCategories == null)
            {
                return false;
            }

            return ProductCategories.Where(p => object.Equals(category.CategoryString, p.CategoryString)).Count() >= 1;
        }
        /// <summary> Determines if this product contains the specified category entity. </summary>
        /// <param name="categoryName"> The specified category name to lookup. </param>
        /// <returns> Returns true if this entity contains the category, or false otherwise. </returns>
        public bool HasProductCategory(string categoryName)
        {
            if (ProductCategories == null)
            {
                return false;
            }

            return ProductCategories.Where(p => object.Equals(categoryName, p.CategoryString)).Count() >= 1;
        }

        #endregion

        /// <summary> The name of the product. </summary>
        public string ProductName { get; set; }

        /// <summary> A collection of product review entities for this product. </summary>
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        #region ProductReviews

        /// <summary> Counts the number of reviews for this product. </summary>
        /// <returns> Returns the number of reviews for this product. </returns> 
        public int TotalReviews()
        {
            if (ProductReviews == null)
            {
                return 0;
            }

            return ProductReviews.Count();
        }
        /// <summary> Counts the number of positive reviews for this product. </summary>
        /// <returns> Returns the number of positive reviews for this product. </returns> 
        public int PositiveReviews()
        {
            if (ProductReviews == null)
            {
                return 0;
            }

            return ProductReviews.Where(r => r.IsRecommended == true).Count();
        }

        #endregion

        /// <summary> A string based identifier for this game or app for an external site. </summary>
        /// <remarks> Some external sites don't use a public facing identity system, but instead use a unique identifier relating to the game or app's name. </remarks>
        public string StringID { get; set; }

        /// <summary> A collection of tag entities attached to this product. </summary>
        public virtual ICollection<Tag> Tags { get; set; }
        #region Tags

        /// <summary> Determines if this product has the specified tag. </summary>
        /// <param name="tag"> The specified tag entity to lookup. </param>
        /// <returns> Returns true if this product contains the specified tag, or false otherwise. </returns> 
        public bool HasTag(Tag tag)
        {
            if (Tags == null)
            {
                return false;
            }

            return Tags.Where(t => object.Equals(tag.TagName, t.TagName)).Count() >= 1;
        }
        /// <summary> Determines if this product has the specified tag. </summary>
        /// <param name="tagName"> The specified tag name to lookup. </param>
        /// <returns> Returns true if this product contains the specified tag, or false otherwise. </returns> 
        public bool HasTag(string tagName)
        {
            if (Tags == null)
            {
                return false;
            }

            return Tags.Where(t => object.Equals(tagName, t.TagName)).Count() >= 1;
        }
        /// <summary> Adds the specified tag entity to the this product. </summary>
        /// <param name="tag"> The specified tag entity to add. </param>
        public void AddTag(Tag tag)
        {
            if (Tags == null)
            {
                Tags = new HashSet<Tag>();
            }

            Tags.Add(tag);

            if (tag.Products == null)
            {
                tag.Products = new HashSet<Product>();
            }

            tag.Products.Add(this);
        }

        #endregion
    }
}