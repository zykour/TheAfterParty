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

namespace TheAfterParty.Domain.Services
{
    public class AuctionService : IAuctionService
    {
        private IUnitOfWork unitOfWork;
        private IUserRepository userRepository;
        private IListingRepository listingRepository;
        private IAuctionRepository auctionRepository;
        public AppUserManager UserManager { get; private set; }
        public string userName { get; set; }

        public AuctionService(IAuctionRepository auctionRepository, IUserRepository userRepository, IListingRepository listingRepository, IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.auctionRepository = auctionRepository;
            this.userRepository = userRepository;
            this.listingRepository = listingRepository;
            this.unitOfWork = unitOfWork;
            userName = "";
        }
        protected AuctionService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }


        public IEnumerable<Auction> GetAuctions()
        {
            return auctionRepository.GetAuctions();
        }
        public Auction GetAuctionByID(int id)
        {
            return auctionRepository.GetAuctionByID(id);
        }
        public void AddAuction(Auction auction)
        {
            auctionRepository.InsertAuction(auction);

            unitOfWork.Save();
        }
        public void EditAuction(Auction auction)
        {
            auctionRepository.UpdateAuction(auction);

            unitOfWork.Save();
        }
        public void DeleteAuction(int id)
        {
            if (auctionRepository.GetAuctionByID(id) != null)
            {
                auctionRepository.DeleteAuction(id);

                unitOfWork.Save();
            }
        }

        public bool AddAuctionBid(AuctionBid bid, Auction auction)
        {
            bid.Auction = auction;

            if (bid.AppUser == null)
            {
                return false;
            }

            if (bid.AuctionID != 0 && bid.Auction == null)
            {
                bid.Auction = GetAuctionByID(bid.AuctionID);
            }

            if (bid.Auction == null)
            {
                return false;
            }
            
            if (bid.Auction.EndTime.CompareTo(DateTime.Now) < 0)
            {
                return false;
            }

            AppUser user = bid.AppUser;

            if (bid.BidAmount == 0 && bid.Auction.IsSilent)
            {
                AuctionBid oldBid = bid.Auction.UserAuctionBid(user);

                if (oldBid != null)
                {
                    DeleteAuctionBid(oldBid.AuctionBidID);
                }

                return true;
            }
            else if (bid.BidAmount < bid.Auction.MinimumBid)
            {
                return false;
            }

            if (bid.Auction.IsSilent == false)
            {
                if (bid.Auction.AuctionBids.Count > 0)
                {
                    if (bid.BidAmount < (bid.Auction.PublicWinningBid() + bid.Auction.Increment) && bid.Auction.UserIsWinningBid(user) == false)
                    {
                        return false;
                    }
                    else if (bid.BidAmount < bid.Auction.PublicWinningBid() && bid.Auction.UserIsWinningBid(user) == true)
                    {
                        return false;
                    }
                }
            }

            bool success = false;

            if (bid.Auction.AuctionBids != null)
            {
                if (bid.Auction.IsSilent)
                {
                    AuctionBid myBid = bid.Auction.UserAuctionBid(bid.AppUser);

                    if (myBid != null)
                    {
                        if ((user.Balance - user.ReservedBalance() + (myBid.BidAmount - bid.BidAmount)) >= 0 && myBid.BidAmount != bid.BidAmount)
                        {
                            myBid.BidAmount = bid.BidAmount;
                            auctionRepository.UpdateAuctionBid(myBid);
                            unitOfWork.Save();

                            success = true;
                        }
                    }
                    else
                    {
                        if ((user.Balance - user.ReservedBalance() - bid.BidAmount) >= 0)
                        {
                            bid.AddAuction(auction);
                            auctionRepository.InsertAuctionBid(bid);
                            unitOfWork.Save();

                            success = true;
                        }
                    }
                }
                else
                {
                    AuctionBid winningBid = bid.Auction.WinningAuctionBid();

                    if (winningBid != null && winningBid.AppUser.Id.CompareTo(user.Id) == 0)
                    {
                        if ((user.Balance - user.ReservedBalance() + (winningBid.BidAmount - bid.BidAmount)) >= 0)
                        {
                            winningBid.BidAmount = bid.BidAmount;
                            auctionRepository.UpdateAuctionBid(winningBid);
                            unitOfWork.Save();

                            success = true;
                        }
                    }
                    else
                    {
                        if ((user.Balance - user.ReservedBalance() - bid.BidAmount) >= 0)
                        {
                            bid.AddAuction(auction);
                            auctionRepository.InsertAuctionBid(bid);
                            unitOfWork.Save();

                            success = true;
                        }
                    }
                }
            }
            else
            {
                if ((user.Balance - user.ReservedBalance() - bid.BidAmount) >= 0)
                {
                    bid.AddAuction(auction);
                    auctionRepository.InsertAuctionBid(bid);
                    unitOfWork.Save();

                    success = true;
                }
            }

            return success;
        }

        public void DrawAuctionWinners(int id)
        {
            Auction auction = GetAuctionByID(id);

            if (auction == null)
            {
                return;
            }

            if (auction.Winners?.Count > 0 || auction.IsOpen())
            {
                return;
            }

            List<AuctionBid> winningBids = auction.WinningBids().ToList();

            if (winningBids == null)
            {
                return;
            }

            auction.Winners = new HashSet<AppUser>();
            List<string> keys = auction.GetKeys().ToList();

            foreach (AuctionBid bid in winningBids)
            {
                auction.Winners.Add(bid.AppUser);
                if (bid.AppUser.WonAuctions == null)
                {
                    bid.AppUser.WonAuctions = new HashSet<Auction>();
                }
                bid.AppUser.WonAuctions.Add(auction);

                string key = String.Empty;

                if (keys.Count > 0)
                {
                    key = keys.First();
                    keys.Remove(key);
                }

                ClaimedProductKey userKey;
                BalanceEntry balanceEntry;
                ProductKey pKey;

                if (String.IsNullOrEmpty(key) == false)
                {
                    pKey = new ProductKey(auction.ListingID, key);
                }
                else
                {
                    pKey = new ProductKey(auction.ListingID, true);
                }

                userKey = new ClaimedProductKey(pKey, bid.AppUser, DateTime.Now, auction.Prize() + " Auction");

                int cost = bid.BidAmount;

                if (auction.IsSilent == false)
                {
                    cost = auction.PublicWinningBid();
                }

                balanceEntry = new BalanceEntry(bid.AppUser, auction.Prize() + " Auction", 0 - cost, DateTime.Now);

                bid.AppUser.AddBalanceEntry(balanceEntry);
                bid.AppUser.AddClaimedProductKey(userKey);
                bid.AppUser.Balance -= cost;

                userRepository.UpdateAppUserSynch(bid.AppUser);
            }

            unitOfWork.Save();
        }

        public void DeleteAuctionBid(int id)
        {
            if (auctionRepository.GetAuctionBidByID(id) != null)
            {
                auctionRepository.DeleteAuctionBid(id);

                unitOfWork.Save();
            }
        }

        public async Task PopulateNewAuction(Auction auction)
        {
            if (auction.MinimumBid < 1)
            {
                auction.MinimumBid = 1;
            }

            if (auction.Increment < 1)
            {
                auction.Increment = 1;
            }

            Listing listing = listingRepository.GetListingByID(auction.ListingID);

            if (listing == null && auction.Listing == null)
            {
                auction.ListingID = 0;
            }
            else if (listing != null && auction.Listing == null)
            {
                auction.AddListing(listing);
            }

            if (auction.Copies < 1)
            {
                auction.Copies = 1;
            }

            if (auction.EndTime.CompareTo(DateTime.Now.AddMinutes(30)) <= 0)
            {
                auction.EndTime = DateTime.Now.AddMinutes(30);
            }
            
            auction.AddCreator(await GetCurrentUser());
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
            return await UserManager.FindByNameAsyncWithStoreFilters(userName);
        }
        
        public async Task<AppUser> GetCurrentUser(string name)
        {
            return await UserManager.FindByNameAsync(name);
        }
    }
}
