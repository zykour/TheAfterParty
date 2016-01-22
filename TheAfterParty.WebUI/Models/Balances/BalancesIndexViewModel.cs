using System.Collections.Generic;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Balances
{
    public class BalancesIndexViewModel
    {
        public AppUser LoggedInUser { get; set; }
        public IEnumerable<AppUser> SiteUsers { get; set; }
    }
}