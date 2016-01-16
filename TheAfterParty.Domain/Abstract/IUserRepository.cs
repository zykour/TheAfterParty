using System;
using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty.Domain.Abstract
{
    public interface IUserRepository : IDisposable
    {
        /*	
            UserNotifications
	        Mail
	        OwnedGames
	        WishlistEntries
	        ClaimedProductKey
	        Gift
	        BalanceEntries
	        Order
		        ProductOrderEntry
            ShoppingCartEntry
        

        IEnumerable<UserNotification> GetUserNotifications();
        UserNotification GetUserNotificationByID(int userNotificationId);
        void InsertUserNotification(UserNotification userNotification);
        void UpdateUserNotification(UserNotification userNotification);
        void DeleteUserNotification(int userNotificationId);
        */

        AppIdentityDbContext GetContext();

        IEnumerable<ShoppingCartEntry> GetShoppingCartEntries();
        ShoppingCartEntry GetShoppingCartEntryByID(int shoppingCartEntryId);
        void InsertShoppingCartEntry(ShoppingCartEntry shoppingCartEntry);
        void UpdateShoppingCartEntry(ShoppingCartEntry shoppingCartEntry);
        void DeleteShoppingCartEntry(int shoppingCartEntryId);

        void Save();
    }
}
