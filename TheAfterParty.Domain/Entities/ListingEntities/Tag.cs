using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{
    /// <summary>
    /// An entity that describes an externally or user defined tag for a game or app (e.g. "Horror", "First-person", "Boardgame")
    /// </summary>
    public class Tag
    {
        #region Constructors
        
        /// <summary> Creates a new blank tag entity. </summary>
        public Tag()
        {
            Products = new HashSet<Product>();
            UserTags = new HashSet<UserTag>();
        }
        /// <summary> Creates a new tag entity with the specified name. </summary>
        /// <param name="name"> The specified name of the tag. </param>
        public Tag(string name) : this()
        {
            TagName = name;
        }

        #endregion

        /// <summary> The entity framework identity for this entity. </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int TagID { get; set; }
        
        /// <summary> A collection of products that have this tag. </summary>
        public virtual ICollection<Product> Products { get; set; }

        /// <summary> The application user navigation property. </summary>
        public string TagName { get; set; }

        /// <summary> A collection of user tag entities that map users to tags. </summary>
        public virtual ICollection<UserTag> UserTags { get; set; }
    }
}
