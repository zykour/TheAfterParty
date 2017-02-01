using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Account
{
    public class MySettingsViewModel : NavModel
    {
        //public string TimeZoneID { get; set; }
        public string PaginationPreference { get; set; }
        public string AppIDs { get; set; }
        public int SuccessfulItemsAdded { get; set; }
    }
}