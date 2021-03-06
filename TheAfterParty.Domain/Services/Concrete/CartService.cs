﻿using System;
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
            userName = "";
        }
        protected CartService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }
        
        public Order GetOrderByID(int id)
        {
            return userRepository.GetOrderByID(id);
        }
        public int GetNewestOrderId(string id)
        {
            return userRepository.GetOrdersQuery().Where(a => a.UserID == id).OrderByDescending(b => b.SaleDate).FirstOrDefault().OrderID;
        }

        public async Task AddItemToCart(int listingId)
        {
            Listing listing = listingRepository.GetListingByIDStripped(listingId);

            if (listing != null)
            {
                AppUser user = await GetCurrentUserSimple();
                
                ShoppingCartEntry entry = userRepository.GetShoppingCartEntriesQuery().Where(a => a.ListingID == listingId && String.Equals(user.Id, a.UserID)).SingleOrDefault(); //userRepository.GetShoppingCartEntries().Where(e => e.ListingID == listingId && Object.Equals(currentUser.Id, e.UserID)).SingleOrDefault();

                if (entry == null)
                {
                    entry = new ShoppingCartEntry(user, listing, 1);
                    userRepository.InsertShoppingCartEntry(entry);
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
        public IQueryable<ShoppingCartEntry> GetShoppingCartEntries(String userId)
        {
            return userRepository.GetShoppingCartEntriesQuery().Where(e => Object.Equals(userId, e.UserID));
        }

        public async Task<bool> ValidateOrder()
        {
            AppUser user = await GetCurrentUser();

            return user.AssertValidOrder();
        }

        public Listing GetDailyDeal()
        {
            DiscountedListing deal = listingRepository.GetDiscountedListingsQuery().Where(d => d.DailyDeal).FirstOrDefault();

            if (deal == null)
            {
                return null;
            }
            else
            {
                return deal.Listing;
            }
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
            ICollection<ShoppingCartEntry> cartEntries = user.ShoppingCartEntries;
            
            if (user == null || cartEntries == null || cartEntries.Count == 0 || !user.AssertValidOrder())
            {
                return null;
            }

            DateTime orderDate = DateTime.Now;
            Order order = new Order(user, orderDate);
            userRepository.InsertOrder(order);
            unitOfWork.Save();


            foreach (ShoppingCartEntry entry in cartEntries)
            {
                IEnumerable<ProductKey> keys = entry.Listing.ProductKeys.Take(entry.Quantity);
                // part of removed code below
                //int remainingQuantity = entry.Quantity - keys.Count;

                foreach (ProductKey productKey in keys)
                {
                    productKey.Listing.Quantity--;

                    listingRepository.UpdateListingSimple(productKey.Listing);
                    listingRepository.DeleteProductKey(productKey.ProductKeyID);

                    ClaimedProductKey claimedKey = new ClaimedProductKey(productKey, user, orderDate, "Purchase - Order #" + order.OrderID);
                    //user.AddClaimedProductKey(claimedKey);
                    userRepository.InsertClaimedProductKey(claimedKey);

                    ProductOrderEntry orderEntry = new ProductOrderEntry(order, entry);
                    orderEntry.AddClaimedProductKey(claimedKey);

                    order.AddProductOrderEntry(orderEntry);
                }
                #region removed   
                //                // the case where there are some product keys, but not enough to fulfill the order (the remainining are keys of child listings bundled together)
                //                // rarely triggered (if ever)
                //                if (entry.Listing.ChildListings != null && entry.Listing.ChildListings.Count > 0 && keys != null && keys.Count > 0 && keys.Count < entry.Quantity)
                //                {
                //                    foreach (ProductKey productKey in keys)
                //                    {
                //                        productKey.Listing.Quantity--;
                //                        productKey.Listing.UpdateParentQuantities();

                //                        listingRepository.UpdateListing(productKey.Listing);
                //                        listingRepository.DeleteProductKey(productKey.ProductKeyID);

                //                        ClaimedProductKey claimedKey = new ClaimedProductKey(productKey, user, orderDate, "Purchase - Order #" + order.OrderID);
                //                        user.AddClaimedProductKey(claimedKey);
                //                        userRepository.InsertClaimedProductKey(claimedKey);
                //                        //unitOfWork.Save();

                //                        ProductOrderEntry orderEntry = new ProductOrderEntry(order, entry);
                //                        order.AddProductOrderEntry(orderEntry);
                //                        userRepository.InsertProductOrderEntry(orderEntry);
                //                        //unitOfWork.Save();
                //                        orderEntry.AddClaimedProductKey(claimedKey);
                //                    }

                //                    keys = new List<ProductKey>();

                //                    foreach (Listing childListing in entry.Listing.ChildListings)
                //                    {
                //                        keys.AddRange(listingRepository.GetProductKeys().Where(k => k.ListingID == childListing.ListingID).Take(remainingQuantity));
                //                    }
                //                }
                //                else if (entry.Listing.ChildListings != null && entry.Listing.ChildListings.Count > 0 && keys.Count < entry.Quantity)
                //                {
                //                    foreach (Listing childListing in entry.Listing.ChildListings)
                //                    {
                //                        keys.AddRange(listingRepository.GetProductKeys().Where(k => k.ListingID == childListing.ListingID).Take(entry.Quantity));
                //                    }
                //                }

                //                if (entry.Listing.ChildListings == null || entry.Listing.ChildListings.Count == 0 || keys.Count == entry.Quantity)
                //                {
                //                    foreach (ProductKey productKey in keys)
                //                    {
                //                        productKey.Listing.Quantity--;
                //                        productKey.Listing.UpdateParentQuantities();

                //                        listingRepository.UpdateListing(productKey.Listing);
                //                        listingRepository.DeleteProductKey(productKey.ProductKeyID);

                //                        ClaimedProductKey claimedKey = new ClaimedProductKey(productKey, user, orderDate, "Purchase - Order #" + order.OrderID);
                //                        user.AddClaimedProductKey(claimedKey);
                //                        userRepository.InsertClaimedProductKey(claimedKey);
                //                        //unitOfWork.Save();

                //                        ProductOrderEntry orderEntry = new ProductOrderEntry(order, entry);
                //                        userRepository.InsertProductOrderEntry(orderEntry);
                //                       // unitOfWork.Save();
                //                        orderEntry.AddClaimedProductKey(claimedKey);

                //                        order.AddProductOrderEntry(orderEntry);
                //                    }
                //                }
                //                else
                //                {
                //                    for (int i = 0; i < remainingQuantity; i++)
                //                    {
                //                        ProductOrderEntry orderEntry = new ProductOrderEntry(order, entry);
                //                        order.AddProductOrderEntry(orderEntry);
                //                        userRepository.InsertProductOrderEntry(orderEntry);
                //                        //unitOfWork.Save();

                //                        foreach (Listing childListing in entry.Listing.ChildListings)
                //                        {
                //                            ProductKey productKey = keys.Where(k => k.Listing.ListingID == childListing.ListingID).First();
                //                            keys.Remove(productKey);

                //                            productKey.Listing.Quantity--;
                //                           // productKey.Listing.UpdateParentQuantities();

                //                            listingRepository.UpdateListing(productKey.Listing);
                //                            listingRepository.DeleteProductKey(productKey.ProductKeyID);

                //                            ClaimedProductKey claimedKey = new ClaimedProductKey(productKey, user, orderDate, "Purchase - Order #" + order.OrderID);
                //                            userRepository.InsertClaimedProductKey(claimedKey);
                //                            //unitOfWork.Save();
                //                            orderEntry.AddClaimedProductKey(claimedKey);
                //                            user.AddClaimedProductKey(claimedKey);
                //                        }

                //                        order.AddProductOrderEntry(orderEntry);
                //                    }
                //                }

                //                //unitOfWork.Save();
                #endregion
            }

            DeleteShoppingCart(user);

            int totalSalePrice = order.TotalSalePrice();

            BalanceEntry balanceEntry = new BalanceEntry(user, "Purchase - Order #" + order.OrderID, 0 - totalSalePrice, orderDate);
            //user.BalanceEntries.Add(balanceEntry);
            userRepository.InsertBalanceEntry(balanceEntry);

            user.Balance -= totalSalePrice;
            //user.AddOrder(order);
            userRepository.UpdateOrder(order);
            await userRepository.UpdateAppUserSimple(user);

            this.unitOfWork.Save();

            return userRepository.GetOrderByID(order.OrderID);
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
        public void DeleteShoppingCart(AppUser user)
        {
            List<Int32> entryIds = user.ShoppingCartEntries.Select(e => e.ShoppingCartEntryID).ToList();

            foreach (Int32 id in entryIds)
            {
                userRepository.DeleteShoppingCartEntry(id);
            }
        }
        public void DeleteShoppingCart(IEnumerable<ShoppingCartEntry> entries)
        {
            List<Int32> entryIds = entries.Select(e => e.ShoppingCartEntryID).ToList();
            //IEnumerable<ShoppingCartEntry> entries = user.ShoppingCartEntries; //userRepository.GetShoppingCartEntries().Where(e => Object.Equals(user.Id, e.UserID));

            foreach (Int32 id in entryIds)
            {
                userRepository.DeleteShoppingCartEntry(id);
            }
        }

        public void UpdateShoppingCartEntry(int entryId, int quantity)
        {
            ShoppingCartEntry entry = userRepository.GetShoppingCartEntriesQuery().Where(a => a.ShoppingCartEntryID == entryId).SingleOrDefault(); //userRepository.GetShoppingCartEntries().Where(e => e.ListingID == listingId && Object.Equals(currentUser.Id, e.UserID)).SingleOrDefault();

            if (entry != null)
            {
                entry.Quantity = quantity;
                userRepository.UpdateShoppingCartEntry(entry);
            }

            unitOfWork.Save();
        }

        public async Task IncrementCartQuantity(int entryId)
        {
            AppUser user = await GetCurrentUser();
            ShoppingCartEntry entry = user.ShoppingCartEntries.Where(e => e.ShoppingCartEntryID == entryId).Single(); //userRepository.GetShoppingCartEntryByID(entryId);
            entry.Quantity++;
            userRepository.UpdateShoppingCartEntry(entry);

            this.unitOfWork.Save();
        }

        public async Task DecrementCartQuantity(int entryId)
        {
            AppUser user = await GetCurrentUser();
            ShoppingCartEntry entry = user.ShoppingCartEntries.Where(e => e.ShoppingCartEntryID == entryId).Single(); //userRepository.GetShoppingCartEntryByID(entryId);
            entry.Quantity--;

            if (entry.Quantity > 0)
                userRepository.UpdateShoppingCartEntry(entry);
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

        public Listing GetListingByID(int listingId)
        {
            return listingRepository.GetListingByIDStripped(listingId);
        }

        public void Dispose()
        {
            this.unitOfWork.Dispose();
        }

        public AppUser GetCurrentUserSynch()
        {
            return UserManager.FindByName(userName);
        }

        public async Task<AppUser> GetCurrentUserSimple()
        {
            return await UserManager.FindByNameSimple(userName);
        }
        public async Task<AppUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsyncWithCartAndOpenAuctionBids(userName);
        }
        public async Task<AppUser> GetCurrentUserWithOrders()
        {
            return await UserManager.FindByNameAsyncWithCartOpenAuctionBidsAndOrders(userName);
        }
    }
}
