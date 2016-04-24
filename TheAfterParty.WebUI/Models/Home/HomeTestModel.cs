using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Home
{
    public class HomeTestModel : NavModel
    {
        public HomeTestModel()
        {
            MyIntStringBools = new List<HomeIntStringBool>();
        }
        public List<HomeIntStringBool> MyIntStringBools { get; set; }
        public int TestInt { get; set; }
        public int PreviousInt { get; set; }
    }

    public class HomeIntStringBool
    {
        public HomeIntStringBool()
        {
            MyIntString = new HomeIntString();
        }
        public HomeIntString MyIntString { get; set; }
        public bool IsSelected { get; set; }
    }

    public class HomeIntString
    {
        public int MyInt { get; set; }
        public string MyString { get; set; }
    }
}