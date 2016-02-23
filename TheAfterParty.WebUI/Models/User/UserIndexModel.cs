using TheAfterParty.Domain.Entities;
using System.Collections.Generic;

namespace TheAfterParty.WebUI.Models.User
{
    public class UserIndexModel
    {
        public UserIndexModel() { }

        // the user who is making the request/logged in
        public AppUser LoggedInUser { get; set; }
        // all users on the site
        public List<AppUser> Users { get; set; }
    }
}