using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using TheAfterParty.Domain.Model;

namespace TheAfterParty.Domain.Services
{
    public class SiteService : ISiteService
    {
        private IUnitOfWork unitOfWork;
        private IUserRepository userRepository;
        private IListingRepository listingRepository;
        private ISiteRepository siteRepository;
        private IGiveawayRepository giveawayRepository;
        private IAuctionRepository auctionRepository;
        public AppUserManager UserManager { get; private set; }
        public string userName { get; set; }

        public SiteService(ISiteRepository siteRepository, IUserRepository userRepository, IListingRepository listingRepository, IGiveawayRepository giveawayRepository, IAuctionRepository auctionRepository, IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.siteRepository = siteRepository;
            this.userRepository = userRepository;
            this.listingRepository = listingRepository;
            this.giveawayRepository = giveawayRepository;
            this.auctionRepository = auctionRepository;
            this.unitOfWork = unitOfWork;
            userName = "";
        }
        protected SiteService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }

        public IEnumerable<SiteNotification> GetSiteNotifications()
        {
            return siteRepository.GetSiteNotifications();
        }

        public SiteNotification GetSiteNotificationByID(int id)
        {
            return siteRepository.GetSiteNotificationByID(id);
        }

        public void EditSiteNotification(SiteNotification siteNotification)
        {
            siteRepository.UpdateSiteNotification(siteNotification);

            unitOfWork.Save();
        }

        public void AddSiteNotification(SiteNotification siteNotification)
        {
            siteRepository.InsertSiteNotification(siteNotification);

            unitOfWork.Save();
        }

        public void DeleteSiteNotification(int id)
        {
            siteRepository.DeleteSiteNotification(id);

            unitOfWork.Save();
        }
        
        public IEnumerable<POTW> GetPOTWs()
        {
            return siteRepository.GetPOTWs();
        }
        public POTW GetPOTWByID(int id)
        {
            return siteRepository.GetPOTWByID(id);
        }
        public void EditPOTW(POTW potw)
        {
            siteRepository.UpdatePOTW(potw);

            unitOfWork.Save();
        }
        public void AddPOTW(POTW potw)
        {
            siteRepository.InsertPOTW(potw);

            unitOfWork.Save();
        }
        public void DeletePOTW(int id)
        {
            siteRepository.DeletePOTW(id);

            unitOfWork.Save();
        }

        public IEnumerable<GroupEvent> GetGroupEvents()
        {
            return siteRepository.GetGroupEvents();
        }
        public GroupEvent GetGroupEventByID(int id)
        {
            return siteRepository.GetGroupEventByID(id);
        }
        public void EditGroupEvent(GroupEvent groupEvent, Product product = null)
        {
            if (product.ProductID != 0)
            {
                groupEvent.ProductID = product.ProductID;
            }
            else
            {
                listingRepository.InsertProduct(product);
                unitOfWork.Save();
                groupEvent.ProductID = product.ProductID;
            }

            siteRepository.UpdateGroupEvent(groupEvent);

            unitOfWork.Save();
        }
        public void AddGroupEvent(GroupEvent groupEvent)
        {
            groupEvent.EventCreatedTime = DateTime.Now;

            if (groupEvent.Product.ProductID == 0)
            {
                listingRepository.InsertProduct(groupEvent.Product);
            }

            siteRepository.InsertGroupEvent(groupEvent);

            unitOfWork.Save();
        }
        public void DeleteGroupEvent(int id)
        {
            siteRepository.DeleteGroupEvent(id);

            unitOfWork.Save();
        }

        public Product GetProductByAppID(int appId)
        {
            return listingRepository.GetProducts().Where(p => p.AppID == appId).SingleOrDefault();
        }
        public Product GetProductByID(int id)
        {
            return listingRepository.GetProductByID(id);
        }
        public void AddProduct(Product product)
        {
            listingRepository.InsertProduct(product);

            unitOfWork.Save();
        }

        public IEnumerable<ActivityFeedContainer> GetSiteActivityFeedItems()
        {
            List<ActivityFeedContainer> activityFeed = new List<ActivityFeedContainer>();

            foreach (SiteNotification notification in siteRepository.GetSiteNotifications())
            {
                activityFeed.Add(new ActivityFeedContainer(notification, notification.NotificationDate));
            }

            //**
            //foreach (Giveaway giveaway in giveawayRepository.GetGiveaways())
            //{
            //    activityFeed.Add(new ActivityFeedContainer(giveaway, giveaway.CreatedTime));
            //}

    //foreach (Auction auction in auctionRepository.GetAuctions())
    // {
    //      activityFeed.Add(new ActivityFeedContainer(auction, auction.CreatedTime));
    // }
            
            //foreach (GroupEvent groupEvent in siteRepository.GetGroupEvents())
            //{
            //    activityFeed.Add(new ActivityFeedContainer(groupEvent, groupEvent.EventCreatedTime));
            //}

    //foreach (POTW potw in siteRepository.GetPOTWs())
    //{
    //    activityFeed.Add(new ActivityFeedContainer(potw, potw.StartDate));
    //}
            
            return activityFeed.OrderByDescending(a => a.ItemDate);
        }


        public async Task<AppUser> GetAppUserByID(string id)
        {
            return await UserManager.FindByIdAsync(id);
            //return await userRepository.GetAppUserByID(id);
        }

        public void Dispose()
        {
            this.unitOfWork.Dispose();
        }

        public AppUser GetCurrentUserSynch()
        {
            return UserManager.FindByName(userName);
        }

        public async Task<AppUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsyncWithCartAndOpenAuctionBids(userName);
        }
    }
}
