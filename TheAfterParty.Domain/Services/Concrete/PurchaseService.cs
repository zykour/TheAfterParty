using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TheAfterParty.Domain.Services
{
    public class PurchaseService : IPurchaseService
    {
        private IUnitOfWork unitOfWork;
        private IUserRepository userRepository;
        private IListingRepository listingRepository;
        public AppUserManager UserManager { get; private set; }
        public string userName { get; set; }

        public PurchaseService(IUserRepository userRepository, IListingRepository listingRepository, IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.userRepository = userRepository;
            this.listingRepository = listingRepository;
            this.unitOfWork = unitOfWork;
        }
        protected PurchaseService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }

        public void Dispose()
        {
            this.unitOfWork.Dispose();
        }

        public async Task<AppUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsync(userName);
        }
    }
}
