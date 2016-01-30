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

        IEnumerable<Platform> GetPlatforms();
        void EditPlatform(Platform platform);
        IList<String> AddProductKeys(Platform platform, string input);
        Platform GetPlatformByID(int id);
    }
}
