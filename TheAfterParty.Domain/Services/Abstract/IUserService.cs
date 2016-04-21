using System;
using TheAfterParty.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Services
{
    public interface IUserService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        AppUser GetCurrentUserSynch();

        Task AddBlacklistEntry(int listingId);
        Task TransferPoints(int points, string userId);

        AppUser GetRequestedUser(string profileName, bool nickname = false);
        ICollection<AppUser> GetAllUsers();
        Task<AppUser> GetUserByID(string id);

        Task<List<Order>> GetOrders();
        Task<List<ClaimedProductKey>> GetKeys();

        Task<List<ActivityFeedContainer>> GetActivityFeedItems();

        List<Auction> GetAuctions();
        List<Giveaway> GetGiveaways();

        void MarkKeyUsed(int keyId);
        void RevealKey(int keyId);

        void BuildUser(AppUser user, string apiKey);
        bool AddBalances(string input);
    }
}
