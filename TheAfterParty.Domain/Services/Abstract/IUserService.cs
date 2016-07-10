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
        List<AppUser> GetAdmins();
        Task<AppUser> GetUserByID(string id);
        AppUser GetUserByNickname(string nickname);

        Task<List<Order>> GetUserOrders();
        Task<List<ClaimedProductKey>> GetKeys();

        List<ActivityFeedContainer> GetPublicActivityFeedItems(AppUser user);
        Task<List<ActivityFeedContainer>> GetActivityFeedItems(bool includeNegativeBalanceEntries = false);

        List<Auction> GetAuctions();
        List<Giveaway> GetGiveaways();
        Task<int> GetTotalReservedBalance();
        Task<int> GetSilentAuctionReservedBalance();
        Task<int> GetPublicAuctionReservedBalance();
        Task<int> GetCartTotal();

        Task CreateAppUser(AppUser appUser, string roleToAdd, string apiKey);
        Task EditAppUser(AppUser appUser, string roleToAdd, string roleToRemove);

        ICollection<BalanceEntry> GetBalanceEntries();
        Task CreateBalanceEntry(BalanceEntry entry, int objectiveId, string nickname);
        Task CreateBalanceEntry(BalanceEntry entry, int objectiveId);
        Task EditBalanceEntry(BalanceEntry entry, int objectiveId);
        BalanceEntry GetBalanceEntryByID(int id);
        Task DeleteBalanceEntry(int id);

        void CreateClaimedProductKey(ClaimedProductKey key, string nickname);
        void CreateClaimedProductKey(ClaimedProductKey key);
        void EditClaimedProductKey(ClaimedProductKey key);
        ClaimedProductKey GetClaimedProductKeyByID(int id);
        void DeleteClaimedProductKey(int id);
        ICollection<ClaimedProductKey> GetClaimedProductKeys();

        Task CreateOrder(Order order, bool alreadyCharged, bool useDBKey = false);
        void EditOrder(Order order);
        Order GetOrderByID(int id);
        Task DeleteOrder(int id);
        ICollection<Order> GetOrders();
        ProductOrderEntry GetProductOrderEntryByID(int id);
        void EditProductOrderEntry(ProductOrderEntry orderEntry);
        Task DeleteProductOrderEntry(int id);

        ProductKey GetProductKey(int listingId);

        bool MarkKeyUsed(int keyId);
        string RevealKey(int keyId);

        void BuildUser(AppUser user, string apiKey);
        bool IsInRole(AppUser user, string role);
        bool AddBalances(string input);
    }
}
