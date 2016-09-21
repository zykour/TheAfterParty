using System;
using System.Collections.Generic;
using System.Linq;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using System.Data.Entity;

namespace TheAfterParty.Domain.Concrete
{
    public class SiteRepository : ISiteRepository
    {
        private AppIdentityDbContext context;
        public AppIdentityDbContext GetContext()
        {
            return context;
        }

        public SiteRepository(IUnitOfWork unitOfWork)
        {
            this.context = unitOfWork.DbContext;
        }

        public IEnumerable<SiteNotification> GetSiteNotifications()
        {
            return context.SiteNotifications;
        }
        public SiteNotification GetSiteNotificationByID(int siteNotificationID)
        {
            return context.SiteNotifications.Find(siteNotificationID);
        }
        public void InsertSiteNotification(SiteNotification siteNotification)
        {
            context.SiteNotifications.Add(siteNotification);
        }
        public void UpdateSiteNotification(SiteNotification siteNotification)
        {
            SiteNotification targetSiteNotification = context.SiteNotifications.Find(siteNotification.SiteNotificationID);

            if (targetSiteNotification != null)
            {
                targetSiteNotification.Notification = siteNotification.Notification;
                targetSiteNotification.NotificationDate = siteNotification.NotificationDate;
            }
        }
        public void DeleteSiteNotification(int siteNotificationID)
        {
            SiteNotification siteNotification = context.SiteNotifications.Find(siteNotificationID);

            if (siteNotification != null)
            {
                context.SiteNotifications.Remove(siteNotification);
            }
        }

        public IEnumerable<POTW> GetPOTWs()
        {
            return context.POTWs
                                .Include(x => x.AppUser);
        }
        public POTW GetPOTWByID(int potwID)
        {
            return context.POTWs
                                .Include(x => x.AppUser)
                                .SingleOrDefault(x => x.POTWID == potwID);//.Find(potwID);
        }
        public void InsertPOTW(POTW potw)
        {
            context.POTWs.Add(potw);
        }
        public void UpdatePOTW(POTW potw)
        {
            POTW targetPOTW = context.POTWs.Find(potw.POTWID);

            if (targetPOTW != null)
            {
                targetPOTW.StartDate = potw.StartDate;
            }
        }
        public void DeletePOTW(int potwID)
        {
            POTW targetPOTW = context.POTWs.Find(potwID);

            if (targetPOTW != null)
            {
                context.POTWs.Remove(targetPOTW);
            }
        }

        public IEnumerable<GroupEvent> GetGroupEvents()
        {
            return context.GroupEvents;
        }
        public GroupEvent GetGroupEventByID(int groupEventID)
        {
            return context.GroupEvents.Find(groupEventID);
        }
        public void InsertGroupEvent(GroupEvent groupEvent)
        {
            context.GroupEvents.Add(groupEvent);
        }
        public void UpdateGroupEvent(GroupEvent groupEvent)
        {
            GroupEvent targetGroupEvent = context.GroupEvents.Find(groupEvent.GroupEventID);

            if (targetGroupEvent != null)
            {
                if (groupEvent.EventCreatedTime.CompareTo(default(DateTime)) > 0)
                {
                    targetGroupEvent.EventCreatedTime = groupEvent.EventCreatedTime;
                }
                targetGroupEvent.Description = groupEvent.Description;
                if (groupEvent.EventDate.CompareTo(default(DateTime)) > 0)
                {
                    targetGroupEvent.EventDate = groupEvent.EventDate;
                }
                targetGroupEvent.EventName = groupEvent.EventName;
                targetGroupEvent.IsGameNight = groupEvent.IsGameNight;
                targetGroupEvent.ProductID = groupEvent.ProductID;
            }
        }
        public void DeleteGroupEvent(int groupEventID)
        {
            GroupEvent targetGroupEvent = context.GroupEvents.Find(groupEventID);

            if (targetGroupEvent != null)
            {
                context.GroupEvents.Remove(targetGroupEvent);
            }
        }

        // ---- Repository methods

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
