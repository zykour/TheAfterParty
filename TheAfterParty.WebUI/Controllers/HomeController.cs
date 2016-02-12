using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models.Home;

namespace TheAfterParty.WebUI.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Test()
        {
            HomeIntStringBool a = new HomeIntStringBool();
            a.MyIntString.MyInt = 5;
            a.MyIntString.MyString = "Fiver";
            a.IsSelected = true;

            HomeIntStringBool b = new HomeIntStringBool();
            b.MyIntString.MyInt = 2;
            b.MyIntString.MyString = "Twofer";
            b.IsSelected = false;

            HomeTestModel abc = new HomeTestModel();
            abc.MyIntStringBools.Add(a);
            abc.MyIntStringBools.Add(b);

            abc.TestInt = 2;

            return View(abc);
        }

        [HttpPost]
        public ActionResult Test(HomeTestModel model)
        {
            if (model.TestInt != 0)
                model.PreviousInt = model.TestInt;

            ModelState.Clear();

            return View(model);
        }
    }
}