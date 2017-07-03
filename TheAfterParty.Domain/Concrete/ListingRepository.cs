using System;
using System.Collections.Generic;
using System.Linq;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using System.Data.Entity;
using TheAfterParty.Domain.Model;


namespace TheAfterParty.Domain.Concrete
{
    public class ListingRepository : IListingRepository, IDisposable
    {
        private AppIdentityDbContext context;
        public AppIdentityDbContext GetContext()
        {
            return context;
        }

        public ListingRepository(IUnitOfWork unitOfWork)
        {
            this.context = unitOfWork.DbContext;
        }

        public DateTime GetNewestDate()
        {
            return context.Listings.Where(l => l.Quantity > 0).OrderByDescending(x => x.DateEdited).FirstOrDefault().DateEdited.Date;
        }

    // ---- Listing entity persistance
    // ---- Parent entity

/*REMOVE*/public IEnumerable<Listing> GetListingsPlain()
        {
            return context.Listings;
        }
        public IEnumerable<Listing> GetListingsWithFilter(ListingFilter filter, out int TotalItems, List<Listing> listings = null)
        {
            IQueryable<Listing> listingQuery;

            if (listings == null)
            {
                listingQuery = context.Listings
                            .Include(x => x.ChildListings.Select(y => y.Product.Listings.Select(z => z.ChildListings)))
                            .Include(x => x.ChildListings.Select(y => y.Platforms))
                            .Include(x => x.ChildListings.Select(y => y.DiscountedListings))
                            .Include(x => x.DiscountedListings)
                            .Include(x => x.Platforms)
                            .Include(x => x.Product.Listings.Select(z => z.ChildListings))
                            .AsQueryable()
                            .Where(x => x.ListingPrice > 0 && x.Quantity > 0);
            }
            else
            {
                listingQuery = listings.AsQueryable<Listing>();
            }

            if (filter.IsDailyDeal)
            {
                listingQuery = listingQuery.Where(x => x.DiscountedListings.Any(y => y.DailyDeal));
            }

            if (filter.WishlistFilter)
            {
                if (filter.LoggedInUser != null)
                {
                    listingQuery = listingQuery.Where(x => filter.LoggedInUser.WishlistEntries.Select(y => y.AppID).Any(z => z == x.Product.AppID));
                }
                else
                {
                    listingQuery = listingQuery.Where(x => context.WishlistEntries.Where(y => y.UserID.Equals(filter.UserID)).Select(y => y.AppID).Any(z => z == x.Product.AppID));
                }
            }

            if (filter.IsOtherDeal)
            {
                listingQuery = listingQuery.Where(x => x.DiscountedListings.Any(y => y.DailyDeal == false && y.WeeklyDeal == false));
            }

            if (filter.IsWeeklyDeal)
            {
                listingQuery = listingQuery.Where(x => x.DiscountedListings.Any(y => y.WeeklyDeal));
            }

            if (filter.IsAllDeal)
            {
                listingQuery = listingQuery.Where(x => x.DiscountedListings.Any());
            }

            if (filter.IsNewest)
            {
                DateTime date;

                if (filter.NewestDate != null )
                {
                    date = (DateTime)filter.NewestDate;
                }
                else
                {
                    date = context.Listings.Where(l => l.Quantity > 0).OrderByDescending(x => x.DateEdited).FirstOrDefault().DateEdited.Date;
                }

                var day = date.Day;
                var month = date.Month;
                var year = date.Year;

                listingQuery = listingQuery.Where(x => x.DateEdited.Day == day && x.DateEdited.Year == year && x.DateEdited.Month == month);
            }

            if (filter.Date != null)
            {
                var date = (DateTime)filter.Date;
                var day = date.Day;
                var month = date.Month;
                var year = date.Year;

                listingQuery = listingQuery.Where(x => x.DateEdited.Day == day && x.DateEdited.Year == year && x.DateEdited.Month == month);
            }

            if (String.IsNullOrWhiteSpace(filter.SearchText) == false)
            {
                var searchText = filter.SearchText.ToLower();

                listingQuery = listingQuery.Where(x => x.ListingName.ToLower().Contains(searchText));
            }

            if (filter.BeginsWithFilter != filter.BeginsWithSentinel)
            {
                string beginsWithText = filter.BeginsWithFilter.ToString();

                if (filter.BeginsWithSentinel != '0')
                {
                    listingQuery = listingQuery.Where(x => x.ListingName.StartsWith(beginsWithText) || x.ListingName.ToLower().StartsWith(beginsWithText));
                }
                else
                {
                    listingQuery = listingQuery.Where(x => !char.IsLetter(x.ListingName.First()));
                }
            }

            if (String.IsNullOrWhiteSpace(filter.UserID) == false)
            {
                if (filter.UnownedFilter)
                {
                    if (filter.LoggedInUser != null)
                    {
                        listingQuery = listingQuery.Where(x => !filter.LoggedInUser.OwnedGames.Select(y => y.AppID).Any(z => z == x.Product.AppID));
                    }
                    else
                    {
                        listingQuery = listingQuery.Where(x => !context.OwnedGames.Where(y => y.UserID.Equals(filter.UserID)).Select(y => y.AppID).Any(z => z == x.Product.AppID));
                    }
                }
            }

            if ((filter.UnownedFilter | filter.BlacklistFilter | filter.AffordableFilter) == false)
            {
                if (filter.FriendAppIDs.Count() > 0)
                {
                    listingQuery = listingQuery.Where(x => !filter.FriendAppIDs.Any(z => z == x.Product.AppID));
                }
            }

            if (filter.PlatformID != 0)
            {
                listingQuery = listingQuery.Where(x => x.Platforms.Select(y => y.PlatformID).Contains(filter.PlatformID));
            }

            if (String.IsNullOrWhiteSpace(filter.UserID) == false)
            {
                if (filter.BlacklistFilter)
                {
                    if (filter.LoggedInUser != null)
                    {
                        listingQuery = listingQuery.Where(x => !filter.LoggedInUser.BlacklistedListings.Select(z => z.ListingID).Any(a => a == x.ListingID));
                    }
                    else
                    {
                        listingQuery = listingQuery.Where(x => !context.Users.FirstOrDefault(y => y.Id.Equals(filter.UserID)).BlacklistedListings.Select(z => z.ListingID).Any(a => a == x.ListingID));
                    }
                }
            }

            if (String.IsNullOrWhiteSpace(filter.UserID) == false)
            {
                if (filter.AffordableFilter)
                {
                    if (filter.LoggedInUser != null)
                    {
                        listingQuery = listingQuery.Where(x => x.ListingPrice <= filter.LoggedInUser.Balance);
                    }
                    else
                    {
                        listingQuery = listingQuery.Where(x => x.ListingPrice <= context.Users.FirstOrDefault(y => y.Id.Equals(filter.UserID)).Balance);
                    }
                }
            }

            if (filter.TagIDs.Count > 0)
            {
                listingQuery = listingQuery.Where(x => !filter.TagIDs.Except(x.Product.Tags.Select(y => y.TagID)).Any());
            }

            if (filter.ProductCategoryIDs.Count > 0)
            {
                listingQuery = listingQuery.Where(x => !filter.ProductCategoryIDs.Except(x.Product.ProductCategories.Select(y => y.ProductCategoryID)).Any());
            }

            if (filter.GameSort > 0)
            {
                if (filter.GameSort == 1)
                {
                    listingQuery = listingQuery.OrderBy(x => x.ListingName);
                }
                else
                {
                    listingQuery = listingQuery.OrderByDescending(x => x.ListingName);
                }
            }
            else if (filter.PriceSort > 0)
            {
                if (filter.PriceSort == 1)
                {
                    listingQuery = listingQuery.OrderBy(x => x.ListingPrice).ThenBy(x => x.ListingName);
                }
                else
                {
                    listingQuery = listingQuery.OrderByDescending(x => x.ListingPrice).ThenBy(x => x.ListingName);
                }
            }
            else if (filter.QuantitySort > 0)
            {
                if (filter.QuantitySort == 1)
                {
                    listingQuery = listingQuery.OrderBy(x => x.Quantity).ThenBy(x => x.ListingName);
                }
                else
                {
                    listingQuery = listingQuery.OrderByDescending(x => x.Quantity).ThenBy(x => x.ListingName);
                }
            }

            TotalItems = listingQuery.Count();

            if (filter.PaginationNum > 0 && filter.Page > 0)
            {
                listingQuery = listingQuery.Skip((filter.Page - 1) * filter.PaginationNum).Take(filter.PaginationNum);
            }
            else if (filter.PaginationNum > 0)
            {
                listingQuery = listingQuery.Take(filter.PaginationNum);
            }

            return listingQuery;
        }
        public IEnumerable<Listing> SearchListings(string searchText, int resultLimit)
        {
            IQueryable<Listing> listingQuery = context.Listings
                        .Include(x => x.ChildListings.Select(y => y.Product))
                        .Include(x => x.ChildListings.Select(y => y.Platforms))
                        .Include(x => x.ChildListings.Select(y => y.DiscountedListings))
                        .Include(x => x.DiscountedListings)
                        .Include(x => x.Platforms)
                        .Include(x => x.Product)
                        .AsQueryable()
                        .Where(x => x.ListingPrice > 0 && x.Quantity > 0);

            searchText = searchText.Trim().ToLower();

            listingQuery = listingQuery.Where(x => x.ListingName.ToLower().Contains(searchText)).Take(resultLimit);

            return listingQuery;
        }
        public IQueryable<Listing> GetListingsQuery()
        {
            return context.Listings
                        .Include(x => x.Product.Tags)
                        .Include(x => x.Product.ProductCategories)
                        .Include(x => x.Platforms)
                        .Include(x => x.ChildListings.Select(y => y.Product.Listings))
                        .Include(x => x.ChildListings.Select(y => y.Platforms))
                        .Include(x => x.ChildListings.Select(y => y.DiscountedListings))
                        .Include(x => x.DiscountedListings)
                        .Include(x => x.UsersBlacklist)
                        .AsQueryable();
            //                        .ToList();
        }
        public IEnumerable<Listing> GetListings()
        {
            return context.Listings
                        .Include(x => x.Product.Tags)
                        .Include(x => x.Product.ProductCategories)
                        .Include(x => x.Product.Listings.Select(z => z.ChildListings))
                        .Include(x => x.Platforms)
                        .Include(x => x.ChildListings.Select(y => y.Product.Listings))
                        .Include(x => x.ChildListings.Select(y => y.Platforms))
                        .Include(x => x.ChildListings.Select(y => y.DiscountedListings))
                        .Include(x => x.DiscountedListings)
                        .Include(x => x.UsersBlacklist);
//                        .ToList();
        }
        public Listing GetListingByID(int id)
        {
            //return context.Listings.Find(id);

            return context.Listings
                        .Include(x => x.Product.Tags)
                        .Include(x => x.Product.ProductCategories)
                        .Include(x => x.Platforms)
                        .Include(x => x.ChildListings.Select(y => y.Product))
                        .Include(x => x.UsersBlacklist)
                        .Include(x => x.DiscountedListings)
                        .SingleOrDefault(x => x.ListingID == id);
        }
        public void InsertListing(Listing listing)
        {
            context.Listings.Add(listing);

            if (listing.ParentListings != null)
            {
                foreach (Listing parentListing in listing.ParentListings)
                {
                    if (parentListing.ListingID == 0)
                    {
                        InsertListing(parentListing);
                    }
                    else
                    {
                        UpdateListing(parentListing);
                    }
                }
            }

            if (listing.Product != null)
            {
                if (listing.Product.ProductID == 0)
                {
                    InsertProduct(listing.Product);
                }
                else
                {
                    UpdateProduct(listing.Product);
                }
            }

            if (listing.DiscountedListings != null)
            {
                foreach (DiscountedListing entry in listing.DiscountedListings)
                {
                    if (entry.DiscountedListingID == 0)
                    {
                        InsertDiscountedListing(entry);
                    }
                    else
                    {
                        UpdateDiscountedListing(entry);
                    }
                }
            }

            if (listing.ProductKeys != null)
            {
                foreach (ProductKey entry in listing.ProductKeys)
                {
                    if (entry.ProductKeyID == 0)
                    {
                        InsertProductKey(entry);
                    }
                    else
                    {
                        UpdateProductKey(entry);
                    }
                }
            }

            if (listing.ListingComments != null)
            {
                foreach (ListingComment entry in listing.ListingComments)
                {
                    if (entry.ListingCommentID == 0)
                    {
                        InsertListingComment(entry);
                    }
                    else
                    {
                        UpdateListingComment(entry);
                    }
                }
            }
        }
        public void UpdateListing(Listing listing)
        {
            Listing targetListing = context.Listings.Find(listing.ListingID);

            if (targetListing != null)
            {
                targetListing.ListingName = listing.ListingName;
                targetListing.ListingPrice = listing.ListingPrice;
                targetListing.Quantity = listing.Quantity;
                targetListing.DateEdited = listing.DateEdited;
            }

            if (listing.ParentListings != null)
            {
                foreach (Listing parentListing in listing.ParentListings)
                {
                    if (parentListing.ListingID == 0)
                    {
                        InsertListing(parentListing);
                    }
                    else
                    {
                        UpdateListing(parentListing);
                    }
                }
            }

            if (listing.Product != null)
            {
                if (listing.Product.ProductID == 0)
                {
                    InsertProduct(listing.Product);
                }
                else
                {
                    UpdateProduct(listing.Product);
                }
            }

            if (listing.DiscountedListings != null)
            {
                foreach (DiscountedListing entry in listing.DiscountedListings)
                {
                    if (entry.DiscountedListingID == 0)
                    {
                        InsertDiscountedListing(entry);
                    }
                    else
                    {
                        UpdateDiscountedListing(entry);
                    }
                }
            }

            if (listing.ProductKeys != null)
            {
                foreach (ProductKey entry in listing.ProductKeys)
                {
                    if (entry.ProductKeyID == 0)
                    {
                        InsertProductKey(entry);
                    }
                    else
                    {
                        UpdateProductKey(entry);
                    }
                }
            }

            if (listing.ListingComments != null)
            {
                foreach (ListingComment entry in listing.ListingComments)
                {
                    if (entry.ListingCommentID == 0)
                    {
                        InsertListingComment(entry);
                    }
                    else
                    {
                        UpdateListingComment(entry);
                    }
                }
            }
        }
        public void DeleteListing(int listingID)
        {
            Listing listing = context.Listings.Find(listingID);
            context.Listings.Remove(listing);
        }



        // ---- Product entity persistance

        public IEnumerable<Product> GetProducts()
        {
            return context.Products
                            .Include(x => x.ProductCategories)
                            .Include(x => x.Tags);
        }
        public Product GetProductByID(int id)
        {
            //return context.Products.Find(id);

            return context.Products
                            .Include(x => x.ProductCategories)
                            .Include(x => x.Tags)
                            .SingleOrDefault(x => x.ProductID == id);
        }
        public void InsertProduct(Product product)
        {
            context.Products.Add(product);

            if (product.Tags != null)
            {
                foreach (Tag entry in product.Tags)
                {
                    if (entry.TagID == 0)
                    {
                        InsertTag(entry);
                    }
                    else
                    {
                        UpdateTag(entry);
                    }
                }
            }

            if (product.ProductReviews != null)
            {
                foreach (ProductReview entry in product.ProductReviews)
                {
                    if (entry.ProductReviewID == 0)
                    {
                        InsertProductReview(entry);
                    }
                    else
                    {
                        UpdateProductReview(entry);
                    }
                }
            }
            
            if (product.ProductCategories != null)
            {
                foreach (ProductCategory category in product.ProductCategories)
                {
                    if (category.ProductCategoryID == 0)
                    {
                        InsertProductCategory(category);
                    }
                    else
                    {
                        UpdateProductCategory(category);
                    }
                }
            }
        }
        public void UpdateProduct(Product product)
        {
            Product targetProduct = context.Products.Find(product.ProductID);

            if (targetProduct != null)
            {
                targetProduct.AppID = product.AppID;
                targetProduct.ProductName = product.ProductName;
                targetProduct.StringID = product.StringID;
                targetProduct.HeaderImageURL = product.HeaderImageURL;
            }

            if (product.Tags != null)
            {
                foreach (Tag entry in product.Tags)
                {
                    if (entry.TagID == 0)
                    {
                        InsertTag(entry);
                    }
                    else
                    {
                        UpdateTag(entry);
                    }
                }
            }

            if (product.ProductReviews != null)
            {
                foreach (ProductReview entry in product.ProductReviews)
                {
                    if (entry.ProductReviewID == 0)
                    {
                        InsertProductReview(entry);
                    }
                    else
                    {
                        UpdateProductReview(entry);
                    }
                }
            }
            
            if (product.ProductCategories != null)
            {
                foreach (ProductCategory category in product.ProductCategories)
                {
                    if (category.ProductCategoryID == 0)
                    {
                        InsertProductCategory(category);
                    }
                    else
                    {
                        UpdateProductCategory(category);
                    }
                }
            }
        }
        public void DeleteProduct(int productId)
        {
            Product product = context.Products.Find(productId);
            context.Products.Remove(product);
        }

        // ---- DiscoutnedListing entity persistance

        public IEnumerable<DiscountedListing> GetDiscountedListings()
        {
            return context.DiscountedListings
                                .Include(x => x.Listing)
                                .ToList();
        }
        public DiscountedListing GetDiscountedListingByID(int id)
        {
            //return context.DiscountedListings.Find(id);

            return context.DiscountedListings
                                .Include(x => x.Listing)
                                .SingleOrDefault(x => x.DiscountedListingID == id);
        }
        public void InsertDiscountedListing(DiscountedListing discountedListing)
        {
            context.DiscountedListings.Add(discountedListing);
        }
        public void UpdateDiscountedListing(DiscountedListing discountedListing)
        {
            DiscountedListing targetDiscountedListing = context.DiscountedListings.Find(discountedListing.ListingID);

            if (targetDiscountedListing != null)
            {
                targetDiscountedListing.ItemDiscountPercent = discountedListing.ItemDiscountPercent;
                targetDiscountedListing.ItemSaleExpiry = discountedListing.ItemSaleExpiry;
                targetDiscountedListing.DailyDeal = discountedListing.DailyDeal;
                targetDiscountedListing.WeeklyDeal = discountedListing.WeeklyDeal;
            }
        }
        public void DeleteDiscountedListing(int discountedListingId)
        {
            DiscountedListing discountedListing = context.DiscountedListings.Find(discountedListingId);

            if (discountedListing != null)
            {
                context.DiscountedListings.Remove(discountedListing);
            }
        }

        
        //  ---- ListingComment entity persistance

        public IEnumerable<ListingComment> GetListingComments()
        {
            return context.ListingComments.ToList();
        }
        public ListingComment GetListingCommentByID(int id)
        {
            return context.ListingComments.Find(id);
        }
        public void InsertListingComment(ListingComment listingComment)
        {
            context.ListingComments.Add(listingComment);
        }
        public void UpdateListingComment(ListingComment listingComment)
        {
            ListingComment targetListingComment = context.ListingComments.Find(listingComment.ListingCommentID);

            if (targetListingComment != null)
            {
                targetListingComment.Comment = listingComment.Comment;
                targetListingComment.IsEdited = listingComment.IsEdited;
                targetListingComment.LastEdited = listingComment.LastEdited;
                targetListingComment.ListingID = listingComment.ListingID;
                targetListingComment.PostDate = listingComment.PostDate;
                targetListingComment.UserID = listingComment.UserID;
            }
        }
        public void DeleteListingComment(int listingCommentId)
        {
            ListingComment listingComment = context.ListingComments.Find(listingCommentId);
            context.ListingComments.Remove(listingComment);
        }

        // ---- ProductKey entity persistance

        public IEnumerable<ProductKey> GetProductKeys()
        {
            return context.ProductKeys
                                .Include(x => x.Listing)
                                .Include(x => x.Listing.ParentListings)
                                .Include(x => x.Listing.ChildListings)
                                .ToList();
        }
        public ProductKey GetProductKeyByID(int id)
        {
            //return context.ProductKeys.Find(id);
            
            return context.ProductKeys
                                .Include(x => x.Listing)
                                .Include(x => x.Listing.ParentListings)
                                .Include(x => x.Listing.ChildListings)
                                .SingleOrDefault(x => x.ProductKeyID == id);
        }
        public void InsertProductKey(ProductKey productKey)
        {
            context.ProductKeys.Add(productKey);
        }
        public void UpdateProductKey(ProductKey productKey)
        {
            ProductKey targetProductKey = context.ProductKeys.Find(productKey.ProductKeyID);

            if (targetProductKey != null)
            {
                targetProductKey.ListingID = productKey.ListingID;
                targetProductKey.ItemKey = productKey.ItemKey;
                targetProductKey.IsGift = productKey.IsGift;
            }
        }
        public void DeleteProductKey(int productKeyId)
        {
            ProductKey productKey = context.ProductKeys.Find(productKeyId);
            context.ProductKeys.Remove(productKey);
        }


        // ---- ProductReview entity persistance

        public IEnumerable<ProductReview> GetProductReviews()
        {
            return context.ProductReviews.ToList();
        }
        public ProductReview GetProductReviewByID(int productReviewID)
        {
            return context.ProductReviews.Find(productReviewID);
        }
        public void InsertProductReview(ProductReview productReview)
        {
            context.ProductReviews.Add(productReview);
        }
        public void UpdateProductReview(ProductReview productReview)
        {
            ProductReview targetProductReview = context.ProductReviews.Find(productReview.ProductReviewID);

            if (targetProductReview != null)
            {
                targetProductReview.IsEdited = productReview.IsEdited;
                targetProductReview.IsRecommended = productReview.IsRecommended;
                targetProductReview.LastEdited = productReview.LastEdited;
                targetProductReview.ProductID = productReview.ProductID;
                targetProductReview.PostDate = productReview.PostDate;
                targetProductReview.Review = productReview.Review;
                targetProductReview.UserID = productReview.UserID;
            }
        }
        public void DeleteProductReview(int productReviewId)
        {
            ProductReview productReview = context.ProductReviews.Find(productReviewId);
            context.ProductReviews.Remove(productReview);
        }


        // ---- Tag entity persistance

        public IEnumerable<Tag> GetTags()
        {
            return context.Tags;
        }
        public Tag GetTagByID(int id)
        {
            //return context.Tags.Find(id);

            return context.Tags.Find(id);
        }
        public void InsertTag(Tag tag)
        {
            context.Tags.Add(tag);
        }
        public void UpdateTag(Tag tag)
        {
            Tag targetTag = context.Tags.Find(tag.TagID);

            if (targetTag != null)
            {
                targetTag.TagName = tag.TagName;
            }
        }
        public void DeleteTag(int tagId)
        {
            Tag tag = context.Tags.Find(tagId);
            context.Tags.Remove(tag);
        }
        

        // ---- Platform entity persistance

        public IEnumerable<Platform> GetPlatforms()
        {
            return context.Platforms;
        }
        public IEnumerable<Platform> GetActivePlatforms()
        {
            return context.Platforms.Where(p => p.Listings.Any(l => l.Quantity > 0)).OrderBy(p => p.PlatformName);
        }
        public Platform GetPlatformByID(int platformId)
        {
            return context.Platforms.Find(platformId);
        }
        public void InsertPlatform(Platform platform)
        {
            context.Platforms.Add(platform);
        }
        public void UpdatePlatform(Platform platform)
        {
            Platform targetPlatform = context.Platforms.Find(platform.PlatformID);

            if (targetPlatform != null)
            {
                targetPlatform.PlatformIconURL = platform.PlatformIconURL;
                targetPlatform.PlatformName = platform.PlatformName;
                targetPlatform.PlatformURL = platform.PlatformURL;
                targetPlatform.HasAppID = platform.HasAppID;
                targetPlatform.StorePageURL = platform.StorePageURL;
            }
        }
        public void DeletePlatform(int platformId)
        {
            Platform targetPlatform = context.Platforms.Find(platformId);

            if (targetPlatform != null)
            {
                context.Platforms.Remove(targetPlatform);
            }
        }


        // ---- ProductCategory persistance

        public IEnumerable<ProductCategory> GetProductCategories()
        {
            return context.ProductCategories;
        }
        public ProductCategory GetProductCategoryByID(int productCategoryId)
        {
            return context.ProductCategories.Find(productCategoryId);
        }
        public void InsertProductCategory(ProductCategory productCategory)
        {
            context.ProductCategories.Add(productCategory);
        }
        public void UpdateProductCategory(ProductCategory productCategory)
        {
            ProductCategory targetProductCategory = context.ProductCategories.Find(productCategory.ProductCategoryID);

            if (targetProductCategory != null)
            {
                targetProductCategory.CategoryString = productCategory.CategoryString;
                targetProductCategory.CategoryIconURL = productCategory.CategoryIconURL;
            }
        }
        public void DeleteProductCategory(int productCategoryId)
        {
            ProductCategory targetProductCategory = context.ProductCategories.Find(productCategoryId);

            if (targetProductCategory != null)
            {
                context.ProductCategories.Remove(targetProductCategory);
            }
        }

        // ---- Repository methods

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
