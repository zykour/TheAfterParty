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
using TheAfterParty.Domain.Model;
using TheAfterParty.WebUI.Infrastructure;

namespace TheAfterParty.WebUI.Controllers
{
    public class StoreController : Controller
    {
        // razor doesn't handle default char value '\0' very well, use noSelectionSentinel instead
        // this should be a non alpha-numeric character as it's being used to represent a sentinel for "Begins With" filter for game names
        private const char noSelectionSentinel = '.';
        private const string storeFormID = "storeForm";
        private IStoreService storeService;
        //private ICacheService cacheService;

        private const string weeklyActionDest = "Weekly Deals";
        private const string dailyActionDest = "Daily Deals";
        private const string newActionDest = "New Additions";
        private const string otherActionDest = "Other Deals";
        private const string allActionDest = "All Deals";
        private const string platformsActionDest = "All Platforms";
        private const string historyActionDest = "Purchase History";
        
        public StoreController(IStoreService storeService)//, ICacheService cacheService)
        {
            this.storeService = storeService;
            ViewBag.StoreFormID = storeFormID;
            //this.cacheService = cacheService;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                storeService.SetUserName(User.Identity.Name);
                //cacheService.SetUserName(User.Identity.Name);
            }
        }

        private static string GetApiKey()
        {
            return System.Configuration.ConfigurationManager.AppSettings["steamAPIKey"];
        }
        
        #region Admin

        #region Advanced Listing Methods

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AddGames(string id)
        {
            int p_id = 0;

            if (String.IsNullOrEmpty(id) == true)
            {
                id = "0";
            }

            Int32.TryParse(id, out p_id);

            if (p_id == 0)
            {
                Platform plat = storeService.GetPlatforms().FirstOrDefault(p => object.Equals("Steam", p.PlatformName));

                if (plat == null)
                {
                    return RedirectToAction("AdminPlatforms");
                }

                p_id = plat.PlatformID;

                if (p_id == 0)
                {
                    return RedirectToAction("AdminPlatforms");
                }
            }

            AddGamesViewModel model = new AddGamesViewModel();

            model.Platforms = storeService.GetPlatforms().ToList();

            Platform platform = storeService.GetPlatformByID(p_id);

            if (platform == null)
            {
                return RedirectToAction("AdminPlatforms");
            }

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            model.Platform = platform;

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddGames(AddGamesViewModel model)
        {
            Platform platform = storeService.GetPlatformByID(model.Platform.PlatformID);

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();
            model.AddedGames = storeService.AddProductKeys(platform, model.Input);

            model.Input = String.Empty;

            model.Platforms = storeService.GetPlatforms().ToList();

            ModelState.Clear();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AddOrUpdateSteamProducts()
        {
            AddOrUpdateSteamProductsViewModel model = new AddOrUpdateSteamProductsViewModel();
            
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddOrUpdateSteamProducts(AddOrUpdateSteamProductsViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            model.AddedProducts = storeService.BuildOrUpdateProductsWithSteamID(model.Input);
            model.Input = String.Empty;

            ModelState.Clear();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AddListingFromAppID()
        {
            AddOrUpdateSteamProductsViewModel model = new AddOrUpdateSteamProductsViewModel();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddListingFromAppID(AddOrUpdateSteamProductsViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            model.AddedProducts = storeService.BuildListingsWithSteamID(model.Input);
            model.Input = String.Empty;

            ModelState.Clear();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AddNonSteamProducts()
        {
            AddNonSteamProductsViewModel model = new AddNonSteamProductsViewModel();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddNonSteamProducts(AddNonSteamProductsViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            model.AddedProducts = storeService.AddOrUpdateNonSteamProducts(model.Input);
            model.Input = String.Empty;

            ModelState.Clear();

            return View(model);
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
        [HttpGet]
        public async Task<ActionResult> AdminListings()
        {
            AdminListingViewModel model = new AdminListingViewModel();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            model.CurrentPage = 1;
            model.TotalItems = storeService.GetListings().Count();

            if (model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                model.Listings = storeService.GetListings().OrderByDescending(l => l.ListingID).Take(model.LoggedInUser.PaginationPreference);
            }
            else
            {
                model.Listings = storeService.GetListings().OrderByDescending(l => l.ListingID);
            }

            
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AdminListings(AdminListingViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            if (model.SelectedPage != 0)
            {
                model.CurrentPage = model.SelectedPage;
            }
            
            model.TotalItems = storeService.GetListings().Count();

            if (model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                model.Listings = storeService.GetListings().OrderByDescending(l => l.ListingID).Skip((model.CurrentPage - 1) * model.LoggedInUser.PaginationPreference).Take(model.LoggedInUser.PaginationPreference);
            }
            else
            {
                model.Listings = storeService.GetListings().OrderByDescending(l => l.ListingID);
            }
            
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

            storeService.DownloadIconURL(model.Platform, Server.MapPath(@"~/Content/PlatformIcons/"), model.FileExtension);

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

            storeService.DownloadIconURL(model.Platform, Server.MapPath(@"~/Content/PlatformIcons/"), model.FileExtension);
            
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
        [HttpGet]
        public async Task<ActionResult> AdminProducts()
        {
            AdminProductViewModel model = new AdminProductViewModel();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            model.CurrentPage = 1;
            model.TotalItems = storeService.GetProducts().Count();

            if (model.LoggedInUser != null && model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                model.Products = storeService.GetProducts().OrderByDescending(p => p.ProductID).Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.Products = storeService.GetProducts().OrderByDescending(p => p.ProductID).ToList();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AdminProducts(AdminProductViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            if (model.SelectedPage != 0)
            {
                model.CurrentPage = model.SelectedPage;
            }

            model.TotalItems = storeService.GetProducts().Count();

            if (model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                model.Products = storeService.GetProducts().OrderByDescending(p => p.ProductID).Skip((model.CurrentPage - 1) * model.LoggedInUser.PaginationPreference).Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.Products = storeService.GetProducts().OrderByDescending(p => p.ProductID).ToList();
            }

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
        [HttpGet]
        public async Task<ActionResult> AdminProductKeys()
        {
            AdminProductKeyViewModel model = new AdminProductKeyViewModel();

            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            model.CurrentPage = 1;
            model.TotalItems = storeService.GetProductKeys().Count();

            if (model.LoggedInUser != null && model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                model.ProductKeys = storeService.GetProductKeys().OrderByDescending(p => p.ProductKeyID).Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.ProductKeys = storeService.GetProductKeys().OrderByDescending(p => p.ProductKeyID).ToList();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AdminProductKeys(AdminProductKeyViewModel model)
        {
            model.LoggedInUser = await storeService.GetCurrentUser();
            model.FullNavList = CreateStoreControllerAdminNavList();

            if (model.SelectedPage != 0)
            {
                model.CurrentPage = model.SelectedPage;
            }

            model.TotalItems = storeService.GetProductKeys().Count();

            if (model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                model.ProductKeys = storeService.GetProductKeys().OrderByDescending(p => p.ProductKeyID).Skip((model.CurrentPage - 1) * model.LoggedInUser.PaginationPreference).Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.ProductKeys = storeService.GetProductKeys().OrderByDescending(p => p.ProductKeyID).ToList();
            }

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
        public async Task<ActionResult> Index(string id = "")
        {
            StoreIndexViewModel model = new StoreIndexViewModel();

            //IEnumerable<Listing> listings = storeService.GetStockedStoreListings();

            await PopulateStoreIndexViewModelFromGet(model, null, GetApiKey(), id);
            model.FormName = "Index";
            model.FormID = "";

            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> Index(StoreIndexViewModel model, string id = "")
        {
            await PopulateStoreIndexViewModelFromPostback(model, null, GetApiKey(), id);
            
            model.FormName = "Index";
            model.FormID = "";

            // Clear the ModelState so changes in the model are reflected when using HtmlHelpers (their default behavior is to not use changes made to the model when re-rendering a view, not what we want here)
            ModelState.Clear();

            return View(model);
        }
        
        // GET: CoopShop/Store
        [HttpGet]
        public async Task<ActionResult> Date(int month, int day, int year)
        {
            StoreIndexViewModel model = new StoreIndexViewModel();

            //IEnumerable<Listing> listings = storeService.GetStockedStoreListings();

            DateTime date = new DateTime(year, month, day);

            await PopulateStoreIndexViewModelFromGet(model, null, GetApiKey(), null, date);
            model.FormName = "Index";
            model.FormID = "";

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Date(StoreIndexViewModel model, int month, int day, int year)
        {
            DateTime date = new DateTime(year, month, day);

            await PopulateStoreIndexViewModelFromPostback(model, null, GetApiKey(), null, date);

            model.FormName = "Index";
            model.FormID = "";

            // Clear the ModelState so changes in the model are reflected when using HtmlHelpers (their default behavior is to not use changes made to the model when re-rendering a view, not what we want here)
            ModelState.Clear();

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Id(string id = "")
        {
            if (String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            StoreIndexViewModel model = new StoreIndexViewModel();

            await PopulateStoreIndexViewModelFromGet(model, null, GetApiKey(), id);
            
            model.FormName = "Id";
            model.FormID = id;

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Id(StoreIndexViewModel model, string id = "")
        {
            if (String.IsNullOrEmpty(id))
            {
                id = model.FriendSteamID;
            }

            if (String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", new { model = model });
            }
            
            await PopulateStoreIndexViewModelFromPostback(model, null, GetApiKey(), id);

            model.FormName = "Id";
            model.FormID = id;

            // Clear the ModelState so changes in the model are reflected when using HtmlHelpers (their default behavior is to not use changes made to the model when re-rendering a view, not what we want here)
            ModelState.Clear();

            return View("Index", model);
        }

        [HttpGet]
        public async Task<ActionResult> Newest(string id = "")
        {
            StoreIndexViewModel model = new StoreIndexViewModel();

            model.SpecialFilterType = "newest";

            await PopulateStoreIndexViewModelFromGet(model, null, GetApiKey(), id);

            model.FormName = "Newest";
            model.FormID = "";
                        
            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Newest(StoreIndexViewModel model, string id = "")
        {
            model.SpecialFilterType = "newest";

            await PopulateStoreIndexViewModelFromPostback(model, null, GetApiKey(), id);
            
            model.FormName = "Newest";
            model.FormID = "";

            ModelState.Clear();

            return View("Index", model);
        }

        [HttpGet]
        public async Task<ActionResult> Deals(string id = "all")
        {
            // conform to route convention by accepting "id" parameter, but assign it to a new variable to make it clear what purpose it serves
            string subsection = id.ToLower().Trim();

            StoreIndexViewModel model = new StoreIndexViewModel();

            model.SpecialFilterType = subsection;

            if ((String.Compare(subsection, "all") == 0 
                    || String.Compare(subsection, "daily") == 0 
                    || String.Compare(subsection, "weekly") == 0
                    || String.Compare(subsection, "other") == 0) == false)
            {
                return RedirectToAction("Deals", new { id = "all" });
            }

            await PopulateStoreIndexViewModelFromGet(model, null, GetApiKey());

            model.FormName = "Deals";
            model.FormID = id;

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Deals(StoreIndexViewModel model, string id)
        {
            // conform to route convention by accepting "id" parameter, but assign it to a new variable to make it clear what purpose it serves
            string subsection = id.Trim().ToLower();

            model.SpecialFilterType = subsection;

            if ((String.Compare(subsection, "all") == 0
                    || String.Compare(subsection, "daily") == 0
                    || String.Compare(subsection, "weekly") == 0
                    || String.Compare(subsection, "other") == 0) == false)
            {
                return RedirectToAction("Deals", new { id = "all" });
            }
            
            await PopulateStoreIndexViewModelFromPostback(model, null, GetApiKey());

            model.FormName = "Deals";
            model.FormID = id;

            ModelState.Clear();

            return View("Index", model);
        }

        // GET: Store/Game/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Game(int id)
        {
            StoreGameViewModel model = new StoreGameViewModel();

            model.Initialize((await storeService.GetCurrentUser()), CreateStoreControllerStoreNavList(new List<string>()));

            model.TargetListing = storeService.GetListingByID(id);

            if (model.TargetListing == null)
            {
                return RedirectToAction("Index");
            }

            if (model.TargetListing.Product.IsSteamAppID == true && model.TargetListing.Product.AppID > 0)
            {
                model.OwnedModel = new Models.User.UserOwnsViewModel();
                model.OwnedModel.GameOwners = storeService.GetUsersWhoOwn(model.TargetListing.Product.AppID).ToList();
                model.OwnedModel.GameNonOwners = storeService.GetUsersWhoDoNotOwn(model.TargetListing.Product.AppID).ToList();
                model.OwnedModel.GameName = model.TargetListing.ListingName;
                model.OwnedModel.AppID = model.TargetListing.Product.AppID;
            }

            return View(model);
        }

        // GET: Store/History
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> History()
        {
            StoreHistoryViewModel model = new StoreHistoryViewModel();

            model.Initialize((await storeService.GetCurrentUser()), CreateStoreControllerStoreNavList(new List<string>() { historyActionDest }));
            model.PurchaseEntries = model.SkipAndTake<ProductOrderEntry>(storeService.GetStoreHistory());
            
            return View(model);
        }

        // POST: Store/History
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> History(StoreHistoryViewModel model)
        {
            model.Initialize((await storeService.GetCurrentUser()), CreateStoreControllerStoreNavList(new List<string>() { historyActionDest }));
            model.PurchaseEntries = model.SkipAndTake<ProductOrderEntry>(storeService.GetStoreHistory());

            // Clear the ModelState so changes in the model are reflected when using HtmlHelpers (their default behavior is to not use changes made to the model when re-rendering a view, not what we want here)
            ModelState.Clear();

            return View(model);
        }

        #endregion

        #region Auxiliary Methods/Functions

        private async Task PopulateStoreIndexViewModelFromGet(StoreIndexViewModel model, List<String> currentDestName, string apiKey = null, string id = null, DateTime? date = null)
        {
            TheAfterParty.Domain.Concrete.ListingFilter filter = new Domain.Concrete.ListingFilter();
            
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                model.LoggedInUser = await storeService.GetCurrentUserWithStoreFilters();// GetCurrentUser();//await cacheService.TryGetCachedUser(HttpContext.User.Identity.Name);
                filter.LoggedInUser = model.LoggedInUser;
            }

            model.CurrentPage = 1;
            filter.Page = 1;

            if (date != null)
            {
                filter.Date = date;
            }

            if (currentDestName == null)
            {
                currentDestName = new List<String>();
            }

            if (model.SpecialFilterType.ToLower().CompareTo("newest") == 0)
            {
                filter.IsNewest = true;
                currentDestName.Add(newActionDest);
            }
            else if (model.SpecialFilterType.ToLower().CompareTo("all") == 0)
            {
                filter.IsAllDeal = true;
                currentDestName.Add(allActionDest);
            }
            else if (model.SpecialFilterType.ToLower().CompareTo("daily") == 0)
            {
                filter.IsDailyDeal = true;
                currentDestName.Add(dailyActionDest);
            }
            else if (model.SpecialFilterType.ToLower().CompareTo("weekly") == 0)
            {
                filter.IsWeeklyDeal = true;
                currentDestName.Add(weeklyActionDest);
            }
            else if (model.SpecialFilterType.ToLower().CompareTo("other") == 0)
            {
                filter.IsOtherDeal = true;
                currentDestName.Add(otherActionDest);
            }

            model.SpecialFilterType = String.Empty;

            if (String.IsNullOrWhiteSpace(apiKey) == false && String.IsNullOrWhiteSpace(id) == false)
            {
                filter.FriendAppIDs = storeService.GetAppIDsByID(id, apiKey);
                model.FriendSteamID = id;
            }

            if (model.LoggedInUser != null && model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                filter.PaginationNum = model.UserPaginationPreference;  
            }

            if (model.LoggedInUser == null)
            {
                model.UserPaginationPreference = 25;//change
                filter.PaginationNum = model.UserPaginationPreference;
            }

            currentDestName.Add(platformsActionDest);

            int count = 0;

                model.StoreListings = storeService.GetListingsWithFilter(filter, out count);
            //model.StoreListings = cacheService.TryGetCachedListings(filter, out count);

            model.TotalItems = count;

                model.StorePlatforms = storeService.GetPlatforms();
            //model.StorePlatforms = cacheService.TryGetCachedPlatforms();
            
            // TEMP
                model.FullNavList = CreateStoreControllerStoreNavList(model, currentDestName);

                List<SelectedTagMapping> tagMappings = new List<SelectedTagMapping>();

                foreach (Tag tag in storeService.GetTags())
                {
                    tagMappings.Add(new SelectedTagMapping(tag, false));
                }

                model.SelectedTagMappings = tagMappings.OrderBy(t => t.StoreTag.TagName);
            //model.SelectedTagMappings = cacheService.TryGetCachedTags();

            //TEMP
                List<SelectedProductCategoryMapping> categoryMappings = new List<SelectedProductCategoryMapping>();
            
                foreach (ProductCategory category in storeService.GetProductCategories())
                {
                    categoryMappings.Add(new SelectedProductCategoryMapping(category, false));
                }

                model.SelectedProductCategoryMappings = categoryMappings.OrderBy(c => c.ProductCategory.CategoryString);
             //model.SelectedProductCategoryMappings = cacheService.TryGetCachedProductCategories();
            }

        private async Task PopulateStoreIndexViewModelFromPostback(StoreIndexViewModel model, List<String> currentDestName, string apiKey = null, string id = null, DateTime? date = null)
        {
            TheAfterParty.Domain.Concrete.ListingFilter filter = new Domain.Concrete.ListingFilter();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                model.LoggedInUser = await storeService.GetCurrentUserWithStoreFilters();// GetCurrentUser();//await cacheService.TryGetCachedUser(HttpContext.User.Identity.Name);
                filter.LoggedInUser = model.LoggedInUser;
            }

            if (date != null)
            {
                filter.Date = date;
            }

            if (currentDestName == null)
            {
                currentDestName = new List<String>();
            }

            if (model.SpecialFilterType.ToLower().CompareTo("newest") == 0)
            {
                filter.IsNewest = true;
                currentDestName.Add(newActionDest);
            }
            else if (model.SpecialFilterType.ToLower().CompareTo("all") == 0)
            {
                filter.IsAllDeal = true;
                currentDestName.Add(allActionDest);
            }
            else if (model.SpecialFilterType.ToLower().CompareTo("daily") == 0)
            {
                filter.IsDailyDeal = true;
                currentDestName.Add(dailyActionDest);
            }
            else if (model.SpecialFilterType.ToLower().CompareTo("weekly") == 0)
            {
                filter.IsWeeklyDeal = true;
                currentDestName.Add(weeklyActionDest);
            }
            else if (model.SpecialFilterType.ToLower().CompareTo("other") == 0)
            {
                filter.IsOtherDeal = true;
                currentDestName.Add(otherActionDest);
            }

            model.SpecialFilterType = String.Empty;

            if (String.IsNullOrWhiteSpace(apiKey) == false && (model.FilterBlacklist | model.AffordableFilter | model.FilterLibrary | model.FilterWishlist) == false)
            {
                if (String.IsNullOrWhiteSpace(model.FriendSteamID) == false)
                {
                    filter.FriendAppIDs = storeService.GetAppIDsByID(model.FriendSteamID, apiKey);
                    model.PreviousAffordableFilter = false;
                    model.PreviousFilterBlacklist = false;
                    model.PreviousFilterLibrary = false;
                    model.PreviousFilterWishlist = false;
                }
                else if (String.IsNullOrWhiteSpace(id) == false)
                {
                    filter.FriendAppIDs = storeService.GetAppIDsByID(id, apiKey);
                    model.PreviousAffordableFilter = false;
                    model.PreviousFilterBlacklist = false;
                    model.PreviousFilterLibrary = false;
                    model.PreviousFilterWishlist = false;
                    model.FriendSteamID = id;
                }
            }

            if (model.LoggedInUser != null)
            {
                filter.UserID = model.LoggedInUser.Id;
            }

            if (model.SelectedPage > 0)
            {
                model.CurrentPage = model.SelectedPage;
            }
            else
            {
                model.CurrentPage = 1;
            }

            filter.Page = model.CurrentPage;
            
            if (String.IsNullOrWhiteSpace(model.SearchText) == false)
            {
                filter.SearchText = model.SearchText;
            }
            
            if (model?.SelectedProductCategoryMappings?.Count() > 0)
            {
                foreach (SelectedProductCategoryMapping mapping in model.SelectedProductCategoryMappings)
                {
                    if (model.CategoryToChange == mapping.ProductCategory.ProductCategoryID)
                    {
                        mapping.IsSelected = !mapping.IsSelected;
                    }

                    mapping.ProductCategory = storeService.GetProductCategoryByID(mapping.ProductCategory.ProductCategoryID); //cacheService.TryGetProductCategory(mapping.ProductCategory.ProductCategoryID);

                    if (mapping.IsSelected)
                    {
                        filter.ProductCategoryIDs.Add(mapping.ProductCategory.ProductCategoryID);
                    }
                }
            }

            model.CategoryToChange = 0;
            
            //mark
            if (model?.SelectedTagMappings?.Count() > 0)
            {
                foreach (SelectedTagMapping mapping in model.SelectedTagMappings)
                {
                    if (model.TagToChange == mapping.StoreTag.TagID)
                    {
                        mapping.IsSelected = !mapping.IsSelected;
                    }

                    mapping.StoreTag = storeService.GetTagByID(mapping.StoreTag.TagID);  //cacheService.TryGetCachedTags(mapping.StoreTag.TagID);

                    if (mapping.IsSelected)
                    {
                        filter.TagIDs.Add(mapping.StoreTag.TagID);
                    }
                }
            }

            model.TagToChange = 0;
            
            if (model.SelectedPlatformID > 0)
            {
                Platform platform =  storeService.GetPlatformByID(model.SelectedPlatformID); //cacheService.TryGetCachedPlatforms().SingleOrDefault(x => x.PlatformID == model.SelectedPlatformID);
                model.PreviousSelectedPlatformID = model.SelectedPlatformID;
                model.SelectedPlatformID = 0;
                filter.PlatformID = platform.PlatformID;
                currentDestName.Add(platform.PlatformName);
            }
            else if (model.SelectedPlatformID == -1)
            {
                model.PreviousSelectedPlatformID = 0;
                currentDestName.Add(platformsActionDest);
            }
            else if (model.PreviousSelectedPlatformID != 0)
            {
                Platform platform = storeService.GetPlatformByID(model.PreviousSelectedPlatformID); //cacheService.TryGetCachedPlatforms().SingleOrDefault(x => x.PlatformID == model.PreviousSelectedPlatformID);
                filter.PlatformID = platform.PlatformID;
                currentDestName.Add(platform.PlatformName);
            }

            filter.BeginsWithSentinel = noSelectionSentinel;

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
                    filter.BeginsWithFilter = model.BeginsWithFilter;
                }
                model.BeginsWithFilter = noSelectionSentinel;
            }
            else if (model.PreviousBeginsWithFilter != noSelectionSentinel)
            {
                filter.BeginsWithFilter = model.PreviousBeginsWithFilter;
            }

            if (model.FilterLibrary)
            {
                model.PreviousFilterLibrary = !model.PreviousFilterLibrary;
                model.FilterLibrary = false;
            }
            if (model.PreviousFilterLibrary && HttpContext.User.Identity.IsAuthenticated)
            {
                filter.UnownedFilter = true;
            }

            if (model.FilterWishlist)
            {
                model.PreviousFilterWishlist = !model.PreviousFilterWishlist;
                model.FilterWishlist = false;
            }
            if (model.PreviousFilterWishlist && HttpContext.User.Identity.IsAuthenticated)
            {
                filter.WishlistFilter = true;
            }

            if (model.FilterBlacklist)
            {
                model.PreviousFilterBlacklist = !model.PreviousFilterBlacklist;
                model.FilterBlacklist = false;
            }
            if (model.PreviousFilterBlacklist && HttpContext.User.Identity.IsAuthenticated)
            {
                filter.BlacklistFilter = true;
            }

            if (model.AffordableFilter)
            {
                model.PreviousAffordableFilter = !model.PreviousAffordableFilter;
                model.AffordableFilter = false;
            }
            if (model.PreviousAffordableFilter && HttpContext.User.Identity.IsAuthenticated)
            {
                filter.AffordableFilter = true;
            }
            
            if (model.GameSort > 0)
            {
                filter.GameSort = model.GameSort;
                model.PreviousGameSort = model.GameSort;
                model.PreviousPriceSort = 0;
                model.PreviousQuantitySort = 0;
            }
            else if (model.QuantitySort > 0)
            {
                filter.SetQuantitySort(model.QuantitySort);
                model.PreviousQuantitySort = model.QuantitySort;
                model.PreviousPriceSort = 0;
                model.PreviousGameSort = 0;
            }
            else if (model.PriceSort > 0)
            {
                filter.SetPriceSort(model.PriceSort);
                model.PreviousPriceSort = model.PriceSort;
                model.PreviousGameSort = 0;
                model.PreviousQuantitySort = 0;
            }
            else if (model.PreviousGameSort > 0)
            {
                filter.GameSort = model.PreviousGameSort;
            }
            else if (model.PreviousQuantitySort > 0)
            {
                filter.SetQuantitySort(model.PreviousQuantitySort);
            }
            else if (model.PreviousPriceSort > 0)
            {
                filter.SetPriceSort(model.PreviousPriceSort);
            }
                        
            if (model.LoggedInUser != null && model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                filter.PaginationNum = model.LoggedInUser.PaginationPreference;
            }

            if (model.LoggedInUser == null)
            {
                model.UserPaginationPreference = 25;//change
                filter.PaginationNum = model.UserPaginationPreference;
            }

            int count = 0;

            //mark
            model.StoreListings = storeService.GetListingsWithFilter(filter, out count); //cacheService.TryGetCachedListings(filter, out count); 

            model.TotalItems = count;

            //mark
            model.StorePlatforms = storeService.GetPlatforms();
            //model.StorePlatforms = cacheService.TryGetCachedPlatforms();

            model.FullNavList = CreateStoreControllerStoreNavList(model, currentDestName);
        }

        public List<NavGrouping> CreateStoreControllerStoreNavList(StoreIndexViewModel model, List<String> destNames)
        {
            List<NavGrouping> navList = new List<NavGrouping>();
            
            NavGrouping platforms = new NavGrouping();
            platforms.GroupingHeader = "Platforms";
            platforms.NavItems = new List<NavItem>();

            NavItem navItem = new NavItem();
            navItem.IsFormSubmit = true;
            navItem.DestinationName = platformsActionDest;
            navItem.FormName = "SelectedPlatformID";
            navItem.FormValue = "-1";
            navItem.FormID = storeFormID;
            navItem.SetSelected(destNames);

            platforms.NavItems.Add(navItem);

            int i = 0;
            foreach (Platform platform in  model.StorePlatforms)
            {
                navItem = new NavItem();
                navItem.IsFormSubmit = true;
                navItem.DestinationName = platform.PlatformName; 
                navItem.FormName = "SelectedPlatformID";
                navItem.FormValue = platform.PlatformID.ToString();
                navItem.FormID = storeFormID;
                navItem.SetSelected(destNames);

                platforms.NavItems.Add(navItem);
                i++;
            }

            navList.Add(platforms);

            NavGrouping deals = new NavGrouping();
            deals.GroupingHeader = "Deals";
            deals.NavItems = new List<NavItem>();

            NavItem dailyDeals = new NavItem();
            dailyDeals.DestinationName = dailyActionDest;
            dailyDeals.IsFormSubmit = true;
            dailyDeals.FormID = storeFormID;
            dailyDeals.FormAction = "/store/deals/daily";
            dailyDeals.SetSelected(destNames);

            NavItem weeklyDeals = new NavItem();
            weeklyDeals.IsFormSubmit = true;
            weeklyDeals.FormID = storeFormID;
            weeklyDeals.FormAction = "/store/deals/weekly";
            weeklyDeals.DestinationName = weeklyActionDest;
            weeklyDeals.SetSelected(destNames);

            NavItem otherDeals = new NavItem();
            otherDeals.IsFormSubmit = true;
            otherDeals.FormID = storeFormID;
            otherDeals.FormAction = "/store/deals/other";
            otherDeals.DestinationName = otherActionDest;
            otherDeals.SetSelected(destNames);

            NavItem allDeals = new NavItem();
            allDeals.IsFormSubmit = true;
            allDeals.FormID = storeFormID;
            allDeals.FormAction = "/store/deals/all";
            allDeals.DestinationName = allActionDest;
            allDeals.SetSelected(destNames);

            NavItem newestListings = new NavItem();
            newestListings.DestinationName = newActionDest;
            newestListings.IsFormSubmit = true;
            newestListings.FormID = storeFormID;
            newestListings.FormAction = "/store/newest/";
            newestListings.SetSelected(destNames);

            deals.NavItems.Add(dailyDeals);
            deals.NavItems.Add(weeklyDeals);
            deals.NavItems.Add(otherDeals);
            deals.NavItems.Add(allDeals);
            deals.NavItems.Add(newestListings);

            navList.Add(deals);

            NavGrouping misc = new NavGrouping();
            misc.GroupingHeader = "Other";
            misc.NavItems = new List<NavItem>();

            NavItem item = new NavItem();
            item.DestinationName = historyActionDest;
            item.Destination = "/store/history";
            item.SetSelected(destNames);
            misc.NavItems.Add(item);

            navList.Add(misc);            

            return navList;
        }

        public List<NavGrouping> CreateStoreControllerStoreNavList(List<String> destNames)
        {
            List<NavGrouping> navList = new List<NavGrouping>();
            
            NavGrouping grouping = new NavGrouping();
            grouping.GroupingHeader = "Store";
            grouping.NavItems = new List<NavItem>();
            
            NavItem item = new NavItem();
            item.DestinationName = "Store";
            item.Destination = "/store";
            grouping.NavItems.Add(item);
            item = new NavItem();
            item.DestinationName = historyActionDest;
            item.Destination = "/store/history";
            item.SetSelected(destNames);
            grouping.NavItems.Add(item);

            navList.Add(grouping);

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
            item = new NavItem();
            item.DestinationName = "Add/Update Steam Products";
            item.Destination = "/Store/AddOrUpdateSteamProducts";
            grouping.NavItems.Add(item);
            item = new NavItem();
            item.DestinationName = "Add/Update Steam Listings";
            item.Destination = "/Store/AddListingFromAppID";
            grouping.NavItems.Add(item);
            item = new NavItem();
            item.DestinationName = "Add Non-Steam Products";
            item.Destination = "/Store/AddNonSteamProducts";
            grouping.NavItems.Add(item);

            navList.Add(grouping);

            return navList;
        }

        #endregion
    }
}