using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models.Cart;
using TheAfterParty.Domain.Services;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;
using System.Collections.Generic;
using System.Linq;

namespace TheAfterParty.WebUI.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private ICartService cartService;
        
        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            
            cartService.SetUserName(User.Identity.Name);
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

            cartViewModel.FullNavList = CreateCartControllerNavList(cartViewModel.LoggedInUser);

            cartViewModel.CartEntries = cartViewModel.CartEntries.OrderBy(e => e.Listing.ListingName).ToList();

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
                purchase.DestinationName = "Purcase";

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

            Listing listing = cartService.GetListingByID(listingId);
            
            return listing.SaleOrDefaultPrice().ToString();
        }

        public async Task<ActionResult> AddToCart(int listingId, string returnUrl)
        {
            await cartService.AddItemToCart(listingId);

            return RedirectToAction("Index", new { returnUrl } );
        }

        [HttpPost]
        public async Task<ActionResult> IncrementCartQuantity(int shoppingId, string returnUrl)
        {
            await cartService.IncrementCartQuantity(shoppingId);
            ModelState.Clear();

            return RedirectToAction("Index", new { returnUrl });
        }

        [HttpPost]
        public async Task<ActionResult> DecrementCartQuantity(int shoppingId, string returnUrl)
        {
            await cartService.DecrementCartQuantity(shoppingId);
            ModelState.Clear();

            return RedirectToAction("Index", new { returnUrl });
        }

        public ActionResult RemoveItem(int shoppingId, string returnUrl)
        {
            cartService.DeleteShoppingCartEntry(shoppingId);

            return RedirectToAction("Index", new { returnUrl });
        }

        public async Task<ActionResult> EmptyCart(string returnUrl)
        {
            await cartService.DeleteShoppingCart();

            return RedirectToAction("Index", new { returnUrl });
        }

        public ActionResult ModifyCartQuantity(int shoppingId, int newValue, string returnUrl)
        {
            cartService.UpdateShoppingCartEntry(shoppingId, newValue);

            return RedirectToAction("Index", new { returnUrl });
        }
        
        public async Task<ActionResult> Purchase(string returnUrl)
        {
            Order order = await cartService.CreateOrder();

            if (order == null)  return RedirectToAction("Index");
            else                return RedirectToAction("Success", new { id = order.OrderID });
        }

        public async Task<ActionResult> Success(string id)
        {
            PurchaseViewModel model = new PurchaseViewModel();

            int orderId = System.Int32.Parse(id);

            model.Order = cartService.GetOrderByID(orderId);
            model.LoggedInUser = await cartService.GetCurrentUser();
            model.FullNavList = CreatePurchaseViewModelNavList();

            return View(model);
        }

        public List<NavGrouping> CreatePurchaseViewModelNavList()
        {
            List<NavGrouping> grouping = new List<NavGrouping>();

            NavGrouping actions = new NavGrouping();
            actions.GroupingHeader = "Actions";

            NavItem store = new NavItem();
            store.Destination = "/Store";
            store.DestinationName = "Return To Store";

            NavItem account = new NavItem();
            account.DestinationName = "My Account";
            account.Destination = "/Account";

            actions.NavItems.Add(account);
            actions.NavItems.Add(store);

            grouping.Add(actions);

            return grouping;
        }
    }
}