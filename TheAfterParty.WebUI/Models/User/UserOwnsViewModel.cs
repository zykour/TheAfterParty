using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.User
{
    public class UserOwnsViewModel : NavModel
    {
        public String GameName { get; set; }
        public int AppID { get; set; }
        public List<AppUser> GameOwners { get; set; }
        public List<AppUser> GameNonOwners { get; set; }

        public String GetComputedStorePageURL()
        {
            return String.Format("http://store.steampowered.com/app/{0}/", AppID);
        }

        public String GetComputedBannerURL()
        {
            return String.Format("https://steamcdn-a.akamaihd.net/steam/apps/{0}/header_292x136.jpg", AppID);
        }
    }
}