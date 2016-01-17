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
using TheAfterParty.Domain.Services;

namespace TheAfterParty.WebUI.Areas.Members.Controllers
{
    public class CartController : Controller
    {
        private ICartService cartService;

        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
            cartService.SetUserName(User.Identity.Name);
        }


        // GET: Members/Cart
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

        [Authorize]
        public ActionResult AddToCart(int listingId, string returnUrl)
        {
            cartService.AddItemToCart(listingId);

            return RedirectToAction("Index", new { returnUrl } );
        }

        [Authorize]
        public async Task<RedirectToRouteResult> Checkout(string returnUrl)
        {
            Order order = await cartService.CreateOrder();
        }
    }
}