using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.User
{
    public class UserAddBalancesViewModel : NavModel
    {
        public string Input { get; set; }
        public List<AppUser> Users { get; set; }
    }
}