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

        Task<List<Order>> GetUserOrders();
        Task<List<ClaimedProductKey>> GetKeys();

        Task<List<ActivityFeedContainer>> GetActivityFeedItems();

        List<Auction> GetAuctions();
        List<Giveaway> GetGiveaways();
        Task<int> GetTotalReservedBalance();
        Task<int> GetSilentAuctionReservedBalance();
        Task<int> GetPublicAuctionReservedBalance();
        Task<int> GetCartTotal();

        Task CreateAppUser(AppUser appUser, string roleToAdd, string apiKey);
        Task EditAppUser(AppUser appUser, string roleToAdd, string roleToRemove);

        ICollection<BalanceEntry> GetBalanceEntries();
        void CreateBalanceEntry(BalanceEntry entry, int objectiveId, string nickname);
        void EditBalanceEntry(BalanceEntry entry);
        BalanceEntry GetBalanceEntryByID(int id);
        void DeleteBalanceEntry(int id);

        void CreateClaimedProductKey(ClaimedProductKey key, string nickname);
        void EditClaimedProductKey(ClaimedProductKey key);
        ClaimedProductKey GetClaimedProductKeyByID(int id);
        void DeleteClaimedProductKey(int id);
        ICollection<ClaimedProductKey> GetClaimedProductKeys();

        void CreateOrder(Order order, string nickname, ProductOrderEntry entry, bool alreadyCharged);
        void EditOrder(Order order);
        Order GetOrderByID(int id);
        void DeleteOrder(int id);
        ICollection<Order> GetOrders();
        ProductOrderEntry GetProductOrderEntryByID(int id);
        void EditProductOrderEntry(ProductOrderEntry orderEntry);
        void DeleteProductOrderEntry(int id);

        void MarkKeyUsed(int keyId);
        void RevealKey(int keyId);

        void BuildUser(AppUser user, string apiKey);
        bool AddBalances(string input);
    }
}
