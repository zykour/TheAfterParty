﻿using System;
using TheAfterParty.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.Domain.Services
{
    public interface IStoreService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        Task<AppUser> GetCurrentUserWithStoreFilters();
        AppUser GetCurrentUserWithStoreFiltersSynch();
        AppUser GetCurrentUserSynch();
        /*REMOVE*/IEnumerable<Listing> GetListingsPlain();
        DateTime GetNewestDate();
        IEnumerable<Listing> GetStockedStoreListings();
        IEnumerable<AppUser> GetAppUsers();
        IEnumerable<Listing> GetListingsWithDeals();
        IEnumerable<Listing> GetListings();
        IEnumerable<Listing> GetListingsWithFilter(ListingFilter filter, out int TotalItems, List<Listing> listings = null);
        IEnumerable<Tag> GetTags();
        IEnumerable<ProductCategory> GetProductCategories();
        IEnumerable<Platform> GetPlatforms();
        IEnumerable<Platform> GetActivePlatforms();
        IEnumerable<Product> GetProducts();
        IEnumerable<ProductKey> GetProductKeys();
        IEnumerable<ProductOrderEntry> GetStoreHistory();
        IEnumerable<AppUser> GetUsersWhoOwn(int appId);
        IEnumerable<AppUser> GetUsersWhoDoNotOwn(int appId);

        DiscountedListing GetDiscountedListingByID(int id);
        
        Listing GetListingByAppID(int id, string platformName);
        Listing GetListingByID(int id);

        Platform GetPlatformByID(int id);

        Product GetProductByID(int id);

        ProductCategory GetProductCategoryByName(string name);
        ProductCategory GetProductCategoryByID(int id);

        ProductKey GetProductKeyByID(int id);

        Tag GetTagByName(string name);
        Tag GetTagByID(int id);

        void AddListing(Listing listing);

        List<String> BuildListingsWithSteamID(string appIDCsv);
        List<String> AddProductKeys(Platform platform, string input);
        List<String> BuildOrUpdateProductsWithSteamID(string appIDCsv);
        List<String> AddOrUpdateNonSteamProducts(string input);
        bool BuildOrUpdateSteamProduct(int appId, Product product);

        void AddProductCategory(ProductCategory category);

        void AddDiscountedListing(DiscountedListing discountedListing, int daysDealLast);
        void EditDiscountedListing(DiscountedListing discountedListing, int daysToAdd);
        void DeleteDiscountedListing(int id);

        void EditListing(Listing listing);
        void DeleteListing(int listingId);

        void AddPlatform(Platform platform);
        void EditPlatform(Platform platform);
        void DeletePlatform(int platformId);
        void DownloadIconURL(Platform platform, string localPath, string fileExtension);

        void EditProduct(Product product);
        void DeleteProduct(int id);

        void EditProductCategory(ProductCategory category);
        void DeleteProductCategory(int productCategoryId);

        void EditProductKey(ProductKey productKey);
        void DeleteProductKey(int productKeyId);

        IEnumerable<int> GetAppIDsByID(string id, string apiKey);
        List<Listing> FilterListingsByUserSteamID(List<Listing> currentListing, string id, string apiKey);
    }
}
