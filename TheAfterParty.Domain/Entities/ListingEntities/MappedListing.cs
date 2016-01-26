using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class MappedListing
    {
        public MappedListing() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int MappedListingID { get; set; }

        public int ParentListingID { get; set; }

        public int ChildListingID { get; set; }

        public virtual Listing ParentListing { get; set; }

        public virtual Listing ChildListing { get; set; }
    }
}
