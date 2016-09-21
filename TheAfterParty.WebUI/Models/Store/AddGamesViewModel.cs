using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Store
{
    public class AddGamesViewModel : NavModel
    {
        public AddGamesViewModel()
        {
            AddedGames = new List<String>();
            Platforms = new List<Platform>();
        }
        public List<Platform> Platforms { get; set; }
        public Platform Platform { get; set; }
        public string Input { get; set; }
        public List<String> AddedGames { get; set; }
    }
}