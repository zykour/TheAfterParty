using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.Domain.Services;
using TheAfterParty.WebUI.Models.Store;
using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;
using System.Text.RegularExpressions;

namespace TheAfterParty.WebUI.Controllers
{
    public class StoreController : Controller
    {
        // razor doesn't handle default char value '\0' very well, use noSelectionSentinel instead
        // this should be a non alpha-numeric character as it's being used to represent a sentinel for "Begins With" filter for game names
        private const char noSelectionSentinel = '.';
        private IStoreService storeService;

        public StoreController(IStoreService storeService)
        {
            this.storeService = storeService;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                storeService.SetUserName(User.Identity.Name);
            }
        }

        // GET: CoopShop/Store
        public ActionResult Index()
        {
            StoreIndexViewModel model = new StoreIndexViewModel();
            
            List<SelectedTagMapping> tagMappings = new List<SelectedTagMapping>();

            foreach (Tag tag in storeService.GetTags())
            {
                tagMappings.Add(new SelectedTagMapping(tag, false));
            }

            List<SelectedProductCategoryMapping> categoryMappings = new List<SelectedProductCategoryMapping>();

            foreach (ProductCategory category in storeService.GetProductCategories())
            {
                categoryMappings.Add(new SelectedProductCategoryMapping(category, false));
            }

            model.StoreListings = storeService.GetStockedStoreListings().OrderBy(l => l.ListingName).ToList();
            model.SelectedTagMappings = tagMappings.OrderBy(t => t.StoreTag.TagName).ToList();
            model.SelectedProductCategoryMappings = categoryMappings.OrderBy(c => c.ProductCategory.CategoryString).ToList();
            model.StorePlatforms = storeService.GetPlatforms().OrderByDescending(p => p.Listings.Count).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(StoreIndexViewModel model)
        {
            model.StoreListings = storeService.GetStockedStoreListings().ToList();//.Where(l => l.ListingName.ToLower().Contains(model.SearchText.Trim().ToLower())).ToList();

            if (model.SearchTextBool == true && !String.IsNullOrEmpty(model.SearchText))
            {
                if (!String.IsNullOrEmpty(model.SearchText))
                {
                    model.StoreListings = model.StoreListings.Where(l => l.ListingName.ToLower().Contains(model.SearchText.Trim().ToLower())).ToList();
                }
            }

            foreach (SelectedProductCategoryMapping mapping in model.SelectedProductCategoryMappings)
            {
                if (model.CategoryToChange == mapping.ProductCategory.ProductCategoryID)
                {
                    mapping.IsSelected = !mapping.IsSelected;
                }

                mapping.ProductCategory = storeService.GetProductCategoryByID(mapping.ProductCategory.ProductCategoryID);
                
                if (mapping.IsSelected)
                {
                    model.StoreListings = model.StoreListings.Where(l => l.ContainsProductCategory(mapping.ProductCategory)).ToList();
                }
            }

            model.CategoryToChange = 0;

            foreach (SelectedTagMapping mapping in model.SelectedTagMappings)
            {
                if (model.TagToChange == mapping.StoreTag.TagID)
                {
                    mapping.IsSelected = !mapping.IsSelected;
                }

                mapping.StoreTag = storeService.GetTagByID(mapping.StoreTag.TagID);

                if (mapping.IsSelected)
                {
                    model.StoreListings = model.StoreListings.Where(l => l.ContainsTag(mapping.StoreTag)).ToList();
                }
            }

            model.TagToChange = 0;

            if (model.SelectedPlatformID != 0)
            {
                Platform platform = storeService.GetPlatformByID(model.SelectedPlatformID);
                model.PreviousSelectedPlatformID = model.SelectedPlatformID;
                model.SelectedPlatformID = 0;
                model.PreviousSelectedDealsPlatformID = 0;
                model.StoreListings = model.StoreListings.Where(l => l.ContainsPlatform(platform)).ToList();
            }
            else if (model.PreviousSelectedPlatformID != 0)
            {
                Platform platform = storeService.GetPlatformByID(model.PreviousSelectedPlatformID);
                model.StoreListings = model.StoreListings.Where(l => l.ContainsPlatform(platform)).ToList();
            }

            if (model.SelectedDealsPlatformID != 0)
            {
                Platform platform = storeService.GetPlatformByID(model.SelectedDealsPlatformID);
                model.PreviousSelectedDealsPlatformID = model.SelectedDealsPlatformID;
                model.SelectedDealsPlatformID = 0;
                model.PreviousSelectedPlatformID = 0;
                model.StoreListings = model.StoreListings.Where(l => l.HasSale() && l.ContainsPlatform(platform)).ToList();
            }
            else if (model.PreviousSelectedDealsPlatformID != 0)
            {
                Platform platform = storeService.GetPlatformByID(model.PreviousSelectedDealsPlatformID);
                model.StoreListings = model.StoreListings.Where(l => l.HasSale() && l.ContainsPlatform(platform)).ToList();
            }

            if (model.BeginsWithFilter != noSelectionSentinel)
            {
                // this case means that the character was already selected and highlighted and now the user wishes to unselect it
                if (model.BeginsWithFilter == model.PreviousBeginsWithFilter)
                {
                    model.PreviousBeginsWithFilter = noSelectionSentinel;
                }
                else
                {
                    model.PreviousBeginsWithFilter = model.BeginsWithFilter;
                    if (model.BeginsWithFilter == '0')
                    {
                        model.StoreListings = model.StoreListings.Where(l => Regex.IsMatch(l.ListingName, @"^\d.*")).ToList();
                    }
                    else
                    {
                        model.StoreListings = model.StoreListings.Where(l => l.ListingName.ToLower().StartsWith(model.BeginsWithFilter.ToString())).ToList();
                    }
                }
                model.BeginsWithFilter = noSelectionSentinel;
            }
            else if (model.PreviousBeginsWithFilter != noSelectionSentinel)
            {
                if (model.PreviousBeginsWithFilter == '0')
                {
                    model.StoreListings = model.StoreListings.Where(l => Regex.IsMatch(l.ListingName, @"^\d.*")).ToList();
                }
                else
                {
                    model.StoreListings = model.StoreListings.Where(l => l.ListingName.ToLower().StartsWith(model.PreviousBeginsWithFilter.ToString())).ToList();
                }
            }

            if (model.FilterLibrary)
            {
                model.PreviousFilterLibrary = !model.PreviousFilterLibrary;

                if (model.PreviousFilterLibrary && HttpContext.User.Identity.IsAuthenticated)
                {
                    AppUser user = await storeService.GetCurrentUser();

                    if (user.OwnedGames != null)
                    {
                        model.StoreListings = model.StoreListings.Where(l => !user.OwnsListing(l)).ToList();
                    }
                }
            }

            if (model.FilterBlacklist)
            {
                model.PreviousFilterBlacklist = !model.PreviousFilterBlacklist;

                if (model.PreviousFilterBlacklist && HttpContext.User.Identity.IsAuthenticated)
                {
                    AppUser user = await storeService.GetCurrentUser();

                    if (user.BlacklistedListings != null)
                    {
                        model.StoreListings = model.StoreListings.Where(l => !user.BlacklistedListings.Contains(l)).ToList();
                    }
                }
            }

            if (model.AffordableFilter)
            {
                model.PreviousAffordableFilter = !model.PreviousAffordableFilter;
            }

            if (model.PreviousAffordableFilter && !model.CartAffordableFilter)
            {
                model.PreviousCartAffordableFilter = false;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    AppUser user = await storeService.GetCurrentUser();

                    int targetMaxPrice = user.Balance - user.ReservedBalance();

                    model.StoreListings = model.StoreListings.Where(l => l.SaleOrDefaultPrice() <= targetMaxPrice).ToList();
                }
            }

            if (model.CartAffordableFilter)
            {
                model.PreviousCartAffordableFilter = !model.PreviousCartAffordableFilter;
            }

            if (model.PreviousCartAffordableFilter)
            {
                model.PreviousAffordableFilter = false;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    AppUser user = await storeService.GetCurrentUser();

                    int targetMaxPrice = user.Balance - user.ReservedBalance() - user.GetCartTotal();

                    model.StoreListings = model.StoreListings.Where(l => l.SaleOrDefaultPrice() <= targetMaxPrice).ToList();
                }
            }

            if (model.GameSort > 0)
            {
                if (model.GameSort == 1)
                {
                    model.StoreListings = model.StoreListings.OrderBy(l => l.ListingName).ToList();
                }
                else
                {
                    model.StoreListings = model.StoreListings.OrderByDescending(l => l.ListingName).ToList();
                }
                model.PreviousGameSort = model.GameSort;
                model.PreviousPriceSort = 0;
                model.PreviousQuantitySort = 0;
            }
            else if (model.QuantitySort > 0)
            {
                if (model.QuantitySort == 1)
                {
                    model.StoreListings = model.StoreListings.OrderBy(l => l.Quantity).ToList();
                }
                else
                {
                    model.StoreListings = model.StoreListings.OrderByDescending(l => l.Quantity).ToList();
                }
                model.PreviousQuantitySort = model.QuantitySort;
                model.PreviousPriceSort = 0;
                model.PreviousGameSort = 0;
            }
            else if (model.PriceSort > 0)
            {
                if (model.PriceSort == 1)
                {
                    model.StoreListings = model.StoreListings.OrderBy(l => l.ListingPrice).ToList();
                }
                else
                {
                    model.StoreListings = model.StoreListings.OrderByDescending(l => l.ListingPrice).ToList();
                }
                model.PreviousPriceSort = model.PriceSort;
                model.PreviousGameSort = 0;
                model.PreviousQuantitySort = 0;
            }
            else if (model.PreviousGameSort > 0)
            {
                if (model.PreviousGameSort == 1)
                {
                    model.StoreListings = model.StoreListings.OrderBy(l => l.ListingName).ToList();
                }
                else
                {
                    model.StoreListings = model.StoreListings.OrderByDescending(l => l.ListingName).ToList();
                }
            }
            else if (model.PreviousQuantitySort > 0)
            {
                if (model.PreviousQuantitySort == 1)
                {
                    model.StoreListings = model.StoreListings.OrderBy(l => l.Quantity).ToList();
                }
                else
                {
                    model.StoreListings = model.StoreListings.OrderByDescending(l => l.Quantity).ToList();
                }
            }
            else if (model.PreviousPriceSort > 0)
            {
                if (model.PreviousPriceSort == 1)
                {
                    model.StoreListings = model.StoreListings.OrderBy(l => l.ListingPrice).ToList();
                }
                else
                {
                    model.StoreListings = model.StoreListings.OrderByDescending(l => l.ListingPrice).ToList();
                }
            }

            model.StorePlatforms = storeService.GetPlatforms().ToList();

            // Clear the ModelState so changes in the model are reflected when using HtmlHelpers (their default behavior is to not use changes made to the model when re-rendering a view, not what we want here)
            ModelState.Clear();

            return View(model);
        }
        
        public async Task<ActionResult> UserBalances()
        {
            StoreBalancesViewModel model = new StoreBalancesViewModel();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                model.LoggedInUser = await storeService.GetCurrentUser();
                model.Users = storeService.GetAppUsers();
            }
            
            return View();
        }
        
        public ActionResult Deals(string id = "all")
        {
            // conform to route convention by accepting "id" parameter, but assign it to a new variable to make it clear what purpose it serves
            string subsection = id;

            StoreDealsViewModel model = new StoreDealsViewModel();

            if (String.Compare(subsection.ToLower(), "all") == 0)
            {
                model.ListingsWithDeals = storeService.GetListingsWithDeals();
                model.DealType = "All Deals";
            }
            else if (String.Compare(subsection.ToLower(), "daily") == 0)
            {
                model.ListingsWithDeals = storeService.GetListingsWithDeals().Where(l => l.HasDailyDeal());
                model.IsDaily = true;
                model.DealType = "Daily Deals";
            }
            else if (String.Compare(subsection.ToLower(), "weekly") == 0)
            {
                model.ListingsWithDeals = storeService.GetListingsWithDeals().Where(l => l.HasWeeklyDeal());
                model.IsWeekly = true;
                model.DealType = "Weekly Deals";
            }
            else if (String.Compare(subsection.ToLower(), "other") == 0)
            {
                model.ListingsWithDeals = storeService.GetListingsWithDeals().Where(d => d.HasDailyDeal() == false && d.HasWeeklyDeal() == false);
                model.DealType = "Other Deals";
            }
            else
            {
                return RedirectToAction("Deals", new { id = "all" });
            }

            return View(model);
        }



        // Role = admin
        public ActionResult AddGames(string id = "Steam")
        {
            // use the id convention, but place the id into a new variable to make it clear what it represents
            string platformName = id;

            AddGamesViewModel model = new AddGamesViewModel();
            
            foreach (Platform platform in storeService.GetPlatforms())
            {
                if (platform.PlatformName.ToLower().CompareTo(platformName.ToLower()) == 0)
                {
                    model.Platform = platform;
                    return View(model);
                }
            }

            return RedirectToAction("EditPlatform", new { id = platformName });
        }

        // Role = admin
        [HttpPost]
        public ActionResult AddGames(AddGamesViewModel model)
        {
            AddedListingsViewModel outputModel = new AddedListingsViewModel();

            Platform platform = storeService.GetPlatformByID(model.Platform.PlatformID);

            outputModel.NewListings = storeService.AddProductKeys(platform, model.Input);//.ToList();

            return View("AddGamesSuccess", outputModel);
        }

        // role admin
        public ActionResult EditPlatform(string id)
        {
            string platformName = id;
            Platform existingPlatform = storeService.GetPlatforms().Where(p => object.Equals(platformName, p.PlatformName)).SingleOrDefault();

            AddEditPlatformViewModel model = new AddEditPlatformViewModel();

            if (existingPlatform != null)
            {
                model.Platform = existingPlatform;
            }
            else
            {
                model.Platform = new Platform();
                model.Platform.PlatformName = platformName;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditPlatform(AddEditPlatformViewModel model)
        {
            storeService.EditPlatform(model.Platform);

            model.Success = true;

            return View(model);
        }
    }
}