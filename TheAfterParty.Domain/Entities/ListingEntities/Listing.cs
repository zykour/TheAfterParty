using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System;
using System.Text;

namespace TheAfterParty.Domain.Entities
{
    /// <summary>
    /// A platform-dependent entity that describes a game or app for sale.
    /// </summary>
    public class Listing
    {
        #region Constructors

        /// <summary>
        /// Creates a new blank Listing</summary>
        public Listing()
        {
            DateEdited = DateTime.Now;
            ChildListings = new HashSet<Listing>();
            ClaimedProductKeys = new HashSet<ClaimedProductKey>();
            DiscountedListings = new HashSet<DiscountedListing>();
            Giveaways = new HashSet<Giveaway>();
            ListingComments = new HashSet<ListingComment>();
            ParentListings = new HashSet<Listing>();
            Platforms = new HashSet<Platform>();
            ProductKeys = new HashSet<ProductKey>();
            ShoppingCartEntries = new HashSet<ShoppingCartEntry>();
            UsersBlacklist = new HashSet<AppUser>();
            WishlistEntries = new HashSet<WishlistEntry>();
        }
        /// <summary>
        /// Creates a new Listing with the specified name</summary>
        /// <param name="listingName">The name of the product</param>
        public Listing(string listingName) : this()
        {
            this.ListingName = listingName;
        }
        /// <summary>
        /// Creates a new Listing with the specified price</summary>
        /// <param name="listingPrice">The price of the listing</param>
        public Listing(int listingPrice) : this()
        {
            this.ListingPrice = listingPrice;
        }
        /// <summary>
        /// Creates a new Listing with the specified price</summary>
        /// <param name="listingName">The name of the product</param>
        /// <param name="listingPrice">The price of the listing</param>
        public Listing(string listingName, int listingPrice) : this()
        {
            this.ListingName = listingName;
            this.ListingPrice = listingPrice;
        }
        /// <summary>
        /// Creates a new Listing with the specified price</summary>
        /// <param name="listingName">The name of the product</param>
        /// <param name="listingPrice">The price of the listing</param>
        /// <param name="dateEdited">The date this item was last edited</param>
        public Listing(string listingName, int listingPrice, DateTime dateEdited) : this()
        {
            this.ListingName = listingName;
            this.ListingPrice = listingPrice;
            this.DateEdited = dateEdited;
        }
        /// <summary>
        /// Creates a new Listing with the specified price</summary>
        /// <param name="listingId">The id. </param>
        /// <param name="listingName">The name of the product</param>
        /// <param name="listingPrice">The price of the listing</param>
        /// <param name="quantity">The quantity of this item</param>
        public Listing(int listingId, string listingName, int listingPrice, int quantity) : this()
        {
            this.ListingID = listingId;
            this.ListingName = listingName;
            this.ListingPrice = listingPrice;
            this.Quantity = quantity;
        }

        #endregion

        /// <summary> The Entity Framework identity of this listing</summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ListingID { get; set; }

        /// <summary> A collection of auctions entities associated with this listing. </summary>
        public virtual ICollection<Auction> Auctions { get; set; }
        #region Auctions

        /// <summary> Adds the specified auction entity to this listing. </summary>
        /// <param name="auction"> The specified auction entity to add to this listing. </param>
        public void AddAuction(Auction auction)
        {
            if (Auctions == null)
            {
                Auctions = new HashSet<Auction>();
            }

            auction.Listing = this;
            Auctions.Add(auction);
        }
        /// <summary> Removes the specified auction entity from this listing. </summary>
        /// <param name="auction"> The specified auction entity to remove from this listing. </param>
        /// <returns> Returns the removed auction entity if it existed, otherwise returns null. </returns> 
        public Auction RemoveAuction(Auction auction)
        {
            if (this?.Auctions?.Count == 0)
            {
                return null;
            }

            if (Auctions.Contains(auction))
            {
                Auctions.Remove(auction);
                return auction;
            }

            return null;
        }

        #endregion

        /// <summary> A collection of child listings for complex listings</summary>
        public virtual ICollection<Listing> ChildListings { get; set; }
        #region Child Listings

        /// <summary> Adds a child listing to a complex listing's child listings</summary>
        /// <param name="listing">The child listing to add</param>
        public void AddChildListing(Listing listing)
        {
            // initialize a ChildListings list for this listing, and a ParentListings list for the child element to be added, if need be
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

            // since this is a complex listing, adding a child listing may effect the quantity of this listing
            UpdateQuantity();
        }        
        /// <summary> Determines if this listing is complex (has child listings)</summary>
        /// <returns> Returns true if this listing has child listings, and false otherwise. </returns> 
        public bool IsComplex()
        {
            if (this?.ChildListings?.Count > 0)
            {
                return true;
            }

            return false;
        }

        #endregion

        /// <summary> A collection of claimed product key entities associated with this listing. </summary>
        public virtual ICollection<ClaimedProductKey> ClaimedProductKeys { get; set; }
        #region ClaimedProductKeys

        /// <summary> Adds the specified claimed product key entity to this listing. </summary>
        /// <param name="claimedKey"> The specified claimed product key entity to add to this listing. </param>
        public void AddClaimedProductKey(ClaimedProductKey claimedKey)
        {
            if (ClaimedProductKeys == null)
            {
                ClaimedProductKeys = new HashSet<ClaimedProductKey>();
            }

            claimedKey.Listing = this;
            ClaimedProductKeys.Add(claimedKey);
        }
        /// <summary> Removes the specified claimed product key entity from this listing. </summary>
        /// <param name="claimedKey"> The specified claimed product key entity to remove from this listing. </param>
        /// <returns> Returns the removed claimed product key entity if it existed, otherwise returns null. </returns> 
        public ClaimedProductKey RemoveClaimedProductKey(ClaimedProductKey claimedKey)
        {
            if (this?.ClaimedProductKeys?.Count == 0)
            {
                return null;
            }

            if (ClaimedProductKeys.Contains(claimedKey))
            {
                ClaimedProductKeys.Remove(claimedKey);
                return claimedKey;
            }

            return null;
        }

        #endregion

        /// <summary> The last time this listing has been edited. </summary>
        public DateTime DateEdited { get; set; }

        /// <summary> A collection of all discount entities for this listing.</summary>
        public virtual ICollection<DiscountedListing> DiscountedListings { get; set; }
        #region DiscountedListings

        /// <summary> Adds an associated discount entity to this listing. </summary>
        /// <param name="discountListing">The discount entity to be added. </param>
        public void AddDiscountedListing(DiscountedListing discountListing)
        {
            if (DiscountedListings == null)
            {
                DiscountedListings = new HashSet<DiscountedListing>();
            }

            discountListing.Listing = this;
            DiscountedListings.Add(discountListing);
        }
        /// <summary> Determines if this listing has a discount entity that represents any deal. </summary>
        /// <returns> Returns true if this listing has any deal that isn't expired, or false otherwise. </returns> 
        public bool HasSale()
        {
            if (DiscountedListings.Count > 0)
            {
                foreach (DiscountedListing discount in DiscountedListings)
                {
                    if (discount.IsLive())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        /// <summary> Determines if this listing has a discount entity that represents a daily deal. </summary>
        /// <returns> Returns true if this listing has a daily deal that isn't expired, or false otherwise. </returns> 
        public bool HasDailyDeal()
        {
            foreach (DiscountedListing discount in DiscountedListings)
            {
                if (discount.DailyDeal && discount.IsLive())
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary> Determines if this listing has a discount entity that represents a weekly deal. </summary>
        /// <returns> Returns true if this listing has a weekly deal that isn't expired, or false otherwise. </returns> 
        public bool HasWeeklyDeal()
        {
            foreach (DiscountedListing discount in DiscountedListings)
            {
                if (discount.WeeklyDeal && discount.IsLive())
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary> Determines if this listing has a deal that is neither a daily or weekly deal. </summary>
        /// <returns> Returns true if this listing contains a deal that is neither a daily or weekly deal, or false otherwise. </returns> 
        public bool HasOtherDeal()
        {
            if (DiscountedListings == null)
            {
                return false;
            }

            return DiscountedListings.Where(d => d.DailyDeal == false && d.WeeklyDeal == false).Count() > 0;
        }
        /// <summary> Adds an associated discount entity to this listing. </summary>
        /// <param name="daily">If set to true, the method will determine the expiration of the daily deal </param>
        /// <param name="weekly">If set to true and daily is false, the method will determine the expiration of the weekly deal </param>
        /// <returns> Returns the DateTime that represents the earliest expiration of all discount entities attached to this listing (only checks live deals) </returns> 
        public DateTime? GetEarliestExpiry(bool daily, bool weekly)
        {
            if (this?.DiscountedListings?.Count == 0)
            {
                return null;
            }

            DateTime expiry = DateTime.MinValue;

            if (daily)
            {
                if (DiscountedListings.Where(d => d.DailyDeal).SingleOrDefault() != null)
                {
                    expiry = DiscountedListings.Where(d => d.DailyDeal).SingleOrDefault().ItemSaleExpiry;
                }
            }
            else if (weekly)
            {
                if (DiscountedListings.Where(d => d.WeeklyDeal).SingleOrDefault() != null)
                {
                    expiry = DiscountedListings.Where(d => d.WeeklyDeal).SingleOrDefault().ItemSaleExpiry;
                }
            }
            else
            {
                expiry = DiscountedListings.OrderBy(d => d.ItemSaleExpiry).FirstOrDefault().ItemSaleExpiry;
            }

            if (expiry > DateTime.Now)
            {
                return expiry;
            }
            else
            {
                return null;
            }
        }

        #endregion

        /// <summary> A collection of giveaway entities associated with this listing. </summary>
        public virtual ICollection<Giveaway> Giveaways { get; set; }
        #region Giveaway

        /// <summary> Adds the specified giveaway entity to this listing. </summary>
        /// <param name="giveaway"> The specified giveaway entity to add to this listing. </param>
        public void AddGiveaway(Giveaway giveaway)
        {
            if (Giveaways == null)
            {
                Giveaways = new HashSet<Giveaway>();
            }

            giveaway.Listing = this;
            this.Giveaways.Add(giveaway);
        }
        /// <summary> Removes the specified giveaway entity from this listing. </summary>
        /// <param name="giveaway"> The specified giveaway entity to remove from this listing. </param>
        /// <returns> Returns the removed giveaway entity if it existed, otherwise returns null. </returns> 
        public Giveaway RemoveGiveaway(Giveaway giveaway)
        {
            if (this?.Giveaways?.Count == 0)
            {
                return null;
            }

            if (Giveaways.Contains(giveaway))
            {
                this.Giveaways.Remove(giveaway);
                return giveaway;
            }

            return null;
        }

        #endregion

        /// <summary> A collection of user defined comments for this listing. </summary>
        public virtual ICollection<ListingComment> ListingComments { get; set; }
        #region ListingComments

        /// <summary> Adds an associated listing comment to this listing. </summary>
        /// <param name="comment"> The listing comment entity to add to this listing. </param>
        public void AddListingComment(ListingComment comment)
        {
            if (ListingComments == null)
            {
                ListingComments = new HashSet<ListingComment>();
            }

            comment.Listing = this;
            ListingComments.Add(comment);
        }
        /// <summary> Removes the specified listing comment entity from this listing. </summary>
        /// <param name="comment"> The listing comment entity to remove from this listing </param>
        /// <returns> Returns the removed listing comment entity, if it existed, returns null otherwise. </returns> 
        public ListingComment RemoveListingComment(ListingComment comment)
        {
            if (this?.ListingComments?.Count == 0)
            {
                return null;
            }

            if (ListingComments.Contains(comment))
            {
                ListingComments.Remove(comment);
                return comment;
            }

            return null;
        }

        #endregion

        /// <summary> The name of the listing. </summary>
        public string ListingName { get; set; }

        /// <summary> The price of this listing, listings with a non-positive price won't be viewable by users. </summary>
        public int ListingPrice { get; set; }
        #region ListingPrice
        /// <summary> Determines if the set price is as cheap or cheaper than the sum price of child listings. </summary>
        /// <returns> Returns true if this listing's price is as cheap or cheaper than the sum price of child listings. </returns> 
        public bool ListingPriceIsLowest()
        {
            if (ChildPriceSum() < ListingPrice)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary> Determines the sum price of the child listings, if there are any. </summary>
        /// <returns> Returns the sum price of the child listings if there are any, otherwise it returns this listing's listing price. </returns> 
        public int ChildPriceSum()
        {
            if (ChildListings == null)
            {
                return ListingPrice;
            }

            int sum = 0;

            foreach (Listing listing in ChildListings)
            {
                sum += listing.ChildPriceSum();
            }

            return (sum <= SaleOrDefaultPrice()) ? sum : SaleOrDefaultPrice();
        }
        /// <summary> Determines the actual price of this listing, including discounts. </summary>
        /// <returns> Returns the sale price of this listing if able, otherwise returns the base listing price. </returns> 
        public int SaleOrDefaultPrice()
        {
            if (this?.DiscountedListings?.Count != 0)
            {
                double finalPercent = 1.0;

                foreach (DiscountedListing discount in DiscountedListings)
                {
                    if (discount.IsLive())
                    {
                        finalPercent *= (1 - (discount.ItemDiscountPercent / 100.00));
                    }
                }

                int finalPrice = (int)System.Math.Floor(finalPercent * ListingPrice);

                return (finalPrice > 0) ? finalPrice : 1;
            }
            else
            {
                return ListingPrice;
            }
        }
        /// <summary> Calculates the sales percent as a decimal. </summary>
        /// <returns> Returns the sales percent of this listing as a decimal. </returns> 
        public double GetSalePercent()
        {
            if (DiscountedListings.Count != 0)
            {
                double finalPercent = 1.0;

                foreach (DiscountedListing discount in DiscountedListings)
                {
                    if (discount.IsLive())
                    {
                        finalPercent *= (1 - (discount.ItemDiscountPercent / 100.00));
                    }
                }

                return 1 - finalPercent;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// A simple routine to determine if the price unit should be "point" or "points"
        /// </summary>
        /// <returns> "point" if price is equal to 1, "points" otherwise</returns>
        /// <remarks> Always returns a lowercase string </remarks>
        public string GetPluralizedSalePriceUnit()
        {
            if (SaleOrDefaultPrice() != 1)
            {
                return "points";
            }
            else
            {
                return "point";
            }
        }
        #endregion

        /// <summary> A collection of parent listings for a simple listings</summary>
        public virtual ICollection<Listing> ParentListings { get; set; }
        #region Parent Listings

        /// <summary> Adds a parent listing to this simple listing</summary>
        /// <param name="listing">The parent listing to add</param>
        public void AddParent(Listing listing)
        {
            // initialize a ChildListings list for this listing, and a ParentListings list for the child element to be added, if need be
            if (ParentListings == null)
            {
                ParentListings = new HashSet<Listing>();
            }

            if (listing.ChildListings == null)
            {
                listing.ChildListings = new HashSet<Listing>();
            }

            ParentListings.Add(listing);
            listing.ChildListings.Add(this);

            listing.UpdateQuantity();
        }
        /// <summary> Removes a parent listing from this simple listing</summary>
        /// <param name="listing">The parent listing to add</param>
        /// <returns>The parent listing if it existed, null otherwise. </returns> 
        public Listing RemoveParent(Listing listing)
        {
            if (ParentListings == null)
            {
                return null;
            }

            if (ParentListings.Contains(listing))
            {
                ParentListings.Remove(listing);
                return listing;
            }

            return null;
        }

        #endregion

        /// <summary> A collection of platform entities that this listing is redeemable for, simple listings will only have a single platform. </summary>
        public virtual ICollection<Platform> Platforms { get; set; }
        #region Platforms

        /// <summary> Attaches the specified platform entity to this listing. </summary>
        /// <param name="platform">The platform entity to be attached. </param>
        public void AddPlatform(Platform platform)
        {
            if (Platforms == null)
            {
                Platforms = new HashSet<Platform>();
            }

            if (Platforms.Where(p => object.Equals(p.PlatformName, platform.PlatformName)).Count() == 0)
            {
                Platforms.Add(platform);
            }

            if (platform.Listings == null)
            {
                platform.Listings = new HashSet<Listing>();
            }

            if (platform.Listings.Where(p => object.Equals(p.ListingID, this.ListingID)).Count() == 0)
            {
                platform.Listings.Add(this);
            }
        }
        /// <summary> Determines if this listing contains the specified platform entity. </summary>
        /// <param name="platform">The platform to be tested. </param>
        /// <returns> Returns true if this listing contains the platform, and false otherwise. </returns> 
        public bool ContainsPlatform(Platform platform)
        {
            if (this?.Platforms?.Count == 0)
            {
                return false;
            }

            foreach (Platform listingPlatform in Platforms)
            {
                if (listingPlatform.PlatformName.CompareTo(platform.PlatformName) == 0)
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary> Determines if this listing contains the platform entity with the specified platform name. </summary>
        /// <param name="platformName">The name of the platform to be tested. </param>
        /// <returns> Returns true if this listing contains the platform with the specified name, and false otherwise. </returns> 
        public bool ContainsPlatform(string platformName)
        {
            if (this?.Platforms?.Count == 0)
            {
                return false;
            }

            foreach (Platform listingPlatform in Platforms)
            {
                if (listingPlatform.PlatformName.CompareTo(platformName) == 0)
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary> Builds a fully qualified URL to the listing's remote store page. </summary>
        /// <returns> Returns a fully qualified URL to the listin's remote store page, or an empty string if it's not able to. </returns> 
        public string GetQualifiedStorePageURL()
        {
            // if this is not a complex listing, try to build a qualified URL to the store page for this product
            if (Platforms.Count == 1)
            {
                return Platforms.First().StorePageURL + ((Product.AppID == 0) ? Product.StringID : Product.AppID.ToString());
            }
            else
            {
                //as a fall back try to find Steam
                foreach (Platform platform in Platforms)
                {
                    if (platform.PlatformName.ToLower().CompareTo("steam") == 0)
                    {
                        return platform.StorePageURL + Product.AppID.ToString();
                    }
                }
            }

            return String.Empty;
        }
        public string GetQualifiedSteamStorePageURL()
        {
            foreach (Platform platform in Platforms)
            {
                if (platform.PlatformName.ToLower().CompareTo("steam") == 0)
                {
                    if (IsComplex())
                    {
                        return "https://store.steampowered.com/sub/" + Product.AppID.ToString();
                    }
                    else
                    {
                        return "https://store.steampowered.com/app/" + Product.AppID.ToString();
                    }
                }
            }

            return String.Empty;
        }

        #endregion

        /// <summary> A prize entity associated with this listing. </summary>
        public virtual Prize Prize { get; set; }
        #region Prize

        /// <summary> Attaches the specified prize entity to this listing. </summary>
        /// <param name="prize"> The specified prize entity to attach to this listing. </param>
        public void AddPrize(Prize prize)
        {
            prize.Listing = this;
            this.Prize = prize;
        }

        #endregion Prize

        /// <summary> An associated Product entity that stores platform independent information. </summary>
        public virtual Product Product { get; set; }
        #region Product

        /// <summary> A method for attaching a Product entity to this listing. </summary>
        /// <param name="product">The associated Product entity to attach. </param>
        public void AddProduct(Product product)
        {
            if (product.Listings == null)
            {
                product.Listings = new HashSet<Listing>();
            }

            product.Listings.Add(this);
            Product = product;
        }
        /// <summary> Determines if this listing's product contains the specified tag. </summary>
        /// <param name="tag">The tag to be tested. </param>
        /// <returns> Returns true if this listing's product contains the tag, and false otherwise. </returns> 
        public bool ContainsTag(Tag tag)
        {
            if (Product != null)
            {
                return Product.HasTag(tag);
            }

            return false;
        }
        /// <summary>
        /// Returns the list of tags for this listing as a human readable list
        /// </summary>
        /// <returns> A string of a human readable list tags or an empty string if there are none. </returns>
        public String TagsAsReadableList()
        {
            if (Product == null || Product.Tags == null)
            {
                return String.Empty;
            }

            StringBuilder output = new StringBuilder(String.Empty);
            List<Tag> tags = Product.Tags.OrderBy(p => p.TagName).ToList();

            for (int i = 0; i < tags.Count - 1; i++)
            {
                output.Append(tags[i].TagName + ", ");
            }
            
            output.Append(tags[tags.Count - 1].TagName);

            return output.ToString();
        }
        /// <summary> Determines if this listing's product contains the specified category. </summary>
        /// <param name="category">The category to be tested. </param>
        /// <returns> Returns true if this listing's product contains the category, and false otherwise. </returns> 
        public bool ContainsProductCategory(ProductCategory category)
        {
            if (Product != null)
            {
                return Product.HasProductCategory(category);
            }

            return false;
        }
        /// <summary>
        /// Returns the list of cateogory for this listing as a human readable list
        /// </summary>
        /// <returns> A string of a human readable list categories or an empty string if there are none. </returns>
        public String ProductCategoriesAsReadableList()
        {
            if (Product == null || Product.ProductCategories == null)
            {
                return String.Empty;
            }

            StringBuilder output = new StringBuilder(String.Empty);

            List<ProductCategory> categories = Product.ProductCategories.OrderBy(p => p.CategoryString).ToList();

            for (int i = 0; i < categories.Count - 1; i++)
            {
                output.Append(categories[i].CategoryString + ", ");
            }
            
            output.Append(categories[categories.Count - 1].CategoryString);

            return output.ToString();
        }

        #endregion

        /// <summary> A collection of product keys associated with this listing. </summary>
        public virtual ICollection<ProductKey> ProductKeys { get; set; }
        #region ProductKeys

        /// <summary> Adds the specified product key to this listing and updates this listing's quantity. </summary>
        /// <param name="productKey"> The specified product key to add to this listing. </param>
        public void AddProductKeyAndUpdateQuantity(ProductKey productKey)
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
        /// <summary> Removes the specified product key entity from this listing and update the quantity of this listing. </summary>
        /// <param name="productKey"> The specified product key entity to remove from this listing. </param>
        /// <returns> Returns the removed product key entity if it existed, otherwise returns null. </returns> 
        public ProductKey RemoveProductKeyAndUpdateQuantity(ProductKey productKey)
        {
            if (productKey == null)
            {
                return null;
            }

            if (this?.ProductKeys?.Count == 0)
            {
                return null;
            }

            if (ProductKeys.Contains(productKey))
            {
                ProductKeys.Remove(productKey);
                Quantity--;
                UpdateParentQuantities();

                return productKey;
            }

            return null;
        }
        /// <summary> Determines the number of product keys associated with this listing. </summary>
        /// <returns> Returns the number of product keys associated with this listing. </returns> 
        public int ListingKeysQuantity()
        {
            if (ProductKeys == null)
            {
                return 0;
            }

            return ProductKeys.Count;
        }

        #endregion

        /// <summary> How many copies of this listing are available for purchase. </summary>
        public int Quantity { get; set; }
        #region Quantity

        /// <summary> Updates the quantity of this listing. </summary>
        public void UpdateQuantity()
        {
            if (this?.ChildListings?.Count > 0)
            {
                int min = System.Int32.MaxValue;

                foreach (Listing listing in ChildListings)
                {
                    if (listing.Quantity < min)
                    {
                        min = listing.Quantity;
                    }
                }

                // min represents the minimum quantity of all child listings
                // ListingKeysQuantity represents the number of keys that redeem this listing explicitly
                // thus combining them gets the total number of copies available (the number of copies we get from combining keys from child listings..
                // ... plus the number of copies directly for this listing)
                this.Quantity = min + ListingKeysQuantity();
            }
            else if (ProductKeys != null)
            {
                Quantity = ProductKeys.Count;
            }
            else
            {
                Quantity = 0;
            }
        }
        /// <summary> Updates the quantity of all parent listings of this listing. </summary>
        /// <remarks> Useful for when the quantity of this listing is changed or when adding this as a child listing to another listing. </remarks>
        public void UpdateParentQuantities()
        {
            if (ParentListings == null)
            {
                return;
            }

            foreach (Listing listing in ParentListings)
            {
                listing.UpdateQuantity();

                if (listing.ParentListings != null)
                {
                    listing.UpdateParentQuantities();
                }
            }
        }
        /// <summary>
        /// A simple routine to determine if the quantity unit should be "copy" or "copies"
        /// </summary>
        /// <returns> "copy" if quantity is equal to 1, "copies" otherwise</returns>
        /// <remarks> Always returns a lowercase string </remarks>
        public string GetPluralizedQuantityUnit()
        {
            if (Quantity != 1)
            {
                return "copies";
            }
            else
            {
                return "copy";
            }
        }
        #endregion

        /// <summary> A collection of shopping cart entries associated with this listing. </summary>
        public virtual ICollection<ShoppingCartEntry> ShoppingCartEntries { get; set; }
        #region ShoppingCartEntry

        /// <summary> Adds the specified shopping cart entry entity to this listing. </summary>
        /// <param name="cartEntry"> The specified shopping cart entry entity to add to this listing. </param>
        public void AddShoppingCartEntry(ShoppingCartEntry cartEntry)
        {
            if (ShoppingCartEntries == null)
            {
                ShoppingCartEntries = new HashSet<ShoppingCartEntry>();
            }

            cartEntry.Listing = this;
            this.ShoppingCartEntries.Add(cartEntry);
        }
        /// <summary> Removes the specified shopping cart entry entity from this listing. </summary>
        /// <param name="cartEntry"> The specified shopping cart entry entity to remove from this listing. </param>
        /// <returns> Returns the removed shopping cart entry entity if it existed, otherwise returns null. </returns> 
        public ShoppingCartEntry RemoveShoppingCartEntry(ShoppingCartEntry cartEntry)
        {
            if (this?.ShoppingCartEntries?.Count == 0)
            {
                return null;
            }

            if (ShoppingCartEntries.Contains(cartEntry))
            {
                ShoppingCartEntries.Remove(cartEntry);
                return cartEntry;
            }

            return null;
        }

        #endregion

        /// <summary> A collection of application users who have blacklisted (hidden) this listing. </summary>
        public virtual ICollection<AppUser> UsersBlacklist { get; set; }

        /// <summary> A collection of wishlist entities associated to this listing (a mapping between application users who want this listing and the listing). </summary>
        public virtual ICollection<WishlistEntry> WishlistEntries { get; set; }
        #region WishlistEntries

        /// <summary> Adds an associated wishlist entry entity to this listing. </summary>
        /// <param name="wishlistEntry"> The associated wishlist entry entity to add to this listing. </param>
        public void AddWishlistEntry(WishlistEntry wishlistEntry)
        {
            if (WishlistEntries == null)
            {
                WishlistEntries = new HashSet<WishlistEntry>();
            }

            wishlistEntry.Listing = this;
            WishlistEntries.Add(wishlistEntry);
        }
        /// <summary> Removes the specified wishlist entry entity from this listing. </summary>
        /// <param name="wishlistEntry"> The specified wishlist entry entity to remove from this listing. </param>
        /// <returns> Returns the removed wishlist entry entity if it existed, otherwise returns null. </returns> 
        public WishlistEntry RemoveWishlistEntry(WishlistEntry wishlistEntry)
        {
            if (this?.WishlistEntries?.Count == 0)
            {
                return null;
            }

            if (WishlistEntries.Contains(wishlistEntry))
            {
                WishlistEntries.Remove(wishlistEntry);
                return wishlistEntry;
            }

            return null;
        }

        #endregion
    }
}