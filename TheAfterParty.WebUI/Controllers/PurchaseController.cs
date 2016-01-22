using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using TheAfterParty.WebUI.Models.Cart;
using TheAfterParty.Domain.Services;

namespace TheAfterParty.WebUI.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {
        private IPurchaseService purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            this.purchaseService = purchaseService;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            purchaseService.SetUserName(User.Identity.Name);
        }

        public async Task<ActionResult> Index(string returnUrl)
        {
            await Task.Delay(1000);
            return View();
        }
    }
}