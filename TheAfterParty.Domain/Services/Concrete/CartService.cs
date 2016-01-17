using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using Microsoft.AspNet.Identity.EntityFramework;

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

                ShoppingCartEntry entry = userRepository.GetShoppingCartEntries().Where(e => e.ListingID == listingId && Object.Equals(currentUser.Id, e.UserID)).SingleOrDefault();

                if (entry == null)
                {
                    userRepository.InsertShoppingCartEntry(currentUser.AddShoppingCartEntry(listing));
                }
                else
                {
                    entry.Quantity++;
                    userRepository.UpdateShoppingCartEntry(entry);
                }

                unitOfWork.Save();
            }
        }

        public async Task<IEnumerable<ShoppingCartEntry>> GetShoppingCartEntries()
        {
            AppUser user = await GetCurrentUser();

            return userRepository.GetShoppingCartEntries().Where(e => Object.Equals(user.Id, e.UserID));
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

            ICollection<ShoppingCartEntry> cartEntries = user.ShoppingCartEntries.Where(entry => Object.Equals(entry.UserID, user.Id)).ToList();

            foreach (ShoppingCartEntry entry in cartEntries)
            {
                for (int i = 0; i < entry.Quantity; i++)
                {
                    ProductKey productKey = listingRepository.GetProductKeys().Where(k => k.ListingID == entry.ListingID).First();
                    listingRepository.DeleteProductKey(productKey.KeyID);

                    ClaimedProductKey claimedKey = new ClaimedProductKey(productKey, user, orderDate, "Purchase - Order #" + order.TransactionID);
                    userRepository.InsertClaimedProductKey(claimedKey);

                    ProductOrderEntry orderEntry = new ProductOrderEntry(order, entry, claimedKey);
                    userRepository.InsertProductOrderEntry(orderEntry);

                    userRepository.DeleteShoppingCartEntry(entry.ShoppingID);
                }
            }

            BalanceEntry balanceEntry = new BalanceEntry(user, "Purchase - Order #" + order.TransactionID, user.GetCartTotal(), orderDate);
            order.BalanceEntry = balanceEntry;
            userRepository.InsertBalanceEntry(balanceEntry);

            this.unitOfWork.Save();
            user.Orders.Add(order);

            return order;
        }

        public void Dispose()
        {
            this.unitOfWork.Dispose();
        }

        public async Task<AppUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsync(userName);
        }
    }
}
