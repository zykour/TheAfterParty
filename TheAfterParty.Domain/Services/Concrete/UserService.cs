using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SteamKit2;
using System;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using System.Web;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TheAfterParty.Domain.Services
{
    public class UserService : IUserService
    {
        private IListingRepository listingRepository;
        private IUserRepository userRepository;
        private IUnitOfWork unitOfWork;
        public AppUserManager UserManager { get; private set; }
        public string userName { get; set; }

        public UserService(IListingRepository listingRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.listingRepository = listingRepository;
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }
        protected UserService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }
        
        public async Task AddBlacklistEntry(int listingId)
        {
            AppUser user = await GetCurrentUser();

            user.AddListingBlacklistEntry(listingRepository.GetListingByID(listingId));
            UserManager.Update(user);
            unitOfWork.Save();
        }

        public async Task TransferPoints(int points, string userId)
        {
            AppUser donor = await GetCurrentUser();
            AppUser recipient = await UserManager.FindByIdAsync(userId);

            if (donor.Balance - donor.ReservedBalance() > points)
            {
                donor.Balance -= points;
                recipient.Balance += points;
                UserManager.Update(donor);
                UserManager.Update(recipient);
                unitOfWork.Save();
            }
        }


        // --- GC and User logic

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
            return await UserManager.FindByNameAsync(userName);
        }
    }
}
