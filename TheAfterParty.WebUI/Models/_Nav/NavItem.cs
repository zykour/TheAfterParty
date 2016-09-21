using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheAfterParty.WebUI.Models._Nav
{
    public class NavItem
    {
        public string Destination { get; set; }
        public string DestinationName { get; set; }
        public bool IsFormSubmit { get; set; }
        public string FormName { get; set; }
        public string FormValue { get; set; }
        public string FormID { get; set; }
        public string FormAction { get; set; }
        public bool IsSelected { get; set; }

        public void SetSelected(List<String> destNames)
        {
            foreach (String name in destNames)
            {
                if (DestinationName.CompareTo(name) == 0)
                {
                    IsSelected = true;
                    break;
                }
            }
        }
    }
}