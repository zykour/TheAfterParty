using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.User
{
    public class UserAddBalancesViewModel
    {
        public string Input { get; set; }
        public List<AppUser> Users { get; set; }
    }
}