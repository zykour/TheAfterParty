using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheAfterParty.WebUI.Models._Nav
{
    public class NavGrouping
    {
        public string GroupingHeader { get; set; }
        public List<NavItem> NavItems { get; set; }
    }
}