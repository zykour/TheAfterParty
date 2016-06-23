using System;
using TheAfterParty.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheAfterParty.Domain.Services
{
    public interface ICartService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        AppUser GetCurrentUserSynch();

        Task<IEnumerable<ShoppingCartEntry>> GetShoppingCartEntries();
        
        Task AddItemToCart(int listingId);

        Task<bool> ValidateOrder();
        Task<List<string>> GetOrderValidationErrors();
        Task<Order> CreateOrder();

        void DeleteShoppingCartEntry(int entryId);
        Task UpdateShoppingCartEntry(int entryId, int quantity);
        Task DeleteShoppingCart();
        Task IncrementCartQuantity(int entryId);
        Task DecrementCartQuantity(int entryId);
        Task<bool> ListingQuantityExceedsCartQuantity(int entryId, int listingId);

        Listing GetListingByID(int listingId);

        Order GetOrderByID(int id);
    }
}
