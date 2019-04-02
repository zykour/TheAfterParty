using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;
using System;
using System.Collections.Generic;
using TheAfterParty.WebUI.Models.Store;

namespace TheAfterParty.WebUI.Infrastructure
{ 
    public interface ICacheService
    {
        void SetUserName(string userName);
        
        Task<AppUser> TryGetCachedUser(String userName);
        Task CacheUser(String userName);
        Task<AppUser> GetCacheUser(String userName);
        AppUser TryGetCachedUserSynch(String userName);
        AppUser GetCacheUserSynch(String userName);
        void CacheUserSynch(String userName);
        Tag TryGetCachedTags(int id);
        List<SelectedTagMapping> TryGetCachedTags();
        ProductCategory TryGetProductCategory(int id);
        List<SelectedProductCategoryMapping> TryGetCachedProductCategories();
        List<Platform> TryGetCachedPlatforms();
        DateTime TryGetNewestDate();
        List<Listing> TryGetCachedListings(TheAfterParty.Domain.Concrete.ListingFilter filter, out int count);
        void CacheListings();
        void ForceCacheListings();
    }
}
