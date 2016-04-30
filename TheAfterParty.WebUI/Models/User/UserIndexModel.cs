using TheAfterParty.Domain.Entities;
using System.Collections.Generic;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.User
{
    public class UserIndexModel : NavModel
    {
        public UserIndexModel() { }

        public string Title { get; set; }
        // all users on the site
        public List<AppUser> Users { get; set; }
    }
}