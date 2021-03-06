﻿using System;
using TheAfterParty.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.Domain.Services
{
    public interface IUserService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        AppUser GetCurrentUserSynch();
        AppUserManager GetUserManager();

        String GetGameName(int appId);

        Task<bool> IsBlacklisted(int listingId);
        Task ToggleBlacklist(int listingId);
        Task TransferPoints(int points, string userId);

        AppUser GetRequestedUser(string profileName, bool nickname = false);
        IEnumerable<AppUser> GetAllUsers();
        IEnumerable<AppUser> GetAdmins();
        IEnumerable<AppUser> GetUsersWhoOwn(int appId);
        IEnumerable<AppUser> GetUsersWhoDoNotOwn(int appId);
        Task<AppUser> GetUserByID(string id);
        AppUser GetUserByNickname(string nickname);
        Task UpdateUser(string id, string apiKey);

        Task<int> AddWishlistItems(string appIds);

        Task<IEnumerable<Order>> GetUserOrders();
        Task<IEnumerable<ClaimedProductKey>> GetKeys();

        Task<List<ActivityFeedContainer>> GetPublicActivityFeedItems(AppUser user);
        Task<List<ActivityFeedContainer>> GetActivityFeedItems(bool includeNegativeBalanceEntries = false);

        IEnumerable<Auction> GetAuctions();
        IEnumerable<Giveaway> GetGiveaways();
        Giveaway GetGiveawayByID(int giveawayId);
        void AddGiveaway(Giveaway giveaway);
        void DrawGiveawayWinners(int giveawayId);
        Task<int> GetTotalReservedBalance();
        Task<int> GetSilentAuctionReservedBalance();
        Task<int> GetPublicAuctionReservedBalance();
        Task<int> GetCartTotal();

        Task CreateAppUser(AppUser appUser, string password, string roleToAdd, string apiKey);
        Task EditAppUser(AppUser appUser, string roleToAdd, string roleToRemove);
        Task EditAppUserSettings(AppUser appUser);

        IEnumerable<BalanceEntry> GetBalanceEntries();
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
        IEnumerable<ClaimedProductKey> GetClaimedProductKeys();
        Task RestockProductOrderEntry(int id);
        void PullNewProductKey(int id);

        Task CreateOrder(Order order, bool alreadyCharged, bool useDBKey = false);
        void EditOrder(Order order);
        Order GetOrderByID(int id);
        Task DeleteOrder(int id);
        IEnumerable<Order> GetOrders();
        ProductOrderEntry GetProductOrderEntryByID(int id);
        void EditProductOrderEntry(ProductOrderEntry orderEntry);
        Task DeleteProductOrderEntry(int id);

        ProductKey GetProductKey(int listingId);

        bool MarkKeyUsed(int keyId);
        string RevealKey(int keyId);

        Task BuildUser(AppUser user, string apiKey);
        bool IsInRole(AppUser user, string role);
        bool AddBalances(string input);
    }
}
