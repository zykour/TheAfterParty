using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{
    /// <summary>
    /// An entity that describes a game or app's features.
    /// </summary>
    public class ProductCategory
    {
        #region Constructors

        /// <summary> Creates a blank product category entity. </summary>
        public ProductCategory()
        {
            Products = new HashSet<Product>();
        }
        /// <summary> Creates a new product category entity with the specified name. </summary>
        /// <param name="categoryName"> The specified category name. </param>
        public ProductCategory(string categoryName) : this()
        {
            this.CategoryString = categoryName;
        }

        #endregion

        /// <summary> The entity framework identity for this entity. </summary>
        [Key]
        public int ProductCategoryID { get; set; }

        /// <summary> A url to the categories image, if it exists. </summary>
        public string CategoryIconURL { get; set; }

        /// <summary> The unique name of this category (e.g. "Co-op", "Multiplayer", "Cloud Saves"). </summary>
        public string CategoryString { get; set; }

        /// <summary> A collection of product entities that associate with this tag. </summary>
        public virtual ICollection<Product> Products { get; set; }

    }
}
