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
using TheAfterParty.WebUI.Models.Store; // borrow the selectedtagmapping class

namespace TheAfterParty.WebUI.Controllers
{
    public class ObjectivesController : Controller
    { 
        private IObjectivesService objectiveService;
        private const string objectiveFormID = "objectiveForm";

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

            model.Objectives = objectiveService.GetObjectives().Where(o => o.IsActive).OrderBy(o => o.Title).ToList();

            await PopulateGetObjectives(model);
            model.FormName = "Index";
            model.FormID = "";

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ObjectivesIndexViewModel model)
        {
            model.Objectives = objectiveService.GetObjectives().Where(o => o.IsActive).OrderBy(o => o.Title).ToList();

            await PopulatePostObjectives(model);
            model.FormName = "Index";
            model.FormID = "";

            ModelState.Clear();

            return View(model);
        }

        public async Task<ActionResult> MyObjectives()
        {
            CompletedObjectivesViewModel model = new CompletedObjectivesViewModel();

            model.FullNavList = CreateObjectivesNavList();
            model.LoggedInUser = await objectiveService.GetCurrentUser();

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Boosted()
        {
            ObjectivesIndexViewModel model = new ObjectivesIndexViewModel();

            model.Objectives = objectiveService.GetObjectives().Where(o => o.IsActive && o.BoostedObjective != null).ToList();

            await PopulateGetObjectives(model);

            model.FormName = "Boosted";
            model.FormID = "";

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Boosted(ObjectivesIndexViewModel model)
        {
            model.Objectives = objectiveService.GetObjectives().Where(o => o.IsActive && o.BoostedObjective != null).ToList();

            await PopulatePostObjectives(model);

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

            return View(model);
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

        public async Task PopulateGetObjectives(ObjectivesIndexViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                model.LoggedInUser = await objectiveService.GetCurrentUser();
            }

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                model.FullNavList = CreateObjectivesAdminNavList();
            }
            else
            {
                model.FullNavList = CreateObjectivesNavList();
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

        public async Task PopulatePostObjectives(ObjectivesIndexViewModel model)
        {
            model.LoggedInUser = await objectiveService.GetCurrentUser();

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                model.FullNavList = CreateObjectivesAdminNavList();
            }
            else
            {
                model.FullNavList = CreateObjectivesNavList();
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

        public List<NavGrouping> CreateObjectivesNavList()
        {
            List<NavGrouping> navList = new List<NavGrouping>();

            NavGrouping objective = new NavGrouping();
            objective.GroupingHeader = "Objectives";
            objective.NavItems = new List<NavItem>();
            NavItem objectiveItem = new NavItem();
            objectiveItem.DestinationName = "Objectives";
            objectiveItem.IsFormSubmit = true;
            objectiveItem.FormID = objectiveFormID;
            objectiveItem.FormAction = "/Objectives";
            objective.NavItems.Add(objectiveItem);
            objectiveItem = new NavItem();
            objectiveItem.DestinationName = "Boosted Objectives";
            objectiveItem.IsFormSubmit = true;
            objectiveItem.FormID = objectiveFormID;
            objectiveItem.FormAction = "/Objectives/boosted";
            objective.NavItems.Add(objectiveItem);
            objectiveItem = new NavItem();
            objectiveItem.Destination = "/Objectives/myobjectives";
            objectiveItem.DestinationName = "Completed Objectives";
            objective.NavItems.Add(objectiveItem);



            navList.Add(objective);

            return navList;
        }

        public List<NavGrouping> CreateObjectivesAdminNavList()
        {
            List<NavGrouping> navList = CreateObjectivesNavList();

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