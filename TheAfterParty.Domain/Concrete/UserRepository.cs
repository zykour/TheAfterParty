﻿using System;
using System.Collections.Generic;
using System.Linq;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace TheAfterParty.Domain.Concrete
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private AppIdentityDbContext context;
        public AppIdentityDbContext GetContext()
        {
            return context;
        }
        public AppUserManager userManager;

        public UserRepository(IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.context = unitOfWork.DbContext;
        }
        public UserRepository(AppUserManager userManager)
        {
            this.userManager = userManager;
        }
        
        public IEnumerable<AppUser> GetAppUsers()
        {
            return userManager.Users;
        }
        public async Task<AppUser> GetAppUserByID(string appUserId)
        {
            return await userManager.FindByIdAsync(appUserId);
        }
        public async Task InsertAppUser(AppUser appUser)
        {
            await userManager.CreateAsync(appUser);

            foreach (BalanceEntry entry in appUser.BalanceEntries)
            {
                if (entry.BalanceEntryID == 0)
                {
                    InsertBalanceEntry(entry);
                }
                else
                {
                    UpdateBalanceEntry(entry);
                }
            }

            foreach (ClaimedProductKey entry in appUser.ClaimedProductKeys)
            {
                if (entry.ClaimedProductKeyID == 0)
                {
                    InsertClaimedProductKey(entry);
                }
                else
                {
                    UpdateClaimedProductKey(entry);
                }
            }
            
            foreach (Mail entry in appUser.ReceivedMail)
            {
                if (entry.MailID == 0)
                {
                    InsertMail(entry);
                }
                else
                {
                    UpdateMail(entry);
                }
            }

            foreach (Order entry in appUser.Orders)
            {
                if (entry.OrderID == 0)
                {
                    InsertOrder(entry);
                }
                else
                {
                    UpdateOrder(entry);
                }
            }

            foreach (OwnedGame entry in appUser.OwnedGames)
            {
                if (entry.OwnedGameID == 0)
                {
                    InsertOwnedGame(entry);
                }
            }

            foreach (UserNotification entry in appUser.UserNotifications)
            {
                if (entry.UserNotificationID == 0)
                {
                    InsertUserNotification(entry);
                }
                else
                {
                    UpdateUserNotification(entry);
                }
            }

            foreach (UserTag entry in appUser.UserTags)
            {
                if (entry.UserTagID == 0)
                {
                    InsertUserTag(entry);
                }
            }
        }
        public void UpdateAppUserSynch(AppUser appUser)
        {
            userManager.Update(appUser);

            foreach (BalanceEntry entry in appUser.BalanceEntries)
            {
                if (entry.BalanceEntryID == 0)
                {
                    InsertBalanceEntry(entry);
                }
                else
                {
                    UpdateBalanceEntry(entry);
                }
            }

            foreach (ClaimedProductKey entry in appUser.ClaimedProductKeys)
            {
                if (entry.ClaimedProductKeyID == 0)
                {
                    InsertClaimedProductKey(entry);
                }
                else
                {
                    UpdateClaimedProductKey(entry);
                }
            }

            foreach (Gift entry in appUser.ReceivedGifts)
            {
                if (entry.GiftID == 0)
                {
                    InsertGift(entry);
                }
                else
                {
                    UpdateGift(entry);
                }
            }

            foreach (Mail entry in appUser.ReceivedMail)
            {
                if (entry.MailID == 0)
                {
                    InsertMail(entry);
                }
                else
                {
                    UpdateMail(entry);
                }
            }

            foreach (Order entry in appUser.Orders)
            {
                if (entry.OrderID == 0)
                {
                    InsertOrder(entry);
                }
                else
                {
                    UpdateOrder(entry);
                }
            }

            foreach (OwnedGame entry in appUser.OwnedGames)
            {
                if (entry.OwnedGameID == 0)
                {
                    InsertOwnedGame(entry);
                }
            }

            foreach (UserNotification entry in appUser.UserNotifications)
            {
                if (entry.UserNotificationID == 0)
                {
                    InsertUserNotification(entry);
                }
                else
                {
                    UpdateUserNotification(entry);
                }
            }

            foreach (WishlistEntry entry in appUser.WishlistEntries)
            {
                if (entry.WishlistEntryID == 0)
                {
                    InsertWishlistEntry(entry);
                }
            }

            foreach (UserTag entry in appUser.UserTags)
            {
                if (entry.UserTagID == 0)
                {
                    InsertUserTag(entry);
                }
            }
        }

        public async Task UpdateAppUserSimple(AppUser appUser)
        {
            await userManager.UpdateAsync(appUser);
        }
        public async Task UpdateAppUser(AppUser appUser)
        {
            await userManager.UpdateAsync(appUser);
            
            foreach (BalanceEntry entry in appUser.BalanceEntries)
            {
                if (entry.BalanceEntryID == 0)
                {
                    InsertBalanceEntry(entry);
                }
                else
                {
                    UpdateBalanceEntry(entry);
                }
            }

            foreach (ClaimedProductKey entry in appUser.ClaimedProductKeys)
            {
                if (entry.ClaimedProductKeyID == 0)
                {
                    InsertClaimedProductKey(entry);
                }
                else
                {
                    UpdateClaimedProductKey(entry);
                }
            }

            foreach (Order entry in appUser.Orders)
            {
                if (entry.OrderID == 0)
                {
                    InsertOrder(entry);
                }
                else
                {
                    UpdateOrder(entry);
                }
            }

            foreach (OwnedGame entry in appUser.OwnedGames.Where(x => x.OwnedGameID == 0))
            {
                InsertOwnedGame(entry);
            }

            foreach (WishlistEntry entry in appUser.WishlistEntries.Where(x => x.WishlistEntryID == 0))
            {
                InsertWishlistEntry(entry);
            }

        }
        public async Task DeleteAppUser(string appUserId)
        {
            AppUser user = await userManager.FindByIdAsync(appUserId);
            await userManager.DeleteAsync(user);
        }


        // ---- ShoppingCartEntry entity persistance

        public IEnumerable<ShoppingCartEntry> GetShoppingCartEntries()
        {
            return context.ShoppingCartEntries
                .Include(x => x.Listing.ChildListings.Select(y => y.Product.Listings.Select(z => z.ChildListings))) // this may be more eager than needed
                .Include(x => x.Listing.ChildListings.Select(y => y.DiscountedListings))
                .Include(x => x.Listing.ChildListings.Select(y => y.ProductKeys))
                .Include(x => x.Listing.ChildListings.Select(y => y.Platforms))
                .Include(x => x.AppUser)
                .Include(x => x.Listing.Product)
                .Include(x => x.Listing.DiscountedListings)
                .Include(x => x.Listing.Platforms)
                .Include(x => x.Listing.ProductKeys)
                .AsQueryable();
        }
        public IQueryable<ShoppingCartEntry> GetShoppingCartEntriesQuery()
        {
            return context.ShoppingCartEntries
                .Include(x => x.Listing.ChildListings.Select(y => y.Product.Listings.Select(z => z.ChildListings))) // this may be more eager than needed
                .Include(x => x.Listing.ChildListings.Select(y => y.DiscountedListings))
                .Include(x => x.Listing.ChildListings.Select(y => y.ProductKeys))
                .Include(x => x.Listing.ChildListings.Select(y => y.Platforms))
                .Include(x => x.AppUser)
                .Include(x => x.Listing.Product)
                .Include(x => x.Listing.DiscountedListings)
                .Include(x => x.Listing.Platforms)
                .Include(x => x.Listing.ProductKeys)
                .AsQueryable();
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
            ShoppingCartEntry targetShoppingCartEntry = context.ShoppingCartEntries.Find(shoppingCartEntry.ShoppingCartEntryID);

            if (targetShoppingCartEntry != null)
            {
                targetShoppingCartEntry.Quantity = shoppingCartEntry.Quantity;
                targetShoppingCartEntry.DateAdded = shoppingCartEntry.DateAdded;
                targetShoppingCartEntry.ListingID = shoppingCartEntry.ListingID;
                targetShoppingCartEntry.UserID = shoppingCartEntry.UserID;
            }
        }


        // ---- Order entity persistance

        public IEnumerable<Order> GetOrders()
        {
            return context.Orders
                            .Include(x => x.AppUser)
                            .Include(x => x.ProductOrderEntries)
                            .Include(x => x.ProductOrderEntries.Select(y => y.ClaimedProductKeys))
                            .Include(x => x.ProductOrderEntries.Select(y => y.Listing));
        }
        public IQueryable<Order> GetOrdersQuery()
        {
            return context.Orders
                            .Include(x => x.AppUser)
                            .Include(x => x.ProductOrderEntries)
                            .Include(x => x.ProductOrderEntries.Select(y => y.ClaimedProductKeys))
                            .Include(x => x.ProductOrderEntries.Select(y => y.Listing))
                            .AsQueryable();
        }
        public Order GetOrderByID(int id)
        {
            return context.Orders
                            .Include(x => x.AppUser)
                            .Include(x => x.ProductOrderEntries.Select(a => a.Listing.Product.Listings.Select(b => b.ChildListings)))
                            .Include(x => x.ProductOrderEntries.Select(d => d.Listing.ChildListings.Select(f => f.Product.Listings.Select(g => g.ChildListings))))
                            .Include(x => x.ProductOrderEntries.Select(y => y.ClaimedProductKeys))
                            .Include(x => x.ProductOrderEntries.Select(y => y.Listing.Platforms))
                            .SingleOrDefault(x => x.OrderID == id);
                            //.Find(id);
        }
        public void InsertOrder(Order order)
        {
            context.Orders.Add(order);

            if (order.ProductOrderEntries != null)
            {
                foreach (ProductOrderEntry entry in order.ProductOrderEntries)
                {
                    if (entry.ProductOrderEntryID == 0)
                    {
                        InsertProductOrderEntry(entry);
                    }
                    else
                    {
                        UpdateProductOrderEntry(entry);
                    }
                }
            }
        }
        public void UpdateOrder(Order order)
        {
            Order targetOrder = context.Orders.Find(order.OrderID);

            if (targetOrder != null)
            {
                targetOrder.SaleDate = order.SaleDate;
                targetOrder.UserID = order.UserID;
            }

            if (order.ProductOrderEntries != null)
            {
                foreach (ProductOrderEntry entry in order.ProductOrderEntries)
                {
                    if (entry.ProductOrderEntryID == 0)
                    {
                        InsertProductOrderEntry(entry);
                    }
                    else
                    { 
                        UpdateProductOrderEntry(entry);
                    }
                }
            }
        }
        public void DeleteOrder(int orderId)
        {
            Order order = context.Orders.Find(orderId);
            context.Orders.Remove(order);
        }
        

        // ---- ProductOrderEntry entity persistance

        public IEnumerable<ProductOrderEntry> GetProductOrderEntries()
        {
            return context.ProductOrderEntries
                                    .Include(x => x.Listing)
                                    .Include(x => x.Order.AppUser);
        }
        public ProductOrderEntry GetProductOrderEntryByID(int id)
        {
            return context.ProductOrderEntries
                                    .Include(x => x.Listing)
                                    .Include(x => x.Order)
                                    .Include(x => x.ClaimedProductKeys)
                                    .SingleOrDefault(x => x.ProductOrderEntryID == id);
            //.Find(id);
        }
        public void InsertProductOrderEntry(ProductOrderEntry productOrderEntry)
        {
            context.ProductOrderEntries.Add(productOrderEntry);

            //foreach (ClaimedProductKey key in productOrderEntry.ClaimedProductKeys)
            //{
            //    if (key.ClaimedProductKeyID == 0)
            //    {
            //        InsertClaimedProductKey(key);
            //    }
            //    else
            //    {
            //        UpdateClaimedProductKey(key);
            //    }
            //}
        }
        public void UpdateProductOrderEntry(ProductOrderEntry productOrderEntry)
        {
            ProductOrderEntry targetOrderProduct = context.ProductOrderEntries.Find(productOrderEntry.ProductOrderEntryID);

            if (targetOrderProduct != null)
            {
                targetOrderProduct.SalePrice = productOrderEntry.SalePrice;
                targetOrderProduct.ListingID = productOrderEntry.ListingID;
            }

            //foreach (ClaimedProductKey key in productOrderEntry.ClaimedProductKeys)
            //{
            //    if (key.ClaimedProductKeyID == 0)
            //    {
            //        InsertClaimedProductKey(key);
            //    }
            //    else
            //    {
            //        UpdateClaimedProductKey(key);
            //    }
            //}
        }
        public void DeleteProductOrderEntry(int productOrderEntryId)
        {
            ProductOrderEntry productOrderEntry = context.ProductOrderEntries.Find(productOrderEntryId);
            context.ProductOrderEntries.Remove(productOrderEntry);
        }


        // ---- ClaimedProductKey entity persistance

        public IEnumerable<ClaimedProductKey> GetClaimedProductKeys()
        {
            return context.ClaimedProductKeys
                                        .Include(x => x.AppUser)
                                        .Include(x => x.Listing);
        }
        public ClaimedProductKey GetClaimedProductKeyByID(int id)
        {
            return context.ClaimedProductKeys
                                        .Include(x => x.AppUser)
                                        .Include(x => x.Listing)
                                        .SingleOrDefault(x => x.ClaimedProductKeyID == id);
                                        //.Find(id);
        }
        public void InsertClaimedProductKey(ClaimedProductKey claimedProductKey)
        {
            context.ClaimedProductKeys.Add(claimedProductKey);
        }
        public void UpdateClaimedProductKey(ClaimedProductKey claimedProductKey)
        {
            ClaimedProductKey targetClaimedProductKey = context.ClaimedProductKeys.Find(claimedProductKey.ClaimedProductKeyID);

            if (targetClaimedProductKey != null)
            {
                targetClaimedProductKey.AcquisitionTitle = claimedProductKey.AcquisitionTitle;
                targetClaimedProductKey.Date = claimedProductKey.Date;
                targetClaimedProductKey.IsRevealed = claimedProductKey.IsRevealed;
                targetClaimedProductKey.IsUsed = claimedProductKey.IsUsed;
                targetClaimedProductKey.Key = claimedProductKey.Key;
                targetClaimedProductKey.ListingID = claimedProductKey.ListingID;
                targetClaimedProductKey.UserID = claimedProductKey.UserID;
                targetClaimedProductKey.IsGift = claimedProductKey.IsGift;
            }
        }
        public void DeleteClaimedProductKey(int claimedProductKeyId)
        {
            ClaimedProductKey claimedProductKey = context.ClaimedProductKeys.Find(claimedProductKeyId);
            context.ClaimedProductKeys.Remove(claimedProductKey);
        }


        // ---- BalanceEntry entity persistance

        public IEnumerable<BalanceEntry> GetBalanceEntries()
        {
            return context.BalanceEntries.Include(x => x.AppUser).Include(x => x.Objective.BoostedObjective);
        }
        public BalanceEntry GetBalanceEntryByID(int id)
        {
            return context.BalanceEntries.Find(id);
        }
        public void InsertBalanceEntry(BalanceEntry balanceEntry)
        {
            context.BalanceEntries.Add(balanceEntry);
        }
        public void UpdateBalanceEntry(BalanceEntry balanceEntry)
        {
            BalanceEntry targetBalanceEntry = context.BalanceEntries.Find(balanceEntry.BalanceEntryID);

            if (targetBalanceEntry != null)
            {
                targetBalanceEntry.PointsAdjusted = balanceEntry.PointsAdjusted;
                targetBalanceEntry.Notes = balanceEntry.Notes;
                targetBalanceEntry.UserID = balanceEntry.UserID;
                targetBalanceEntry.Date = balanceEntry.Date;
            }
        }
        public void DeleteBalanceEntry(int balanceEntryId)
        {
            BalanceEntry balanceEntry = context.BalanceEntries.Find(balanceEntryId);
            context.BalanceEntries.Remove(balanceEntry);
        }


        // ---- Gift entity persistance

        public IEnumerable<Gift> GetGifts()
        {
            return context.Gifts;
        }
        public Gift GetGiftByID(int id)
        {
            return context.Gifts.Find(id);
        }
        public void InsertGift(Gift gift)
        {
            context.Gifts.Add(gift);
        }
        public void UpdateGift(Gift gift)
        {
            Gift targetGift = context.Gifts.Find(gift.GiftID);

            if (targetGift != null)
            {
                targetGift.DateReceived = gift.DateReceived;
                targetGift.DateSent = gift.DateSent;
                targetGift.IsPending = gift.IsPending;
                targetGift.ReceiverID = gift.ReceiverID;
                targetGift.SenderID = gift.SenderID;
            }
        }
        public void DeleteGift(int giftId)
        {
            Gift gift = context.Gifts.Find(giftId);
            context.Gifts.Remove(gift);
        }


        // ---- Mail entity persistance

        public IEnumerable<Mail> GetMail()
        {
            return context.Mail;
        }
        public Mail GetMailByID(int id)
        {
            return context.Mail.Find(id);
        }
        public void InsertMail(Mail mail)
        {
            context.Mail.Add(mail);
        }
        public void UpdateMail(Mail mail)
        {
            Mail targetMail = context.Mail.Find(mail.MailID);

            if (targetMail != null)
            {
                targetMail.DateSent = mail.DateSent;
                targetMail.Message = mail.Message;
                targetMail.ReceiverUserID = mail.ReceiverUserID;
                targetMail.SenderUserID = mail.SenderUserID;
                targetMail.Heading = mail.Heading;
            }
        }
        public void DeleteMail(int mailId)
        {
            Mail mail = context.Mail.Find(mailId);
            context.Mail.Remove(mail);
        }


        // ---- UserNotification entity persistance

        public IEnumerable<UserNotification> GetUserNotifications()
        {
            return context.UserNotifications;
        }
        public UserNotification GetUserNotificationByID(int id)
        {
            return context.UserNotifications.Find(id);
        }
        public void InsertUserNotification(UserNotification userNotification)
        {
            context.UserNotifications.Add(userNotification);
        }
        public void UpdateUserNotification(UserNotification userNotification)
        {
            UserNotification targetUserNotification = context.UserNotifications.Find(userNotification.UserNotificationID);

            if (targetUserNotification != null)
            {
                targetUserNotification.DateTime = userNotification.DateTime;
                targetUserNotification.IsRead = userNotification.IsRead;
                targetUserNotification.Message = userNotification.Message;
            }
        }
        public void DeleteUserNotification(int userNotificationId)
        {
            UserNotification userNotification = context.UserNotifications.Find(userNotificationId);
            context.UserNotifications.Remove(userNotification);
        }


        // ---- WishlistEntry entity persistance

        public IEnumerable<WishlistEntry> GetWishlistEntries()
        {
            return context.WishlistEntries;
        }
        public WishlistEntry GetWishlistEntryByID(int wishlistEntryId)
        {
            return context.WishlistEntries.Find(wishlistEntryId);
        }
        public void InsertWishlistEntry(WishlistEntry wishlistEntry)
        {
            context.WishlistEntries.Add(wishlistEntry);
        }
        public void DeleteWishlistEntry(int wishlistEntryId)
        {
            WishlistEntry wishlistEntry = context.WishlistEntries.Find(wishlistEntryId);
            context.WishlistEntries.Remove(wishlistEntry);
        }


        // ---- OwnedGame entity persistance

        public IEnumerable<OwnedGame> GetOwnedGames()
        {
            return context.OwnedGames;
        }
        public IEnumerable<String> GetUsersWhoOwn(int appId)
        {
            return context.OwnedGames.Where(o => o.AppID == appId).Select(o => o.AppUser.UserName);
        }
        public IEnumerable<AppUser> GetAppUsersWhoDoNotOwn(int appId)
        {
            return userManager.Users.Where(u => !u.OwnedGames.Any(o => o.AppID == appId));
        }
        public IEnumerable<AppUser> GetAppUsersWhoOwn(int appId)
        {
            return context.OwnedGames.Where(o => o.AppID == appId).Select(o => o.AppUser);
        }
        public OwnedGame GetOwnedGameByID(int id)
        {
            return context.OwnedGames.Find(id);
        }
        public void InsertOwnedGame(OwnedGame ownedGame)
        {
            context.OwnedGames.Add(ownedGame);
        }
        public void UpdateOwnedGame(OwnedGame ownedGame)
        {
            OwnedGame targetOwnedGame = context.OwnedGames.Find(ownedGame.OwnedGameID);

            if (targetOwnedGame != null)
            {
                targetOwnedGame.MinutesPlayed = ownedGame.MinutesPlayed;
            }
        }
        public void DeleteOwnedGame(int ownedGameId)
        {
            OwnedGame ownedGame = context.OwnedGames.Find(ownedGameId);
            context.OwnedGames.Remove(ownedGame);
        }

        public IEnumerable<UserTag> GetUserTags()
        {
            return context.UserTags;
        }
        public UserTag GetUserTagByID(int userTagId)
        {
            return context.UserTags.Find(userTagId);
        }
        public void InsertUserTag(UserTag userTag)
        {
            context.UserTags.Add(userTag);
        }
        public void DeleteUserTag(int userTagId)
        {
            UserTag targetUserTag = context.UserTags.Find(userTagId);

            if (targetUserTag != null)
            {
                context.UserTags.Remove(targetUserTag);
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
