using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace TheAfterParty.Domain.Entities
{
    public class ProductReview
    {
        public ProductReview() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductReviewID { get; set; }

        // the product this review is about
        public int ProductID { get; set; }

        // the associated product object
        public virtual Product Product { get; set; }

        // the user that posted this review
        public string UserID { get; set; }

        // the user object
        public virtual AppUser AppUser { get; set; }

        // the comment they posted
        public string Review { get; set; }

        // the date they posted the comment
        public DateTime PostDate { get; set; }

        // was the post edited?
        public bool IsEdited { get; set; }

        // the date it was last edited
        public DateTime LastEdited { get; set; }

        // does the user recommend the game?
        public bool IsRecommended { get; set; }
    }
}
