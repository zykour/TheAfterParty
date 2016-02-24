using System;
using TheAfterParty.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Services
{
    public interface IUserService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        AppUser GetCurrentUserSynch();

        Task AddBlacklistEntry(int listingId);
        Task TransferPoints(int points, string userId);

        AppUser GetRequestedUser(string profileName);
        ICollection<AppUser> GetAllUsers();
        Task<AppUser> GetUserByID(string id);

        void BuildUser(AppUser user, string apiKey);
        bool AddBalances(string input);
    }
}
