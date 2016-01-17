using System;
using TheAfterParty.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheAfterParty.Domain.Services
{
    public interface ICartService : IDisposable
    {
        void SetUserName(string userName);

        Task<IEnumerable<ShoppingCartEntry>> GetShoppingCartEntries();
        Task<AppUser> GetCurrentUser();
        
        Task AddItemToCart(int listingId);

        Task<bool> ValidateOrder();
        Task<List<string>> GetOrderValidationErrors();
        Task<Order> CreateOrder();
    }
}
