using TheAfterParty.Domain.Entities;
using System.Collections.Generic;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.User
{
    public class UserIndexModel : NavModel
    {
        public UserIndexModel() { }

        public string Title { get; set; }
        // the user who is making the request/logged in
        public AppUser LoggedInUser { get; set; }
        // all users on the site
        public List<AppUser> Users { get; set; }
    }
}