using System.Web.Mvc;

namespace TheAfterParty.WebUI.Areas.CoopShop
{
    public class CoopShopAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CoopShop";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CoopShop_default",
                "coop-shop/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}