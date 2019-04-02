using System;
using System.Collections.Generic;
using System.Linq;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using System.Data.Entity;

namespace TheAfterParty.Domain.Concrete
{
    public class AuctionRepository : IAuctionRepository, IDisposable
    {
        private AppIdentityDbContext context;
        public AppIdentityDbContext GetContext()
        {
            return context;
        }

        public AuctionRepository(IUnitOfWork unitOfWork)
        {
            this.context = unitOfWork.DbContext;
        }

        public IEnumerable<Auction> GetAuctions()
        {
            return context.Auctions
                        .Include(x => x.AuctionBids.Select(b => b.AppUser))
                        .Include(x => x.Creator.AuctionBids)
                        .Include(x => x.Creator.Auctions)
                        .Include(x => x.Listing.Platforms)
                        .Include(x => x.Listing.Product)
                        .Include(x => x.Winners)
                        .AsQueryable();
        }
        public IQueryable<Auction> GetAuctionsAsQueryable()
        {
            return context.Auctions
                        .Include(x => x.AuctionBids.Select(b => b.AppUser))
                        .Include(x => x.Creator.AuctionBids)
                        .Include(x => x.Creator.Auctions)
                        .Include(x => x.Listing.Platforms)
                        .Include(x => x.Listing.Product)
                        .Include(x => x.Winners)
                        .AsQueryable();
        }
        public Auction GetAuctionByID(int auctionId)
        {
            return context.Auctions
                        .Include(x => x.AuctionBids.Select(b => b.AppUser))
                        .Include(x => x.Creator.AuctionBids)
                        .Include(x => x.Creator.Auctions)
                        .Include(x => x.Listing.Platforms)
                        .Include(x => x.Listing.Product)
                        .Include(x => x.Winners)
                        .SingleOrDefault(a => a.AuctionID == auctionId);
        }
        public void InsertAuction(Auction auction)
        {
            context.Auctions.Add(auction);
        }
        public void UpdateAuction(Auction auction)
        {
            Auction targetAuction = context.Auctions.Find(auction.AuctionID);

            if (targetAuction != null)
            {
                targetAuction.CreatedTime = auction.CreatedTime;
                targetAuction.AlternativePrize = auction.AlternativePrize;
                targetAuction.EndTime = auction.EndTime;
                targetAuction.IsSilent = auction.IsSilent;
                targetAuction.MinimumBid = auction.MinimumBid;
                targetAuction.Copies = auction.Copies;
                targetAuction.Increment = auction.Increment;
                targetAuction.AuctionKeys = auction.AuctionKeys;
            }

            foreach (AuctionBid bid in auction.AuctionBids)
            {
                if (bid.AuctionBidID == 0)
                {
                    InsertAuctionBid(bid);
                }
                else
                {
                    UpdateAuctionBid(bid);
                }
            }
        }
        public void DeleteAuction(int auctionId)
        {
            Auction targetAuction = context.Auctions.Find(auctionId);

            if (targetAuction != null)
            {
                context.Auctions.Remove(targetAuction);
            }
        }

        public IEnumerable<AuctionBid> GetAuctionBids()
        {
            return context.AuctionBids
                            .Include(x => x.AppUser)
                            .Include(x => x.Auction)
                            .AsQueryable();
        }
        public AuctionBid GetAuctionBidByID(int auctionBidId)
        {
            return context.AuctionBids
                            .Include(x => x.AppUser)
                            .Include(x => x.Auction)
                            .SingleOrDefault(x => x.AuctionBidID == auctionBidId);
        }
        public void InsertAuctionBid(AuctionBid auctionBid)
        {
            context.AuctionBids.Add(auctionBid);
        }
        public void UpdateAuctionBid(AuctionBid auctionBid)
        {
            AuctionBid targetAuctionBid = context.AuctionBids.Find(auctionBid.AuctionBidID);

            if (targetAuctionBid != null)
            {
                targetAuctionBid.BidAmount = auctionBid.BidAmount;
                targetAuctionBid.BidDate = auctionBid.BidDate;
            }
        }
        public void DeleteAuctionBid(int auctionBidId)
        {
            AuctionBid targetAuctionBid = context.AuctionBids.Find(auctionBidId);

            if (targetAuctionBid != null)
                context.AuctionBids.Remove(targetAuctionBid);
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
