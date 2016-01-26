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
            
            if (listing.Product.ProductID == 0)
            {
                InsertProduct(listing.Product);
            }
            else
            {
                UpdateProduct(listing.Product);
            }

            if (listing.Platform.PlatformID == 0)
            {
                InsertPlatform(listing.Platform);
            }
            else
            {
                UpdatePlatform(listing.Platform);
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

            foreach (MappedListing entry in listing.ChildListings)
            {
                if (entry.MappedListingID == 0)
                {
                    InsertMappedListing(entry);
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


        // ---- MappedListing entity persistance

        public IEnumerable<MappedListing> GetMappedListings()
        {
            return context.MappedListings.ToList();
        }
        public MappedListing GetMappedListingByID(int id)
        {
            return context.MappedListings.Find(id);
        }
        public void InsertMappedListing(MappedListing mappedListing)
        {
            context.MappedListings.Add(mappedListing);
        }
        public void DeleteMappedListing(int mappedListingId)
        {
            MappedListing mappedListing = context.MappedListings.Find(mappedListingId);
            context.MappedListings.Remove(mappedListing);
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
        }
        // check this once Product Detail is finalized
        public void UpdateProductDetail(ProductDetail productDetail)
        {
            ProductDetail targetProductDetail = context.ProductDetails.Find(productDetail.ProductID);

            if (targetProductDetail != null)
            {
                targetProductDetail.ImageData = productDetail.ImageData;
                targetProductDetail.ImageMimeType = productDetail.ImageMimeType;
                targetProductDetail.ProductName = productDetail.ProductName;
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
            ProductKey targetProductKey = context.ProductKeys.Find(productKey.KeyID, productKey.ItemKey);

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
            Tag targetTag = context.Tags.Find(tag.Id);

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
