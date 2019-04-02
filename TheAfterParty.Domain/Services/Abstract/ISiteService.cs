using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.Domain.Services
{
    public interface ISiteService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        AppUser GetCurrentUserSynch();

        IEnumerable<SiteNotification> GetSiteNotifications();
        SiteNotification GetSiteNotificationByID(int id);
        void EditSiteNotification(SiteNotification siteNotification);
        void AddSiteNotification(SiteNotification siteNotification);
        void DeleteSiteNotification(int id);

        IEnumerable<POTW> GetPOTWs();
        POTW GetPOTWByID(int id);
        void EditPOTW(POTW potw);
        void AddPOTW(POTW potw);
        void DeletePOTW(int id);

        IEnumerable<GroupEvent> GetGroupEvents();
        GroupEvent GetGroupEventByID(int id);
        void EditGroupEvent(GroupEvent groupEvent, Product product);
        void AddGroupEvent(GroupEvent groupEvent);
        void DeleteGroupEvent(int id);

        Product GetProductByAppID(int appId);
        Product GetProductByID(int id);
        void AddProduct(Product product);

        IEnumerable<ActivityFeedContainer> GetSiteActivityFeedItems();

        Task<AppUser> GetAppUserByID(string id);
    }
}
