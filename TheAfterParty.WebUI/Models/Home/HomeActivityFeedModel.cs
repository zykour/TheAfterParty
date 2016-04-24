using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Services;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Home
{
    public class HomeActivityFeedModel : NavModel
    {
        public List<ActivityFeedContainer> ActivityFeedItems { get; set; }
    }
}