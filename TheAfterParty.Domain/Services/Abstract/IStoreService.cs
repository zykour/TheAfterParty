using System;
using TheAfterParty.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Services
{
    public interface IStoreService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        AppUser GetCurrentUserSynch();

        IEnumerable<Listing> GetStockedStoreListings();
        IEnumerable<AppUser> GetAppUsers();
        IEnumerable<Listing> GetListingsWithDeals();
        IEnumerable<Listing> GetListings();
        IEnumerable<Tag> GetTags();
        IEnumerable<ProductCategory> GetProductCategories();
        IEnumerable<Platform> GetPlatforms();
        IEnumerable<Product> GetProducts();
        IEnumerable<ProductKey> GetProductKeys();
        
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

        List<String> AddProductKeys(Platform platform, string input);

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

        List<Listing> FilterListingsByUserSteamID(List<Listing> currentListing, string id, string apiKey);
    }
}
