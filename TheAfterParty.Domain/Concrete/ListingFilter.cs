using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Concrete
{
    public class ListingFilter
    {
        public ListingFilter()
        {
            PlatformID = 0;
            TagIDs = new List<int>();
            ProductCategoryIDs = new List<int>();
            FriendAppIDs = new HashSet<int>();

            UserID = String.Empty;

            BlacklistFilter = false;
            UnownedFilter = false;
            AffordableFilter = false;
            WishlistFilter = false;

            SearchText = String.Empty;
            BeginsWithFilter = '.';
            BeginsWithSentinel = '.';

            QuantitySort = 0;
            PriceSort = 0;
            GameSort = 1;

            Page = 0;
            PaginationNum = 0;
            Date = null;
        }

        public DateTime? Date { get; set; }
        public bool IsNewest { get; set; }
        public bool IsDailyDeal { get; set; }
        public bool IsWeeklyDeal { get; set; }
        public bool IsOtherDeal { get; set; }
        public bool IsAllDeal { get; set; }
        
        public IEnumerable<int> FriendAppIDs { get; set; }
        public int PlatformID { get; set; }
        public List<int> TagIDs { get; set; }
        public List<int> ProductCategoryIDs { get; set; }
        public string UserID { get; set; }
        public bool BlacklistFilter { get; set; }
        public bool UnownedFilter { get; set; }
        public bool AffordableFilter { get; set; }
        public bool WishlistFilter { get; set; }
        public string SearchText { get; set; }
        public char BeginsWithFilter { get; set; }
        public char BeginsWithSentinel { get; set; }
        public int QuantitySort { get; set; }
        public int PriceSort { get; set; }
        public int GameSort { get; set; }
        public int Page { get; set; }
        public int PaginationNum { get; set; }

        public void SetQuantitySort(int sortVal)
        {
            GameSort = 0;
            QuantitySort = sortVal;
        }
        public void SetPriceSort(int sortVal)
        {
            GameSort = 0;
            PriceSort = sortVal;
        }

        public AppUser LoggedInUser { get; set; }
        public DateTime? NewestDate { get; set; }
    }
}
