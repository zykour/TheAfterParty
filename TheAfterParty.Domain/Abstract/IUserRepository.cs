﻿using System;
using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using System.Threading.Tasks;
using System.Linq;

namespace TheAfterParty.Domain.Abstract
{
    public interface IUserRepository : IDisposable
    {
        AppIdentityDbContext GetContext();

        IEnumerable<AppUser> GetAppUsers();
        Task<AppUser> GetAppUserByID(string appUserId);
        Task InsertAppUser(AppUser appUser);
        Task UpdateAppUserSimple(AppUser appUser);
        Task UpdateAppUser(AppUser appUser);
        void UpdateAppUserSynch(AppUser appUser);
        Task DeleteAppUser(string appUserId);

        IEnumerable<ShoppingCartEntry> GetShoppingCartEntries();
        IQueryable<ShoppingCartEntry> GetShoppingCartEntriesQuery();
        ShoppingCartEntry GetShoppingCartEntryByID(int shoppingCartEntryId);
        void InsertShoppingCartEntry(ShoppingCartEntry shoppingCartEntry);
        void UpdateShoppingCartEntry(ShoppingCartEntry shoppingCartEntry);
        void DeleteShoppingCartEntry(int shoppingCartEntryId);

        IQueryable<Order> GetOrdersQuery();
        IEnumerable<Order> GetOrders();
        Order GetOrderByID(int orderId);
        void InsertOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int orderId);

        IEnumerable<ProductOrderEntry> GetProductOrderEntries();
        ProductOrderEntry GetProductOrderEntryByID(int orderProductEntryId);
        void InsertProductOrderEntry(ProductOrderEntry productOrderEntry);
        void UpdateProductOrderEntry(ProductOrderEntry productOrderEntry);
        void DeleteProductOrderEntry(int productOrderEntryId);

        IEnumerable<ClaimedProductKey> GetClaimedProductKeys();
        ClaimedProductKey GetClaimedProductKeyByID(int claimedProductKeyId);
        void InsertClaimedProductKey(ClaimedProductKey claimedProductKey);
        void UpdateClaimedProductKey(ClaimedProductKey claimedProductKey);
        void DeleteClaimedProductKey(int claimedProductKeyId);

        IEnumerable<BalanceEntry> GetBalanceEntries();
        BalanceEntry GetBalanceEntryByID(int balanceEntryId);
        void InsertBalanceEntry(BalanceEntry balanceEntry);
        void UpdateBalanceEntry(BalanceEntry balanceEntry);
        void DeleteBalanceEntry(int balanceEntryId);

        IEnumerable<WishlistEntry> GetWishlistEntries();
        WishlistEntry GetWishlistEntryByID(int wishlistEntryId);
        void InsertWishlistEntry(WishlistEntry wishlistEntry);
        void DeleteWishlistEntry(int wishlistEntryId);

        IEnumerable<Gift> GetGifts();
        Gift GetGiftByID(int giftId);
        void InsertGift(Gift gift);
        void UpdateGift(Gift gift);
        void DeleteGift(int giftId);

        IEnumerable<OwnedGame> GetOwnedGames();
        IEnumerable<String> GetUsersWhoOwn(int appId);
        IEnumerable<AppUser> GetAppUsersWhoOwn(int appId);
        IEnumerable<AppUser> GetAppUsersWhoDoNotOwn(int appId);
        OwnedGame GetOwnedGameByID(int ownedGameId);
        void InsertOwnedGame(OwnedGame ownedGame);
        void UpdateOwnedGame(OwnedGame ownedGame);
        void DeleteOwnedGame(int ownedGameId);

        IEnumerable<UserNotification> GetUserNotifications();
        UserNotification GetUserNotificationByID(int userNotificationId);
        void InsertUserNotification(UserNotification userNotification);
        void UpdateUserNotification(UserNotification userNotification);
        void DeleteUserNotification(int userNotificationId);

        IEnumerable<Mail> GetMail();
        Mail GetMailByID(int mailId);
        void InsertMail(Mail mail);
        void UpdateMail(Mail mail);
        void DeleteMail(int mailId);

        IEnumerable<UserTag> GetUserTags();
        UserTag GetUserTagByID(int userTagId);
        void InsertUserTag(UserTag userTag);
        void DeleteUserTag(int userTagId);

        void Save();
    }
}
