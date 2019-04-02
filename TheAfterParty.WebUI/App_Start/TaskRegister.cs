using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Services;
using TheAfterParty.Domain.Entities;
using Hangfire;

namespace TheAfterParty.WebUI.App_Start
{
    public static class TaskRegister
    {
        public static void RegisterDailyTasks()
        {
            RecurringJob.AddOrUpdate("update-users-owned-games", () => HangfireJobService.UpdateUserOwnedGames(System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]), Cron.Daily(0, 4), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            RecurringJob.AddOrUpdate("daily-roll-over", () => HangfireJobService.RolloverDailyDeal(), Cron.Daily(0, 4), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            RecurringJob.AddOrUpdate("daily-obj-roll-over", () => HangfireJobService.RolloverDailyBoosted(), Cron.Daily(0, 4), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            RecurringJob.AddOrUpdate("weekly-roll-over", () => HangfireJobService.RolloverWeeklyDeals(), Cron.Weekly(DayOfWeek.Friday, 0, 4), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        }

        public static void RegisterAuctions()
        {
            IEnumerable<Auction> closedAuctions = new List<Auction>();

            using (AppIdentityDbContext context = AppIdentityDbContext.Create())
            //using (IUnitOfWork unitOfWork = new UnitOfWork(context))
            {
                IUnitOfWork unitOfWork = new UnitOfWork(context);
                IAuctionRepository auctionRepository = new AuctionRepository(unitOfWork);
                IListingRepository listingRepository = new ListingRepository(unitOfWork);
                IUserRepository userRepository = new UserRepository(unitOfWork);
                IAuctionService auctionService = new AuctionService(auctionRepository, userRepository, listingRepository, unitOfWork);
                
                //List<Auction> openAuctions = auctionService.GetAuctions().Where(a => a.IsOpen()).ToList();
                // get auctions that: are not open (have ended), have some bids (which are assumed to be valid), and has no winners assigned yet
                closedAuctions = auctionService.GetAuctionsAsQueryable().Where(a => a.EndTime < DateTime.Now && a.AuctionBids.Count > 0 && a.Winners.Count > 0);

                foreach (Auction auction in closedAuctions)
                {
                    BackgroundJob.Enqueue(() => RegisterAuction(auction.AuctionID));
                }

                auctionService.Dispose();
                auctionRepository.Dispose();
                listingRepository.Dispose();
                userRepository.Dispose();
                unitOfWork.Dispose();
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public static void RegisterAuction(int id)
        {
            Domain.Services.HangfireJobService.CloseAuction(id);
        }

        public static void RegisterGiveaways()
        {
            List<Giveaway> closedGiveaways = new List<Giveaway>();

            using (AppIdentityDbContext context = AppIdentityDbContext.Create())
            //using (IUnitOfWork unitOfWork = new UnitOfWork(context))
            {
                IUnitOfWork unitOfWork = new UnitOfWork(context);
                IGiveawayRepository giveawayRepository = new GiveawayRepository(unitOfWork);
                IListingRepository listingRepository = new ListingRepository(unitOfWork);
                IUserRepository userRepository = new UserRepository(unitOfWork);
                IUserService userService = new UserService(listingRepository, userRepository, null, giveawayRepository, null, null, unitOfWork);

                //List<Auction> openAuctions = auctionService.GetAuctions().Where(a => a.IsOpen()).ToList();
                // get auctions that: are not open (have ended), have some bids (which are assumed to be valid), and has no winners assigned yet
                closedGiveaways = userService.GetGiveaways().Where(a => a.IsOpen() == false && String.IsNullOrWhiteSpace(a.WinnerID) == true).ToList();

                userService.Dispose();
                giveawayRepository.Dispose();
                listingRepository.Dispose();
                userRepository.Dispose();
                unitOfWork.Dispose();
            }

            foreach (Giveaway giveaway in closedGiveaways)
            {
                BackgroundJob.Enqueue(() => RegisterGiveaways(giveaway.GiveawayID));
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public static void RegisterGiveaways(int id)
        {
            Domain.Services.HangfireJobService.CloseGiveaway(id);
        }
    }
}