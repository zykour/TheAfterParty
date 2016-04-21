using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace TheAfterParty.Domain.Services
{
    public class CartService : ICartService
    {
        private IUnitOfWork unitOfWork;
        private IUserRepository userRepository;
        private IListingRepository listingRepository;
        public AppUserManager UserManager { get; private set; }
        public string userName { get; set; }

        public CartService(IUserRepository userRepository, IListingRepository listingRepository, IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.userRepository = userRepository;
            this.listingRepository = listingRepository;
            this.unitOfWork = unitOfWork;
        }
        protected CartService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }
        
        public async Task AddItemToCart(int listingId)
        {
            Listing listing = listingRepository.GetListingByID(listingId);

            if (listing != null)
            {
                AppUser currentUser = await GetCurrentUser();

                ShoppingCartEntry entry = currentUser.ShoppingCartEntries.Where(e => e.ListingID == listingId && Object.Equals(currentUser.Id, e.UserID)).SingleOrDefault(); //userRepository.GetShoppingCartEntries().Where(e => e.ListingID == listingId && Object.Equals(currentUser.Id, e.UserID)).SingleOrDefault();

                if (entry == null)
                {
                    currentUser.AddShoppingCartEntry(listing);
                    await userRepository.UpdateAppUser(currentUser); //userRepository.InsertShoppingCartEntry(currentUser.AddShoppingCartEntry(listing));
                }
                else
                {
                    entry.Quantity++;
                    await userRepository.UpdateAppUser(currentUser);
                }

                unitOfWork.Save();
            }
        }

        public async Task<IEnumerable<ShoppingCartEntry>> GetShoppingCartEntries()
        {
            AppUser user = await GetCurrentUser();

            return user.ShoppingCartEntries; //userRepository.GetShoppingCartEntries().Where(e => Object.Equals(user.Id, e.UserID));
        }

        public async Task<bool> ValidateOrder()
        {
            AppUser user = await GetCurrentUser();

            return user.AssertValidOrder();
        }

        public async Task<List<string>> GetOrderValidationErrors()
        {
            AppUser user = await GetCurrentUser();
            List<string> errors = new List<string>();

            if (!user.AssertQuantityOfCart())
            {
                errors.Add("Items you have added to your cart exceed the quantity that is available.");
            }

            if (!user.AssertBalanceExceedsCost())
            {
                errors.Add("Your available balance is insufficient to make the current purchase.");
            }

            return errors;
        }

        public async Task<Order> CreateOrder()
        {
            AppUser user = await GetCurrentUser();

            if (!user.AssertValidOrder())
            {
                return null;
            }

            DateTime orderDate = new DateTime();
            orderDate = DateTime.Now;

            Order order = new Order(user, orderDate);
            userRepository.InsertOrder(order);

            ICollection<ShoppingCartEntry> cartEntries = user.ShoppingCartEntries;
            
            foreach (ShoppingCartEntry entry in cartEntries)
            {
                List<ProductKey> keys = listingRepository.GetProductKeys().Where(k => k.ListingID == entry.ListingID).Take(entry.Quantity).ToList();

                foreach (ProductKey productKey in keys)
                {
                    listingRepository.DeleteProductKey(productKey.ProductKeyID);

                    Listing listing = listingRepository.GetListingByID(entry.ListingID);
                    listing.Quantity--;
                    listingRepository.UpdateListing(listing);
                    
                    ClaimedProductKey claimedKey = new ClaimedProductKey(productKey, user, orderDate, "Purchase - Order #" + order.OrderID);
                    user.AddClaimedProductKey(claimedKey);

                    ProductOrderEntry orderEntry = new ProductOrderEntry(order, entry, claimedKey);
                    order.AddProductOrderEntry(orderEntry);
                }
            }

            await DeleteShoppingCart();

            BalanceEntry balanceEntry = new BalanceEntry(user, "Purchase - Order #" + order.OrderID, 0 - order.TotalSalePrice() , orderDate);
            user.BalanceEntries.Add(balanceEntry);
            //userRepository.InsertBalanceEntry(balanceEntry);

            user.Balance -= order.TotalSalePrice();
            user.Orders.Add(order);
            await userRepository.UpdateAppUser(user);

            this.unitOfWork.Save();

            return order;
        }

        public void DeleteShoppingCartEntry(int entryId)
        {
            userRepository.DeleteShoppingCartEntry(entryId);
            this.unitOfWork.Save();
        }

        public async Task DeleteShoppingCart()
        {
            AppUser user = await GetCurrentUser();

            List<Int32> entryIds = user.ShoppingCartEntries.Select(e => e.ShoppingCartEntryID).ToList();
            //IEnumerable<ShoppingCartEntry> entries = user.ShoppingCartEntries; //userRepository.GetShoppingCartEntries().Where(e => Object.Equals(user.Id, e.UserID));

            foreach (Int32 id in entryIds)
            {
                userRepository.DeleteShoppingCartEntry(id);
            }

            this.unitOfWork.Save();
        }

        public async Task UpdateShoppingCartEntry(int entryId, int quantity)
        {
            AppUser user = await GetCurrentUser();
            ShoppingCartEntry entry = user.ShoppingCartEntries.Where(e => e.ShoppingCartEntryID == entryId).Single(); //userRepository.GetShoppingCartEntryByID(entryId);
            entry.Quantity = quantity;
            await userRepository.UpdateAppUser(user);

            this.unitOfWork.Save();
        }

        public async Task IncrementCartQuantity(int entryId)
        {
            AppUser user = await GetCurrentUser();
            ShoppingCartEntry entry = user.ShoppingCartEntries.Where(e => e.ShoppingCartEntryID == entryId).Single(); //userRepository.GetShoppingCartEntryByID(entryId);
            entry.Quantity++;
            await userRepository.UpdateAppUser(user);

            this.unitOfWork.Save();
        }

        public async Task DecrementCartQuantity(int entryId)
        {
            AppUser user = await GetCurrentUser();
            ShoppingCartEntry entry = user.ShoppingCartEntries.Where(e => e.ShoppingCartEntryID == entryId).Single(); //userRepository.GetShoppingCartEntryByID(entryId);
            entry.Quantity--;

            if (entry.Quantity > 0)
                await userRepository.UpdateAppUser(user);
            else
                userRepository.DeleteShoppingCartEntry(entryId);
            
            this.unitOfWork.Save();
        }

        public async Task<bool> ListingQuantityExceedsCartQuantity(int listingId, int entryId)
        {
            AppUser user = await GetCurrentUser();
            ShoppingCartEntry entry = user.ShoppingCartEntries.Where(e => e.ShoppingCartEntryID == entryId).Single(); //userRepository.GetShoppingCartEntryByID(entryId);
            Listing listing = listingRepository.GetListingByID(listingId);

            if (listing.Quantity > entry.Quantity)
                return true;
            else
                return false;
        }

        public void Dispose()
        {
            this.unitOfWork.Dispose();
        }

        public AppUser GetCurrentUserSynch()
        {
            return UserManager.FindByName(userName);
        }

        public async Task<AppUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsync(userName);
        }
    }
}
