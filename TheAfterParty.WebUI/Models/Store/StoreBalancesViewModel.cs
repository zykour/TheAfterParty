using TheAfterParty.Domain.Entities;
using System.Collections.Generic;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Store
{
    public class StoreBalancesViewModel : NavModel
    {
        public AppUser LoggedInUser { get; set; }
        public IEnumerable<AppUser> Users { get; set; }
    }
}