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
using Newtonsoft.Json.Linq;
using System.Data.Entity;

namespace TheAfterParty.Domain.Services
{
    public static class HangfireJobService
    {
        private static int randBase = 10;
        private static int randOffset = 15;

        public static void CloseAuction(int auctionId)
        {
            using (AppIdentityDbContext context = AppIdentityDbContext.Create())
            {
                IUnitOfWork unitOfWork = new UnitOfWork(context);
                IAuctionRepository auctionRepository = new AuctionRepository(unitOfWork);
                IListingRepository listingRepository = new ListingRepository(unitOfWork);
                IUserRepository userRepository = new UserRepository(unitOfWork);
                IAuctionService auctionService = new AuctionService(auctionRepository, userRepository, listingRepository, unitOfWork);

                auctionService.DrawAuctionWinners(auctionId);

                auctionService.Dispose();
                auctionRepository.Dispose();
                listingRepository.Dispose();
                userRepository.Dispose();
                unitOfWork.Dispose();
            }
        }

        public static void CloseGiveaway(int giveawayId)
        {
            using (AppIdentityDbContext context = AppIdentityDbContext.Create())
            {
                IUnitOfWork unitOfWork = new UnitOfWork(context);
                IGiveawayRepository giveawayRepository = new GiveawayRepository(unitOfWork);
                IListingRepository listingRepository = new ListingRepository(unitOfWork);
                IUserRepository userRepository = new UserRepository(unitOfWork);
                IUserService userService = new UserService(listingRepository, userRepository, null, giveawayRepository, null, null, unitOfWork);

                userService.DrawGiveawayWinners(giveawayId);

                userService.Dispose();
                giveawayRepository.Dispose();
                listingRepository.Dispose();
                userRepository.Dispose();
                unitOfWork.Dispose();
            }
        }

        public static void UpdateUserOwnedGames(string apiKey)
        {
            AppIdentityDbContext context;

            int day = (int)DateTime.Now.DayOfWeek;

            using (context = AppIdentityDbContext.Create())
            {
                IUnitOfWork unitOfWork = new UnitOfWork(context);
                IUserRepository userRepository = new UserRepository(unitOfWork);
                AppUserManager UserManager = new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext));

                int usersPerDay = (int)Math.Ceiling((double)UserManager.Users.Count() / 7);

                IList<AppUser> users = UserManager.Users.OrderBy(u => u.Id).Skip(day * usersPerDay).Take(usersPerDay).ToList();

                foreach (AppUser userHolder in users)
                {
                    if (UserManager.IsInRole(userHolder.Id, "Member") == false && UserManager.IsInRole(userHolder.Id, "Admin") == false)
                    {
                        continue;
                    }

                    // we've already updated this user today, the job was suspended when the site was unloaded and is rerunning to completion, thus we ignore users already updated
                    if (userHolder.LastUpdated.DayOfYear == DateTime.Now.DayOfYear)
                    {
                        continue;
                    }

                    AppUser user = UserManager.Users.Include(u => u.OwnedGames).SingleOrDefault(x => userHolder.Id == x.Id);

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

                    user.LastUpdated = DateTime.Now;
                    UserManager.Update(user);
                    unitOfWork.Save();
                }
            }
        }

        public static DateTime GetMidnightEST()
        {
        //    DateTime utcTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        //    TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        //    DateTime convertedTime = TimeZoneInfo.ConvertTime(utcTime, info);

        //    return convertedTime;

            return System.TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, System.TimeZoneInfo.Utc.Id, "Eastern Standard Time").Date;
        }

        public static void RolloverDailyDeal()
        {
            AppIdentityDbContext context; 

            using (context = AppIdentityDbContext.Create())
            {
                IUnitOfWork unitOfWork = new UnitOfWork(context);
                IListingRepository listingRepository = new ListingRepository(unitOfWork);

                DiscountedListing discountedListing = listingRepository.GetDiscountedListings().Where(d => d.DailyDeal).FirstOrDefault();

                if (discountedListing != null)
                {
                    listingRepository.DeleteDiscountedListing(discountedListing.DiscountedListingID);
                }

                List<DiscountedListing> expiredListings = listingRepository.GetDiscountedListings().Where(d => d.IsLive() == false).ToList();

                if (expiredListings != null)
                {
                    foreach (DiscountedListing listing in expiredListings)
                    {
                        listingRepository.DeleteDiscountedListing(listing.DiscountedListingID);
                    }
                }

                DiscountedListing newDiscountedListing = new DiscountedListing();
                
                Listing randomListing = listingRepository.GetListings().Where(l => l.Quantity > 0 && l.ListingPrice > 1).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                if (randomListing == null)
                {
                    unitOfWork.Save();
                    return;
                }

                SelectDealPercent(newDiscountedListing);

                DateTime expiry = GetMidnightEST().AddDays(1);

                newDiscountedListing.DailyDeal = true;
                newDiscountedListing.ItemSaleExpiry = expiry;

                randomListing.AddDiscountedListing(newDiscountedListing);

                listingRepository.UpdateListing(randomListing);

                ISiteRepository siteRepository = new SiteRepository(unitOfWork);

                String urlAndName = String.Empty;

                if (String.IsNullOrWhiteSpace(randomListing.GetQualifiedSteamStorePageURL()) == false)
                {
                    urlAndName = "[url=" + randomListing.GetQualifiedSteamStorePageURL() + "]" + randomListing.ListingName + "[/url]";
                }
                else
                {
                    urlAndName = randomListing.ListingName;
                }

                SiteNotification notification = new SiteNotification() { Notification = "[daily][/daily] Today's [url=https://theafterparty.azurewebsites.net/store/deals/daily]daily deal[/url] is " + urlAndName + ", now on sale for [gtext]" + randomListing.SaleOrDefaultPrice() + "[/gtext] " + randomListing.GetPluralizedSalePriceUnit() + "!", NotificationDate = DateTime.Now };
                siteRepository.InsertSiteNotification(notification);

                unitOfWork.Save();
            }                
        }

        public static void RolloverWeeklyDeals()
        {
            AppIdentityDbContext context;

            using (context = AppIdentityDbContext.Create())
            {
                IUnitOfWork unitOfWork = new UnitOfWork(context);
                IListingRepository listingRepository = new ListingRepository(unitOfWork);
                
                List<DiscountedListing> oldWeeklies = listingRepository.GetDiscountedListings().Where(d => d.IsLive() == false).ToList();

                if (oldWeeklies != null && oldWeeklies.Count > 0)
                {
                    foreach (DiscountedListing oldListing in oldWeeklies)
                    {
                        listingRepository.DeleteDiscountedListing(oldListing.DiscountedListingID);
                    }
                }

                Random rand = new Random();

                int numDeals = rand.Next(randBase) + randOffset;

                List<Listing> newDeals = listingRepository.GetListings().Where(l => l.Quantity > 0 && l.ListingPrice > 1).OrderBy(x => Guid.NewGuid()).Take(numDeals).ToList();

                if (newDeals == null || newDeals.Count == 0)
                {
                    unitOfWork.Save();
                    return;
                }

                DateTime dealExpiry = GetMidnightEST().AddDays(7);

                foreach (Listing listing in newDeals)
                {
                    DiscountedListing newDiscountedListing = new DiscountedListing();

                    SelectDealPercent(newDiscountedListing);

                    newDiscountedListing.WeeklyDeal = true;
                    newDiscountedListing.ItemSaleExpiry = dealExpiry;

                    listing.AddDiscountedListing(newDiscountedListing);

                    listingRepository.UpdateListing(listing);
                }

                ISiteRepository siteRepository = new SiteRepository(unitOfWork);

                SiteNotification notification = new SiteNotification() { Notification = "[weekly][/weekly] There are [gtext]" + numDeals + "[/gtext] new [url=https://theafterparty.azurewebsites.net/store/deals/weekly]deals[/url] available this week!", NotificationDate = DateTime.Now };
                siteRepository.InsertSiteNotification(notification);

                unitOfWork.Save();
            }
        }

        public static void RolloverDailyBoosted()
        {
            AppIdentityDbContext context;

            using (context = AppIdentityDbContext.Create())
            {
                IUnitOfWork unitOfWork = new UnitOfWork(context);
                IObjectiveRepository objectiveRepository = new ObjectiveRepository(unitOfWork);

                List<BoostedObjective> boostedObjectives = objectiveRepository.GetBoostedObjectives().Where(b => b.IsLive() == false).ToList();

                foreach (BoostedObjective objective in boostedObjectives)
                {
                    objectiveRepository.DeleteBoostedObjective(objective.BoostedObjectiveID);
                }

                BoostedObjective newBoostedObjective = new BoostedObjective();

                // grab an active objective that has a product associated it (objectives without products are special and shouldn't be boosted via this method)
                Objective randomObjective = objectiveRepository.GetObjectives().Where(l => l.IsActive && l.Product != null && l.RequiresAdmin == false).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                if (randomObjective == null)
                {
                    unitOfWork.Save();
                    return;
                }

                SelectBoostAmount(newBoostedObjective);

                DateTime endDate = GetMidnightEST().AddDays(1);

                newBoostedObjective.EndDate = endDate;
                newBoostedObjective.IsDaily = true;
                
                randomObjective.AddBoostedObjective(newBoostedObjective);

                objectiveRepository.UpdateObjective(randomObjective);
                
                ISiteRepository siteRepository = new SiteRepository(unitOfWork);

                String urlAndName = String.Empty;

                if (randomObjective.Product.IsSteamAppID)
                {
                    urlAndName = "[url=https://store.steampowered.com/app/" + randomObjective.Product.AppID + "]" + randomObjective.Title + "[/url]";
                }
                else
                {
                    urlAndName = randomObjective.Title;
                }

                SiteNotification notification = new SiteNotification() { Notification = "[boosted][/boosted] Objective [ptext]\"" + randomObjective.ObjectiveName + "\"[/ptext] for " + randomObjective.Title + " is [url=https://theafterparty.azurewebsites.net/objectives/boosted]boosted[/url] by [gtext]" + randomObjective.BoostedObjective.BoostAmount + "x[/gtext] for a boosted award of [gtext]" + randomObjective.FixedReward() + "[/gtext]!", NotificationDate = DateTime.Now };
                siteRepository.InsertSiteNotification(notification);

                unitOfWork.Save();
            }
        }

        private static void SelectDealPercent(DiscountedListing discountedListing)
        {
            Random rand = new Random();

            int ran = rand.Next(100);

            if (ran < 40)
            {
                discountedListing.ItemDiscountPercent = 33;
            }
            else if (ran < 65)
            {
                discountedListing.ItemDiscountPercent = 50;
            }
            else if (ran < 85)
            {
                discountedListing.ItemDiscountPercent = 66;
            }
            else if (ran < 99)
            {
                discountedListing.ItemDiscountPercent = 75;
            }
            else
            {
                discountedListing.ItemDiscountPercent = 85;
            }
        }

        private static void SelectBoostAmount(BoostedObjective boostedObjective)
        {
            Random rand = new Random();

            int ran = rand.Next(100);

            if (ran < 50)
            {
                boostedObjective.BoostAmount = 1.5;
            }
            else if (ran < 75)
            {
                boostedObjective.BoostAmount = 2.0;
            }
            else if (ran < 90)
            {
                boostedObjective.BoostAmount = 2.5;
            }
            else if (ran < 99)
            {
                boostedObjective.BoostAmount = 3;
            }
            else
            {
                boostedObjective.BoostAmount = 5;
            }
        }
    }
}
