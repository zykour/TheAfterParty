using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models.Cart;
using TheAfterParty.Domain.Services;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;
using System.Collections.Generic;
using System.Linq;
using System;
using TheAfterParty.WebUI.Infrastructure;

namespace TheAfterParty.WebUI.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private ICartService cartService;
        private ICacheService cacheService;
        
        public CartController(ICartService cartService, ICacheService cacheService)
        {
            this.cartService = cartService;
            this.cacheService = cacheService;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            
            cartService.SetUserName(User.Identity.Name);
            cacheService.SetUserName(User.Identity.Name);
        }


        // GET: /Cart
        [Authorize]
        public async Task<ActionResult> Index(string returnUrl)
        {
            CartIndexViewModel cartViewModel = new CartIndexViewModel
            {
                LoggedInUser = await cartService.GetCurrentUser(),
                CartEntries = await cartService.GetShoppingCartEntries(),
                ReturnUrl = returnUrl
            };

            cartViewModel.FullNavList = CreateCartViewModelNavList();

            cartViewModel.CartEntries = cartViewModel.CartEntries.OrderBy(e => e.Listing.ListingName);

            return View(cartViewModel);
        }

        public async Task<int> ShoppingCartQuantity()
        {
            AppUser user = await cartService.GetCurrentUser();

            return user.GetCartQuantity();
        }

        public async Task<int> ShoppingCartTotal()
        {
            AppUser user = await cartService.GetCurrentUser();

            return user.GetCartTotal();
        }

        public List<NavGrouping> CreateCartControllerNavList(AppUser user)
        {
            List<NavGrouping> grouping = new List<NavGrouping>();

            NavGrouping actions = new NavGrouping();
            actions.GroupingHeader = "Actions";
            actions.NavItems = new List<NavItem>();

            NavItem continueShopping = new NavItem();
            continueShopping.Destination = "/Store";
            continueShopping.DestinationName = "Continue Shopping";

            NavItem clearCart = new NavItem();
            clearCart.Destination = "/Cart/EmptyCart";
            clearCart.DestinationName = "Empty Cart";

            actions.NavItems.Add(continueShopping);
            actions.NavItems.Add(clearCart);

            if (user.AssertValidOrder())
            {
                NavItem purchase = new NavItem();
                purchase.Destination = "/Cart/Purchase";
                purchase.DestinationName = "Purchase";

                actions.NavItems.Add(purchase);
            }

            grouping.Add(actions);

            return grouping;
        }

        // Navbar cart info
        public PartialViewResult ShoppingCart()
        {
            CartLayoutViewModel model = new CartLayoutViewModel();
            model.LoggedInUser = cartService.GetCurrentUserSynch();

            return PartialView("~/Views/Shared/_ShoppingCart.cshtml", model);
        }

        [HttpPost]
        public async Task<string> AjaxAddToCart(int listingId)
        {
            await cartService.AddItemToCart(listingId);
            //await cacheService.CacheUser(HttpContext.User.Identity.Name);

            Listing listing = cartService.GetListingByID(listingId);

            return listing.SaleOrDefaultPrice().ToString();
        }

        public async Task<ActionResult> AddToCart(int listingId, string returnUrl)
        {
            await cartService.AddItemToCart(listingId);
            //await cacheService.CacheUser(HttpContext.User.Identity.Name);

            return RedirectToAction("Index", new { returnUrl } );
        }

        [HttpPost]
        public async Task<ActionResult> IncrementCartQuantity(int shoppingId, string returnUrl)
        {
            await cartService.IncrementCartQuantity(shoppingId);
            //await cacheService.CacheUser(HttpContext.User.Identity.Name);
            ModelState.Clear();

            return RedirectToAction("Index", new { returnUrl });
        }

        [HttpPost]
        public async Task<ActionResult> DecrementCartQuantity(int shoppingId, string returnUrl)
        {
            await cartService.DecrementCartQuantity(shoppingId);
            //await cacheService.CacheUser(HttpContext.User.Identity.Name);
            ModelState.Clear();

            return RedirectToAction("Index", new { returnUrl });
        }

        public ActionResult RemoveItem(int shoppingId, string returnUrl)
        {
            cartService.DeleteShoppingCartEntry(shoppingId);
            //cacheService.CacheUserSynch(HttpContext.User.Identity.Name);

            return RedirectToAction("Index", new { returnUrl });
        }

        public async Task<ActionResult> EmptyCart(string returnUrl)
        {
            await cartService.DeleteShoppingCart();
            //await cacheService.CacheUser(HttpContext.User.Identity.Name);

            return RedirectToAction("Index", new { returnUrl });
        }

        public ActionResult ModifyCartQuantity(int shoppingId, int newValue, string returnUrl)
        {
            cartService.UpdateShoppingCartEntry(shoppingId, newValue);
            //cacheService.CacheUserSynch(HttpContext.User.Identity.Name);

            return RedirectToAction("Index", new { returnUrl });
        }
        
        public async Task<ActionResult> Purchase(string returnUrl)
        {
            Order order = await cartService.CreateOrder();
            //await cacheService.CacheUser(HttpContext.User.Identity.Name);
            //cacheService.ForceCacheListings();

            Listing deal = cartService.GetDailyDeal();

            if (deal != null && deal.Quantity == 0)
            {
                Hangfire.RecurringJob.Trigger("daily-roll-over");
            }

            if (order == null)  return RedirectToAction("Index");
            else                return RedirectToAction("Success", new { id = order.OrderID });
        }

        public async Task<ActionResult> Success(string id)
        {
            PurchaseViewModel model = new PurchaseViewModel();

            int orderId = System.Int32.Parse(id);

            model.LoggedInUser = await cartService.GetCurrentUser();

            model.Order = model.LoggedInUser.Orders.SingleOrDefault(o => o.OrderID == orderId);

            if (model.Order == null)
            {
                return RedirectToAction("Orders", "Account", null);
            }

            model.Order.ProductOrderEntries = model.Order.ProductOrderEntries.OrderBy(p => p.Listing.ListingName).ToList();
            model.FullNavList = CreateCartViewModelNavList();

            return View(model);
        }

        public List<NavGrouping> CreateCartViewModelNavList()
        {
            List<NavGrouping> grouping = new List<NavGrouping>();

            NavGrouping actions = new NavGrouping();
            actions.GroupingHeader = "Navigation";

            NavItem store = new NavItem();
            store.Destination = "/store";
            store.DestinationName = "Store";
            NavItem account = new NavItem();
            account.DestinationName = "My Account";
            account.Destination = "/account";

            actions.NavItems.Add(account);
            actions.NavItems.Add(store);

            grouping.Add(actions);

            return grouping;
        }
    }
}