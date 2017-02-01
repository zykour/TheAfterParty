using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models.Home;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;
using System.Threading.Tasks;
using System.Web.Routing;
using TheAfterParty.Domain.Services;
using CodeKicker.BBCode;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private ISiteService siteService;
        private const string indexActionDest = "Home";

        public HomeController(ISiteService siteService)
        {
            this.siteService = siteService;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            siteService.SetUserName(User.Identity.Name);
        }

        // GET: /Home
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            HomeIndexViewModel model = new HomeIndexViewModel();

            model.Parser = CreateBBCodeParser();
            
            model.Initialize((await siteService.GetCurrentUser()), CreateHomeNavList(new List<string>() { indexActionDest }));
            model.ActivityFeedList = model.SkipAndTake<ActivityFeedContainer>(siteService.GetSiteActivityFeedItems());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(HomeIndexViewModel model)
        {
            model.Parser = CreateBBCodeParser();

            model.Initialize((await siteService.GetCurrentUser()), CreateHomeNavList(new List<string>() { indexActionDest }));
            model.ActivityFeedList = model.SkipAndTake<ActivityFeedContainer>(siteService.GetSiteActivityFeedItems());

            ModelState.Clear();

            return View(model);
        }

        #region Admin

        #region GroupEvent

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminGroupEvents()
        {
            AdminGroupEventsViewModel model = new AdminGroupEventsViewModel();

            model.GroupEvents = siteService.GetGroupEvents().OrderByDescending(g => g.EventDate).ToList();
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AddGroupEvent(string id)
        {
            AddEditGroupEventViewModel model = new AddEditGroupEventViewModel();

            model.GroupEvent.AppUserID = id;
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddGroupEvent(AddEditGroupEventViewModel model)
        {
            if (model.AppID != 0)
            {
                model.GroupEvent.AddProduct(siteService.GetProductByAppID(model.AppID));
            }
            else if (String.IsNullOrEmpty(model.Product.ProductName) == false)
            {
                model.GroupEvent.AddProduct(model.Product);
            }
            else if (model.GroupEvent?.ProductID != 0)
            {
                model.GroupEvent.AddProduct(siteService.GetProductByID((int)model.GroupEvent.ProductID));
            }

            model.GroupEvent.AppUser = await siteService.GetAppUserByID(model.GroupEvent.AppUserID);
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            if (ModelState.IsValid)
            {
                siteService.AddGroupEvent(model.GroupEvent);
            }
            else
            {
                return View(model);
            }

            model.GroupEvent = siteService.GetGroupEventByID(model.GroupEvent.GroupEventID);

            return View("EditGroupEvent", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> EditGroupEvent(int id)
        {
            AddEditGroupEventViewModel model = new AddEditGroupEventViewModel();

            model.GroupEvent = siteService.GetGroupEventByID(id);
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            if (model.GroupEvent == null) { return View("AdminGroupEvents"); }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> EditGroupEvent(AddEditGroupEventViewModel model)
        {
            Product product = new Product();

            if (model.AppID != 0)
            {
                product = siteService.GetProductByAppID(model.AppID);
            }
            else if (String.IsNullOrEmpty(model.Product.ProductName) == false)
            {
                product = model.Product;
            }
            else if (model.GroupEvent?.ProductID != 0)
            {
                product = siteService.GetProductByID((int)model.GroupEvent.ProductID);
            }

            if (ModelState.IsValid)
            {
                siteService.EditGroupEvent(model.GroupEvent, product);
            }

            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            ModelState.Clear();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteGroupEvent(int id)
        {
            siteService.DeleteGroupEvent(id);

            return RedirectToAction("AdminGroupEvents");
        }

        #endregion

        #region POTW

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AdminPOTWs()
        {
            AdminPOTWsViewModel model = new AdminPOTWsViewModel();

            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            model.CurrentPage = 1;
            model.TotalItems = siteService.GetPOTWs().Count();

            if (model.LoggedInUser != null && model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                model.POTWs = siteService.GetPOTWs().OrderByDescending(p => p.StartDate).Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.POTWs = siteService.GetPOTWs().OrderByDescending(p => p.StartDate).ToList();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AdminPOTWs(AdminPOTWsViewModel model)
        {
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            if (model.SelectedPage != 0)
            {
                model.CurrentPage = model.SelectedPage;
            }

            model.TotalItems = siteService.GetPOTWs().Count();

            if (model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                model.POTWs = siteService.GetPOTWs().OrderByDescending(p => p.StartDate).Skip((model.CurrentPage - 1) * model.LoggedInUser.PaginationPreference).Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.POTWs = siteService.GetPOTWs().OrderByDescending(p => p.StartDate).ToList();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AddPOTW(string id)
        {
            AddEditPOTWViewModel model = new AddEditPOTWViewModel();

            model.POTW.AppUserID = id;
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddPOTW(AddEditPOTWViewModel model)
        {
            model.POTW.AppUser = await siteService.GetAppUserByID(model.POTW.AppUserID);

            if (ModelState.IsValid)
            {
                siteService.AddPOTW(model.POTW);
            }

            model.POTW = siteService.GetPOTWByID(model.POTW.POTWID);
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            return View("EditPOTW", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> EditPOTW(int id)
        {
            AddEditPOTWViewModel model = new AddEditPOTWViewModel();

            model.POTW = siteService.GetPOTWByID(id);
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            if (model.POTW == null) { return View("AdminPOTWs"); }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> EditPOTW(AddEditPOTWViewModel model)
        {
            if (ModelState.IsValid)
            {
                siteService.EditPOTW(model.POTW);
            }

            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();
            model.POTW = siteService.GetPOTWByID(model.POTW.POTWID);

            ModelState.Clear();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeletePOTW(int id)
        {
            siteService.DeletePOTW(id);

            return RedirectToAction("AdminPOTWs");
        }

        #endregion

        #region SiteNotification

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminSiteNotifications()
        {
            AdminSiteNotificationsViewModel model = new AdminSiteNotificationsViewModel();

            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            model.CurrentPage = 1;
            model.TotalItems = siteService.GetSiteNotifications().Count();

            if (model.LoggedInUser != null && model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                model.SiteNotifications = siteService.GetSiteNotifications().OrderByDescending(p => p.NotificationDate).Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.SiteNotifications = siteService.GetSiteNotifications().OrderByDescending(p => p.NotificationDate).ToList();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AdminSiteNotifications(AdminSiteNotificationsViewModel model)
        {
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            if (model.SelectedPage != 0)
            {
                model.CurrentPage = model.SelectedPage;
            }

            model.TotalItems = siteService.GetSiteNotifications().Count();

            if (model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                model.SiteNotifications = siteService.GetSiteNotifications().OrderByDescending(p => p.NotificationDate).Skip((model.CurrentPage - 1) * model.LoggedInUser.PaginationPreference).Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.SiteNotifications = siteService.GetSiteNotifications().OrderByDescending(p => p.NotificationDate).ToList();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AddSiteNotification()
        {
            AddEditSiteNotificationViewModel model = new AddEditSiteNotificationViewModel();

            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddSiteNotification(AddEditSiteNotificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                siteService.AddSiteNotification(model.SiteNotification);
            }

            model.SiteNotification = siteService.GetSiteNotificationByID(model.SiteNotification.SiteNotificationID);
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            return View("EditSiteNotification", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> EditSiteNotification(int id)
        {
            AddEditSiteNotificationViewModel model = new AddEditSiteNotificationViewModel();

            model.SiteNotification = siteService.GetSiteNotificationByID(id);
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            if (model.SiteNotification == null) { return View("AdminSiteNotifications"); }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> EditSiteNotification(AddEditSiteNotificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                siteService.EditSiteNotification(model.SiteNotification);
            }
            
            model.LoggedInUser = await siteService.GetCurrentUser();
            model.FullNavList = CreateHomeAdminNavList();

            ModelState.Clear();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteSiteNotification(int id)
        {
            siteService.DeleteSiteNotification(id);

            return RedirectToAction("AdminSiteNotifications");
        }

        #endregion

#endregion

        public static BBCodeParser CreateBBCodeParser()
        {            
            return new BBCodeParser(new[]
            {
                new BBTag("b", "<b>", "</b>"),
                    new BBTag("i", "<span style=\"font-style:italic;\">", "</span>"),
                    new BBTag("u", "<span style=\"text-decoration:underline;\">", "</span>"),
                    new BBTag("code", "<pre class=\"prettyprint\">", "</pre>"),
                    new BBTag("ptext", "<span class=\"text-purple\">", "</span>"),
                    new BBTag("gtext", "<span class=\"text-success\">", "</span>"),
                    new BBTag("boosted", "<span class=\"fa fa-fw fa-lg fa-exclamation-circle text-purple\">", "</span>"),
                    new BBTag("daily", "<span class=\"fa fa-fw fa-lg fa-sun-o icon-yellow\">", "</span>"),
                    new BBTag("weekly", "<span class=\"fa fa-fw fa-lg fa-calendar text-purple\">", "</span>"),
                    new BBTag("new", "<span class=\"fa fa-fw fa-lg fa-plus-circle icon-green\">", "</span>"),
                    new BBTag("img", "<img src=\"${content}\" />", "", false, true),
                    new BBTag("quote", "<blockquote>", "</blockquote>"),
                    new BBTag("list", "<ul>", "</ul>"),
                    new BBTag("*", "<li>", "</li>", true, false),
                    new BBTag("url", "<a href=\"${href}\">", "</a>", new BBAttribute("href", ""), new BBAttribute("href", "href")),
                    new BBTag("purl", "<a href=\"${href}\" class=\"text-purple\">", "</a>", new BBAttribute("href", ""), new BBAttribute("href", "href")),
            });
        }
        
        public List<NavGrouping> CreateHomeNavList(List<String> destNames)
        {
            List<NavGrouping> navList = new List<NavGrouping>();

            NavGrouping navGrouping = new NavGrouping();
            NavItem navItem = new NavItem();

            navGrouping.GroupingHeader = "Site";

            navItem.Destination = "/home/";
            navItem.DestinationName = indexActionDest;
            navItem.SetSelected(destNames);
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/store/";
            navItem.DestinationName = "Store";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/user/";
            navItem.DestinationName = "Users";
            navGrouping.NavItems.Add(navItem);

            navList.Add(navGrouping);

            return navList;
        }

        public List<NavGrouping> CreateHomeAdminNavList(List<String> destNames = null)
        {
            if (destNames == null)
            {
                destNames = new List<String>();
            }

            List<NavGrouping> navList = CreateHomeNavList(destNames);

            NavGrouping navGrouping = new NavGrouping();
            NavItem navItem = new NavItem();

            navGrouping.GroupingHeader = "Admin";

            navItem.Destination = "/home";
            navItem.DestinationName = "Home";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/home/admingroupevents";
            navItem.DestinationName = "View Events";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/home/adminPOTWs";
            navItem.DestinationName = "View POTWs";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/Home/AdminSiteNotifications";
            navItem.DestinationName = "View Notifications";
            navGrouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/Home/AddSiteNotification";
            navItem.DestinationName = "Add Notification";
            navGrouping.NavItems.Add(navItem);

            navList.Add(navGrouping);

            return navList;
        }
    }
}