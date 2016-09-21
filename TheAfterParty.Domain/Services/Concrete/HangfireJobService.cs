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
    public static class HangfireJobService
    {
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

                DateTime expiry = DateTime.Now.AddDays(1).AddMinutes(-5);

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

                SiteNotification notification = new SiteNotification() { Notification = "[daily][/daily] Today's [url=https://theafterparty.azurewebsites.net/store/deals/daily]daily deal[/url] is " + urlAndName + ", now on sale for [gtext]" + randomListing.SaleOrDefaultPrice() + "[/gtext] points!", NotificationDate = DateTime.Now };
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

                int numDeals = rand.Next(51) + 50;

                List<Listing> newDeals = listingRepository.GetListings().Where(l => l.Quantity > 0 && l.ListingPrice > 1).OrderBy(x => Guid.NewGuid()).Take(numDeals).ToList();

                if (newDeals == null || newDeals.Count == 0)
                {
                    unitOfWork.Save();
                    return;
                }

                DateTime dealExpiry = DateTime.Now.AddDays(7).AddMinutes(-5);

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

                SiteNotification notification = new SiteNotification() { Notification = "[weekly][/weekly] There are [gtext]" + numDeals + "[gtext] new [url=https://theafterparty.azurewebsites.net/store/deals/weekly]deals[/url] available this week!", NotificationDate = DateTime.Now };
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

                DateTime endDate = DateTime.Now.AddDays(1).AddMinutes(-5);

                newBoostedObjective.EndDate = endDate;
                newBoostedObjective.IsDaily = true;
                
                randomObjective.AddBoostedObjective(newBoostedObjective);

                objectiveRepository.UpdateObjective(randomObjective);
                
                ISiteRepository siteRepository = new SiteRepository(unitOfWork);

                String urlAndName = String.Empty;

                if (randomObjective.Product.IsSteamAppID)
                {
                    urlAndName = "[url=http://store.steampowered.com/app/" + randomObjective.Product.AppID + "]" + randomObjective.Title + "[/url]";
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
