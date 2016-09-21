﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Services
{
    public interface IApiService : IDisposable
    {
        IEnumerable<Listing> SearchListings(string searchText, int resultLimit);
        Listing GetListingByAppID(int appId);
        Listing GetDailyDeal();
        IEnumerable<Listing> GetWeeklyDeals();
        IEnumerable<Listing> GetOtherDeals();
        IEnumerable<Listing> GetWeeklyDeals(int resultsLimit);
        IEnumerable<Listing> GetOtherDeals(int resultsLimit);
        Listing GetListingByID(int id);
        int GetNumDeals();

        Order BuyAndRevealListing(AppUser user, int listingId, int price);

        IEnumerable<Objective> SearchObjectives(string searchText, int resultLimit);

        AppUser GetUserByNickName(string nickname);
        AppUser GetUserByUserName(string username);
        AppUser GetUserBySteamID(long id);
        AppUser GetUserBySteamID(UInt64 id);
        AppUser GetUserWithHighestBalance();
        AppUser GetUser(string identifier);
        AppUser GetPOTW();
        void SetPOTW(AppUser user);

        Objective GetObjectiveByID(int objectiveId);
        List<String> AddBalance(int points, string[] userNickNames);
        List<String> AddBalanceForObjective(Objective objective, string[] userNickNames);

        IEnumerable<Auction> GetOpenAuctions();
        IEnumerable<BoostedObjective> GetLiveBoostedObjectives();
        IEnumerable<DiscountedListing> GetLiveDiscountedListings();

        void RolloverDailyDeal();
        void CreateNewWeeklyDeals(int numDeals);

        bool TransferPoints(AppUser sender, AppUser recipient, int points);

        void AddSiteNotification(string notification);

        Objective GetObjectiveWithBoostedDaily();
    }
}
