using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models._Nav
{
    public class NavModel
    {
        public NavModel()
        {
            UserPaginationPreference = 0;
            SelectedPage = 0;
        }

        public List<NavGrouping> FullNavList { get; set; }
        public bool ModelHasNavList()
        {
            return FullNavList != null;
        }
        public AppUser LoggedInUser { get; set; }
        public bool ModelHasUser()
        {
            return LoggedInUser != null;
        }

        public DateTime GetConvertedDateTime(DateTime time)
        {
            if (LoggedInUser == null)
            {
                return time;
            }

            return LoggedInUser.GetLocalUserTime(time);
        }
        
        public int UserPaginationPreference { get; set; }
        public int TotalItems { get; set; }
        public int SelectedPage { get; set; }
        public int MaxPage()
        {
            if (UserPaginationPreference == 0)
            {
                return 1;
            }

            return (int)Math.Ceiling((decimal)TotalItems / UserPaginationPreference);
        }
        public int CurrentPage { get; set; }
    }
}