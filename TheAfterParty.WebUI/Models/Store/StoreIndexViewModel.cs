using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Store
{
    public class StoreIndexViewModel
    {
        public StoreIndexViewModel()
        {
            SearchText = "";
            SearchTextBool = false;
            FilterLibrary = false;
            BeginsWithFilter = '.';
            PreviousBeginsWithFilter = '.';
            SelectedPlatformID = 0;
            SelectedDealsPlatformID = 0;
            PreviousSelectedDealsPlatformID = 0;
            PreviousSelectedPlatformID = 0;
            PreviousSearchText = "";

            QuantitySort = 0;
            PriceSort = 0;
            GameSort = 0;

            PreviousQuantitySort = 0;
            PreviousPriceSort = 0;
            PreviousGameSort = 1;
        }

        public List<Listing> StoreListings { get; set; }
        public List<Platform> StorePlatforms { get; set; }

        // Container objects for storing the entity (tags and ProductCategories) and whether or not that tag is selected)
        public List<SelectedTagMapping> SelectedTagMappings { get; set; }
        public List<SelectedProductCategoryMapping> SelectedProductCategoryMappings { get; set; }

        // The entered Search text
        public string SearchText { get; set; }
        // Was the 'Search' button pressed?
        public bool SearchTextBool { get; set; }

        // We may want to let the user enter some new text without hitting the search button, so we'll need to save the actual text we're using to filter the listings
        public string PreviousSearchText { get; set; }

        public bool FilterLibrary { get; set; }

        // character results must start with
        public char BeginsWithFilter { get; set; }
        public char PreviousBeginsWithFilter { get; set; }

        // Which Platform did the user select
        public int SelectedPlatformID { get; set; }
        public int PreviousSelectedPlatformID { get; set; }

        // Which Platform did the user select to see deals for
        public int SelectedDealsPlatformID { get; set; }
        public int PreviousSelectedDealsPlatformID { get; set; }

        public int QuantitySort { get; set; }
        public int PriceSort { get; set; }
        public int GameSort { get; set; }

        public int PreviousQuantitySort { get; set; }
        public int PreviousPriceSort { get; set; }
        public int PreviousGameSort { get; set; }
        // add pagination support
    }
}