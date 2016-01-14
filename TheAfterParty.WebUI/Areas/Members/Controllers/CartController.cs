using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Areas.Members.Controllers
{
    public class CartController : Controller
    {
        private IRepository repository;

        public CartController(IRepository repository)
        {
            this.repository = repository;
        }
        
        public RedirectToRouteResult AddToCart(int listingId, string returnUrl)
        {
            Listing listing = repository.Listings.FirstOrDefault(l => l.ListingID == listingId);
            
            if (listing != null)
            {
                //repository.SaveShoppingCartEntry()
            }

            return RedirectToAction("Index", new { returnUrl });
        }
    }
}