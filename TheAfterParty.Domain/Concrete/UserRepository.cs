using System;
using System.Collections.Generic;
using System.Linq;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Concrete
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private AppIdentityDbContext context;
        public AppIdentityDbContext GetContext()
        {
            return context;
        }

        public UserRepository(AppIdentityDbContext context)
        {
            this.context = context;
        }



        // ---- Shopping Cart persistance

        public IEnumerable<ShoppingCartEntry> GetShoppingCartEntries()
        {
            return context.ShoppingCartEntries.ToList();
        }
        public ShoppingCartEntry GetShoppingCartEntryByID(int id)
        {
            return context.ShoppingCartEntries.Find(id);
        }
        public void InsertShoppingCartEntry(ShoppingCartEntry shoppingCartEntry)
        {
            context.ShoppingCartEntries.Add(shoppingCartEntry);
        }
        public void DeleteShoppingCartEntry(int shoppingCartEntryID)
        {
            ShoppingCartEntry cartEntry = context.ShoppingCartEntries.Find(shoppingCartEntryID);
            context.ShoppingCartEntries.Remove(cartEntry);
        }
        public void UpdateShoppingCartEntry(ShoppingCartEntry shoppingCartEntry)
        {
            ShoppingCartEntry targetShoppingCartEntry = context.ShoppingCartEntries.Find(shoppingCartEntry.ShoppingID);

            if (targetShoppingCartEntry != null)
            {
                targetShoppingCartEntry.Quantity = shoppingCartEntry.Quantity;
                targetShoppingCartEntry.DateAdded = shoppingCartEntry.DateAdded;
                targetShoppingCartEntry.ListingID = shoppingCartEntry.ListingID;
                targetShoppingCartEntry.UserID = shoppingCartEntry.UserID;
            }
        }



        // ---- Repository methods

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
