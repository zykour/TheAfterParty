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
        IEnumerable<Tag> GetTags();
        IEnumerable<ProductCategory> GetProductCategories();
        IEnumerable<Platform> GetPlatforms();

        void EditPlatform(Platform platform);
        Platform GetPlatformByID(int id);

        List<String> AddProductKeys(Platform platform, string input);

        void AddListing(Listing listing);
        Listing GetListingByAppID(int id, string platformName);

        ProductCategory GetProductCategoryByName(string name);
        void AddProductCategory(ProductCategory category);
        ProductCategory GetProductCategoryByID(int id);

        Tag GetTagByName(string name);
        Tag GetTagByID(int id);
    }
}
