using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class Listing
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ListingID { get; set; }
        
        public virtual ICollection<MappedListing> ChildListings { get; set; }

        public virtual ICollection<MappedListing> ParentListings { get; set; }
        
        // the product object
        public virtual Product Product { get; set; }

        // the name of this listing, in practice, most/all parent nodes should be given a name
        public string ListingName { get; set; }

        // the price of this listing
        public int? ListingPrice { get; set; }

        // a discountedlisting object if this listing is on sale
        public virtual DiscountedListing DiscountedListing { get; set; }

        public virtual ICollection<ListingComment> ListingComments { get; set; }

        public virtual ICollection<WishlistEntry> WishlistEntries { get; set; }

        public virtual ICollection<Auction> Auctions { get; set; }

        public virtual ICollection<ShoppingCartEntry> ShoppingCartEntries { get; set; }

        public virtual ICollection<ClaimedProductKey> ClaimedProductKeys { get; set; }

        public virtual ICollection<Giveaway> Giveaways { get; set; }

        public virtual Prize Prize { get; set; }

        public virtual ICollection<ProductKey> ProductKeys { get; set; }
    }

    public class MappedListing
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }

        public int ParentListingID { get; set; }

        public int ChildListingID { get; set; }

        public virtual Listing ParentListing { get; set; }

        public virtual Listing ChildListing { get; set; }
    }
}
