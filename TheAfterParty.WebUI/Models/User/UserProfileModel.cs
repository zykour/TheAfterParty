using TheAfterParty.Domain.Entities;
using System.Collections.Generic;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Services;

namespace TheAfterParty.WebUI.Models.User
{
    public class UserProfileModel : NavModel
    {
        public UserProfileModel() { }
        
        // the user this page is about, i.e. the one we are viewing
        public AppUser RequestedUser { get; set; }

        public List<ActivityFeedContainer> ActivityFeedList { get; set; }
        public string HighestRole { get; set; }
    }
}