using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Services;

namespace TheAfterParty.WebUI.Models.Account
{
    public class AccountIndexModel
    {
        public AppUser LoggedInUser { get; set; }
        public List<ActivityFeedContainer> ActivityFeedList { get; set; }
    }
}