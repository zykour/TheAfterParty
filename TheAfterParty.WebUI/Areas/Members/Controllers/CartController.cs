using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using Microsoft.AspNet.Identity.Owin;
using TheAfterParty.Domain.Concrete;
using Microsoft.AspNet.Identity.EntityFramework;
using TheAfterParty.WebUI.Areas.Members.Models.Cart;

namespace TheAfterParty.WebUI.Areas.Members.Controllers
{
    public class CartController : Controller
    {
        private IListingRepository listingRepository;
        private IUserRepository userRepository;
        public AppUserManager UserManager { get; private set; }

        public CartController(IListingRepository listingRepository, IUserRepository userRepository) : this(new AppUserManager(new UserStore<AppUser>(listingRepository.GetContext())))
        {
            this.listingRepository = listingRepository;
            this.userRepository = userRepository;
        }
        protected CartController(AppUserManager userManager = null)
        {
            if (userManager == null)
                userManager = HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

            UserManager = userManager;
        }


        // GET: Members/Cart
        public async Task<ActionResult> Index(string returnUrl)
        {
            AppUser user = await GetCurrentUser();

            CartIndexViewModel cartViewModel = new CartIndexViewModel
            {
                LoggedInUser = user,
                CartEntries = user.ShoppingCartEntries,
                ReturnUrl = returnUrl
            };

            return View(cartViewModel);
        }

        [Authorize]
        public async Task<RedirectToRouteResult> AddToCart(int listingId, string returnUrl)
        {
            Listing listing = listingRepository.GetListingByID(listingId);//.FirstOrDefault(l => l.ListingID == listingId);
            
            if (listing != null)
            {
                AppUser currentUser = await GetCurrentUser();

                ShoppingCartEntry entry = userRepository.GetShoppingCartEntries().Where(e => e.ListingID == listingId && Object.Equals(currentUser.Id, e.UserID)).SingleOrDefault();

                if (entry == null)
                {
                    userRepository.InsertShoppingCartEntry(currentUser.AddShoppingCartEntry(listing));
                }
                else
                {
                    entry.Quantity++;
                    userRepository.UpdateShoppingCartEntry(entry);
                }

                userRepository.Save();
            }

            return RedirectToAction("Index", new { returnUrl } );
        }

        /*[Authorize]
        public async Task<RedirectToRouteResult> Checkout(string returnUrl)
        {
            AppUser user = await GetCurrentUser();
            DateTime orderDate = DateTime.Now;
            Order order; 

            if (!user.AssertValidOrder())
            {
                return RedirectToAction("Index", "Cart");
            }
            
            order = new Order(user, orderDate);

            ICollection<ShoppingCartEntry> cartEntries = user.ShoppingCartEntries.Where(entry => Object.Equals(entry.UserID, user.Id)).ToList();

            foreach (ShoppingCartEntry entry in cartEntries)
            {
                for (int i = 0; i < entry.Quantity; i++)
                {

                    //return new ClaimedProductKey(listing.RemoveProductKey(listing.ListingID), this, dateClaimed, note);
                    ClaimedProductKey claimedKey = new ClaimedProductKey(entry.Listing, orderDate, "Purchase - Order #" + order.TransactionID);
                    ProductOrderEntry orderEntry = new ProductOrderEntry(order, entry, claimedKey);
                }
            }

            repository.SaveOrder(order);

            return RedirectToAction("Success", new { returnUrl });
        }*/

        private async Task<AppUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsync(User.Identity.Name);
        }
    }
}