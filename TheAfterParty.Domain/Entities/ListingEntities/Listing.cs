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
            DiscountedListings = new HashSet<DiscountedListing>();
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
        public int ListingPrice { get; set; }
        public bool ListingPriceIsLowest()
        {
            if (ChildPriceSum() < ListingPrice)
                return false;
            else
                return true;
        }
        public int ChildPriceSum()
        {
            if (ChildListings == null)
            {
                return SaleOrDefaultPrice();
            }

            int sum = 0;
            
            foreach (MappedListing cl in ChildListings)
            {
                sum += cl.ChildListing.ChildPriceSum();
            }

            return (sum <= SaleOrDefaultPrice()) ? sum : SaleOrDefaultPrice();
        }
        public int SaleOrDefaultPrice()
        {
            if (DiscountedListings.Count != 0)
            {
                double finalPercent = 1.0;

                foreach (DiscountedListing discount in DiscountedListings)
                {
                    finalPercent *= (1 - (discount.ItemDiscountPercent / 100.00));
                }
                
                int finalPrice = (int)System.Math.Floor(finalPercent * ListingPrice);

                return (finalPrice > 0) ? finalPrice : 1;
            }
            else
                return ListingPrice;
        }

        // a discountedlisting object if this listing is on sale
        public virtual ICollection<DiscountedListing> DiscountedListings { get; set; }
        public void AddDiscountedListing(DiscountedListing  discountListing)
        {
            discountListing.Listing = this;
            DiscountedListings.Add(discountListing);
        }
        public bool HasSale()
        {
            if (DiscountedListings.Count > 0)
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
}
