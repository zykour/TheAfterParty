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

        public AppUser GetRequestedUser(string profileName)
        {
            return UserManager.Users.Where(u => object.Equals(u.UserName.ToLower(), profileName.Trim().ToLower())).SingleOrDefault();
        }

        public ICollection<AppUser> GetAllUsers()
        {
            return UserManager.Users.ToList();
        }

        public void BuildUser(AppUser user, string apiKey)
        {
            if (user.UserSteamID == 0)
            {
                // possibly add default data here
                return;
            }

            string playerURL = String.Format("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={0}&steamids={1}&format=json", apiKey, user.UserSteamID);

            string result = new System.Net.WebClient().DownloadString(playerURL);

            JObject playerData = JObject.Parse(result);

            if (playerData["response"] != null && playerData["response"]["players"] != null)
            {
                user.LargeAvatar = (string)playerData["response"]["players"][0]["avatarfull"] ?? "";
                user.MediumAvatar = (string)playerData["response"]["players"][0]["avatarmedium"] ?? "";
                user.SmallAvatar = (string)playerData["response"]["players"][0]["avatar"] ?? "";
            }

            string gamesURL = String.Format("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={0}&steamid={1}&format=json", apiKey, user.UserSteamID);

            result = new System.Net.WebClient().DownloadString(gamesURL);

            JObject gameData = JObject.Parse(result);
            
            if (gameData["response"] != null && gameData["response"]["games"] != null)
            {
                JArray jGames = (JArray)gameData["response"]["games"];
                for (int i = 0; i < jGames.Count; i++)
                {
                    user.AddOwnedGame(new OwnedGame((int)jGames[i]["appid"], (int)jGames[i]["playtime_forever"]));
                }
            }

                UserManager.Update(user);
            unitOfWork.Save();            
        }

        public async Task<AppUser> GetUserByID(string id)
        {
            return await userRepository.GetAppUserByID(id);
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
