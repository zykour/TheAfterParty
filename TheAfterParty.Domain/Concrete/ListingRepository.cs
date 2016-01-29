using System;
using System.Collections.Generic;
using System.Linq;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;

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



        // ---- Listing entity persistance
            // ---- Parent entity

        public IEnumerable<Listing> GetListings()
        {
            return context.Listings.ToList();
        }
        public Listing GetListingByID(int id)
        {
            return context.Listings.Find(id);
        }
        public void InsertListing(Listing listing)
        {
            context.Listings.Add(listing);

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
        public void DeleteListing(int listingID)
        {
            Listing listing = context.Listings.Find(listingID);
            context.Listings.Remove(listing);
        }



        // ---- Product entity persistance

        public IEnumerable<Product> GetProducts()
        {
            return context.Products.ToList();
        }
        public Product GetProductByID(int id)
        {
            return context.Products.Find(id);
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

            if (product.ProductDetail != null)
            {
                if (product.ProductDetail.ProductDetailID == 0)
                {
                    InsertProductDetail(product.ProductDetail);
                }
                else
                {
                    UpdateProductDetail(product.ProductDetail);
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
            }

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

            if (product.ProductDetail != null)
            {
                if (product.ProductDetail.ProductDetailID == 0)
                {
                    InsertProductDetail(product.ProductDetail);
                }
                else
                {
                    UpdateProductDetail(product.ProductDetail);
                }
            }

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
        public void DeleteProduct(int productId)
        {
            Product product = context.Products.Find(productId);
            context.Products.Remove(product);
        }

        // ---- DiscoutnedListing entity persistance

        public IEnumerable<DiscountedListing> GetDiscountedListings()
        {
            return context.DiscountedListings.ToList();
        }
        public DiscountedListing GetDiscountedListingByID(int id)
        {
            return context.DiscountedListings.Find(id);
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
                targetDiscountedListing.ListingID = discountedListing.ListingID;
                targetDiscountedListing.DailyDeal = discountedListing.DailyDeal;
                targetDiscountedListing.WeeklyDeal = discountedListing.WeeklyDeal;
            }
        }
        public void DeleteDiscountedListing(int discountedListingId)
        {
            DiscountedListing discountedListing = context.DiscountedListings.Find(discountedListingId);
            context.DiscountedListings.Remove(discountedListing);
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


        // ---- ProductDetail entity persistance

        public IEnumerable<ProductDetail> GetProductDetails()
        {
            return context.ProductDetails.ToList();
        }
        public ProductDetail GetProductDetailByID(int id)
        {
            return context.ProductDetails.Find(id);
        }
        public void InsertProductDetail(ProductDetail productDetail)
        {
            context.ProductDetails.Add(productDetail);

            if (productDetail.AppMovies != null)
            {
                foreach (AppMovie movie in productDetail.AppMovies)
                {
                    if (movie.AppMovieID == 0)
                    {
                        InsertAppMovie(movie);
                    }
                    else
                    {
                        UpdateAppMovie(movie);
                    }
                }
            }

            if (productDetail.AppScreenshots != null)
            {
                foreach (AppScreenshot screenshot in productDetail.AppScreenshots)
                {
                    if (screenshot.AppScreenshotID == 0)
                    {
                        InsertAppScreenshot(screenshot);
                    }
                    else
                    {
                        UpdateAppScreenshot(screenshot);
                    }
                }
            }
        }
        public void UpdateProductDetail(ProductDetail productDetail)
        {
            ProductDetail targetProductDetail = context.ProductDetails.Find(productDetail.ProductID);

            if (targetProductDetail != null)
            {
                targetProductDetail.ImageData = productDetail.ImageData;
                targetProductDetail.ImageMimeType = productDetail.ImageMimeType;
                targetProductDetail.ProductName = productDetail.ProductName;
                targetProductDetail.AboutTheGame = productDetail.AboutTheGame;
                targetProductDetail.AgeRequirement = productDetail.AgeRequirement;
                targetProductDetail.AppID = productDetail.AppID;
                targetProductDetail.AvailableOnLinux = productDetail.AvailableOnLinux;
                targetProductDetail.AvailableOnMac = productDetail.AvailableOnMac;
                targetProductDetail.AvailableOnPC = productDetail.AvailableOnPC;
                targetProductDetail.BaseProductID = productDetail.BaseProductID;
                targetProductDetail.BaseProductName = productDetail.BaseProductName;
                targetProductDetail.CurrencyType = productDetail.CurrencyType;
                targetProductDetail.DemoAppID = productDetail.DemoAppID;
                targetProductDetail.DemoRestrictions = productDetail.DemoRestrictions;
                targetProductDetail.DetailedDescription = productDetail.DetailedDescription;
                targetProductDetail.Developers = productDetail.Developers;
                targetProductDetail.DiscountPercent = productDetail.DiscountPercent;
                targetProductDetail.DLCAppIDs = productDetail.DLCAppIDs;
                targetProductDetail.FinalPrice = productDetail.FinalPrice;
                targetProductDetail.Genres = productDetail.Genres;
                targetProductDetail.HeaderImageURL = productDetail.HeaderImageURL;
                targetProductDetail.InitialPrice = productDetail.InitialPrice;
                targetProductDetail.LinuxMinimumRequirements = productDetail.LinuxMinimumRequirements;
                targetProductDetail.LinuxRecommendedRequirements = productDetail.LinuxRecommendedRequirements;
                targetProductDetail.MacMinimumRequirements = productDetail.MacMinimumRequirements;
                targetProductDetail.MacRecommendedRequirements = productDetail.MacRecommendedRequirements;
                targetProductDetail.MetacriticScore = productDetail.MetacriticScore;
                targetProductDetail.MetacriticURL = productDetail.MetacriticURL;
                targetProductDetail.NumAchievements = productDetail.NumAchievements;
                targetProductDetail.PackageIDs = productDetail.PackageIDs;
                targetProductDetail.PCMinimumRequirements = productDetail.PCMinimumRequirements;
                targetProductDetail.PCRecommendedRequirements = productDetail.PCRecommendedRequirements;
                targetProductDetail.ProductID = productDetail.ProductID;
                targetProductDetail.ProductType = productDetail.ProductType;
                targetProductDetail.ProductWebsite = productDetail.ProductWebsite;
                targetProductDetail.Publishers = productDetail.Publishers;
                targetProductDetail.ReleaseDate = productDetail.ReleaseDate;
                targetProductDetail.SupportedLanguages = productDetail.SupportedLanguages;
                targetProductDetail.TotalRecommendations = productDetail.TotalRecommendations;
                targetProductDetail.Unreleased = productDetail.Unreleased;
            }

            foreach (AppMovie movie in productDetail.AppMovies)
            {
                if (movie.AppMovieID == 0)
                {
                    InsertAppMovie(movie);
                }
                else
                {
                    UpdateAppMovie(movie);
                }
            }

            foreach (AppScreenshot screenshot in productDetail.AppScreenshots)
            {
                if (screenshot.AppScreenshotID == 0)
                {
                    InsertAppScreenshot(screenshot);
                }
                else
                {
                    UpdateAppScreenshot(screenshot);
                }
            }
        }
        public void DeleteProductDetail(int productDetailId)
        {
            ProductDetail productDetail = context.ProductDetails.Find(productDetailId);
            context.ProductDetails.Remove(productDetail);
        }


        // ---- ProductKey entity persistance

        public IEnumerable<ProductKey> GetProductKeys()
        {
            return context.ProductKeys.ToList();
        }
        public ProductKey GetProductKeyByID(int id)
        {
            return context.ProductKeys.Find(id);
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
            return context.Tags.ToList();
        }
        public Tag GetTagByID(int id)
        {
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
            return context.Platforms.ToList();
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
                targetPlatform.PlatformIcon = platform.PlatformIcon;
                targetPlatform.PlatformIconMimeType = platform.PlatformIconMimeType;
                targetPlatform.PlatformName = platform.PlatformName;
                targetPlatform.PlatformURL = platform.PlatformURL;
                targetPlatform.HasAppID = platform.HasAppID;
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
            return context.ProductCategories.ToList();
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
                targetProductCategory.ProductCategoryIcon = productCategory.ProductCategoryIcon;
                targetProductCategory.ProductCategoryMimeType = productCategory.ProductCategoryMimeType;
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


        // ---- AppMovie persistance

        public IEnumerable<AppMovie> GetAppMovies()
        {
            return context.AppMovies.ToList();
        }
        public AppMovie GetAppMovieByID(int appMovieId)
        {
            return context.AppMovies.Find(appMovieId);
        }
        public void InsertAppMovie(AppMovie appMovie)
        {
            context.AppMovies.Add(appMovie);
        }
        public void UpdateAppMovie(AppMovie appMovie)
        {
            AppMovie targetAppMovie = context.AppMovies.Find(appMovie.AppMovieID);

            if (targetAppMovie != null)
            {
                targetAppMovie.Highlight = appMovie.Highlight;
                targetAppMovie.LargeMovieURL = appMovie.LargeMovieURL;
                targetAppMovie.Name = appMovie.Name;
                targetAppMovie.ProductDetailID = appMovie.ProductDetailID;
                targetAppMovie.SmallMovieURL = appMovie.SmallMovieURL;
                targetAppMovie.ThumbnailURL = appMovie.ThumbnailURL;
            }
        }
        public void DeleteAppMovie(int appMovieId)
        {
            AppMovie targetAppMovie = context.AppMovies.Find(appMovieId);

            if (targetAppMovie != null)
            {
                context.AppMovies.Remove(targetAppMovie);
            }
        }


        // ---- AppScreenshot persistance

        public IEnumerable<AppScreenshot> GetAppScreenshots()
        {
            return context.AppScreenshots.ToList();
        }
        public AppScreenshot GetAppScreenshotByID(int appScreenshotId)
        {
            return context.AppScreenshots.Find(appScreenshotId);
        }
        public void InsertAppScreenshot(AppScreenshot appScreenshot)
        {
            context.AppScreenshots.Add(appScreenshot);
        }
        public void UpdateAppScreenshot(AppScreenshot appScreenshot)
        {
            AppScreenshot targetAppScreenshot = context.AppScreenshots.Find(appScreenshot.AppScreenshotID);

            if (targetAppScreenshot != null)
            {
                targetAppScreenshot.FullSizeURL = appScreenshot.FullSizeURL;
                targetAppScreenshot.ProductDetailID = appScreenshot.ProductDetailID;
                targetAppScreenshot.Screenshot = appScreenshot.Screenshot;
                targetAppScreenshot.ScreenshotMimeType = appScreenshot.ScreenshotMimeType;
                targetAppScreenshot.ThumbnailURL = appScreenshot.ThumbnailURL;
            }
        }
        public void DeleteAppScreenshot(int appScreenshotId)
        {
            AppScreenshot targetAppScreenshot = context.AppScreenshots.Find(appScreenshotId);

            if (targetAppScreenshot != null)
            {
                context.AppScreenshots.Remove(targetAppScreenshot);
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
