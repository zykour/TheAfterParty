using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Concrete;
using System.Collections.Generic;
using System.Linq;
using SteamKit2;
using System;

namespace TheAfterParty.Domain.Services
{
    class StoreService
    {
        private const int __Steam = 1;
        private IRepository _repository;

        public StoreService(IRepository repository)
        {
            _repository = repository;
        }

        public void AddKeyWithExistingListing(ProductKey productKey, int appId, int platform)
        {
            Listing listing = _repository.Listings.Where(l => (l.Product.AppID == appId) && (l.Product.Platform == platform)).SingleOrDefault();

            listing.AddProductKey(productKey);
            _repository.SaveProductKey(productKey);
        }

        public void AddKey(ProductKey productKey, int appId, int platform, int price)
        {
            Listing listing = _repository.Listings.Where(l => (l.Product.AppID == appId) && (l.Product.Platform == platform)).SingleOrDefault();

            if (listing != null)
            {
                listing.ListingPrice = price;
                listing.AddProductKey(productKey);
                _repository.SaveProductKey(productKey);
                _repository.SaveListing(listing);
            }
            else
            {
                AddKeyWithNewListing(productKey, new Product(appId, platform), new Listing(price));
            }
        }

        public void AddKeyWithNewListing(ProductKey productKey, Product product, Listing listing, string name = "")
        {
            ProductDetail productDetail = new ProductDetail();

            if (product.Platform == __Steam)
            {
                productDetail = GetSteamProductDetails(product.AppID);
                name = productDetail.ProductName;
            }

            listing.AddProductKey(productKey);

            product.ProductName = name;

            product.AddProductDetail(productDetail);
            listing.AddProduct(product);

            _repository.SaveListing(listing);
            _repository.SaveProductKey(productKey);
            _repository.SaveProduct(product);
            _repository.SaveProductDetail(productDetail);
        }

        public ProductDetail GetSteamProductDetails(int appId)
        {

            return new ProductDetail();
        }

        public void CloneProductDetails(Listing blueprintListing, Listing recipientListing)
        {
            _repository.DeleteProductDetail(recipientListing.Product.ProductDetail.ProductID);
            recipientListing.Product.ProductDetail = blueprintListing.Product.ProductDetail;
            recipientListing.Product.ProductDetail.Products.Add(recipientListing.Product);
            _repository.SaveListing(recipientListing);
            _repository.SaveProductDetail(recipientListing.Product.ProductDetail);
        }
    }
}
