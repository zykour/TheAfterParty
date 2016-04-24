using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Services;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.User
{
    public class UserActivityFeedModel : NavModel
    {
        public List<ActivityFeedContainer> ActivityFeedItems { get; set; }
    }
}