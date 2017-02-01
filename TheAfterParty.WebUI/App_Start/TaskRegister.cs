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
            List<Auction> closedAuctions = new List<Auction>();

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
                closedAuctions = auctionService.GetAuctions().Where(a => a.IsOpen() == false && a.AuctionBidsIsNullOrEmpty() == false && a.GetSelectedWinnersCount() == 0).ToList();

                auctionService.Dispose();
                auctionRepository.Dispose();
                listingRepository.Dispose();
                userRepository.Dispose();
                unitOfWork.Dispose();
            }

            foreach (Auction auction in closedAuctions)
            {
                BackgroundJob.Enqueue(() => RegisterAuction(auction.AuctionID));
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public static void RegisterAuction(int id)
        {
            Domain.Services.HangfireJobService.CloseAuction(id);
        }
    }
}