using System;
using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using System.Linq;

namespace TheAfterParty.Domain.Abstract
{
    public interface IAuctionRepository : IDisposable
    {
        AppIdentityDbContext GetContext();
        
        IEnumerable<Auction> GetAuctions();
        IQueryable<Auction> GetAuctionsAsQueryable();
        Auction GetAuctionByID(int auctionId);
        void InsertAuction(Auction auction);
        void UpdateAuction(Auction auction);
        void DeleteAuction(int auctionId);

        IEnumerable<AuctionBid> GetAuctionBids();
        AuctionBid GetAuctionBidByID(int auctionBidId);
        void InsertAuctionBid(AuctionBid auctionBid);
        void UpdateAuctionBid(AuctionBid auctionBid);
        void DeleteAuctionBid(int auctionBidId);

        void Save();
    }
}
