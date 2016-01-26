using System;
using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty.Domain.Abstract
{
    public interface IListingRepository : IDisposable
    {
        // other product detail tables?

        AppIdentityDbContext GetContext();

        IEnumerable<Listing> GetListings();
        Listing GetListingByID(int listingId);
        void InsertListing(Listing listing);
        void UpdateListing(Listing listing);
        void DeleteListing(int listingId);

        IEnumerable<DiscountedListing> GetDiscountedListings();
        DiscountedListing GetDiscountedListingByID(int discountedListingId);
        void InsertDiscountedListing(DiscountedListing discountedListing);
        void UpdateDiscountedListing(DiscountedListing discoutnedListing);
        void DeleteDiscountedListing(int discountedListingId);

        IEnumerable<MappedListing> GetMappedListings();
        MappedListing GetMappedListingByID(int mappedListingId);
        void InsertMappedListing(MappedListing mappedListing);
        //void UpdateMappedListing(MappedListing mappedListing);
        void DeleteMappedListing(int mappedListingId);

        IEnumerable<ListingComment> GetListingComments();
        ListingComment GetListingCommentByID(int listingCommentId);
        void InsertListingComment(ListingComment listingComment);
        void UpdateListingComment(ListingComment listingComment);
        void DeleteListingComment(int listingCommentId);

        IEnumerable<Product> GetProducts();
        Product GetProductByID(int productId);
        void InsertProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int productId);

        IEnumerable<ProductDetail> GetProductDetails();
        ProductDetail GetProductDetailByID(int productDetailId);
        void InsertProductDetail(ProductDetail productDetail);
        void UpdateProductDetail(ProductDetail productDetail);
        void DeleteProductDetail(int productDetailId);

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
        void DeleteTag(int tagId)

        void Save();
    }
}
