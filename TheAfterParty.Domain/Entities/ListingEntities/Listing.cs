using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System;

namespace TheAfterParty.Domain.Entities
{
    public class Listing
    {
        public Listing()
        {
            this.DateEdited = DateTime.Now;
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
        public Listing(string listingName, int listingPrice, DateTime dateEdited)
        {
            this.ListingName = listingName;
            this.ListingPrice = listingPrice;
            this.DateEdited = dateEdited;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ListingID { get; set; }
        
        public virtual ICollection<Listing> ChildListings { get; set; }
        public void AddChildListing(Listing listing)
        {
            if (ChildListings == null)
            {
                ChildListings = new HashSet<Listing>();
            }

            if (listing.ParentListings == null)
            {
                listing.ParentListings = new HashSet<Listing>();
            }

            ChildListings.Add(listing);
            listing.ParentListings.Add(this);
        }

        public virtual ICollection<Listing> ParentListings { get; set; }
        public void AddParent(Listing listing)
        {
            ParentListings.Add(listing);
            listing.ChildListings.Add(this);
        }
        public Listing RemoveParent(Listing listing)
        {
            if (ParentListings.Contains(listing))
            {
                ParentListings.Remove(listing);
                return listing;
            }

            return null;
        }
        
        // the product object
        public virtual Product Product { get; set; }
        public void AddProduct(Product product)
        {
            if (product.Listings == null)
            {
                product.Listings = new HashSet<Listing>();
            }

            product.Listings.Add(this);
            Product = product;
        }
        public bool ContainsTag(Tag tag)
        {
            return Product.HasTag(tag);
        }
        public bool ContainsProductCategory(ProductCategory category)
        {
            return Product.HasProductCategory(category);
        }

        // a child listing will have a single platform, compositite listings will have a null here
        public virtual ICollection<Platform> Platforms { get; set; }
        public void AddPlatform(Platform platform)
        {
            if (Platforms == null)
                Platforms = new HashSet<Platform>();

            if (Platforms.Where(p => object.Equals(p.PlatformName, platform.PlatformName)).Count() == 0)
                Platforms.Add(platform);

            if (platform.Listings == null)
                platform.Listings = new HashSet<Listing>();

            platform.Listings.Add(this);
        }
        public bool ContainsPlatform(Platform platform)
        {
            foreach (Platform listingPlatform in Platforms)
            {
                if (listingPlatform.PlatformName.CompareTo(platform.PlatformName) == 0)
                {
                    return true;
                }
            }

            return false;
        }
        public bool ContainsPlatform(string platformName)
        {
            foreach (Platform listingPlatform in Platforms)
            {
                if (listingPlatform.PlatformName.CompareTo(platformName) == 0)
                {
                    return true;
                }
            }

            return false;
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
            
            foreach (Listing listing in ChildListings)
            {
                sum += listing.ChildPriceSum();
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
            {
                return ListingPrice;
            }
        }

        // a discountedlisting object if this listing is on sale
        public virtual ICollection<DiscountedListing> DiscountedListings { get; set; }
        public void AddDiscountedListing(DiscountedListing  discountListing)
        {
            if (DiscountedListings == null)
            {
                DiscountedListings = new HashSet<DiscountedListing>();
            }

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
        public bool HasDailyDeal()
        {
            foreach (DiscountedListing discount in DiscountedListings)
            {
                if (discount.DailyDeal)
                {
                    return true;
                }
            }

            return false;
        }
        public bool HasWeeklyDeal()
        {
            foreach (DiscountedListing discount in DiscountedListings)
            {
                if (discount.WeeklyDeal)
                {
                    return true;
                }
            }

            return false;
        }
        public DateTime? GetEarliestExpiry(bool daily, bool weekly)
        {
            if (DiscountedListings == null)
                return null;

            if (daily)
            {
                if (DiscountedListings.Where(d => d.DailyDeal).SingleOrDefault() != null)
                {
                    return DiscountedListings.Where(d => d.DailyDeal).SingleOrDefault().ItemSaleExpiry;
                }

                return null;
            }

            if (weekly)
            {
                if (DiscountedListings.Where(d => d.WeeklyDeal).SingleOrDefault() != null)
                {
                    return DiscountedListings.Where(d => d.WeeklyDeal).SingleOrDefault().ItemSaleExpiry;
                }

                return null;
            }

            return DiscountedListings.OrderBy(d => d.ItemSaleExpiry).FirstOrDefault().ItemSaleExpiry;
        }
        
        public DateTime DateEdited { get; set; }

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

            foreach (Listing listing in ChildListings)
            {
                if (listing.Quantity < min)
                    min = listing.Quantity;
            }

            this.Quantity = min;
        }
        public void UpdateParentQuantities()
        {
            if (ParentListings == null)
                return;

            foreach (Listing listing in ParentListings)
            {
                listing.UpdateQuantity();

                if (listing.ParentListings != null)
                {
                    listing.UpdateParentQuantities();
                }
            }
        }

        public virtual ICollection<AppUser> UsersBlacklist { get; set; }
    }
}
