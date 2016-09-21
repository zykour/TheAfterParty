using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TheAfterParty.Domain.Services;
using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models.Api;
using Ninject;
using TheAfterParty.WebUI.App_Start;

namespace TheAfterParty.WebUI.Controllers
{
    public class TapController : ApiController
    {
        private IApiService apiService;

        public TapController(IApiService apiService)
        {
            this.apiService = apiService;
        }
        public TapController()
        {
            apiService = NinjectWebCommon.CreateKernel().Get<IApiService>();
        }

        [HttpGet]
        public IHttpActionResult PingTap()
        {
            return Ok(true);
        }

        // id = searchtext
        [HttpGet]
        public IHttpActionResult SearchListings(string id, int resultsLimit)
        {
            return Ok(apiService.SearchListings(id, resultsLimit).Select(x => new ApiListingModel(x)));
        }

        [HttpGet]
        public ApiListingModel DailyDeal()
        {
            ApiListingModel model = new ApiListingModel(apiService.GetDailyDeal());

            return model;
        }

        [HttpGet]
        public IEnumerable<ApiListingModel> WeeklyDeals()
        {
            return apiService.GetWeeklyDeals().Select(x => new ApiListingModel(x));
        }

        [HttpGet]
        public IEnumerable<ApiListingModel> OtherDeals()
        {
            return apiService.GetOtherDeals().Select(x => new ApiListingModel(x));
        }

        [HttpGet]
        public ApiListingModel GetListingByAppID(int appId)
        {
            Listing listing = apiService.GetListingByAppID(appId);
            ApiListingModel model = null;
            
            if (listing != null)
            {
                model = new ApiListingModel(listing);
            }

            return model;
        }

        [HttpGet]
        public IEnumerable<ApiObjectiveModel> SearchObjectives(string searchText, int resultsLimit)
        {
            return apiService.SearchObjectives(searchText, resultsLimit).Select(o => new ApiObjectiveModel(o));
        }
        
        [HttpGet]
        public ApiAppUserModel GetUserBySteamID(long id)
        {
            AppUser user = apiService.GetUserBySteamID(id);

            if (user != null)
            {
                return new ApiAppUserModel(user);
            }

            return null;
        }

        [HttpGet]
        public ApiAppUserModel GetUserWithHighestBalance()
        {
            return new ApiAppUserModel(apiService.GetUserWithHighestBalance());
        }

        [HttpGet]
        public ApiAppUserModel GetPOTW()
        {
            AppUser user = apiService.GetPOTW();

            if (user != null)
            {
                return new ApiAppUserModel(user);
            }

            return null;
        }

        [HttpGet]
        public IEnumerable<ApiAuctionModel> GetOpenAuctions()
        {
            return apiService.GetOpenAuctions().Select(a => new ApiAuctionModel(a));
        }

        [HttpGet]
        public IEnumerable<ApiBoostedObjectiveModel> GetLiveBoostedObjectives()
        {
            return apiService.GetLiveBoostedObjectives().Select(b => new ApiBoostedObjectiveModel(b));
        }

        [HttpGet]
        public IEnumerable<ApiDiscountedListingModel> GetLiveDiscountedListings()
        {
            return apiService.GetLiveDiscountedListings().Select(d => new ApiDiscountedListingModel(d));
        }

        [HttpGet]
        [Authorize]
        public IHttpActionResult DailyDealRollover()
        {
            apiService.RolloverDailyDeal();

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public IHttpActionResult CreateNewWeeklyDeals(int numDeals)
        {
            apiService.CreateNewWeeklyDeals(numDeals);

            return Ok();
        }

    }
}
