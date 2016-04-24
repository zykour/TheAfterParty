using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Balances
{
    public class BalancesIndexViewModel : NavModel
    {
        public AppUser LoggedInUser { get; set; }
        public IEnumerable<AppUser> SiteUsers { get; set; }
    }
}