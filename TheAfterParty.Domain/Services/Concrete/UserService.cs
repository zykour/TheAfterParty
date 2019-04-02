using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SteamKit2;
using System;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using System.Web;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using TheAfterParty.Domain.Services;
using System.Web.Mvc;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.Domain.Services
{
    public class UserService : IUserService
    {
        private IListingRepository listingRepository;
        private IUserRepository userRepository;
        private IPrizeRepository prizeRepository;
        private IGiveawayRepository giveawayRepository;
        private IAuctionRepository auctionRepository;
        private IObjectiveRepository objectiveRepository;
        private IUnitOfWork unitOfWork;
        public AppUserManager UserManager { get; private set; }
        public string userName { get; set; }

        public UserService(IListingRepository listingRepository, IUserRepository userRepository, IPrizeRepository prizeRepository, IGiveawayRepository giveawayRepository, IAuctionRepository auctionRepository, IObjectiveRepository objectiveRepository, IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.listingRepository = listingRepository;
            this.userRepository = userRepository;
            this.prizeRepository = prizeRepository;
            this.giveawayRepository = giveawayRepository;
            this.auctionRepository = auctionRepository;
            this.objectiveRepository = objectiveRepository;
            this.unitOfWork = unitOfWork;
            userName = "";
        }
        protected UserService(AppUserManager userManager)
        {
            UserManager = userManager;
            UserManager.UserValidator = new UserValidator<AppUser>(userManager) { AllowOnlyAlphanumericUserNames = false };
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }

        public AppUserManager GetUserManager()
        {
            return UserManager;
        }


        public async Task<int> GetTotalReservedBalance()
        {
            int total = 0;

            total += await GetSilentAuctionReservedBalance();
            total += await GetPublicAuctionReservedBalance();
            total += await GetCartTotal();

            return total;
        }
        public async Task<int> GetSilentAuctionReservedBalance()
        {
            AppUser user = await GetCurrentUserWithAuctions();

            return user.GetSilentAuctionReservedBalance();
        }
        public async Task<int> GetPublicAuctionReservedBalance()
        {
            AppUser user = await GetCurrentUserWithAuctions();

            return user.GetPublicAuctionReservedBalance();
        }
        public async Task<int> GetCartTotal()
        {
            AppUser user = await GetCurrentUser();

            return user.GetCartTotal();
        }

        public AppUser GetUserByNickname(string nickname)
        {
            return UserManager.Users.Where(u => object.Equals(nickname.ToUpper(), u.Nickname.ToUpper())).SingleOrDefault();
        }

        public String GetGameName(int appId)
        {
            if (appId <= 0)
            {
                return String.Empty;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            String url = String.Format("https://store.steampowered.com/api/appdetails?appids={0}&filters=basic", appId.ToString());

            string result = new System.Net.WebClient().DownloadString(url);

            JObject jsonResult = JObject.Parse(result);

            string appID = appId.ToString();

            if (jsonResult == null || jsonResult[appID] == null || jsonResult[appID]["data"] == null)
            {
                return String.Empty;
            }

            JToken appData = jsonResult[appID]["data"];

            return (string)appData["name"] ?? String.Empty;
        }

        public async Task<bool> IsBlacklisted(int listingId)
        {
            Listing listing = listingRepository.GetListingByID(listingId);

            AppUser user = await GetCurrentUserWithBlacklist();

            return user.IsBlacklisted(listing);
        }

        public async Task ToggleBlacklist(int listingId)
        {
            AppUser user = await GetCurrentUserWithBlacklist();

            Listing listing = listingRepository.GetListingByID(listingId);

            if (user.IsBlacklisted(listing))
            {
                user.RemoveListingBlacklistEntry(listing);
            }
            else
            {
                user.AddListingBlacklistEntry(listing);
            }

            UserManager.Update(user);
            unitOfWork.Save();
        }

        public async Task TransferPoints(int points, string userId)
        {
            AppUser donor = await GetCurrentUserNoProperties();
            AppUser recipient = await UserManager.FindByIdAsync(userId);

            if (donor.Balance - donor.ReservedBalance() >= points)
            {
                donor.CreateBalanceEntry("Transfer of points to " + recipient.UserName, 0 - points, DateTime.Now);
                recipient.CreateBalanceEntry("Gift of points from " + donor.UserName, points, DateTime.Now);
                UserManager.Update(donor);
                UserManager.Update(recipient);
                unitOfWork.Save();
            }
        }

        #region Getters
        public BalanceEntry GetBalanceEntryByID(int id)
        {
            return userRepository.GetBalanceEntryByID(id);
        }
        public ClaimedProductKey GetClaimedProductKeyByID(int id)
        {
            return userRepository.GetClaimedProductKeyByID(id);
        }
        public ProductKey GetProductKey(int listingId)
        {
            return listingRepository.GetProductKeys().Where(k => k.ListingID == listingId).FirstOrDefault();
        }
        public Order GetOrderByID(int id)
        {
            return userRepository.GetOrderByID(id);
        }
        public ProductOrderEntry GetProductOrderEntryByID(int id)
        {
            return userRepository.GetProductOrderEntryByID(id);
        }
        public IEnumerable<Order> GetOrders()
        {
            return userRepository.GetOrders();
        }
        public IEnumerable<BalanceEntry> GetBalanceEntries()
        {
            return userRepository.GetBalanceEntries();
        }
        public IEnumerable<ClaimedProductKey> GetClaimedProductKeys()
        {
            return userRepository.GetClaimedProductKeys();
        }
        public IEnumerable<Auction> GetAuctions()
        {
            return auctionRepository.GetAuctions();
        }
        public IEnumerable<Giveaway> GetGiveaways()
        {
            return giveawayRepository.GetGiveaways();
        }
        public Giveaway GetGiveawayByID(int giveawayId)
        {
            return giveawayRepository.GetGiveawayByID(giveawayId);
        }
        public IEnumerable<AppUser> GetUsersWhoOwn(int appId)
        {
            return userRepository.GetAppUsersWhoOwn(appId).OrderBy(a => a.UserName);
        }
        public IEnumerable<AppUser> GetUsersWhoDoNotOwn(int appId)
        {
            return userRepository.GetAppUsersWhoDoNotOwn(appId).OrderBy(a => a.UserName);
        }
        public AppUser GetRequestedUser(string profileName, bool nickname = false)
        {
            if (nickname)
            {
                AppUser user = UserManager.Users.Where(u => object.Equals(u.Nickname.ToLower(), profileName.Trim().ToLower())).SingleOrDefault();

                if (user != null)
                {
                    return user;
                }
                else
                {
                    return UserManager.Users.Where(u => object.Equals(u.UserName.ToLower(), profileName.Trim().ToLower())).SingleOrDefault();
                }
            }
            else
            {
                return UserManager.Users.Where(u => object.Equals(u.UserName.ToLower(), profileName.Trim().ToLower())).SingleOrDefault();
            }
        }
        public IEnumerable<AppUser> GetAllUsers()
        {
            return userRepository.GetAppUsers();
        }
        public async Task<AppUser> GetUserByID(string id)
        {
            return await UserManager.FindByIdAsync(id);
            //return await userRepository.GetAppUserByID(id);
        }
        public IEnumerable<AppUser> GetAdmins()
        {
            return userRepository.GetAppUsers().Where(au => UserManager.IsInRole(au.Id, "Admin")).ToList();
        }
        public async Task<IEnumerable<Order>> GetUserOrders()
        {
            AppUser user = await GetCurrentUserNoProperties();

            return userRepository.GetOrders().Where(o => object.Equals(o.UserID, user.Id));
        }
        public async Task<IEnumerable<ClaimedProductKey>> GetKeys()
        {
            AppUser user = await GetCurrentUserNoProperties();

            return userRepository.GetClaimedProductKeys().Where(o => object.Equals(o.UserID, user.Id));
        }
        public async Task<IEnumerable<WonPrize>> GetWonPrizes()
        {
            AppUser user = await GetCurrentUserNoProperties();

            return prizeRepository.GetWonPrizes().Where(p => object.Equals(p.UserID, user.Id));
        }
        public async Task<IEnumerable<AuctionBid>> GetAuctionBids()
        {
            AppUser user = await GetCurrentUserNoProperties();

            return auctionRepository.GetAuctionBids().Where(b => object.Equals(b.UserID, user.Id));
        }
        public async Task<IEnumerable<Auction>> GetCreatedAuctions()
        {
            AppUser user = await GetCurrentUserNoProperties();

            return auctionRepository.GetAuctions().Where(a => object.Equals(a.CreatorID, user.Id));
        }
        public async Task<IEnumerable<GiveawayEntry>> GetGiveawayEntries()
        {
            AppUser user = await GetCurrentUserNoProperties();

            return giveawayRepository.GetGiveawayEntries().Where(ge => object.Equals(ge.UserID, user.Id));
        }
        public async Task<IEnumerable<Giveaway>> GetCreatedGiveaways()
        {
            AppUser user = await GetCurrentUserNoProperties();

            return giveawayRepository.GetGiveaways().Where(g => object.Equals(g.CreatorID, user.Id));
        }
        #endregion

        #region Admin Actions
        public async Task CreateAppUser(AppUser appUser, string password, string roleToAdd, string apiKey)
        {
            appUser.MemberSince = DateTime.Now;
            appUser.LastLogon = DateTime.Now;
            appUser.LastUpdated = DateTime.Now;

            if (String.IsNullOrEmpty(password) == false)
            {
                appUser.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
                await UserManager.CreateAsync(appUser, password);
            }
            else
            {
                await UserManager.CreateAsync(appUser);
            }

            if (String.IsNullOrEmpty(roleToAdd) == false)
            {
                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(unitOfWork.DbContext));

                if (roleToAdd.Contains(","))
                {
                    string[] rolesToAdd = roleToAdd.Split(new char[] { ',' });

                    foreach (string role in rolesToAdd)
                    {
                        if (roleManager.RoleExists(role) == false)
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }

                        await UserManager.AddToRoleAsync(appUser.Id, role);
                    }
                }
                else
                {
                    if (roleManager.RoleExists(roleToAdd) == false)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleToAdd));
                    }

                    await UserManager.AddToRoleAsync(appUser.Id, roleToAdd);
                }
            }

            Task task = new Task(new Action(() => BuildUser(appUser, apiKey)));
            task.Start();
        }
        public async Task<AppUser> GetUserByIdentifier(string identifier)
        {
            AppUser user = await GetUserByID(identifier);

            if (user == null)
            {
                user = GetRequestedUser(identifier);
            }

            if (user == null)
            {
                user = GetUserByNickname(identifier);
            }

            return user;
        }
        public async Task UpdateUser(string userIdentifier, string apiKey)
        {
            AppUser user = await GetUserByIdentifier(userIdentifier);

            if (user != null)
            {
                Task task = new Task(new Action(() => BuildUser(user, apiKey)));
                task.Start();
            }
        }
        public async Task EditAppUser(AppUser appUser, string roleToAdd, string roleToRemove)
        {
            AppUser updatedUser = UserManager.Users.Where(u => Object.Equals(u.Id, appUser.Id)).SingleOrDefault();

            if (updatedUser == null)
            {
                return;
            }

            updatedUser.Balance = appUser.Balance;
            updatedUser.Nickname = appUser.Nickname;
            updatedUser.UserSteamID = appUser.UserSteamID;
            updatedUser.UserName = appUser.UserName;
            updatedUser.MemberSince = appUser.MemberSince;
            updatedUser.TimeZoneID = appUser.TimeZoneID;
            updatedUser.PaginationPreference = appUser.PaginationPreference;

            await UserManager.UpdateAsync(updatedUser);

            if (String.IsNullOrEmpty(roleToAdd) == false)
            {
                if (roleToAdd.Contains(","))
                {
                    string[] rolesToAdd = roleToAdd.Split(new char[] { ',' });
                    RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());

                    foreach (string role in rolesToAdd)
                    {
                        if (roleManager.RoleExists(role) == false)
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                        await UserManager.AddToRoleAsync(appUser.Id, role);
                    }
                }
                else
                {
                    await UserManager.AddToRoleAsync(appUser.Id, roleToAdd);
                }
            }

            if (String.IsNullOrEmpty(roleToRemove) == false)
            {
                if (roleToRemove.Contains(","))
                {
                    string[] rolesToRemove = roleToRemove.Split(new char[] { ',' });

                    foreach (string role in rolesToRemove)
                    {
                        await UserManager.RemoveFromRoleAsync(appUser.Id, role);
                    }
                }
                else
                {
                    await UserManager.RemoveFromRoleAsync(appUser.Id, roleToRemove);
                }
            }
        }

        public async Task CreateBalanceEntry(BalanceEntry entry, int objectiveId, string nickname)
        {
            if (objectiveId != 0)
            {
                entry.Objective = objectiveRepository.GetObjectiveByID(objectiveId);
                if (entry.PointsAdjusted == 0)
                {
                    entry.PointsAdjusted = entry.Objective.FixedReward();
                }
            }

            if (String.IsNullOrEmpty(nickname) == false)
            {
                entry.AppUser = userRepository.GetAppUsers().Where(a => object.Equals(nickname.ToUpper(), a.UserName.ToUpper())).SingleOrDefault();
            }

            if (entry.AppUser == null)
            {
                return;
            }

            entry.AppUser.Balance += entry.PointsAdjusted;

            await UserManager.UpdateAsync(entry.AppUser);

            entry.Date = DateTime.Now;

            userRepository.InsertBalanceEntry(entry);
            unitOfWork.Save();
        }
        public async Task CreateBalanceEntry(BalanceEntry entry, int objectiveId)
        {
            if (objectiveId != 0)
            {
                entry.AddObjective(objectiveRepository.GetObjectiveByID(objectiveId));

                if (entry.PointsAdjusted == 0)
                {
                    entry.PointsAdjusted = entry.Objective.FixedReward();
                }
            }

            entry.AppUser.Balance += entry.PointsAdjusted;

            await UserManager.UpdateAsync(entry.AppUser);

            entry.Date = DateTime.Now;

            userRepository.InsertBalanceEntry(entry);
            unitOfWork.Save();
        }
        public async Task EditBalanceEntry(BalanceEntry entry, int objectiveId)
        {
            BalanceEntry updatedEntry = userRepository.GetBalanceEntryByID(entry.BalanceEntryID);

            int pointsChange = 0;

            if (objectiveId == 0)
            {
                pointsChange = entry.PointsAdjusted - updatedEntry.PointsAdjusted;
            }
            else
            {
                if (updatedEntry.Objective == null)
                {
                    updatedEntry.AddObjective(objectiveRepository.GetObjectiveByID(objectiveId));
                }
                else if (updatedEntry.Objective.ObjectiveID != objectiveId)
                {
                    updatedEntry.AddObjective(objectiveRepository.GetObjectiveByID(objectiveId));
                }

                pointsChange = updatedEntry.Objective.FixedReward() - updatedEntry.PointsAdjusted;
            }

            updatedEntry.AppUser.Balance += pointsChange;

            await UserManager.UpdateAsync(updatedEntry.AppUser);

            updatedEntry.PointsAdjusted = entry.PointsAdjusted;
            updatedEntry.Notes = entry.Notes;

            userRepository.UpdateBalanceEntry(updatedEntry);
            unitOfWork.Save();
        }
        public async Task DeleteBalanceEntry(int id)
        {
            BalanceEntry entry = userRepository.GetBalanceEntryByID(id);

            entry.AppUser.Balance -= entry.PointsAdjusted;

            await UserManager.UpdateAsync(entry.AppUser);

            userRepository.DeleteBalanceEntry(id);

            unitOfWork.Save();
        }

        public void CreateClaimedProductKey(ClaimedProductKey key, string nickname)
        {
            if (key.ListingID != 0)
            {
                key.Listing = listingRepository.GetListingByID(key.ListingID);
            }

            if (String.IsNullOrEmpty(nickname) == false)
            {
                key.AppUser = userRepository.GetAppUsers().Where(a => object.Equals(nickname.ToUpper(), a.UserName.ToUpper())).SingleOrDefault();
            }

            if (key.AppUser == null)
            {
                return;
            }

            key.Date = DateTime.Now;

            userRepository.InsertClaimedProductKey(key);
            unitOfWork.Save();
        }
        public void CreateClaimedProductKey(ClaimedProductKey key)
        {
            if (key.ListingID != 0)
            {
                key.Listing = listingRepository.GetListingByID(key.ListingID);
            }

            key.Date = DateTime.Now;

            userRepository.InsertClaimedProductKey(key);
            unitOfWork.Save();
        }
        public void EditClaimedProductKey(ClaimedProductKey key)
        {
            ClaimedProductKey updatedKey = userRepository.GetClaimedProductKeyByID(key.ClaimedProductKeyID);

            updatedKey.AcquisitionTitle = key.AcquisitionTitle;
            updatedKey.ListingID = key.ListingID;
            updatedKey.IsGift = key.IsGift;
            updatedKey.Key = key.Key;

            userRepository.UpdateClaimedProductKey(updatedKey);
            unitOfWork.Save();
        }
        public void DeleteClaimedProductKey(int id)
        {
            userRepository.DeleteClaimedProductKey(id);
            unitOfWork.Save();
        }

        public async Task CreateOrder(Order order, bool alreadyCharged, bool useDBKey = false)
        {
            // New orders should only have one product order entry
            ProductOrderEntry entry = order.ProductOrderEntries.FirstOrDefault();

            if (entry.ListingID != 0)
            {
                entry.Listing = listingRepository.GetListingByID(entry.ListingID);
            }

            DateTime date = DateTime.Now;
            order.SaleDate = date;

            String note = "Admin-created order";

            ClaimedProductKey newKey = new ClaimedProductKey();

            int priceToCharge = 0;

            if (order.ProductOrderEntries.Count() > 0)
            {
                priceToCharge = order.ProductOrderEntries.First().SalePrice;
            }

            if (useDBKey)
            {
                ProductKey key = GetProductKey(entry.ListingID);
                newKey = new ClaimedProductKey(key, order.AppUser, date, note);
                listingRepository.DeleteProductKey(key.ProductKeyID);
                unitOfWork.Save();

                priceToCharge = newKey.Listing.SaleOrDefaultPrice();

                entry.AddClaimedProductKey(newKey);//userRepository.InsertClaimedProductKey(newKey);
            }

            if (alreadyCharged == false)
            {
                BalanceEntry balanceEntry = new BalanceEntry();
                balanceEntry.Date = date;
                balanceEntry.AppUser = order.AppUser;
                balanceEntry.Notes = note;
                balanceEntry.PointsAdjusted = priceToCharge;
                order.AppUser.Balance -= priceToCharge;

                //userRepository.UpdateAppUser(order.AppUser);
                //userRepository.InsertBalanceEntry(balanceEntry);
                order.AppUser.AddBalanceEntry(balanceEntry);
            }

            entry.SalePrice = priceToCharge;
            order.AppUser.AddOrder(order);
            //userRepository.InsertOrder(order);
            await userRepository.UpdateAppUser(order.AppUser);
            unitOfWork.Save();
        }
        public void EditOrder(Order order)
        {
            Order updatedOrder = userRepository.GetOrderByID(order.OrderID);

            updatedOrder.SaleDate = order.SaleDate;

            userRepository.UpdateOrder(updatedOrder);
            unitOfWork.Save();
        }
        public async Task DeleteOrder(int id)
        {
            Order order = userRepository.GetOrderByID(id);

            // order has cascade delete on product order entries

            if (order.BalanceEntryID != 0)
            {
                await DeleteBalanceEntry(order.BalanceEntryID);
            }
            else
            {
                order.AppUser.CreateBalanceEntry("Refunded/Deleted order", order.TotalSalePrice(), DateTime.Now);
                await UserManager.UpdateAsync(order.AppUser);
            }

            userRepository.DeleteOrder(id);
            unitOfWork.Save();
        }

        public void EditProductOrderEntry(ProductOrderEntry orderEntry)
        {
            ProductOrderEntry updatedEntry = userRepository.GetProductOrderEntryByID(orderEntry.ProductOrderEntryID);

            if (orderEntry.ListingID != 0)
            {
                updatedEntry.ListingID = orderEntry.ListingID;
            }

            updatedEntry.SalePrice = orderEntry.SalePrice;

            userRepository.UpdateProductOrderEntry(updatedEntry);
            unitOfWork.Save();
        }
        public async Task DeleteProductOrderEntry(int id)
        {
            ProductOrderEntry entry = userRepository.GetProductOrderEntryByID(id);

            ClaimedProductKey[] keys = entry.ClaimedProductKeys.ToArray();
            for (int i = 0; i < entry.ClaimedProductKeys.Count; i++)
            {
                userRepository.DeleteClaimedProductKey(keys[i].ClaimedProductKeyID);
            }

            if (entry.SalePrice != 0)
            {
                entry.Order.AppUser.CreateBalanceEntry("Refunded/Deleted a partial order", entry.SalePrice, DateTime.Now);
                await UserManager.UpdateAsync(entry.Order.AppUser);
            }

            userRepository.DeleteProductOrderEntry(id);
            unitOfWork.Save();
        }
        public async Task RestockProductOrderEntry(int id)
        {
            ProductOrderEntry entry = userRepository.GetProductOrderEntryByID(id);

            if (entry.ClaimedProductKeys.Any(c => c.IsRevealed == true))
            {
                return;
            }

            ClaimedProductKey[] keys = entry.ClaimedProductKeys.ToArray();
            for (int i = 0; i < entry.ClaimedProductKeys.Count; i++)
            {
                keys[i].Listing.AddProductKeyAndUpdateQuantity(new ProductKey(keys[i].IsGift, keys[i].Key));
                listingRepository.UpdateListing(keys[i].Listing);
                userRepository.DeleteClaimedProductKey(keys[i].ClaimedProductKeyID);
            }

            if (entry.SalePrice != 0)
            {
                entry.Order.AppUser.CreateBalanceEntry("Refunded/Deleted a partial order", entry.SalePrice, DateTime.Now);
                await UserManager.UpdateAsync(entry.Order.AppUser);
            }

            userRepository.DeleteProductOrderEntry(id);
            unitOfWork.Save();
        }
        public void PullNewProductKey(int id)
        {
            ProductOrderEntry entry = userRepository.GetProductOrderEntryByID(id);

            if (entry.Listing.Quantity == 0)
            {
                return;
            }

            ClaimedProductKey[] keys = entry.ClaimedProductKeys.ToArray();

            if (entry.Listing.ProductKeys.Count > 0)
            {
                ProductKey key = entry.Listing.ProductKeys.First();                

                if (keys.Count() > 1)
                {
                    for (int i = 0; i < keys.Count(); i++)
                    {
                        userRepository.DeleteClaimedProductKey(keys[i].ClaimedProductKeyID);
                    }

                    entry.AddClaimedProductKey(new ClaimedProductKey(key, entry.Order.AppUser, DateTime.Now, "Admin pulled new key"));
                }
                else
                {
                    keys[0].Key = key.ItemKey;
                    keys[0].IsGift = key.IsGift;
                }

                Listing listing = key.Listing;
                key.Listing.RemoveProductKeyAndUpdateQuantity(key);
                listingRepository.DeleteProductKey(key.ProductKeyID);
                listingRepository.UpdateListing(listing);
            }
            else if (entry.Listing.ChildListings.All(l => l.ProductKeys.Count > 0))
            {
                for (int i = 0; i < keys.Count(); i++)
                {
                    userRepository.DeleteClaimedProductKey(keys[i].ClaimedProductKeyID);
                }

                foreach (Listing childListing in entry.Listing.ChildListings)
                {
                    ProductKey key = childListing.ProductKeys.First();

                    userRepository.InsertClaimedProductKey(new ClaimedProductKey(key, entry.Order.AppUser, DateTime.Now, "Admin pulled new key"));

                    Listing listing = key.Listing;
                    key.Listing.RemoveProductKeyAndUpdateQuantity(key);
                    listingRepository.DeleteProductKey(key.ProductKeyID);
                    listingRepository.UpdateListing(listing);
                }
            }
            
            userRepository.UpdateProductOrderEntry(entry);
            unitOfWork.Save();
        }
        #endregion

        public async Task EditAppUserSettings(AppUser appUser)
        {
            AppUser updatedUser = UserManager.FindById(appUser.Id);

            if (updatedUser == null)
            {
                return;
            }

            updatedUser.TimeZoneID = appUser.TimeZoneID;
            updatedUser.PaginationPreference = appUser.PaginationPreference;

            await UserManager.UpdateAsync(updatedUser);

            unitOfWork.Save();
        }

        public async Task<int> AddWishlistItems(string appIds)
        {
            string[] apps = appIds.Split(new char[] { ',' });

            if (apps.Count() != 0)
            {
                List<WishlistEntry> wishlistEntries = userRepository.GetWishlistEntries().ToList();

                foreach (WishlistEntry entry in wishlistEntries)
                {
                    if (!apps.Any(x => entry.AppID.ToString().CompareTo(x) == 0))
                    {
                        userRepository.DeleteWishlistEntry(entry.WishlistEntryID);
                    }
                }

                int numAdded = 0;
                int appId = 0;
                AppUser user = await GetCurrentUser();

                foreach (String app in apps)
                { 
                    Int32.TryParse(app, out appId);
                    
                    if (appId != 0)
                    {
                        if (!wishlistEntries.Any(w => w.AppID == appId))
                        {
                            userRepository.InsertWishlistEntry(new WishlistEntry(user, appId));
                        }
                        numAdded++;
                    }

                    appId = 0;
                }

                unitOfWork.Save();

                return numAdded;
            }

            return 0;
        }

        public async Task<List<ActivityFeedContainer>> GetPublicActivityFeedItems(AppUser user)
        {
            List<ActivityFeedContainer> activityFeed = new List<ActivityFeedContainer>();

            AppUser loggedInUser = await GetCurrentUserWithActivityProperties();

            TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(loggedInUser.TimeZoneID);

            if (user.CreatedGiveaways != null)
            {
                foreach (Giveaway entry in user.CreatedGiveaways)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.CreatedTime, info)));
                }
            }

            if (user.GiveawayEntries != null)
            {
                foreach (Giveaway entry in user.WonGiveaways)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.EndDate, info)));
                }
            }

            if (user.Auctions != null)
            {
                foreach (Auction entry in user.Auctions)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.CreatedTime, info)));
                }
            }

            if (user.AuctionBids != null)
            {
                foreach (Auction entry in user.WonAuctions)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.EndTime, info)));
                }
            }

            if (user.ProductReviews != null)
            {
                foreach (ProductReview entry in user.ProductReviews)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.PostDate, info)));
                }
            }

            if (user.Orders != null)
            {
                foreach (Order order in user.Orders)
                {
                    foreach (ProductOrderEntry entry in order.ProductOrderEntries)
                    {
                        activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime((DateTime)order.SaleDate, info)));
                    }
                }
            }

            if (user.BalanceEntries != null)
            {
                foreach (BalanceEntry entry in user.BalanceEntries.Where(b => b.PointsAdjusted > 0))
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.Date, info)));
                }
            }

            return activityFeed.OrderByDescending(a => a.ItemDate).ToList();
        }

        // Balances with negative adjustments are mainly caused by purchases, this does mean transfers will be omitted, however
        public async Task<List<ActivityFeedContainer>> GetActivityFeedItems(bool includeNegativeBalanceEntries = false)
        {
            AppUser user = await GetCurrentUserWithActivityProperties();

            TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneID);

            List<ActivityFeedContainer> activityFeed = new List<ActivityFeedContainer>();

            IEnumerable<Order> orders = await GetUserOrders();

            if (orders != null)
            {
                foreach (Order order in orders)
                {
                    activityFeed.Add(new ActivityFeedContainer(order, ConvertDateTime(((DateTime)order.SaleDate), info)));
                }
            }

            if (user.GiveawayEntries != null)
            {
                foreach (GiveawayEntry entry in user.GiveawayEntries)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.EntryDate, info)));
                }
            }

            if (user.CreatedGiveaways != null)
            {
                foreach (Giveaway entry in user.CreatedGiveaways)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.CreatedTime, info)));
                }
            }

            if (user.AuctionBids != null)
            {
                foreach (AuctionBid entry in user.AuctionBids)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.BidDate, info)));
                }
            }

            if (user.Auctions != null)
            {
                foreach (Auction entry in user.Auctions)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.CreatedTime, info)));
                }
                foreach (Auction entry in user.WonAuctions)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.EndTime, info)));
                }
            }

            if (user.BalanceEntries != null)
            {
                if (includeNegativeBalanceEntries)
                {
                    foreach (BalanceEntry entry in user.BalanceEntries)
                    {
                        activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.Date, info)));
                    }
                }
                else
                {
                    foreach (BalanceEntry entry in user.BalanceEntries.Where(b => b.PointsAdjusted > 0))
                    {
                        activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.Date, info)));
                    }
                }
            }

            if (user.WonPrizes != null)
            {
                foreach (WonPrize entry in user.WonPrizes)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.TimeWon, info)));
                }
            }

            if (user.ProductReviews != null)
            {
                foreach (ProductReview entry in user.ProductReviews)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry, ConvertDateTime(entry.PostDate, info)));
                }
            }

            return activityFeed.OrderByDescending(a => a.ItemDate).ToList();
        }

        public string RevealKey(int keyId)
        {
            ClaimedProductKey key = userRepository.GetClaimedProductKeyByID(keyId);

            key.IsRevealed = true;

            userRepository.UpdateClaimedProductKey(key);
            this.unitOfWork.Save();

            return key.Key;
        }
        public bool MarkKeyUsed(int keyId)
        {
            ClaimedProductKey key = userRepository.GetClaimedProductKeyByID(keyId);

            key.IsUsed = !key.IsUsed;

            userRepository.UpdateClaimedProductKey(key);
            this.unitOfWork.Save();

            return key.IsUsed;
        }

        public async Task BuildUser(AppUser user, string apiKey)
        {
            UnitOfWork tempUOW = new UnitOfWork(AppIdentityDbContext.Create());
            AppUserManager tempUM = new AppUserManager(new UserStore<AppUser>(tempUOW.DbContext));

            user = await tempUM.FindByIdAsyncWithStoreFilters(user.Id);

            if (user.UserSteamID == 0)
            {
                // possibly add default data here
                return;
            }

            string playerURL = String.Format("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={0}&steamids={1}&format=json", apiKey, user.UserSteamID);

            string result = new System.Net.WebClient().DownloadString(playerURL);

            JObject playerData = JObject.Parse(result);

            if (playerData["response"] != null && playerData["response"]["players"] != null)
            {
                user.LargeAvatar = (string)playerData["response"]["players"][0]["avatarfull"] ?? "";
                user.MediumAvatar = (string)playerData["response"]["players"][0]["avatarmedium"] ?? "";
                user.SmallAvatar = (string)playerData["response"]["players"][0]["avatar"] ?? "";
            }

            string gamesURL = String.Format("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={0}&steamid={1}&format=json", apiKey, user.UserSteamID);

            result = new System.Net.WebClient().DownloadString(gamesURL);

            JObject gameData = JObject.Parse(result);

            if (gameData["response"] != null && gameData["response"]["games"] != null)
            {
                JArray jGames = (JArray)gameData["response"]["games"];
                for (int i = 0; i < jGames.Count; i++)
                {
                    int appId = (int)jGames[i]["appid"];
                    if (user.OwnedGames.Any(o => o.AppID == appId) == false)
                    {
                        user.AddOwnedGame(new OwnedGame((int)jGames[i]["appid"], (int)jGames[i]["playtime_forever"]));
                    }
                }
            }

            tempUM.Update(user);
            tempUOW.Save();
            tempUOW.Dispose();
        }

        public bool AddBalances(string input)
        {
            /*
                SAMPLE:

                Game Night 02-24
                Bob     10
                Fred    8
                George  8
                Objective "Beat Me at Chess"
                Fred    5
            
                Each balance entry should use the plain text note preceding it as a note
                Convention is to use a tab delimiter for balance entries and no tab in the note
            */

            bool fullSuccess = true;

            string currentNote = "";
            DateTime currentTime = DateTime.Now;

            input = input.Replace("\r\n", "\r");

            List<String> balanceEntries = input.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            Regex userBalance = new Regex("^(-?[^\t]+)\t+([0-9]+)$");
            Regex note = new Regex("^([^\t]+)$");

            for (int i = 0; i < balanceEntries.Count; i++)
            {
                if (note.Match(balanceEntries[i]).Success)
                {
                    currentNote = note.Match(balanceEntries[i]).Groups[1].Value;
                }
                else if (userBalance.Match(balanceEntries[i]).Success)
                {
                    Match userBalanceMatch = userBalance.Match(balanceEntries[i]);
                    AppUser user = GetRequestedUser(userBalanceMatch.Groups[1].Value.Trim(), true);

                    int points = Int32.Parse(userBalanceMatch.Groups[2].Value, System.Globalization.NumberStyles.AllowLeadingSign);

                    user.CreateBalanceEntry(currentNote, points, currentTime);
                    UserManager.Update(user);
                }
                else
                {
                    fullSuccess = false;
                }
            }

            // If all rows have successfully been parsed, then persist!
            if (fullSuccess)
            {
                unitOfWork.Save();
            }

            return fullSuccess;
        }

        public DateTime ConvertDateTime(DateTime time, TimeZoneInfo info)
        {
            DateTime utcTime = new DateTime(time.Ticks, DateTimeKind.Utc);

            DateTime convertedTime = TimeZoneInfo.ConvertTime(utcTime, info);

            return convertedTime;
        }

        public bool IsInRole(AppUser user, string role)
        {
            return UserManager.IsInRole(user.Id, role);
        }

        public void AddGiveaway(Giveaway giveaway)
        {
            throw new NotImplementedException();

            // Check Validity?
            // Add to Giveaways
            // Schedule drawing?
        }

        public void DrawGiveawayWinners(int giveawayID)
        {
            throw new NotImplementedException();

            // Check if it's valid to draw winner (closed, has entries.)
            // Draw winners randomly and assign to Giveaway.Winners
            
        }

        // --- GC and User logic

        public void Dispose()
        {
            this.unitOfWork.Dispose();
        }

        public AppUser GetCurrentUserSynch()
        {
            return UserManager.FindByName(userName);
        }

        private async Task<AppUser> GetCurrentUserWithActivityProperties()
        {
            return await UserManager.FindByNameAsyncWithActivityProperties(userName);
        }

        private async Task<AppUser> GetCurrentUserNoProperties()
        {
            return await UserManager.FindByNameAsync(userName);
        }

        private async Task<AppUser> GetCurrentUserWithBlacklist()
        {
            return await UserManager.FindByNameAsyncWithBlacklist(userName);
        }

        private async Task<AppUser> GetCurrentUserWithAuctions()
        {
            return await UserManager.FindByNameAsyncWithAuctions(userName);
        }

        public async Task<AppUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsyncWithCartAndOpenAuctionBids(userName);
        }
    }
}
