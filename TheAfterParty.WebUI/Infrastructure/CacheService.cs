using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Concrete;
using Microsoft.AspNet.Identity.EntityFramework;
using TheAfterParty.WebUI.Models.Store;

namespace TheAfterParty.WebUI.Infrastructure
{
    class CacheService : ICacheService
    {
        private const string tagsCacheKey = "Tags";
        private const string productCategoryCacheKey = "Categories";
        private const string stockedListingsCacheKey = "Listings";
        private const string newestDateCacheKey = "Newest";
        private const string loggedUserCacheKey = "User";
        private const string platformsCacheKey = "Platforms";

        private IMemoryCache _cache;
        static int cacheDuration = 240;
        static int slidingCacheDuration = 60;
        private static Object locker = new Object();

        private IListingRepository listingRepository;
        private IUserRepository userRepository;
        private ISiteRepository siteRepository;
        private IUnitOfWork unitOfWork;
        public AppUserManager UserManager { get; private set; }
        public string userName { get; set; }

        public CacheService(IListingRepository listingRepository, IUserRepository userRepository, ISiteRepository siteRepository, IUnitOfWork unitOfWork, IMemoryCache _cache) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this._cache = _cache;
            this.listingRepository = listingRepository;
            this.userRepository = userRepository;
            this.siteRepository = siteRepository;
            this.unitOfWork = unitOfWork;
            userName = "";
        }
        protected CacheService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }

        //userName = HttpContext.User.Identity.Name
        // Use when you want to attempt to get a cahced value, before resorting to filling the cache
        public async Task<AppUser> TryGetCachedUser(String userName)
        {
            AppUser user = null;

            if (!_cache.TryGetValue(loggedUserCacheKey + userName, out user))
            {
                user = await GetCacheUser(userName);
            }

            return user;
        }

        // Use when you want to force a re-cache
        public async Task CacheUser(String userName)
        {
            AppUser user = await GetCurrentUserWithStoreFilters();

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
            _cache.Set(loggedUserCacheKey + userName, user, cacheEntryOptions);
        }

        // Use when you want to force a re-cache
        public async Task<AppUser> GetCacheUser(String userName)
        {
            AppUser user = await GetCurrentUserWithStoreFilters();

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
            _cache.Set(loggedUserCacheKey + userName, user, cacheEntryOptions);

            return user;
        }

        public AppUser TryGetCachedUserSynch(String userName)
        {
            AppUser user = null;

            if (!_cache.TryGetValue(loggedUserCacheKey + userName, out user))
            {
                user = GetCacheUserSynch(userName);
            }

            return user;
        }

        public void CacheUserSynch(String userName)
        {
            AppUser user = GetCurrentUserWithStoreFiltersSynch();

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
            _cache.Set(loggedUserCacheKey + userName, user, cacheEntryOptions);
        }

        public AppUser GetCacheUserSynch(String userName)
        {
            AppUser user = GetCurrentUserWithStoreFiltersSynch();

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
            _cache.Set(loggedUserCacheKey + userName, user, cacheEntryOptions);

            return user;
        }

        public Tag TryGetCachedTags(int id)
        {
            List<SelectedTagMapping> tagMappings = new List<SelectedTagMapping>();

            if (!_cache.TryGetValue(tagsCacheKey, out tagMappings))
            {
                tagMappings = new List<SelectedTagMapping>();

                foreach (Tag tag in GetTags())
                {
                    tagMappings.Add(new SelectedTagMapping(tag, false));
                }

                tagMappings = tagMappings.OrderBy(t => t.StoreTag.TagName).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
                _cache.Set(tagsCacheKey, tagMappings, cacheEntryOptions);
            }

            return tagMappings.SingleOrDefault(x => x.StoreTag.TagID == id).StoreTag;
        }

        public List<SelectedTagMapping> TryGetCachedTags()
        {
            List<SelectedTagMapping> tagMappings = new List<SelectedTagMapping>();

            if (!_cache.TryGetValue(tagsCacheKey, out tagMappings))
            {
                tagMappings = new List<SelectedTagMapping>();

                foreach (Tag tag in GetTags())
                {
                    tagMappings.Add(new SelectedTagMapping(tag, false));
                }

                tagMappings = tagMappings.OrderBy(t => t.StoreTag.TagName).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
                _cache.Set(tagsCacheKey, tagMappings, cacheEntryOptions);
            }

            return tagMappings;
        }

        public ProductCategory TryGetProductCategory(int id)
        {
            List<SelectedProductCategoryMapping> categoryMappings = new List<SelectedProductCategoryMapping>();

            if (!_cache.TryGetValue(productCategoryCacheKey, out categoryMappings))
            {
                categoryMappings = new List<SelectedProductCategoryMapping>();

                foreach (ProductCategory category in GetProductCategories())
                {
                    categoryMappings.Add(new SelectedProductCategoryMapping(category, false));
                }

                categoryMappings = categoryMappings.OrderBy(c => c.ProductCategory.CategoryString).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
                _cache.Set(productCategoryCacheKey, categoryMappings, cacheEntryOptions);
            }

            return categoryMappings.SingleOrDefault(x => x.ProductCategory.ProductCategoryID == id).ProductCategory;
        }

        public List<SelectedProductCategoryMapping> TryGetCachedProductCategories()
        {
            List<SelectedProductCategoryMapping> categoryMappings = new List<SelectedProductCategoryMapping>();

            if (!_cache.TryGetValue(productCategoryCacheKey, out categoryMappings))
            {
                categoryMappings = new List<SelectedProductCategoryMapping>();

                foreach (ProductCategory category in GetProductCategories())
                {
                    categoryMappings.Add(new SelectedProductCategoryMapping(category, false));
                }

                categoryMappings = categoryMappings.OrderBy(c => c.ProductCategory.CategoryString).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
                _cache.Set(productCategoryCacheKey, categoryMappings, cacheEntryOptions);
            }

            return categoryMappings;
        }

        public List<Platform> TryGetCachedPlatforms()
        {
            List<Platform> platforms = null;

            if (!_cache.TryGetValue(platformsCacheKey, out platforms))
            {
                platforms = GetActivePlatforms().ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
                _cache.Set(platformsCacheKey, platforms, cacheEntryOptions);
            }

            return platforms;
        }

        public DateTime TryGetNewestDate()
        {
            DateTime newestDate;

            if (!_cache.TryGetValue<DateTime>(newestDateCacheKey, out newestDate))
            {
                newestDate = GetNewestDate();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
                _cache.Set(newestDateCacheKey, newestDate, cacheEntryOptions);
            }

            return newestDate;
        }

        //change this to just cache and rework the other method to accept the list of items to be queried?
        public List<Listing> TryGetCachedListings(TheAfterParty.Domain.Concrete.ListingFilter filter, out int count)
        {
            /*List<Listing> listings = null;

            if (!_cache.TryGetValue(stockedListingsCacheKey, out listings))
            {
                Task.Run(() => FillListingCache(_cache));
            }
            else
            {
                filter.NewestDate = listings.Where(l => l.Quantity > 0).OrderByDescending(x => x.DateEdited).FirstOrDefault().DateEdited.Date;
            }
            
             
            return GetListingsWithFilter(filter, out count, listings).ToList();
             */

            return GetListingsWithFilter(filter, out count).ToList();
        }

        public void ForceCacheListings()
        {
            Task.Run(() => ForceFillListingCache(_cache));
        }

        private static void ForceFillListingCache(IMemoryCache _cache)
        {
            List<Listing> listings = null;

            if (Monitor.TryEnter(locker))
            {
                using (TheAfterParty.Domain.Concrete.AppIdentityDbContext context = TheAfterParty.Domain.Concrete.AppIdentityDbContext.Create())
                {
                    TheAfterParty.Domain.Concrete.ListingRepository _repo = new TheAfterParty.Domain.Concrete.ListingRepository(new Domain.Concrete.UnitOfWork(context));

                    listings = _repo.GetListingsQuery().Where(l => l.Quantity > 0 && l.ListingPrice > 0).ToList();
                }
                //listings = storeService.GetStockedStoreListings().ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
                _cache.Set(stockedListingsCacheKey, listings, cacheEntryOptions);

                Monitor.Exit(locker);
            }
        }

        public void CacheListings()
        {
            Task.Run(() => FillListingCache(_cache));
        }

        private static void FillListingCache(IMemoryCache _cache)
        {   
            List<Listing> listings = null;

            if (!_cache.TryGetValue(stockedListingsCacheKey, out listings))
            {
                if (Monitor.TryEnter(locker))
                {
                    using (TheAfterParty.Domain.Concrete.AppIdentityDbContext context = TheAfterParty.Domain.Concrete.AppIdentityDbContext.Create())
                    {
                        TheAfterParty.Domain.Concrete.ListingRepository _repo = new TheAfterParty.Domain.Concrete.ListingRepository(new Domain.Concrete.UnitOfWork(context));

                        listings = _repo.GetListingsQuery().Where(l => l.Quantity > 0 && l.ListingPrice > 0).ToList();
                    }
                    //listings = storeService.GetStockedStoreListings().ToList();

                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(slidingCacheDuration)).SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheDuration));
                    _cache.Set(stockedListingsCacheKey, listings, cacheEntryOptions);

                    Monitor.Exit(locker);
                }
            }
        }

        private DateTime GetNewestDate()
        {
            return listingRepository.GetNewestDate();
        }

        private IEnumerable<Listing> GetListingsWithFilter(ListingFilter filter, out int TotalItems, List<Listing> listings = null)
        {
            return listingRepository.GetListingsWithFilter(filter, out TotalItems, listings);
        }

        private IEnumerable<Tag> GetTags()
        {
            return listingRepository.GetTags();
        }

        private IEnumerable<ProductCategory> GetProductCategories()
        {
            return listingRepository.GetProductCategories();
        }

        private IEnumerable<Platform> GetActivePlatforms()
        {
            return listingRepository.GetActivePlatforms();
        }

        private async Task<AppUser> GetCurrentUserWithStoreFilters()
        {
            return await UserManager.FindByNameAsyncWithStoreFilters(userName);
        }

        private AppUser GetCurrentUserWithStoreFiltersSynch()
        {
            return UserManager.FindByNameAsyncWithStoreFiltersSynch(userName);
        }
    }
}
