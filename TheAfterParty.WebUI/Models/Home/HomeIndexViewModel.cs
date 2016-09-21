using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Services;
using TheAfterParty.WebUI.Models._Nav;
using CodeKicker.BBCode;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.WebUI.Models.Home
{
    public class HomeIndexViewModel : NavModel
    {
        public IEnumerable<ActivityFeedContainer> ActivityFeedList { get; set; } 
        public BBCodeParser Parser { get; set; }
    }
}