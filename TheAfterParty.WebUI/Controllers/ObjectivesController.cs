using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TheAfterParty.Domain.Services;
using TheAfterParty.Domain.Entities;
using System.Threading.Tasks;
using TheAfterParty.WebUI.Models._Nav;
using System.Web.Routing;
using TheAfterParty.WebUI.Models.Objectives;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.WebUI.Controllers
{
    public class ObjectivesController : Controller
    { 
        private IObjectivesService objectiveService;
        private const string objectiveFormID = "objectiveForm";
        private const string indexDestName = "Objectives";
        private const string boostedDestName = "Boosted";
        private const string completedDestName = "Completed";

        public ObjectivesController(IObjectivesService objectiveService)
        {
            this.objectiveService = objectiveService;
            ViewBag.StoreForMID = objectiveFormID;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                objectiveService.SetUserName(User.Identity.Name);
            }
        }
        // GET: CoopShop/Objectives
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ObjectivesIndexViewModel model = new ObjectivesIndexViewModel();

            model.MiscObjectives = objectiveService.GetObjectives().Where(o => o.IsActive && o.Product == null).OrderBy(o => o.Title).ToList();
            model.Objectives = objectiveService.GetObjectives().Where(o => o.IsActive && o.Product != null).OrderBy(o => o.Title).ToList();

            List<String> destNames = new List<String>() { indexDestName };
            await PopulateGetObjectives(model, destNames);
            model.FormName = "Index";
            model.FormID = "";

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ObjectivesIndexViewModel model)
        {
            model.MiscObjectives = objectiveService.GetObjectives().Where(o => o.IsActive && o.Product == null).OrderBy(o => o.Title).ToList();
            model.Objectives = objectiveService.GetObjectives().Where(o => o.IsActive && o.Product != null).OrderBy(o => o.Title).ToList();

            List<String> destNames = new List<String>() { indexDestName };
            await PopulatePostObjectives(model, destNames);
            model.FormName = "Index";
            model.FormID = "";

            ModelState.Clear();

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> MyObjectives()
        {
            CompletedObjectivesViewModel model = new CompletedObjectivesViewModel();

            List<String> destNames = new List<String>() { completedDestName };
            model.FullNavList = CreateNonFormObjectiveNavList(destNames);
            model.LoggedInUser = await objectiveService.GetCurrentUser();

            model.CurrentPage = 1;
            IEnumerable<BalanceEntry> list = model.LoggedInUser.BalanceEntries.Where(e => e.Objective != null).OrderByDescending(e => e.Date);
            model.TotalItems = list.Count();


            if (model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;

                model.BalanceEntries = list.Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.BalanceEntries = list.ToList();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> MyObjectives(CompletedObjectivesViewModel model)
        {
            List<String> destNames = new List<String>() { completedDestName };
            model.FullNavList = CreateNonFormObjectiveNavList(destNames);
            model.LoggedInUser = await objectiveService.GetCurrentUser();

            model.CurrentPage = model.SelectedPage;

            if (model.CurrentPage < 1)
            {
                model.CurrentPage = 1;
            }

            IEnumerable<BalanceEntry> list = model.LoggedInUser.BalanceEntries.Where(e => e.Objective != null).OrderByDescending(e => e.Date);
            model.TotalItems = list.Count();
            
            if (model.LoggedInUser.PaginationPreference != 0)
            {
                model.UserPaginationPreference = model.LoggedInUser.PaginationPreference;

                model.BalanceEntries = list.Skip((model.CurrentPage - 1) * model.UserPaginationPreference).Take(model.UserPaginationPreference).ToList();
            }
            else
            {
                model.BalanceEntries = list.ToList();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Boosted()
        {
            ObjectivesIndexViewModel model = new ObjectivesIndexViewModel();

            model.MiscObjectives = objectiveService.GetObjectives().Where(o => o.IsActive && o.BoostedObjective != null && o.Product == null).ToList();
            model.Objectives = objectiveService.GetObjectives().Where(o => o.IsActive && o.BoostedObjective != null && o.Product != null).ToList();

            List<String> destNames = new List<String>() { boostedDestName };
            await PopulateGetObjectives(model, destNames);

            model.FormName = "Boosted";
            model.FormID = "";

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Boosted(ObjectivesIndexViewModel model)
        {

            model.MiscObjectives = objectiveService.GetObjectives().Where(o => o.IsActive && o.BoostedObjective != null && o.Product == null).ToList();
            model.Objectives = objectiveService.GetObjectives().Where(o => o.IsActive && o.BoostedObjective != null && o.Product != null).ToList();

            List<String> destNames = new List<String>() { boostedDestName };
            await PopulatePostObjectives(model, destNames);

            model.FormName = "Boosted";
            model.FormID = "";

            ModelState.Clear();

            return View("Index", model);
        }

        #region Admin actions

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminObjectives()
        {
            AdminObjectivesViewModel model = new AdminObjectivesViewModel();

            model.Objectives = objectiveService.GetObjectives();
            model.BoostedObjectives = objectiveService.GetBoostedObjectives();
            model.LoggedInUser = await objectiveService.GetCurrentUser();
            model.FullNavList = CreateObjectivesAdminNavList();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddObjective()
        {
            AddEditObjectivesViewModel model = new AddEditObjectivesViewModel();

            model.Objective = new Objective();
            model.LoggedInUser = await objectiveService.GetCurrentUser();
            model.FullNavList = CreateObjectivesAdminNavList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddObjective(AddEditObjectivesViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ProductID != 0)
                {
                    model.Objective.AddProduct(objectiveService.GetProductByID(model.ProductID));
                }
                else if (String.IsNullOrEmpty(model.Objective.Title))
                {
                    model.Objective.Title = "Misc.";
                }
                
                objectiveService.AddObjective(model.Objective);
                model.LoggedInUser = await objectiveService.GetCurrentUser();
                model.FullNavList = CreateObjectivesAdminNavList();

                return View("EditObjective", model);
            }
            else
            {
                model.LoggedInUser = await objectiveService.GetCurrentUser();
                model.FullNavList = CreateObjectivesAdminNavList();

                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditObjective(int id)
        {
            AddEditObjectivesViewModel model = new AddEditObjectivesViewModel();

            model.Objective = objectiveService.GetObjectiveByID(id);
            model.LoggedInUser = await objectiveService.GetCurrentUser();
            model.FullNavList = CreateObjectivesAdminNavList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditObjective(AddEditObjectivesViewModel model)
        {
            if (ModelState.IsValid)
            {
                objectiveService.EditObjective(model.Objective, model.ProductID);
            }

            model.LoggedInUser = await objectiveService.GetCurrentUser();
            model.FullNavList = CreateObjectivesAdminNavList();

            ModelState.Clear();

            return View(model);
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddBoostedObjective()
        {
            AddEditBoostedObjectiveViewModel model = new AddEditBoostedObjectiveViewModel();

            model.BoostedObjective = new BoostedObjective();
            model.LoggedInUser = await objectiveService.GetCurrentUser();
            model.FullNavList = CreateObjectivesAdminNavList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddBoostedObjective(AddEditBoostedObjectiveViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.BoostedObjective.EndDate = DateTime.Today.AddDays(model.DaysToAdd);

                objectiveService.AddBoostedObjective(model.BoostedObjective);
                model.LoggedInUser = await objectiveService.GetCurrentUser();
                model.FullNavList = CreateObjectivesAdminNavList();

                return View("EditBoostedObjective", model);
            }
            else
            {
                model.LoggedInUser = await objectiveService.GetCurrentUser();
                model.FullNavList = CreateObjectivesAdminNavList();

                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditBoostedObjective(int id)
        {
            AddEditBoostedObjectiveViewModel model = new AddEditBoostedObjectiveViewModel();

            model.BoostedObjective = objectiveService.GetBoostedObjectiveByID(id);
            model.LoggedInUser = await objectiveService.GetCurrentUser();
            model.FullNavList = CreateObjectivesAdminNavList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditBoostedObjective(AddEditBoostedObjectiveViewModel model)
        {
            if (ModelState.IsValid)
            {
                objectiveService.EditBoostedObjective(model.BoostedObjective, model.DaysToAdd);
            }

            model.LoggedInUser = await objectiveService.GetCurrentUser();
            model.FullNavList = CreateObjectivesAdminNavList();

            ModelState.Clear();

            return View(model);
        }

        public ActionResult DeleteBoostedObjective(int id)
        {
            objectiveService.DeleteBoostedObjective(id);

            return RedirectToAction("AdminObjectives");
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddObjectiveWithProductID(int id)
        {
            AddEditObjectivesViewModel model = new AddEditObjectivesViewModel();

            model.Objective = new Objective();
            model.LoggedInUser = await objectiveService.GetCurrentUser();
            model.FullNavList = CreateObjectivesAdminNavList();
            model.ProductID = id;

            return View("AddObjective", model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddBoostedObjectiveWithObjectiveID(int id)
        {
            AddEditBoostedObjectiveViewModel model = new AddEditBoostedObjectiveViewModel();

            model.BoostedObjective = new BoostedObjective();
            model.LoggedInUser = await objectiveService.GetCurrentUser();
            model.FullNavList = CreateObjectivesAdminNavList();
            model.BoostedObjective.BoostedObjectiveID = id;

            return View("AddBoostedObjective", model);
        }

#endregion

        public async Task PopulateGetObjectives(ObjectivesIndexViewModel model, List<String> destNames)
        {
            if (User.Identity.IsAuthenticated)
            {
                model.LoggedInUser = await objectiveService.GetCurrentUser();
            }

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                model.FullNavList = CreateObjectivesAdminNavList(destNames);
            }
            else
            {
                model.FullNavList = CreateObjectivesNavList(destNames);
            }

            if (model.Objectives != null)
            {
                List<SelectedTagMapping> tagMappings = new List<SelectedTagMapping>();

                foreach (Tag tag in objectiveService.GetTags())
                {
                    if (model.Objectives.Any(o => o.Product.HasTag(tag)))
                    {
                        tagMappings.Add(new SelectedTagMapping(tag, false));
                    }
                }

                model.SelectedTagMappings = tagMappings.OrderBy(t => t.StoreTag.TagName).ToList();
            }
        }

        public async Task PopulatePostObjectives(ObjectivesIndexViewModel model, List<String> destNames)
        {
            model.LoggedInUser = await objectiveService.GetCurrentUser();

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                model.FullNavList = CreateObjectivesAdminNavList(destNames);
            }
            else
            {
                model.FullNavList = CreateObjectivesNavList(destNames);
            }

            if (model.Objectives != null)
            {
                if (model.FilterLibrary && User.Identity.IsAuthenticated)
                {
                    model.PreviousFilterLibrary = !model.PreviousFilterLibrary;

                    if (model.PreviousFilterLibrary && HttpContext.User.Identity.IsAuthenticated)
                    {
                        AppUser user = await objectiveService.GetCurrentUser();

                        if (user.OwnedGames != null)
                        {
                            model.Objectives = model.Objectives.Where(o => o.Product == null || user.OwnsProduct(o.Product)).ToList();
                        }
                    }
                }

                model.FilterLibrary = false;
            }
            
            foreach (SelectedTagMapping mapping in model.SelectedTagMappings)
            {
                if (model.TagToChange == mapping.StoreTag.TagID)
                {
                    mapping.IsSelected = !mapping.IsSelected;
                }

                mapping.StoreTag = objectiveService.GetTagByID(mapping.StoreTag.TagID);

                if (mapping.IsSelected && model.Objectives != null)
                {
                    model.Objectives = model.Objectives.Where(o => o.Product.HasTag(mapping.StoreTag)).ToList();
                }
            }

            model.TagToChange = 0;
        }
        
        public List<NavGrouping> CreateObjectivesNavList(List<String> destNames)
        {
            List<NavGrouping> navList = new List<NavGrouping>();

            NavGrouping objective = new NavGrouping();
            objective.GroupingHeader = "Objectives";
            objective.NavItems = new List<NavItem>();
            NavItem objectiveItem = new NavItem();
            objectiveItem.DestinationName = indexDestName;
            objectiveItem.IsFormSubmit = true;
            objectiveItem.FormID = objectiveFormID;
            objectiveItem.FormAction = "/objectives";
            objectiveItem.SetSelected(destNames);
            objective.NavItems.Add(objectiveItem);
            objectiveItem = new NavItem();
            objectiveItem.DestinationName = boostedDestName;
            objectiveItem.IsFormSubmit = true;
            objectiveItem.FormID = objectiveFormID;
            objectiveItem.FormAction = "/objectives/boosted";
            objectiveItem.SetSelected(destNames);
            objective.NavItems.Add(objectiveItem);


            if (User.Identity.IsAuthenticated)
            {
                objectiveItem = new NavItem();
                objectiveItem.Destination = "/objectives/myobjectives";
                objectiveItem.DestinationName = completedDestName;
                objectiveItem.SetSelected(destNames);
                objective.NavItems.Add(objectiveItem);
            }

            navList.Add(objective);

            return navList;
        }

        public List<NavGrouping> CreateNonFormObjectiveNavList(List<String> destNames)
        {
            List<NavGrouping> navList = new List<NavGrouping>();

            NavGrouping objective = new NavGrouping();
            objective.GroupingHeader = "Objectives";
            objective.NavItems = new List<NavItem>();
            NavItem objectiveItem = new NavItem();
            objectiveItem.DestinationName = indexDestName;
            objectiveItem.Destination = "/objectives/";
            objectiveItem.SetSelected(destNames);
            objective.NavItems.Add(objectiveItem);
            objectiveItem = new NavItem();
            objectiveItem.DestinationName = boostedDestName;
            objectiveItem.Destination = "/objectives/boosted";
            objectiveItem.SetSelected(destNames);
            objective.NavItems.Add(objectiveItem);


            if (User.Identity.IsAuthenticated)
            {
                objectiveItem = new NavItem();
                objectiveItem.Destination = "/objectives/myobjectives";
                objectiveItem.DestinationName = completedDestName;
                objectiveItem.SetSelected(destNames);
                objective.NavItems.Add(objectiveItem);
            }

            navList.Add(objective);

            return navList;
        }

        public List<NavGrouping> CreateObjectivesAdminNavList(List<String> destNames = null)
        {
            if (destNames == null)
            {
                destNames = new List<String>();
            }

            List<NavGrouping> navList = CreateObjectivesNavList(destNames);

            NavGrouping admin = new NavGrouping();
            admin.GroupingHeader = "Admin";
            admin.NavItems = new List<NavItem>();
            NavItem adminItem = new NavItem();
            adminItem.Destination = "/Objectives/AdminObjectives";
            adminItem.DestinationName = "View All";
            admin.NavItems.Add(adminItem);
            adminItem = new NavItem();
            adminItem.Destination = "/Objectives/AddObjective";
            adminItem.DestinationName = "Add Objective";
            admin.NavItems.Add(adminItem);
            adminItem = new NavItem();
            adminItem.Destination = "/Objectives/AddBoostedObjective";
            adminItem.DestinationName = "Add Boosted Objective";
            admin.NavItems.Add(adminItem);
            
            navList.Add(admin);

            return navList;
        }
    }
}