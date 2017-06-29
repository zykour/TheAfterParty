using System;
using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using System.Linq;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.Domain.Abstract
{
    public interface IListingRepository : IDisposable
    {
        // other product detail tables?
        
        AppIdentityDbContext GetContext();

        DateTime GetNewestDate();
/*REMOVE*/IEnumerable<Listing> GetListingsPlain();
        IQueryable<Listing> GetListingsQuery();
        IEnumerable<Listing> GetListings();
        IEnumerable<Listing> SearchListings(string searchText, int resultLimit);
        IEnumerable<Listing> GetListingsWithFilter(ListingFilter filter, out int TotalItems, List<Listing> listings = null);
        Listing GetListingByID(int listingId);
        void InsertListing(Listing listing);
        void UpdateListing(Listing listing);
        void DeleteListing(int listingId);

        IEnumerable<DiscountedListing> GetDiscountedListings();
        DiscountedListing GetDiscountedListingByID(int discountedListingId);
        void InsertDiscountedListing(DiscountedListing discountedListing);
        void UpdateDiscountedListing(DiscountedListing discoutnedListing);
        void DeleteDiscountedListing(int discountedListingId);
        
        IEnumerable<ListingComment> GetListingComments();
        ListingComment GetListingCommentByID(int listingCommentId);
        void InsertListingComment(ListingComment listingComment);
        void UpdateListingComment(ListingComment listingComment);
        void DeleteListingComment(int listingCommentId);

        IEnumerable<Platform> GetPlatforms();
        IEnumerable<Platform> GetActivePlatforms();
        Platform GetPlatformByID(int platformId);
        void InsertPlatform(Platform platform);
        void UpdatePlatform(Platform platform);
        void DeletePlatform(int platformId);

        IEnumerable<Product> GetProducts();
        Product GetProductByID(int productId);
        void InsertProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int productId);

        IEnumerable<ProductCategory> GetProductCategories();
        ProductCategory GetProductCategoryByID(int productCategoryId);
        void InsertProductCategory(ProductCategory productCategory);
        void UpdateProductCategory(ProductCategory productCategory);
        void DeleteProductCategory(int productCategoryId);
        
        IEnumerable<ProductKey> GetProductKeys();
        ProductKey GetProductKeyByID(int productKeyId);
        void InsertProductKey(ProductKey productKey);
        void UpdateProductKey(ProductKey productKey);
        void DeleteProductKey(int productKeyId);

        IEnumerable<ProductReview> GetProductReviews();
        ProductReview GetProductReviewByID(int productReviewId);
        void InsertProductReview(ProductReview productReview);
        void UpdateProductReview(ProductReview productReview);
        void DeleteProductReview(int productReviewId);

        IEnumerable<Tag> GetTags();
        Tag GetTagByID(int tagId);
        void InsertTag(Tag tag);
        void UpdateTag(Tag tag);
        void DeleteTag(int tagId);

        void Save();
    }
}
