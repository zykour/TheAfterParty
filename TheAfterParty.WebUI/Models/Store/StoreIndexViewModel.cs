using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.WebUI.Models.Store
{
    public class StoreIndexViewModel : NavModel
    {
        public StoreIndexViewModel()
        {
            SearchText = "";
            SearchTextBool = false;
            FilterLibrary = false;
            BeginsWithFilter = '.';
            PreviousBeginsWithFilter = '.';
            SelectedPlatformID = 0;
            PreviousSelectedPlatformID = 0;
            PreviousSearchText = "";

            QuantitySort = 0;
            PriceSort = 0;
            GameSort = 0;

            PreviousQuantitySort = 0;
            PreviousPriceSort = 0;
            PreviousGameSort = 1;

            TagToChange = 0;
            CategoryToChange = 0;

            AffordableFilter = false;
            FilterBlacklist = false;
            FilterLibrary = false;
            PreviousAffordableFilter = false;
            PreviousFilterBlacklist = false;
            PreviousFilterLibrary = false;

            StorePlatforms = new List<Platform>();

            FormID = "";

            SpecialFilterType = String.Empty;
        }

        public string FormName { get; set; }
        public string FormID { get; set; }

        public string SpecialFilterType { get; set; }

        public IEnumerable<Listing> StoreListings { get; set; }
        public List<Platform> StorePlatforms { get; set; }

        // Container objects for storing the entity (tags and ProductCategories) and whether or not that tag is selected)
        public List<SelectedTagMapping> SelectedTagMappings { get; set; }
        public List<SelectedProductCategoryMapping> SelectedProductCategoryMappings { get; set; }
        public int TagToChange { get; set; }
        public int CategoryToChange { get; set; }

        public string FriendSteamID { get; set; }
        public bool FriendSteamIDBool { get; set; }
        
        public bool AffordableFilter { get; set; }
        public bool PreviousAffordableFilter { get; set; }
        public bool FilterLibrary { get; set; }
        public bool PreviousFilterLibrary { get; set; }
        public bool FilterBlacklist { get; set; }
        public bool PreviousFilterBlacklist { get; set; }

        // The entered Search text
        public string SearchText { get; set; }
        // Was the 'Search' button pressed?
        public bool SearchTextBool { get; set; }

        // We may want to let the user enter some new text without hitting the search button, so we'll need to save the actual text we're using to filter the listings
        public string PreviousSearchText { get; set; }


        // character results must start with
        public char BeginsWithFilter { get; set; }
        public char PreviousBeginsWithFilter { get; set; }

        // Which Platform did the user select
        public int SelectedPlatformID { get; set; }
        public int PreviousSelectedPlatformID { get; set; }

        public int QuantitySort { get; set; }
        public int PriceSort { get; set; }
        public int GameSort { get; set; }

        public int PreviousQuantitySort { get; set; }
        public int PreviousPriceSort { get; set; }
        public int PreviousGameSort { get; set; }
        // add pagination support
    }
}