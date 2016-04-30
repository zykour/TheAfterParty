using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.User
{
    public class UserProfileModel : NavModel
    {
        public UserProfileModel() { }
        
        // the user this page is about, i.e. the one we are viewing
        public AppUser RequestedUser { get; set; }
    }
}