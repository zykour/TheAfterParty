﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.User
{
    public class AddEditAppUserViewModel : NavModel
    {
        public AddEditAppUserViewModel()
        {
            Password = "";
        }

        public AppUser AppUser { get; set; }
        public string RoleToAdd { get; set; }
        public string RoleToRemove { get; set; }
        // for bot account only
        public string Password { get; set; }
    }
}