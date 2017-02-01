using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Services;
using System.Web.Routing;
using TheAfterParty.WebUI.Models.Auctions;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Controllers
{
    [Authorize]
    public class AuctionsController : Controller
    {
        private IAuctionService auctionService;
        private const string openDestName = "Open Auctions";
        private const string closedDestName = "Closed Auctions";
        private const string mywinningDestName = "My Winning Bids";
        private const string livebidsDestName = "My Current Bids";
        private const string allbidsDestName = "My Bid History";
        private const string myaucsDestName = "My Auctions";

        public AuctionsController(IAuctionService auctionService)
        {
            this.auctionService = auctionService;
        }
        
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (User.Identity.IsAuthenticated)
            {
                auctionService.SetUserName(User.Identity.Name);
            }
        }

        // GET: Auction
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            AuctionsIndexViewModel model = new AuctionsIndexViewModel();

            ViewBag.Title = "Open Auctions";
            model.Auctions = auctionService.GetAuctions().Where(a => a.IsOpen()).ToList();
            List<String> destNames = new List<String>() { openDestName };
            model.FullNavList = CreateAuctionNavList(destNames);
            model.LoggedInUser = await auctionService.GetCurrentUser();

            model.ActionName = "Index";

            PopulateAuctionModel(model);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(AuctionsIndexViewModel model)
        {
            ViewBag.Title = "Open Auctions";
            model.Auctions = auctionService.GetAuctions().Where(a => a.IsOpen()).ToList();
            List<String> destNames = new List<String>() { openDestName };
            model.FullNavList = CreateAuctionNavList(destNames);
            model.LoggedInUser = await auctionService.GetCurrentUser();

            model.ActionName = "Index";

            PopulateAuctionModel(model);

            ModelState.Clear();

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Auction(int id)
        {
            AuctionViewModel model = new AuctionViewModel();

            model.Auction = auctionService.GetAuctionByID(id);
            List<String> destNames = new List<String>();
            if (model.Auction.IsOpen())
            {
                destNames.Add(openDestName);
            }
            else
            {
                destNames.Add(closedDestName);
            }
            model.FullNavList = CreateAuctionNavList(destNames);
            model.LoggedInUser = await auctionService.GetCurrentUser();

            if (model.Auction == null) { return RedirectToAction("Index"); }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Auction(AuctionViewModel model)
        {
            if (model?.Auction?.AuctionID == 0)
            {
                return RedirectToAction("Index");
            }

            List<String> destNames = new List<String>();

            model.Auction = auctionService.GetAuctionByID(model.Auction.AuctionID);

            if (model.Auction.IsOpen())
            {
                destNames.Add(openDestName);
            }
            else
            {
                destNames.Add(closedDestName);
                model.FullNavList = CreateAuctionNavList(destNames);
                model.LoggedInUser = await auctionService.GetCurrentUser();
                return View(model);
            }

            model.FullNavList = CreateAuctionNavList(destNames);
            model.LoggedInUser = await auctionService.GetCurrentUser();

            if (ModelState.IsValidField("AuctionBid.BidAmount") == false)
            {
                ModelState.AddModelError(String.Empty, "Bid amount submitted was not a number.");
                model.AuctionBid.BidAmount = 0;

                return View(model);
            }

            if (model.Auction.IsSilent == false)
            {
                if ((model.Auction.AuctionBids.Count > 0 && (model.Auction.PublicWinningBid() + model.Auction.Increment) > model.AuctionBid.BidAmount)
                    || (model.Auction.AuctionBids.Count == 0 && model.AuctionBid.BidAmount < model.Auction.MinimumBid))
                {
                    ModelState.AddModelError(String.Empty, "Invalid auction bid. Ensure you have enough points and that you bid enough to meet the requirements.");

                    return View(model);
                }
                
                return RedirectToAction("BidConfirmation", new { id = model.Auction.AuctionID, bid = model.AuctionBid.BidAmount });
            }

            model.AuctionBid.BidDate = DateTime.Now;
            model.AuctionBid.AppUser = model.LoggedInUser;
            model.AuctionBid.Auction = model.Auction;

            bool success = auctionService.AddAuctionBid(model.AuctionBid, model.Auction);
            
            //model.AuctionBid = new AuctionBid();

            if (success == false)
            {
                ModelState.AddModelError(String.Empty, "Invalid auction bid. Ensure you have enough points and that you bid enough to meet the requirements.");
            }

            return View(model);
        }

        // bid confirmation page for non-silent auctions
        [HttpGet]
        public async Task<ActionResult> BidConfirmation(int id, int bid)
        {
            if (id == 0 || bid == 0)
            {
                return RedirectToAction("Index");
            }

            BidConfirmationViewModel model = new BidConfirmationViewModel();

            model.FullNavList = CreateAuctionNavList(new List<string>());
            model.LoggedInUser = await auctionService.GetCurrentUser();
            //model.AuctionBid.AddAppUser(model.LoggedInUser);

            model.Auction = auctionService.GetAuctionByID(id);
            model.AuctionBid.Auction = model.Auction;
            model.AuctionBid.BidAmount = bid;
            model.AuctionBid.AppUser = model.LoggedInUser;
            model.AuctionBid.BidDate = DateTime.Now;
            
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> BidConfirmation(BidConfirmationViewModel model)
        {
            if (model.IsConfirmed == false)
            {
                return RedirectToAction("Index");
            }

            model.Auction = auctionService.GetAuctionByID(model.Auction.AuctionID);
            model.AuctionBid.BidDate = DateTime.Now;
            AppUser user = await auctionService.GetCurrentUser();
            model.AuctionBid.AppUser = user;
            model.AuctionBid.Auction = model.Auction;

            bool success = auctionService.AddAuctionBid(model.AuctionBid, model.Auction);
            
            if (success == false)
            {
                ModelState.AddModelError(String.Empty, "Invalid auction bid. Ensure you have enough points and that you bid enough to meet the requirements.");

                return View(model);
            }

            return RedirectToAction("Auction", new { id = model.Auction.AuctionID });
        }

        [HttpGet]
        public async Task<ActionResult> Closed()
        {
            AuctionsIndexViewModel model = new AuctionsIndexViewModel();

            ViewBag.Title = "Closed Auctions";
            model.Auctions = auctionService.GetAuctions().Where(a => a.IsOpen() == false).ToList();
            List<String> destNames = new List<String>() { closedDestName };
            model.FullNavList = CreateAuctionNavList(destNames);
            model.LoggedInUser = await auctionService.GetCurrentUser();

            model.ActionName = "Closed";

            PopulateAuctionModel(model);

            model.Auctions = model.Auctions.OrderByDescending(a => a.EndTime).ToList();

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Closed(AuctionsIndexViewModel model)
        {
            ViewBag.Title = "Closed Auctions";
            model.Auctions = auctionService.GetAuctions().Where(a => a.IsOpen() == false).ToList();
            List<String> destNames = new List<String>() { closedDestName };
            model.FullNavList = CreateAuctionNavList(destNames);
            model.LoggedInUser = await auctionService.GetCurrentUser();

            model.ActionName = "Closed";

            PopulateAuctionModel(model);

            model.Auctions = model.Auctions.OrderByDescending(a => a.EndTime).ToList();

            ModelState.Clear();

            return View("Index", model);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> MyAuctions()
        {
            AuctionsIndexViewModel model = new AuctionsIndexViewModel();

            ViewBag.Title = "My Auctions";
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.Auctions = model.LoggedInUser.Auctions.ToList();
            List<String> destNames = new List<String>() { myaucsDestName };
            model.FullNavList = CreateAuctionNavList(destNames);

            model.ActionName = "MyAuctions";

            PopulateAuctionModel(model);

            return View("Index", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> MyAuctions(AuctionsIndexViewModel model)
        {
            ViewBag.Title = "My Auctions";
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.Auctions = model.LoggedInUser.Auctions.ToList();
            List<String> destNames = new List<String>() { myaucsDestName };
            model.FullNavList = CreateAuctionNavList(destNames);

            model.ActionName = "MyAuctions";

            PopulateAuctionModel(model);

            ModelState.Clear();

            return View("Index", model);
        }
        
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> MyCurrentBids()
        {
            AuctionsIndexViewModel model = new AuctionsIndexViewModel();

            model.ActionName = "MyCurrentBids";

            ViewBag.Title = "My Bids";
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.Auctions = auctionService.GetAuctions().Where(a => a.ContainsBidBy(model.LoggedInUser) && a.IsOpen()).ToList();
            List<String> destNames = new List<String>() { livebidsDestName };
            model.FullNavList = CreateAuctionNavList(destNames);

            PopulateAuctionModel(model);

            return View("Index", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> MyCurrentBids(AuctionsIndexViewModel model)
        {
            ViewBag.Title = "My Bids";
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.Auctions = auctionService.GetAuctions().Where(a => a.ContainsBidBy(model.LoggedInUser) && a.IsOpen()).ToList();
            List<String> destNames = new List<String>() { livebidsDestName };
            model.FullNavList = CreateAuctionNavList(destNames);

            model.ActionName = "MyCurrentBids";

            PopulateAuctionModel(model);

            ModelState.Clear();

            return View("Index", model);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> MyWinningBids()
        {
            AuctionsIndexViewModel model = new AuctionsIndexViewModel();

            ViewBag.Title = "Winning Bids";
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.Auctions = auctionService.GetAuctions().Where(a => a.UserIsWinningBid(model.LoggedInUser) && a.IsSilent == false && a.IsOpen()).ToList();
            List<String> destNames = new List<String>() { mywinningDestName };
            model.FullNavList = CreateAuctionNavList(destNames);

            model.ActionName = "MyWinningBids";

            PopulateAuctionModel(model);

            return View("Index", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> MyWinningBids(AuctionsIndexViewModel model)
        {
            ViewBag.Title = "Winning Bids";
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.Auctions = auctionService.GetAuctions().Where(a => a.UserIsWinningBid(model.LoggedInUser) && a.IsSilent == false && a.IsOpen()).ToList();
            List<String> destNames = new List<String>() { mywinningDestName };
            model.FullNavList = CreateAuctionNavList(destNames);

            model.ActionName = "MyWinningBids";

            PopulateAuctionModel(model);

            ModelState.Clear();

            return View("Index", model);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> MyBidHistory()
        {
            AuctionsIndexViewModel model = new AuctionsIndexViewModel();

            ViewBag.Title = "My Bids";
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.Auctions = auctionService.GetAuctions().Where(a => a.ContainsBidBy(model.LoggedInUser)).ToList();
            List<String> destNames = new List<String>() { allbidsDestName };
            model.FullNavList = CreateAuctionNavList(destNames);

            model.ActionName = "MyBidHistory";

            PopulateAuctionModel(model);

            model.Auctions = model.Auctions.OrderByDescending(a => a.EndTime).ToList();

            return View("Index", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> MyBidHistory(AuctionsIndexViewModel model)
        {
            ViewBag.Title = "My Bids";
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.Auctions = auctionService.GetAuctions().Where(a => a.ContainsBidBy(model.LoggedInUser)).ToList();
            List<String> destNames = new List<String>() { allbidsDestName };
            model.FullNavList = CreateAuctionNavList(destNames);

            model.ActionName = "MyBidHistory";

            PopulateAuctionModel(model);

            model.Auctions = model.Auctions.OrderByDescending(a => a.EndTime).ToList();

            ModelState.Clear();

            return View("Index", model);
        }

        [Authorize]
        public async Task<bool> AjaxSubmitBid(int auctionId, int bid)
        {
            AppUser user = await auctionService.GetCurrentUser(HttpContext.User.Identity.Name);

            Auction auction = auctionService.GetAuctionByID(auctionId);

            if (auction == null || user == null)
            {
                return false;
            }
            
            AuctionBid auctionBid = new AuctionBid();

            auctionBid.BidAmount = bid;
            auctionBid.AppUser = user;
            auctionBid.BidDate = DateTime.Now;
            auctionBid.Auction = auction;
            
            return auctionService.AddAuctionBid(auctionBid, auction);
        }

        #region Admin

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminAuctions()
        {
            AdminAuctionsViewModel model = new AdminAuctionsViewModel();

            model.Auctions = auctionService.GetAuctions().OrderByDescending(a => a.CreatedTime).ToList();
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.FullNavList = CreateAuctionAdminNavList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> AddAuction()
        {
            AddEditAuctionViewModel model = new AddEditAuctionViewModel();
            
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.FullNavList = CreateAuctionAdminNavList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddAuction(AddEditAuctionViewModel model)
        {
            await auctionService.PopulateNewAuction(model.Auction);

            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.FullNavList = CreateAuctionAdminNavList();

            if (ModelState.IsValid)
            {
                auctionService.AddAuction(model.Auction);
                TimeSpan offset = model.Auction.EndTime - DateTime.Now.AddSeconds(-1);

                Hangfire.BackgroundJob.Schedule(() => App_Start.TaskRegister.RegisterAuction(model.Auction.AuctionID), offset);
            }
            else
            {
                return View(model);
            }
            //model.Auction = auctionService.GetAuctionByID(model.Auction.AuctionID);

            return View("EditAuction", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> EditAuction(int id)
        {
            AddEditAuctionViewModel model = new AddEditAuctionViewModel();

            model.Auction = auctionService.GetAuctionByID(id);
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.FullNavList = CreateAuctionAdminNavList();

            if (model.Auction == null) { return View("AdminAuctions"); }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> EditAuction(AddEditAuctionViewModel model)
        {
            model.LoggedInUser = await auctionService.GetCurrentUser();
            model.FullNavList = CreateAuctionAdminNavList();

            if (ModelState.IsValid)
            {
                auctionService.EditAuction(model.Auction);
            }
            else
            {
                model.Auction = auctionService.GetAuctionByID(model.Auction.AuctionID);

                return View(model);
            }

            model.Auction = auctionService.GetAuctionByID(model.Auction.AuctionID);

            ModelState.Clear();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAuction(int id)
        {
            auctionService.DeleteAuction(id);
            
            return RedirectToAction("AdminAuctions");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAuctionBid(int id)
        {
            auctionService.DeleteAuctionBid(id);

            return RedirectToAction("AdminAuctions");
        }

        #endregion

        public void PopulateAuctionModel(AuctionsIndexViewModel model)
        {
            if (model.FilterAll == true)
            {
                model.PreviousFilterPublic = false;
                model.PreviouslyFilterSilent = false;
            }

            if (model.FilterPublic == true)
            {
                model.PreviouslyFilterSilent = false;
                model.PreviousFilterPublic = !model.PreviousFilterPublic;
            }

            if (model.FilterSilent == true)
            {
                model.PreviousFilterPublic = false;
                model.PreviouslyFilterSilent = !model.PreviouslyFilterSilent;
            }

            if (model.PreviousFilterPublic == true)
            {
                model.Auctions = model.Auctions.Where(a => a.IsSilent == false).ToList();
            }

            if (model.PreviouslyFilterSilent == true)
            {
                model.Auctions = model.Auctions.Where(a => a.IsSilent == true).ToList();
            }

            if (String.IsNullOrEmpty(model.SearchText) == false)
            {
                model.Auctions = model.Auctions.Where(a => a.Prize().ToLower().Contains(model.SearchText.Trim().ToLower())).ToList();
            }
            
            model.TotalItems = model.Auctions.Count;

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
                model.Auctions = model.Auctions.OrderBy(a => a.EndTime).Skip((model.CurrentPage - 1) * model.UserPaginationPreference).Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.CurrentPage = 1;
                model.Auctions = model.Auctions.OrderBy(a => a.EndTime).ToList();
            }
        }

        public List<NavGrouping> CreateAuctionNavList(List<String> destNames)
        {
            List<NavGrouping> navList = new List<NavGrouping>();

            NavGrouping grouping = new NavGrouping();

            grouping.GroupingHeader = "Auctions";

            NavItem navItem = new NavItem();

            navItem.Destination = "/auctions/";
            navItem.DestinationName = openDestName;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/auctions/closed";
            navItem.DestinationName = closedDestName;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/auctions/mywinningbids";
            navItem.DestinationName = mywinningDestName;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/auctions/mycurrentbids";
            navItem.DestinationName = livebidsDestName;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/auctions/mybidhistory";
            navItem.DestinationName = allbidsDestName;
            navItem.SetSelected(destNames);
            grouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/auctions/myauctions";
            navItem.DestinationName = myaucsDestName;
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

            List<NavGrouping> navList = CreateAuctionNavList(destNames);

            NavGrouping grouping = new NavGrouping();

            grouping.GroupingHeader = "Admin Actions";

            NavItem navItem = new NavItem();

            navItem.Destination = "/Auctions/AdminAuctions";
            navItem.DestinationName = "Admin Auctions";
            grouping.NavItems.Add(navItem);
            navItem = new NavItem();
            navItem.Destination = "/Auctions/AddAuction";
            navItem.DestinationName = "Add Auction";
            grouping.NavItems.Add(navItem);

            navList.Add(grouping);

            return navList;
        }
    }
}