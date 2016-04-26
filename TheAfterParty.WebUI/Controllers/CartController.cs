using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models.Cart;
using TheAfterParty.Domain.Services;
using TheAfterParty.Domain.Entities;

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
        public async Task<ActionResult> Index(string returnUrl)
        {
            CartIndexViewModel cartViewModel = new CartIndexViewModel
            {
                LoggedInUser = await cartService.GetCurrentUser(),
                CartEntries = await cartService.GetShoppingCartEntries(),
                ReturnUrl = returnUrl
            };

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

        // Navbar cart info
        public PartialViewResult ShoppingCart()
        {
            CartLayoutViewModel model = new CartLayoutViewModel();
            model.LoggedInUser = cartService.GetCurrentUserSynch();

            return PartialView("~/Views/Shared/_ShoppingCart.cshtml", model);
        }

        [HttpPost]
        public async Task<ActionResult> AjaxAddToCart(int listingId)
        {
            CartLayoutViewModel model = new CartLayoutViewModel();
            await cartService.AddItemToCart(listingId);

            model.LoggedInUser = await cartService.GetCurrentUser();

            return PartialView("~/Views/Shared/_ShoppingCart.cshtml", model);
        }

        public async Task<ActionResult> AddToCart(int listingId, string returnUrl)
        {
            await cartService.AddItemToCart(listingId);

            return RedirectToAction("Index", new { returnUrl } );
        }

        public ActionResult IncrementCartQuantity(int shoppingId, string returnUrl)
        {
            cartService.IncrementCartQuantity(shoppingId);

            return RedirectToAction("Index", new { returnUrl });
        }

        public ActionResult DecrementCartQuantity(int shoppingId, string returnUrl)
        {
            cartService.DecrementCartQuantity(shoppingId);

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
        
        public async Task<ViewResult> Purchase(string returnUrl)
        {
            Order order = await cartService.CreateOrder();

            return View(order);
        }
    }
}