using System;
using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty.Domain.Abstract
{
    public interface ISiteRepository : IDisposable
    {
        AppIdentityDbContext GetContext();

        IEnumerable<SiteNotification> GetSiteNotifications();
        SiteNotification GetSiteNotificationByID(int siteNotificationID);
        void InsertSiteNotification(SiteNotification siteNotification);
        void UpdateSiteNotification(SiteNotification siteNotification);
        void DeleteSiteNotification(int siteNotificationID);

        IEnumerable<POTW> GetPOTWs();
        POTW GetPOTWByID(int potwID);
        void InsertPOTW(POTW potw);
        void UpdatePOTW(POTW potw);
        void DeletePOTW(int potwID);

        IEnumerable<GroupEvent> GetGroupEvents();
        GroupEvent GetGroupEventByID(int GroupEventID);
        void InsertGroupEvent(GroupEvent GroupEvent);
        void UpdateGroupEvent(GroupEvent GroupEvent);
        void DeleteGroupEvent(int GroupEventID);

        void Save();
    }
}
