using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;


namespace TheAfterParty.WebUI.Models.Auctions
{
    public class AuctionsIndexViewModel : NavModel
    {
        public AuctionsIndexViewModel()
        {
            Auctions = new List<Auction>();
            Title = "";
            SearchText = "";
        }

        public IEnumerable<Auction> Auctions { get; set; }
        public string Title { get; set; }

        public string SearchText { get; set; }
        
        public bool PreviouslyFilterSilent { get; set; }
        public bool FilterSilent { get; set; }

        public bool PreviousFilterPublic { get; set; }
        public bool FilterPublic { get; set; }

        public bool FilterAll { get; set; }

        public string ActionName { get; set; }
    }
}