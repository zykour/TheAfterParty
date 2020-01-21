using System;
using TheAfterParty.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TheAfterParty.Domain.Services
{
    public interface ICartService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        Task<AppUser> GetCurrentUserWithOrders();
        AppUser GetCurrentUserSynch();
        Listing GetDailyDeal();

        Task<IEnumerable<ShoppingCartEntry>> GetShoppingCartEntries();
        IQueryable<ShoppingCartEntry> GetShoppingCartEntries(String userId);

        Task AddItemToCart(int listingId);

        Task<bool> ValidateOrder();
        Task<List<string>> GetOrderValidationErrors();
        Task<Order> CreateOrder();

        void DeleteShoppingCartEntry(int entryId);
        void UpdateShoppingCartEntry(int entryId, int quantity);
        Task DeleteShoppingCart();
        void DeleteShoppingCart(AppUser user);
        Task IncrementCartQuantity(int entryId);
        Task DecrementCartQuantity(int entryId);
        Task<bool> ListingQuantityExceedsCartQuantity(int entryId, int listingId);

        Listing GetListingByID(int listingId);

        int GetNewestOrderId(string id);
        Order GetOrderByID(int id);
    }
}
