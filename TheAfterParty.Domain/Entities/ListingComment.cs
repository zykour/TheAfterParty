using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;


namespace TheAfterParty.Domain.Entities
{
    public class ListingComment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ListingCommentID { get; set; }

        // the product this comment is about
        public int ListingID { get; set; }

        // the product object
        public virtual Listing Listing { get; set; }

        // the user that posted this comment
        public string UserID { get; set; }

        // the user object
        public virtual AppUser AppUser { get; set; }

        // the comment they posted
        public string Comment { get; set; }

        // the date they posted the comment
        public DateTime PostDate { get; set; }

        // was the post edited?
        public bool IsEdited { get; set; }

        // the date it was last edited
        public DateTime LastEdited { get; set; }
    }
}
