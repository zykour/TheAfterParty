using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models.Giveaways;
using System.Web.Routing;
using TheAfterParty.Domain.Services;
using System.Threading.Tasks;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;


namespace TheAfterParty.WebUI.Controllers
{
    public class GivesController : Controller
    {
        private IUserService userService;

        private const string vanityText = "Gives";
        private const string createGivesText = "Create Give";
        private const string openGivesText = "Open " + vanityText;
        private const string closedGivesText = "Closed " + vanityText;
        private const string myGivesText = "My " + vanityText;
        private const string myEntriesText = "My Entries";
        private const string myTakesText = "My Takes";

        public GivesController(IUserService userService)
        {
            this.userService = userService;
        }


        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (User.Identity.IsAuthenticated)
            {
                userService.SetUserName(User.Identity.Name);
            }
        }

        // GET: Gives
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            GiveawaysViewModel model = new GiveawaysViewModel();

            ViewBag.Title = "Open Gives";
            model.Giveaways = userService.GetGiveaways().Where(g => g.IsOpen()).ToList();
            List<String> destNames = new List<String>() { openGivesText };
            model.FullNavList = CreateGiveawayNavList(destNames);
            model.LoggedInUser = await userService.GetCurrentUser();

            model.ActionName = "Index";

            PopulateGiveawayModel(model);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(GiveawaysViewModel model)
        {
            ViewBag.Title = "Open Gives";
            model.Giveaways = userService.GetGiveaways().Where(g => g.IsOpen()).ToList();
            List<String> destNames = new List<String>() { openGivesText };
            model.FullNavList = CreateGiveawayNavList(destNames);
            model.LoggedInUser = await userService.GetCurrentUser();

            model.ActionName = "Index";

            PopulateGiveawayModel(model);

            ModelState.Clear();

            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> Closed()
        {
            GiveawaysViewModel model = new GiveawaysViewModel();

            ViewBag.Title = "Closed Gives";
            model.Giveaways = userService.GetGiveaways().Where(a => a.IsOpen() == false).ToList();
            List<String> destNames = new List<String>() { closedGivesText };
            model.FullNavList = CreateGiveawayNavList(destNames);
            model.LoggedInUser = await userService.GetCurrentUser();

            model.ActionName = "Closed";

            PopulateGiveawayModel(model);

            model.Giveaways = model.Giveaways.OrderByDescending(a => a.EndDate).ToList();

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Closed(GiveawaysViewModel model)
        {
            ViewBag.Title = "Closed Auctions";
            model.Giveaways = userService.GetGiveaways().Where(a => a.IsOpen() == false).ToList();
            List<String> destNames = new List<String>() { closedGivesText };
            model.FullNavList = CreateGiveawayNavList(destNames);
            model.LoggedInUser = await userService.GetCurrentUser();

            model.ActionName = "Closed";

            PopulateGiveawayModel(model);

            model.Giveaways = model.Giveaways.OrderByDescending(a => a.EndDate).ToList();

            ModelState.Clear();

            return View("Index", model);
        }

        [HttpGet]
        public async Task<ActionResult> Giveaway(int id)
        {
            GiveawayViewModel model = new GiveawayViewModel();

            model.Giveaway = userService.GetGiveawayByID(id);
            List<String> destNames = new List<String>();

            if (model.Giveaway.IsOpen())
            {
                destNames.Add(openGivesText);
            }
            else
            {
                destNames.Add(closedGivesText);
            }

            model.FullNavList = CreateGiveawayNavList(destNames);
            model.LoggedInUser = await userService.GetCurrentUser();

            if (model.Giveaway == null) { return RedirectToAction("Index"); }

            return View(model);
        }

        /*[HttpGet]
        public async Task<ActionResult> Create()
        {
            GiveawayViewModel model = new GiveawayViewModel();
            
            List<String> destNames = new List<String>();
            
            destNames.Add(createGivesText);
            model.FullNavList = CreateGiveawayNavList(destNames);
            model.LoggedInUser = await userService.GetCurrentUser();
            
            return View(model);
        }*/

        [HttpGet]
        public async Task<ActionResult> Create(int ClaimedProductKeyID = 0)
        {
            GiveawayViewModel model = new GiveawayViewModel();

            model.LoggedInUser = await userService.GetCurrentUser();

            if (ClaimedProductKeyID != 0)
            {
                ClaimedProductKey key = model.LoggedInUser.ClaimedProductKeys.SingleOrDefault(x => x.ClaimedProductKeyID == ClaimedProductKeyID);

                if (key != null)
                {
                    model.Giveaway.Listing = key.Listing;
                    model.Giveaway.ListingID = key.ListingID;
                    model.Giveaway.Prize = key.Key;
                }
            }

            /* ADD KEY TO GIVEAWAY, REMOVE USELESS FIELDS */

            List<String> destNames = new List<String>();

            destNames.Add(createGivesText);
            model.FullNavList = CreateGiveawayNavList(destNames);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(GiveawayViewModel model)
        {
            List<String> destNames = new List<String>();

            model.FullNavList = CreateGiveawayNavList(destNames);
            model.LoggedInUser = await userService.GetCurrentUser();


            if (model.Giveaway == null) { return View(model); }

            // link data (user, created time), validate model

            userService.AddGiveaway(model.Giveaway);

            // change this based on success (Create vs. Giveaway)
            destNames.Add(createGivesText);

            // get new ID
            int id = 0;
            
            return RedirectToAction("Giveaway", new { id });
        }

        public void PopulateGiveawayModel(GiveawaysViewModel model)
        {

            if (String.IsNullOrEmpty(model.SearchText) == false)
            {
                model.Giveaways = model.Giveaways.Where(a => a.PrizeText().ToLower().Contains(model.SearchText.Trim().ToLower())).ToList();
            }

            model.TotalItems = model.Giveaways.Count;

            if (model.LoggedInUser.PaginationPreference != 0)
            {
                if (model.SelectedPage != 0)
                {
                    model.CurrentPage = model.SelectedPage;
                }
                if (model.CurrentPage < 1)
                {
                    model.CurrentPage = 1;
                }
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;
                if (model.ActionName.CompareTo("Closed") == 0)
                {
                    model.Giveaways = model.Giveaways.OrderByDescending(a => a.EndDate).Skip((model.CurrentPage - 1) * model.UserPaginationPreference).Take(model.UserPaginationPreference).ToList();
                }
                else
                {
                    model.Giveaways = model.Giveaways.OrderBy(a => a.EndDate).Skip((model.CurrentPage - 1) * model.UserPaginationPreference).Take(model.UserPaginationPreference).ToList();
                }
            }
            else
            {
                model.CurrentPage = 1;
                model.Giveaways = model.Giveaways.OrderBy(a => a.EndDate).ToList();
            }
        }

        public List<NavGrouping> CreateGiveawayNavList(List<String> destNames)
        {
            List<NavGrouping> navList = new List<NavGrouping>();

            NavGrouping grouping = new NavGrouping();

            grouping.GroupingHeader = "Actions";

            NavItem navItem = new NavItem();

            navItem.Destination = "/gives/create";
            navItem.DestinationName = createGivesText;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);

            navList.Add(grouping);

            grouping.GroupingHeader = "Gives";

            navItem = new NavItem();

            navItem.Destination = "/gives/";
            navItem.DestinationName = openGivesText;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/gives/closed";
            navItem.DestinationName = closedGivesText;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/gives/entries";
            navItem.DestinationName = myEntriesText;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/gives/takes";
            navItem.DestinationName = myTakesText;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/gives/my-gives";
            navItem.DestinationName = myGivesText;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);

            navList.Add(grouping);

            return navList;
        }

        public List<NavGrouping> CreateAuctionAdminNavList(List<String> destNames = null)
        {
            if (destNames == null)
            {
                destNames = new List<String>();
            }

            List<NavGrouping> navList = CreateGiveawayNavList(destNames);

            NavGrouping grouping = new NavGrouping();

            grouping.GroupingHeader = "Admin Gives";

            NavItem navItem = new NavItem();

            //navItem.Destination = "/Auctions/AdminAuctions";
            //navItem.DestinationName = "Admin Auctions";
            //grouping.NavItems.Add(navItem);
            //navItem = new NavItem();
            //navItem.Destination = "/Auctions/AddAuction";
            //navItem.DestinationName = "Add Auction";
            //grouping.NavItems.Add(navItem);

            navList.Add(grouping);

            return navList;
        }
    }
}