using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.Domain.Abstract;

namespace TheAfterParty.WebUI.Areas.CoopShop.Controllers
{
    public class StoreController : Controller
    {
        private IRepository repository;

        public StoreController(IRepository repository)
        {
            this.repository = repository;
        }

        // GET: CoopShop/Store
        public ActionResult Index()
        {
            return View(repository.Products);    //repository.StockedProducts);
        }
    }
}