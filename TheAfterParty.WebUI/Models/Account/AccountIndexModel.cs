using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Services;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Account
{
    public class AccountIndexModel : NavModel
    {
        public AppUser LoggedInUser { get; set; }
        public List<ActivityFeedContainer> ActivityFeedList { get; set; }
    }
}