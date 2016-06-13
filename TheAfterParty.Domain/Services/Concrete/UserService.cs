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
        }
        protected UserService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
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
            AppUser user = await GetCurrentUser();

            return user.GetSilentAuctionReservedBalance();
        }

        public async Task<int> GetPublicAuctionReservedBalance()
        {
            AppUser user = await GetCurrentUser();

            return user.GetPublicAuctionReservedBalance();
        }

        public async Task<int> GetCartTotal()
        {
            AppUser user = await GetCurrentUser();

            return user.GetCartTotal();
        }

        public async Task AddBlacklistEntry(int listingId)
        {
            AppUser user = await GetCurrentUser();

            user.AddListingBlacklistEntry(listingRepository.GetListingByID(listingId));
            UserManager.Update(user);
            unitOfWork.Save();
        }

        public async Task TransferPoints(int points, string userId)
        {
            AppUser donor = await GetCurrentUser();
            AppUser recipient = await UserManager.FindByIdAsync(userId);

            if (donor.Balance - donor.ReservedBalance() > points)
            {
                donor.CreateBalanceEntry("Transfer of points to " + recipient.UserName, 0 - points, DateTime.Now);
                recipient.CreateBalanceEntry("Gift of points from " + donor.UserName, points, DateTime.Now);
                UserManager.Update(donor);
                UserManager.Update(recipient);
                unitOfWork.Save();
            }
        }

        public async Task CreateAppUser(AppUser appUser, string roleToAdd, string apiKey)
        {
            BuildUser(appUser, apiKey);
            appUser.MemberSince = DateTime.Now;

            await UserManager.CreateAsync(appUser);

            if (String.IsNullOrEmpty(roleToAdd) == false)
            {
                if (roleToAdd.Contains(","))
                {
                    string[] rolesToAdd = roleToAdd.Split(new char[] { ',' });

                    foreach (string role in rolesToAdd)
                    {
                        await UserManager.AddToRoleAsync(appUser.Id, roleToAdd);
                    }
                }
                else
                {
                    await UserManager.AddToRoleAsync(appUser.Id, roleToAdd);
                }
            }
        }

        public async Task EditAppUser(AppUser appUser, string roleToAdd, string roleToRemove)
        {
            await UserManager.UpdateAsync(appUser);

            if (String.IsNullOrEmpty(roleToAdd) == false)
            {
                if (roleToAdd.Contains(","))
                {
                    string[] rolesToAdd = roleToAdd.Split(new char[] { ',' });

                    foreach (string role in rolesToAdd)
                    {
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

        public void CreateBalanceEntry(BalanceEntry entry, int objectiveId, string nickname)
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

            entry.Date = DateTime.Now;

            userRepository.InsertBalanceEntry(entry);
            unitOfWork.Save();
        }

        public void EditBalanceEntry(BalanceEntry entry)
        {
            userRepository.UpdateBalanceEntry(entry);
            unitOfWork.Save();
        }

        public BalanceEntry GetBalanceEntryByID(int id)
        {
            return userRepository.GetBalanceEntryByID(id);
        }

        public void DeleteBalanceEntry(int id)
        {
            BalanceEntry entry = userRepository.GetBalanceEntryByID(id);

            entry.AppUser.Balance -= entry.PointsAdjusted;

            UserManager.UpdateAsync(entry.AppUser);

            userRepository.DeleteBalanceEntry(id);

            unitOfWork.Save();
        }

        public ICollection<BalanceEntry> GetBalanceEntries()
        {
            return userRepository.GetBalanceEntries().ToList();
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

        public void EditClaimedProductKey(ClaimedProductKey key)
        {
            userRepository.UpdateClaimedProductKey(key);
            unitOfWork.Save();
        }

        public ClaimedProductKey GetClaimedProductKeyByID(int id)
        {
            return userRepository.GetClaimedProductKeyByID(id);
        }

        public void DeleteClaimedProductKey(int id)
        {
            userRepository.DeleteClaimedProductKey(id);
            unitOfWork.Save();
        }

        public ICollection<ClaimedProductKey> GetClaimedProductKeys()
        {
            return userRepository.GetClaimedProductKeys().ToList();
        }

        public void CreateOrder(Order order, string nickname, ProductOrderEntry entry, bool alreadyCharged)
        {
            if (String.IsNullOrEmpty(nickname) == false)
            {
                order.AppUser = userRepository.GetAppUsers().Where(a => object.Equals(nickname.ToUpper(), a.Nickname.ToUpper())).SingleOrDefault();
            }

            if (entry.ListingID != 0)
            {
                entry.Listing = listingRepository.GetListingByID(entry.ListingID);
            }

            order.ProductOrderEntries = new List<ProductOrderEntry>() { entry };
            order.SaleDate = DateTime.Now;

            if (alreadyCharged == false)
            {
                BalanceEntry balanceEntry = new BalanceEntry();
                balanceEntry.Date = DateTime.Now;
                balanceEntry.AppUser = order.AppUser;
                balanceEntry.Notes = "Admin-created order";
                balanceEntry.PointsAdjusted = order.TotalSalePrice();

                userRepository.InsertBalanceEntry(balanceEntry);
            }

            userRepository.InsertOrder(order);
            unitOfWork.Save();
        }

        public void EditOrder(Order order)
        {
            userRepository.UpdateOrder(order);
            unitOfWork.Save();
        }

        public Order GetOrderByID(int id)
        {
            return userRepository.GetOrderByID(id);
        }

        public void DeleteOrder(int id)
        {
            Order order = userRepository.GetOrderByID(id);

            // order has cascade delete on product order entries

            if (order.BalanceEntryID != 0)
            {
                DeleteBalanceEntry(order.BalanceEntryID);
            }
            else
            {
                order.AppUser.CreateBalanceEntry("Refunded/Deleted order", order.TotalSalePrice(), DateTime.Now);
                UserManager.UpdateAsync(order.AppUser);
            }

            userRepository.DeleteOrder(id);
            unitOfWork.Save();
        }

        public ICollection<Order> GetOrders()
        {
            return userRepository.GetOrders().ToList();
        }

        public void EditProductOrderEntry(ProductOrderEntry orderEntry)
        {
            userRepository.UpdateProductOrderEntry(orderEntry);
            unitOfWork.Save();
        }

        public void DeleteProductOrderEntry(int id)
        {
            ProductOrderEntry entry = userRepository.GetProductOrderEntryByID(id);

            userRepository.DeleteClaimedProductKey(entry.ClaimedProductKey.ClaimedProductKeyID);

            if (entry.SalePrice != 0)
            {
                entry.Order.AppUser.CreateBalanceEntry("Refunded/Deleted a partial order", entry.SalePrice, DateTime.Now);
                UserManager.UpdateAsync(entry.Order.AppUser);
            }

            userRepository.DeleteProductOrderEntry(id);
            unitOfWork.Save();
        }

        public ProductOrderEntry GetProductOrderEntryByID(int id)
        {
            return userRepository.GetProductOrderEntryByID(id);
        }

        public async Task<List<ActivityFeedContainer>> GetActivityFeedItems()
        {
            AppUser user = await GetCurrentUser();

            List<ActivityFeedContainer> activityFeed = new List<ActivityFeedContainer>();
            
            List<Order> orders = await GetUserOrders();

            if (orders != null)
            {
                foreach (Order order in orders)
                {
                    activityFeed.Add(new ActivityFeedContainer(order));
                }
            }

            if (user.GiveawayEntries != null)
            {
                foreach (GiveawayEntry entry in user.GiveawayEntries)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry));
                }
            }

            if (user.CreatedGiveaways != null)
            {
                foreach (Giveaway entry in user.CreatedGiveaways)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry));
                }
            }

            if (user.AuctionBids != null)
            {
                foreach (AuctionBid entry in user.AuctionBids)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry));
                }
            }

            if (user.Auctions != null)
            {
                foreach (Auction entry in user.Auctions)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry));
                }
            }

            if (user.BalanceEntries != null)
            {
                foreach (BalanceEntry entry in user.BalanceEntries)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry));
                }
            }

            if (user.WonPrizes != null)
            {
                foreach (WonPrize entry in user.WonPrizes)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry));
                }
            }

            if (user.ProductReviews != null)
            {
                foreach (ProductReview entry in user.ProductReviews)
                {
                    activityFeed.Add(new ActivityFeedContainer(entry));
                }
            }

            return activityFeed.OrderBy(a => a.ItemDate).ToList();
        }

        public void RevealKey(int keyId)
        {
            ClaimedProductKey key = userRepository.GetClaimedProductKeyByID(keyId);

            key.IsRevealed = true;

            userRepository.UpdateClaimedProductKey(key);
        }

        public void MarkKeyUsed(int keyId)
        {
            ClaimedProductKey key = userRepository.GetClaimedProductKeyByID(keyId);

            key.IsUsed = true;

            userRepository.UpdateClaimedProductKey(key);
        }

        public List<Auction> GetAuctions()
        {
            return auctionRepository.GetAuctions().ToList();
        }

        public List<Giveaway> GetGiveaways()
        {
            return giveawayRepository.GetGiveaways().ToList();
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

        public ICollection<AppUser> GetAllUsers()
        {
            return UserManager.Users.ToList();
        }

        public void BuildUser(AppUser user, string apiKey)
        {
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
                    user.AddOwnedGame(new OwnedGame((int)jGames[i]["appid"], (int)jGames[i]["playtime_forever"]));
                }
            }

                UserManager.Update(user);
            unitOfWork.Save();            
        }

        public async Task<AppUser> GetUserByID(string id)
        {
            return await userRepository.GetAppUserByID(id);
        }

        public List<AppUser> GetAdmins()
        {
            return userRepository.GetAppUsers().Where(au => UserManager.IsInRole(au.Id, "Admin")).ToList();
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

        public async Task<List<Order>> GetUserOrders()
        {
            AppUser user = await GetCurrentUser();

            return userRepository.GetOrders().Where(o => object.Equals(o.UserID, user.Id)).ToList();
        }

        public async Task<List<ClaimedProductKey>> GetKeys()
        {
            AppUser user = await GetCurrentUser();

            return userRepository.GetClaimedProductKeys().Where(o => object.Equals(o.UserID, user.Id)).ToList();
        }

        public async Task<List<WonPrize>> GetWonPrizes()
        {
            AppUser user = await GetCurrentUser();

            return prizeRepository.GetWonPrizes().Where(p => object.Equals(p.UserID, user.Id)).ToList();
        }

        public async Task<List<AuctionBid>> GetAuctionBids()
        {
            AppUser user = await GetCurrentUser();

            return auctionRepository.GetAuctionBids().Where(b => object.Equals(b.UserID, user.Id)).ToList();
        }

        public async Task<List<Auction>> GetCreatedAuctions()
        {
            AppUser user = await GetCurrentUser();

            return auctionRepository.GetAuctions().Where(a => object.Equals(a.CreatorID, user.Id)).ToList();
        }

        public async Task<List<GiveawayEntry>> GetGiveawayEntries()
        {
            AppUser user = await GetCurrentUser();

            return giveawayRepository.GetGiveawayEntries().Where(ge => object.Equals(ge.UserID, user.Id)).ToList();
        }

        public async Task<List<Giveaway>> GetCreatedGiveaways()
        {
            AppUser user = await GetCurrentUser();

            return giveawayRepository.GetGiveaways().Where(g => object.Equals(g.UserID, user.Id)).ToList();
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

        public async Task<AppUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsync(userName);
        }
    }
}
