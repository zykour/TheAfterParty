using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.Domain.Services;
using TheAfterParty.WebUI.Models.Store;
using System.Threading.Tasks;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;
using System.Text.RegularExpressions;

namespace TheAfterParty.WebUI.Controllers
{
    public class StoreController : Controller
    {
        // razor doesn't handle default char value '\0' very well, use noSelectionSentinel instead
        // this should be a non alpha-numeric character as it's being used to represent a sentinel for "Begins With" filter for game names
        private const char noSelectionSentinel = '.';
        private const string storeFormID = "storeForm";
        private IStoreService storeService;

        public StoreController(IStoreService storeService)
        {
            this.storeService = storeService;
            ViewBag.StoreFormID = storeFormID;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                storeService.SetUserName(User.Identity.Name);
            }
        }


        #region Admin

        #region Advanced Listing Methods

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddGames(AddGamesViewModel model)
        {
            AddedListingsViewModel outputModel = new AddedListingsViewModel();

            Platform platform = storeService.GetPlatformByID(model.Platform.PlatformID);

            outputModel.NewListings = storeService.AddProductKeys(platform, model.Input);//.ToList();

            return View("AddGamesSuccess", outputModel);
        }

        #endregion

        #region DiscountedListings

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddDiscountedListing(int id)
        {
            AddEditDiscountedListingViewModel model = new AddEditDiscountedListingViewModel();

            model.DiscountedListing.ListingID = id;

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddDiscountedListing(AddEditDiscountedListingViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();
            
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            storeService.AddDiscountedListing(model.DiscountedListing, model.DaysDealLast);

            return RedirectToAction("AdminListings");
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditDiscountedListing(int id)
        {
            AddEditDiscountedListingViewModel model = new AddEditDiscountedListingViewModel();

            model.DiscountedListing = storeService.GetDiscountedListingByID(id);
            
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditDiscountedListing(AddEditDiscountedListingViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            storeService.EditDiscountedListing(model.DiscountedListing, model.DaysDealLast);

            return RedirectToAction("AdminListings");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteDiscountedListing(int id)
        {
            storeService.DeleteDiscountedListing(id);

            return RedirectToAction("AdminListings");
        }

        #endregion

        #region Listings

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminListings()
        {
            AdminListingViewModel model = new AdminListingViewModel();

            model.Listings = storeService.GetListings();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditListing(int id)
        {
            AddEditListingViewModel model = new AddEditListingViewModel();

            model.Listing = storeService.GetListingByID(id);

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditListing(AddEditListingViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            storeService.EditListing(model.Listing);

            return RedirectToAction("AdminListings");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteListing(int id)
        {
            storeService.DeleteListing(id);

            return RedirectToAction("AdminListings");
        }

        #endregion

        #region Platforms

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminPlatforms()
        {
            AdminPlatformViewModel model = new AdminPlatformViewModel();

            model.Platforms = storeService.GetPlatforms();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddPlatform()
        {
            AddEditPlatformViewModel model = new AddEditPlatformViewModel();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddPlatform(AddEditPlatformViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            storeService.AddPlatform(model.Platform);

            return RedirectToAction("AdminPlatforms");
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditPlatform(int id)
        {
            AddEditPlatformViewModel model = new AddEditPlatformViewModel();

            model.Platform = storeService.GetPlatformByID(id);

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        /*[Authorize(Roles = "Admin")]
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
        }*/

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditPlatform(AddEditPlatformViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            storeService.EditPlatform(model.Platform);

            return RedirectToAction("AdminPlatforms");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeletePlatform(int id)
        {
            storeService.DeletePlatform(id);

            return RedirectToAction("AdminPlatforms");
        }

        #endregion

        #region Products

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminProducts()
        {
            AdminProductViewModel model = new AdminProductViewModel();

            model.Products = storeService.GetProducts().Where(p => p?.Listings.Count == 0).ToList();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditProduct(int id)
        {
            AddEditProductViewModel model = new AddEditProductViewModel();

            model.Product = storeService.GetProductByID(id);

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditProduct(AddEditProductViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            storeService.EditProduct(model.Product);

            if (model.Product?.Listings.Count > 0)      { return RedirectToAction("AdminListings"); }
            else                                        { return RedirectToAction("AdminProducts"); }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteProduct(int id)
        {
            Product product = storeService.GetProductByID(id);

            storeService.DeleteProduct(id);
            
            if (product?.Listings.Count > 0)    { return RedirectToAction("AdminListings"); }
            else                                { return RedirectToAction("AdminProducts"); }
        }

        #endregion

        #region ProductCategories

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminProductCategories()
        {
            AdminProductCategoryViewModel model = new AdminProductCategoryViewModel();

            model.ProductCategories = storeService.GetProductCategories();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditProductCategory(int id)
        {
            AddEditProductCategoryViewModel model = new AddEditProductCategoryViewModel();

            model.ProductCategory = storeService.GetProductCategoryByID(id);

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditProductCategory(AddEditProductCategoryViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            storeService.EditProductCategory(model.ProductCategory);

            return RedirectToAction("AdminProductCategories");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteProductCategory(int id)
        {
            storeService.DeleteProductCategory(id);

            return RedirectToAction("AdminProductCategories");
        }

        #endregion

        #region ProductKeys

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminProductKeys()
        {
            AdminProductKeyViewModel model = new AdminProductKeyViewModel();

            model.ProductKeys = storeService.GetProductKeys();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditProductKey(int id)
        {
            AddEditProductKeyViewModel model = new AddEditProductKeyViewModel();

            model.ProductKey = storeService.GetProductKeyByID(id);

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditProductKey(AddEditProductKeyViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            storeService.EditProductKey(model.ProductKey);

            return RedirectToAction("AdminProductKeys");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteProductKey(int id)
        {
            storeService.DeleteProductKey(id);

            return RedirectToAction("AdminProductKeys");
        }

        #endregion

        #endregion

        #region Store Action Results

        // GET: CoopShop/Store
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            StoreIndexViewModel model = new StoreIndexViewModel();

            model.StoreListings = storeService.GetStockedStoreListings().ToList();

            await PopulateStoreIndexViewModelFromGet(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Index(string id)
        {
            StoreIndexViewModel model = new StoreIndexViewModel();

            model.StoreListings = storeService.GetStockedStoreListings().ToList();

            await PopulateStoreIndexViewModelFromGet(model);

            model.StoreListings = storeService.FilterListingsByUserSteamID(model.StoreListings, id, System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]);
            model.FriendSteamID = id;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(StoreIndexViewModel model)
        {
            model.StoreListings = storeService.GetStockedStoreListings().ToList();

            bool filterOwnLibrary = model.FilterLibrary;

            await PopulateStoreIndexViewModelFromPostback(model);

            if (filterOwnLibrary == false && String.IsNullOrEmpty(model.FriendSteamID) == false && model.FriendSteamIDBool)
            {
                model.StoreListings = storeService.FilterListingsByUserSteamID(model.StoreListings, model.FriendSteamID, System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]);
            }

            // Clear the ModelState so changes in the model are reflected when using HtmlHelpers (their default behavior is to not use changes made to the model when re-rendering a view, not what we want here)
            ModelState.Clear();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(StoreIndexViewModel model, string id)
        {
            model.StoreListings = storeService.GetStockedStoreListings().ToList();

            bool filterOwnLibrary = model.FilterLibrary;

            await PopulateStoreIndexViewModelFromPostback(model);

            // don't want users filtering their own libraries along with their friend's library (this just doesn't make sense)
            // so if the passed in id is a relic make sure to 
            if (filterOwnLibrary == false)
            {
                model.StoreListings = storeService.FilterListingsByUserSteamID(model.StoreListings, id, System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"]);
                model.FriendSteamID = id;
            }            

            // Clear the ModelState so changes in the model are reflected when using HtmlHelpers (their default behavior is to not use changes made to the model when re-rendering a view, not what we want here)
            ModelState.Clear();

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Newest()
        {
            StoreIndexViewModel model = new StoreIndexViewModel();

            DateTime maxDate = storeService.GetStockedStoreListings().Select(l => l.DateEdited).Max().Date;

            model.StoreListings = storeService.GetStockedStoreListings().Where(l => l.DateEdited.Date.CompareTo(maxDate) == 0).OrderBy(l => l.ListingName).ToList();

            await PopulateStoreIndexViewModelFromGet(model);
            
            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Newest(StoreIndexViewModel model)
        {
            DateTime maxDate = storeService.GetStockedStoreListings().Select(l => l.DateEdited).Max().Date;

            model.StoreListings = storeService.GetStockedStoreListings().Where(l => l.DateEdited.Date.CompareTo(maxDate) == 0).OrderBy(l => l.ListingName).ToList();

            await PopulateStoreIndexViewModelFromPostback(model);

            return View("Index", model);
        }

        [HttpGet]
        public async Task<ActionResult> Deals(string id = "all")
        {
            // conform to route convention by accepting "id" parameter, but assign it to a new variable to make it clear what purpose it serves
            string subsection = id;

            StoreIndexViewModel model = new StoreIndexViewModel();

            if (String.Compare(subsection.ToLower(), "all") == 0)
            {
                model.StoreListings = storeService.GetListingsWithDeals().ToList();
                ViewBag.Title = "All Deals";
            }
            else if (String.Compare(subsection.ToLower(), "daily") == 0)
            {
                model.StoreListings = storeService.GetListingsWithDeals().Where(l => l.HasDailyDeal()).ToList();
                ViewBag.Title = "Daily Deals";
            }
            else if (String.Compare(subsection.ToLower(), "weekly") == 0)
            {
                model.StoreListings = storeService.GetListingsWithDeals().Where(l => l.HasWeeklyDeal()).ToList();
                ViewBag.Title = "Weekly Deals";
            }
            else if (String.Compare(subsection.ToLower(), "other") == 0)
            {
                model.StoreListings = storeService.GetListingsWithDeals().Where(d => d.HasDailyDeal() == false && d.HasWeeklyDeal() == false).ToList();
                ViewBag.Title = "Other Deals";
            }
            else
            {
                return RedirectToAction("Deals", new { id = "all" });
            }

            await PopulateStoreIndexViewModelFromGet(model);

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Deals(string id, StoreIndexViewModel model)
        {
            // conform to route convention by accepting "id" parameter, but assign it to a new variable to make it clear what purpose it serves
            string subsection = id;

            if (String.Compare(subsection.ToLower(), "all") == 0)
            {
                model.StoreListings = storeService.GetListingsWithDeals().ToList();
                ViewBag.Title = "All Deals";
            }
            else if (String.Compare(subsection.ToLower(), "daily") == 0)
            {
                model.StoreListings = storeService.GetListingsWithDeals().Where(l => l.HasDailyDeal()).ToList();
                ViewBag.Title = "Daily Deals";
            }
            else if (String.Compare(subsection.ToLower(), "weekly") == 0)
            {
                model.StoreListings = storeService.GetListingsWithDeals().Where(l => l.HasWeeklyDeal()).ToList();
                ViewBag.Title = "Weekly Deals";
            }
            else if (String.Compare(subsection.ToLower(), "other") == 0)
            {
                model.StoreListings = storeService.GetListingsWithDeals().Where(d => d.HasDailyDeal() == false && d.HasWeeklyDeal() == false).ToList();
                ViewBag.Title = "Other Deals";
            }
            else
            {
                return RedirectToAction("Deals", new { id = "all" });
            }

            await PopulateStoreIndexViewModelFromPostback(model);

            return View("Index", model);
        }

        #endregion

        #region Other Action Results

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

        #endregion

        #region Auxiliary Methods/Functions

        private async Task PopulateStoreIndexViewModelFromGet(StoreIndexViewModel model)
        {
            model.StoreListings = model.StoreListings.OrderBy(l => l.ListingName).ToList();

            model.LoggedInUser = await storeService.GetCurrentUser();

            List<Platform> platforms = storeService.GetPlatforms().ToList();

            foreach (Platform platform in platforms)
            {
                if (model.StoreListings.Any(l => l.ContainsPlatform(platform)))
                {
                    model.StorePlatforms.Add(platform);
                }
            }

            model.StorePlatforms.OrderBy(p => p.PlatformName).ToList();
            model.FullNavList = CreateStoreControllerStoreNavList(model);

            List<SelectedTagMapping> tagMappings = new List<SelectedTagMapping>();

            foreach (Tag tag in storeService.GetTags())
            {
                if (model.StoreListings.Any(l => l.ContainsTag(tag)))
                {
                    tagMappings.Add(new SelectedTagMapping(tag, false));
                }
            }

            model.SelectedTagMappings = tagMappings.OrderBy(t => t.StoreTag.TagName).ToList();

            List<SelectedProductCategoryMapping> categoryMappings = new List<SelectedProductCategoryMapping>();

            foreach (ProductCategory category in storeService.GetProductCategories())
            {
                if (model.StoreListings.Any(l => l.ContainsProductCategory(category)))
                {
                    categoryMappings.Add(new SelectedProductCategoryMapping(category, false));
                }
            }

            model.SelectedProductCategoryMappings = categoryMappings.OrderBy(c => c.ProductCategory.CategoryString).ToList();
        }

        // Populate StoreIndexViewModel for various actions
        private async Task PopulateStoreIndexViewModelFromPostback(StoreIndexViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();

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
                    model.StoreListings = model.StoreListings.OrderBy(l => l.SaleOrDefaultPrice()).ToList();
                }
                else
                {
                    model.StoreListings = model.StoreListings.OrderByDescending(l => l.SaleOrDefaultPrice()).ToList();
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

            foreach (Platform platform in storeService.GetPlatforms().ToList())
            {
                if (model.StoreListings.Any(l => l.ContainsPlatform(platform)))
                {
                    model.StorePlatforms.Add(platform);
                }
            }

            model.StorePlatforms.OrderBy(p => p.PlatformName).ToList();
            model.FullNavList = CreateStoreControllerStoreNavList(model);
        }

        public List<NavGrouping> CreateStoreControllerStoreNavList(StoreIndexViewModel model)
        {
            List<NavGrouping> navList = new List<NavGrouping>();
            
            NavGrouping actions = new NavGrouping();
            actions.GroupingHeader = "Actions";
            actions.NavItems = new List<NavItem>();
            NavItem clearSearch = new NavItem();
            clearSearch.DestinationName = "Clear Search";
            clearSearch.Destination = "/Store/";
            actions.NavItems.Add(clearSearch);

            navList.Add(actions);

            NavGrouping platforms = new NavGrouping();
            platforms.GroupingHeader = "Platforms";
            platforms.NavItems = new List<NavItem>();

            NavGrouping platformDeals = new NavGrouping();
            platformDeals.GroupingHeader = "Deals By Platform";
            platformDeals.NavItems = new List<NavItem>();

            for (int i = 0; i < model.StorePlatforms.Count; i++)
            {
                int count = model.StoreListings.Where(l => l.ContainsPlatform(model.StorePlatforms[i])).Count();
                string countText = "";
                if (count > 0)
                {
                    countText = " (" + count + ")";
                }

                NavItem navItem = new NavItem();
                navItem.IsFormSubmit = true;
                navItem.DestinationName = model.StorePlatforms[i].PlatformName + countText;
                navItem.FormName = "SelectedPlatformID";
                navItem.FormValue = model.StorePlatforms[i].PlatformID.ToString();
                navItem.FormID = storeFormID;

                platforms.NavItems.Add(navItem);

                if (model.StorePlatforms[i].Listings.Any(x => x.HasSale()))
                {
                    int dealsCount = model.StoreListings.Where(l => l.ContainsPlatform(model.StorePlatforms[i]) && l.HasSale()).Count();

                    countText = "";
                    if (count > 0)
                    {
                        countText = " (" + count + ")";
                    }

                    NavItem dealNavItem = new NavItem();
                    dealNavItem.IsFormSubmit = true;
                    dealNavItem.DestinationName = model.StorePlatforms[i].PlatformName + countText;
                    dealNavItem.FormName = "SelectedPlatformID";
                    dealNavItem.FormValue = model.StorePlatforms[i].PlatformID.ToString();
                    dealNavItem.FormID = storeFormID;
                    platformDeals.NavItems.Add(dealNavItem);
                }
            }

            navList.Add(platforms);
            navList.Add(platformDeals);

            NavGrouping deals = new NavGrouping();
            deals.GroupingHeader = "Deals";
            deals.NavItems = new List<NavItem>();

            NavItem weeklyDeals = new NavItem();
            weeklyDeals.Destination = "/Store/Deals/weekly";
            weeklyDeals.DestinationName = "Weekly Deals";

            NavItem dailyDeals = new NavItem();
            dailyDeals.DestinationName = "Daily Deals";
            dailyDeals.Destination = "/Store/Deals/daily";

            NavItem newestListings = new NavItem();
            newestListings.DestinationName = "New Additions";
            newestListings.Destination = "/Store/Newest/";

            deals.NavItems.Add(weeklyDeals);
            deals.NavItems.Add(dailyDeals);
            deals.NavItems.Add(newestListings);

            navList.Add(deals);

            return navList;
        }

        public List<NavGrouping> CreateStoreControllerAdminNavList()
        {
            List<NavGrouping> navList = new List<NavGrouping>();

            NavGrouping grouping = new NavGrouping();
            grouping.GroupingHeader = "Non-Admin Pages";
            grouping.NavItems = new List<NavItem>();
            NavItem item = new NavItem();
            item.DestinationName = "Store Index";
            item.Destination = "/Store/";
            grouping.NavItems.Add(item);

            navList.Add(grouping);

            grouping = new NavGrouping();
            grouping.GroupingHeader = "Admin Pages";
            grouping.NavItems = new List<NavItem>();
            item = new NavItem();
            item.DestinationName = "View Listings";
            item.Destination = "/Store/AdminListings";
            grouping.NavItems.Add(item);
            item = new NavItem();
            item.DestinationName = "View Platforms";
            item.Destination = "/Store/AdminPlatforms";
            grouping.NavItems.Add(item);
            item = new NavItem();
            item.DestinationName = "View Products";
            item.Destination = "/Store/AdminProducts";
            grouping.NavItems.Add(item);
            item = new NavItem();
            item.DestinationName = "View Categories";
            item.Destination = "/Store/AdminProductCategories";
            grouping.NavItems.Add(item);
            item = new NavItem();
            item.DestinationName = "View Keys";
            item.Destination = "/Store/AdminProductKeys";
            grouping.NavItems.Add(item);
            item = new NavItem();
            item.DestinationName = "Add Listings";
            item.Destination = "/Store/AddGames";
            grouping.NavItems.Add(item);
            item = new NavItem();
            item.DestinationName = "Add Platform";
            item.Destination = "/Store/AddPlatform";
            grouping.NavItems.Add(item);

            navList.Add(grouping);

            return navList;
        }

        #endregion
    }
}