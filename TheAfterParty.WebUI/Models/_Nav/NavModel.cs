using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models._Nav
{
    public class NavModel
    {
        public List<NavGrouping> FullNavList { get; set; }
        public AppUser LoggedInUser { get; set; }
    }
}