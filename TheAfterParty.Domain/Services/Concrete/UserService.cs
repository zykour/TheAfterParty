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
                donor.CreateBalanceEntry("Transfer of points to " + recipient.UserName, 0 - points, DateTime.Now);
                recipient.CreateBalanceEntry("Gift of points from " + donor.UserName, points, DateTime.Now);
                UserManager.Update(donor);
                UserManager.Update(recipient);
                unitOfWork.Save();
            }
        }

        public AppUser GetRequestedUser(string profileName, bool nickname = false)
        {
            if (nickname)
            {
                AppUser user = UserManager.Users.Where(u => object.Equals(u.Nickname.ToLower(), profileName.Trim().ToLower())).SingleOrDefault();

                if (user != null)
                {
                    return user;
                }
                else
                {
                    return UserManager.Users.Where(u => object.Equals(u.UserName.ToLower(), profileName.Trim().ToLower())).SingleOrDefault();
                }
            }
            else
            {
                return UserManager.Users.Where(u => object.Equals(u.UserName.ToLower(), profileName.Trim().ToLower())).SingleOrDefault();
            }
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

        public bool AddBalances(string input)
        {
            /*
                SAMPLE:

                Game Night 02-24
                Bob     10
                Fred    8
                George  8
                Objective "Beat Me at Chess"
                Fred    5
            
                Each balance entry should use the plain text note preceding it as a note
                Convention is to use a tab delimiter for balance entries and no tab in the note
            */

            bool fullSuccess = true;

            string currentNote = "";
            DateTime currentTime = DateTime.Now;

            input = input.Replace("\r\n", "\r");

            List<String> balanceEntries = input.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            Regex userBalance = new Regex("^(-?[^\t]+)\t+([0-9]+)$");
            Regex note = new Regex("^([^\t]+)$");

            for (int i = 0; i < balanceEntries.Count; i++)
            {
                if (note.Match(balanceEntries[i]).Success)
                {
                    currentNote = note.Match(balanceEntries[i]).Groups[1].Value;
                }
                else if (userBalance.Match(balanceEntries[i]).Success)
                {
                    Match userBalanceMatch = userBalance.Match(balanceEntries[i]);
                    AppUser user = GetRequestedUser(userBalanceMatch.Groups[1].Value.Trim(), true);

                    int points = Int32.Parse(userBalanceMatch.Groups[2].Value, System.Globalization.NumberStyles.AllowLeadingSign);

                    user.CreateBalanceEntry(currentNote, points, currentTime);
                    UserManager.Update(user);
                }
                else
                {
                    fullSuccess = false;
                }
            }

            // If all rows have successfully been parsed, then persist!
            if (fullSuccess)
            {
                unitOfWork.Save();
            }

            return fullSuccess;
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
