using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Services;

namespace TheAfterParty.WebUI.Models.User
{
    public class UserActivityFeedModel
    {
        public List<ActivityFeedContainer> ActivityFeedItems { get; set; }
    }
}