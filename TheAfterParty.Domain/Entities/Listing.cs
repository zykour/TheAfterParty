using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TheAfterParty.Domain.Entities
{
    public class Listing
    {
        public Listing()
        {
            ChildListings = new HashSet<MappedListing>();
            ParentListings = new HashSet<MappedListing>();
            ListingComments = new HashSet<ListingComment>();
            WishlistEntries = new HashSet<WishlistEntry>();
            Auctions = new HashSet<Auction>();
            ShoppingCartEntries = new HashSet<ShoppingCartEntry>();
            ClaimedProductKeys = new HashSet<ClaimedProductKey>();
            Giveaways = new HashSet<Giveaway>();
            ProductKeys = new HashSet<ProductKey>();
        }
        public Listing(string listingName) : this()
        {
            this.ListingName = listingName;
        }
        public Listing(int listingPrice) : this()
        {
            this.ListingPrice = listingPrice;
        }
        public Listing(string listingName, int listingPrice) : this()
        {
            this.ListingName = listingName;
            this.ListingPrice = listingPrice;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ListingID { get; set; }
        
        public virtual ICollection<MappedListing> ChildListings { get; set; }

        public virtual ICollection<MappedListing> ParentListings { get; set; }
        public void AddParent(MappedListing ml)
        {
            ParentListings.Add(ml);
            ml.ParentListing.ChildListings.Add(ml);
        }
        public MappedListing RemoveParent(MappedListing ml)
        {
            if (ParentListings.Contains(ml))
            {
                ParentListings.Remove(ml);
                return ml;
            }

            return null;
        }
        
        // the product object
        public virtual Product Product { get; set; }
        public void AddProduct(Product product)
        {
            product.Listing = this;
            Product = product;
        }

        // the name of this listing, in practice, most/all parent nodes should be given a name
        public string ListingName { get; set; }

        // the price of this listing
        public int? ListingPrice { get; set; }
        public bool ListingPriceIsLowest()
        {
            if (ChildPriceSum() < (int)ListingPrice)
                return false;
            else
                return true;
        }
        public int ChildPriceSum()
        {
            if (ChildListings == null)
            {
                SaleOrDefaultPrice();
            }

            int sum = 0;
            
            foreach (MappedListing cl in ChildListings)
            {
                sum += cl.ChildListing.ChildPriceSum();
            }


            if (DiscountedListing != null)
                return (DiscountedListing.ItemDiscountedPrice < sum) ? DiscountedListing.ItemDiscountedPrice : sum;

            return (ListingPrice == null) ? sum : (sum < (int)ListingPrice) ? sum : (int)ListingPrice;
        }
        public int SaleOrDefaultPrice()
        {
            if (DiscountedListing != null)
                return DiscountedListing.ItemDiscountedPrice;
            else
                return (ListingPrice == null) ? 0 : (int)ListingPrice;
        }

        // a discountedlisting object if this listing is on sale
        public virtual DiscountedListing DiscountedListing { get; set; }
        public void AddDiscountedListing(DiscountedListing  discountListing)
        {
            discountListing.Listing = this;
            DiscountedListing = discountListing;
        }
        public DiscountedListing RemoveDiscountedListing(DiscountedListing discountListing)
        {
            DiscountedListing tempDiscountListing = DiscountedListing;
            DiscountedListing = null;
            return tempDiscountListing;            
        }
        public bool HasSale()
        {
            if (DiscountedListing != null)
                return true;
            else
                return false;
        }

        public virtual ICollection<ListingComment> ListingComments { get; set; }
        public void  AddListingComment(ListingComment comment)
        {
            comment.Listing = this;
            ListingComments.Add(comment);
        }
        public ListingComment RemoveListingComment(ListingComment comment)
        {
            if (ListingComments.Contains(comment))
            {
                ListingComments.Remove(comment);
                return comment;
            }

            return null;
        }

        public virtual ICollection<WishlistEntry> WishlistEntries { get; set; }
        public void AddWishlistEntry(WishlistEntry wishlistEntry)
        {
            wishlistEntry.Listing = this;
            WishlistEntries.Add(wishlistEntry);
        }
        public WishlistEntry RemoveWishlistEntry(WishlistEntry wishlistEntry)
        {
            if (WishlistEntries.Contains(wishlistEntry))
            {
                WishlistEntries.Remove(wishlistEntry);
                return wishlistEntry;
            }

            return null;
        }

        public virtual ICollection<Auction> Auctions { get; set; }
        public void AddAuction(Auction auction)
        {
            auction.Listing = this;
            Auctions.Add(auction);
        }
        public Auction RemoveAuction(Auction auction)
        {
            if (Auctions.Contains(auction))
            {
                Auctions.Remove(auction);
                return auction;
            }

            return null;
        }

        public virtual ICollection<ShoppingCartEntry> ShoppingCartEntries { get; set; }
        public void AddShoppingCartEntry(ShoppingCartEntry cartEntry)
        {
            cartEntry.Listing = this;
            this.ShoppingCartEntries.Add(cartEntry);
        }
        public ShoppingCartEntry RemoveShoppingCartEntry(ShoppingCartEntry cartEntry)
        {
            if (ShoppingCartEntries.Contains(cartEntry))
            {
                ShoppingCartEntries.Remove(cartEntry);
                return cartEntry;
            }

            return null;
        }

        public virtual ICollection<ClaimedProductKey> ClaimedProductKeys { get; set; }
        public void AddClaimedProductKey(ClaimedProductKey claimedKey)
        {
            claimedKey.Listing = this;
            ClaimedProductKeys.Add(claimedKey);
        }
        public ClaimedProductKey RemoveClaimedProductKey(ClaimedProductKey claimedKey)
        {
            if (ClaimedProductKeys.Contains(claimedKey))
            {
                ClaimedProductKeys.Remove(claimedKey);
                return claimedKey;
            }

            return null;
        }

        public virtual ICollection<Giveaway> Giveaways { get; set; }
        public void AddGiveaway(Giveaway giveaway)
        {
            giveaway.Listing = this;
            this.Giveaways.Add(giveaway);
        }
        public Giveaway RemoveGiveaway(Giveaway giveaway)
        {
            if (Giveaways.Contains(giveaway))
            {
                this.Giveaways.Remove(giveaway);
                return giveaway;
            }

            return null;
        }

        public virtual Prize Prize { get; set; }
        public void AddPrize(Prize prize)
        {
            prize.Listing = this;
            this.Prize = prize;
        }

        public virtual ICollection<ProductKey> ProductKeys { get; set; }
        public void AddProductKey(ProductKey productKey)
        {
            productKey.Listing = this;
            if (ProductKeys == null)
            {
                ProductKeys = new HashSet<ProductKey>();
            }
            this.ProductKeys.Add(productKey);
            Quantity++;

            UpdateParentQuantities();
        }
        public ProductKey RemoveProductKey(ProductKey productKey)
        {
            if (ProductKeys.Contains(productKey))
            {
                ProductKeys.Remove(productKey);
                Quantity--;
                UpdateParentQuantities();

                return productKey;
            }

            return null;
        }
        public ProductKey RemoveProductKey(int listingId)
        {
            return RemoveProductKey(ProductKeys.Where(key => key.ListingID == listingId).Single());
        }

        public int Quantity { get; set; }
        public void UpdateQuantity()
        {
            int min = System.Int32.MaxValue;

            foreach (MappedListing cl in ChildListings)
            {
                if (cl.ChildListing.Quantity < min)
                    min = cl.ChildListing.Quantity;
            }

            this.Quantity = min;
        }
        public void UpdateParentQuantities()
        {
            if (ParentListings == null)
                return;

            foreach (MappedListing pl in ParentListings)
            {
                pl.ParentListing.UpdateQuantity();

                if (pl.ParentListing.ParentListings != null)
                {
                    pl.ParentListing.UpdateParentQuantities();
                }
            }
        }
    }

    public class MappedListing
    {
        public MappedListing() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }

        public int ParentListingID { get; set; }

        public int ChildListingID { get; set; }

        public virtual Listing ParentListing { get; set; }

        public virtual Listing ChildListing { get; set; }
    }
}
