using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Services;

namespace TheAfterParty.WebUI.Models.Home
{
    public class HomeActivityFeedModel
    {
        public List<ActivityFeedContainer> ActivityFeedItems { get; set; }
    }
}