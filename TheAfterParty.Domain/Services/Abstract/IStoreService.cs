using System;
using TheAfterParty.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Services.Abstract
{
    public interface IStoreService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        AppUser GetCurrentUserSynch();

        IEnumerable<Listing> GetStockedStoreListings();
        IEnumerable<AppUser> GetAppUsers();
        IEnumerable<DiscountedListing> GetDeals();
    }
}
