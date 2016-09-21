using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Services
{
    public interface IAuctionService : IDisposable
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        Task<AppUser> GetCurrentUser(string name);
        AppUser GetCurrentUserSynch();

        IEnumerable<Auction> GetAuctions();
        Auction GetAuctionByID(int id);
        void AddAuction(Auction auction);
        void EditAuction(Auction auction);
        void DeleteAuction(int id);

        bool AddAuctionBid(AuctionBid bid, Auction auction);
        void DeleteAuctionBid(int id);

        void DrawAuctionWinners(int id);

        Task PopulateNewAuction(Auction auction);
    }
}
