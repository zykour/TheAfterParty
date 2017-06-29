using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models._Nav
{
    public class NavModel
    {
        private const int anonymousPaginationPref = 25;

        public NavModel()
        {
            UserPaginationPreference = anonymousPaginationPref;
            SelectedPage = 0;
        }

        public List<NavGrouping> FullNavList { get; set; }
        public bool ModelHasNavList()
        {
            return FullNavList != null;
        }
        public AppUser LoggedInUser { get; set; }
        public bool ModelHasUser()
        {
            return LoggedInUser != null;
        }

        public DateTime GetConvertedDateTime(DateTime time)
        {
            if (LoggedInUser == null)
            {
                return time;
            }

            return LoggedInUser.GetLocalUserTime(time);
        }
        
        public int UserPaginationPreference { get; set; }
        public int TotalItems { get; set; }
        public int SelectedPage { get; set; }
        public int MaxPage()
        {
            if (UserPaginationPreference == 0)
            {
                return 1;
            }

            return (int)Math.Ceiling((decimal)TotalItems / UserPaginationPreference);
        }
        public int CurrentPage { get; set; }

        public void Initialize(AppUser user, List<NavGrouping> navList, int selectedPage = 0, int totalItems = 0)
        {
            FullNavList = navList;
            LoggedInUser = user;
            CurrentPage = 1;

            if (user != null && user.PaginationPreference != 0)
            {
                UserPaginationPreference = user.PaginationPreference;
                if (selectedPage != 0)
                {
                    CurrentPage = selectedPage;
                }
                else if (SelectedPage != 0)
                {
                    CurrentPage = SelectedPage;
                }
            }
            else
            {
                if (selectedPage != 0)
                {
                    CurrentPage = selectedPage;
                }
                else if (SelectedPage != 0)
                {
                    CurrentPage = SelectedPage;
                }
                UserPaginationPreference = anonymousPaginationPref;
            }

            TotalItems = totalItems;
        }

        /// <summary>
        /// Adds the correct linq query to the IEnumerable for this user's pagination preference. Sets TotalItems if necessary
        /// </summary>
        /// <typeparam name="T"> The type being paginated</typeparam>
        /// <param name="list"> The </param>
        /// <returns> The IEnumerable with the correct Skip and Take linq query</returns>
        /// <remarks> If the user's pagination preference is set to show all, this just returns the IEnumerable back to the user as is </remarks>
        public IEnumerable<T> SkipAndTake<T>(IEnumerable<T> list)
        {
            if (TotalItems == 0)
            {
                TotalItems = list.Count();
            }

            if (UserPaginationPreference > 0)
            {
                return list.Skip((CurrentPage - 1) * UserPaginationPreference).Take(UserPaginationPreference);
            }
            else
            {
                return list.Skip((CurrentPage - 1) * anonymousPaginationPref).Take(anonymousPaginationPref);
            }
        }
    }
}