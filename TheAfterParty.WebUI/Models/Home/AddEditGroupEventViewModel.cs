using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Home
{
    public class AddEditGroupEventViewModel : NavModel 
    {
        public AddEditGroupEventViewModel()
        {
            GroupEvent = new GroupEvent();
            Product = new Product();
        }

        public GroupEvent GroupEvent { get; set; }
        public int AppID { get; set; }
        public Product Product { get; set; }
    }
}