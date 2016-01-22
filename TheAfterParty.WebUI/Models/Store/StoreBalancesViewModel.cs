using TheAfterParty.Domain.Entities;
using System.Collections.Generic;

namespace TheAfterParty.WebUI.Models.Store
{
    public class StoreBalancesViewModel
    {
        public AppUser LoggedInUser { get; set; }
        public IEnumerable<AppUser> Users { get; set; }
    }
}