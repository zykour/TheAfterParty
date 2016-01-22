using System;
using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Services
{
    public interface IPurchaseService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
    }
}
