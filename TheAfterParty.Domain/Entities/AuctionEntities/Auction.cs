using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TheAfterParty.Domain.Entities
{
    public class Auction
    {
        public Auction()
        {
            CreatedTime = DateTime.Now;
            AlternativePrize = String.Empty;
            ListingID = 0;
            IsSilent = false;
            Increment = 1;
            MinimumBid = 1;
            Copies = 1;
            EndTime = DateTime.Now.AddHours(1);

            AuctionBids = new HashSet<AuctionBid>();
            AuctionKeys = String.Empty;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuctionID { get; set; }

        // the id of the product being auctioned
        public int ListingID { get; set; }

        // the product being auctioned
        public virtual Listing Listing { get; set; }
        public void AddListing(Listing listing)
        {
            Listing = listing;

            if (listing.Auctions == null)
            {
                listing.Auctions = new HashSet<Auction>();
            }

            listing.Auctions.Add(this);
        }

        // if the prize is not a Product in the db (custom prize)
        public string AlternativePrize { get; set; }
        public string Prize()
        {
            if (Listing != null)
            {
                return Listing.ListingName;
            }
            else
            {
                return AlternativePrize;
            }
        }

        public DateTime CreatedTime { get; set; }
        // when the auction ends
        public DateTime EndTime { get; set; }
        public bool HasDaysOpen()
        {
            DateTime now = DateTime.Now;

            TimeSpan ts = EndTime - now;

            return ts.Days > 0;
        }
        public int DaysLeft()
        {
            DateTime now = DateTime.Now;

            TimeSpan ts = EndTime - now;

            return ts.Days;
        }
        public bool HasHoursOpen()
        {
            DateTime now = DateTime.Now;

            TimeSpan ts = EndTime - now;

            if (ts.Days > 0)
            {
                return false;
            }

            return ts.Hours > 0;
        }
        public int HoursLeft()
        {
            DateTime now = DateTime.Now;

            TimeSpan ts = EndTime - now;

            return ts.Hours;
        }
        public bool HasMinutesOpen()
        {
            DateTime now = DateTime.Now;

            TimeSpan ts = EndTime - now;

            if (ts.Days > 0 || ts.Hours > 0)
            {
                return false;
            }

            return ts.Minutes > 0;
        }
        public int MinutesLeft()
        {
            DateTime now = DateTime.Now;

            TimeSpan ts = EndTime - now;

            return ts.Minutes;
        }
        public bool HasSecondsOpen()
        {
            DateTime now = DateTime.Now;

            TimeSpan ts = EndTime - now;

            if (ts.Days > 0 || ts.Hours > 0 || ts.Minutes > 0)
            {
                return false;
            }

            return ts.Seconds > 0;
        }
        public int SecondsLeft()
        {
            DateTime now = DateTime.Now;

            TimeSpan ts = EndTime - now;

            return ts.Seconds;
        }
        public bool IsOpen()
        {
            return EndTime.CompareTo(DateTime.Now) > 0;
        }

        // a list of bids on the item
        public virtual ICollection<AuctionBid> AuctionBids { get; set; }
        public bool AuctionBidsIsNullOrEmpty()
        {
            return AuctionBids == null || AuctionBids.Count == 0;
        }
        public bool ContainsBidBy(AppUser user)
        {
            return AuctionBids.Any(a => object.Equals(user.Id, a.AppUser.Id));
        }
        public ICollection<AuctionBid> GetUserBids(AppUser user)
        {
            return AuctionBids.Where(a => object.Equals(a.AppUser.Id, user.Id)).OrderByDescending(b => b.BidAmount).ToList();
        }
        public void AddAuctionBid(AuctionBid auctionBid)
        {
            if (AuctionBids == null)
            {
                AuctionBids = new HashSet<AuctionBid>();
            }

            AuctionBids.Add(auctionBid);

            if (!IsSilent)
            {
                MinimumBid = (int)Math.Max(MinimumBid, auctionBid.BidAmount + Increment);
            }
        }
        public int WinningBid()
        {
            if (AuctionBidsIsNullOrEmpty())
            {
                return 0;
            }

            int max = 0;

            foreach (AuctionBid bid in AuctionBids)
            {
                max = (max < bid.BidAmount) ? bid.BidAmount : max;
            }

            return max;
        }
        public int LowestWinningBid()
        {
            if (AuctionBidsIsNullOrEmpty())
            {
                return MinimumBid;
            }

            if (AuctionBids.Count == 1)
            {
                return AuctionBids.First().BidAmount;
            }

            if (Copies == 1)
            {
                return WinningBid();
            }
            

            int lowestBid = AuctionBids.GroupBy(a => a.AppUser.Id)
                                    .Select(g => g.OrderByDescending(x => x.BidAmount).First())
                                    .OrderByDescending(a => a.BidAmount)
                                    .Skip(Copies - 1)
                                    .Take(1)
                                    .FirstOrDefault()
                                    .BidAmount;

            return lowestBid;
        }
        public AuctionBid LowestWinningAuctionBid()
        {
            if (AuctionBidsIsNullOrEmpty())
            {
                return null;
            }

            if (AuctionBids.Count == 1 || Copies == 1)
            {
                return WinningAuctionBid();
            }
            
            return AuctionBids.GroupBy(a => a.AppUser.Id)
                                    .Select(g => g.OrderByDescending(x => x.BidAmount).First())
                                    .OrderByDescending(a => a.BidAmount)
                                    .Skip(Copies - 1)
                                    .Take(1)
                                    .FirstOrDefault();
        }
        public IEnumerable<AuctionBid> WinningBids()
        {
            if (AuctionBidsIsNullOrEmpty())
            {
                return null;
            }

            if (AuctionBids.Count == 1)
            {
                return AuctionBids;
            }

            if (Copies == 1)
            {
                return new List<AuctionBid>() { WinningAuctionBid() };
            }

            return AuctionBids.GroupBy(a => a.AppUser.Id)
                                   .Select(g => g.OrderByDescending(x => x.BidAmount).First())
                                   .OrderByDescending(a => a.BidAmount)
                                   .Take(Copies);
        }
        public AuctionBid WinningAuctionBid()
        {
            if (AuctionBidsIsNullOrEmpty())
            {
                return null;
            }

            AuctionBid max = AuctionBids.First();

            foreach (AuctionBid bid in AuctionBids)
            {
                max = (max.BidAmount < bid.BidAmount) ? bid : max;
            }

            return max;
        }
        public AuctionBid UserAuctionBid(AppUser user)
        {
            if (AuctionBidsIsNullOrEmpty())
            {
                return null;
            }

            return AuctionBids.Where(a => object.Equals(user.Id, a.AppUser.Id)).FirstOrDefault();
        }
        public bool UserIsWinningBid(AppUser user)
        {
            if (AuctionBidsIsNullOrEmpty())
            {
                return false;
            }

            if (Copies == 1)
            {
                AuctionBid winningBid = WinningAuctionBid();

                return (winningBid.AppUser.Id.CompareTo(user.Id) == 0);
            }

            foreach (AuctionBid bid in WinningBids())
            {
                if (bid.AppUser.Id.CompareTo(user.Id) == 0)
                {
                    return true;
                }
            }

            return false;
        }
        public AuctionBid UserWinningBid(AppUser user)
        {
            if (AuctionBidsIsNullOrEmpty())
            {
                return null;
            }

            return WinningBids().Where(a => object.Equals(user.Id, a.AppUser.Id)).FirstOrDefault();
        }
        public int WinningBidsCount()
        {
            return AuctionBids.GroupBy(a => a.AppUser.Id)
                                   .Select(g => g.OrderByDescending(x => x.BidAmount).First())
                                   .OrderByDescending(a => a.BidAmount)
                                   .Take(Copies)
                                   .Count();
        }
        public bool HasMoreCopiesThanWinningBids()
        {
            return Copies > WinningBidsCount();
        }

        public int Increment { get; set; }
        
        // the appuser object of the winner
        public virtual ICollection<AppUser> Winners { get; set; }
        public bool IsWinner(AppUser user)
        {
            if (Winners == null)
            {
                return false;
            }

            if (Winners.Where(w => object.Equals(w.Id, user.Id)).Count() > 0)
            {
                return true;
            }

            return false;
        }
        public int GetSelectedWinnersCount()
        {
            if (Winners == null)
            {
                return 0;
            }
            else
            {
                return Winners.Count();
            }

        }

        // is the auction a silent auction
        public bool IsSilent { get; set; }

        // minimum bid amount (or starting bid) if any
        public int MinimumBid { get; set; }

        public string CreatorID { get; set; }

        public virtual AppUser Creator { get; set; }
        public bool IsCreator(AppUser user)
        {
            return (Creator == null) ? false : Object.Equals(user.Id, Creator.Id);
        }
        public void AddCreator(AppUser user)
        {
            Creator = user;

            if (user.Auctions == null)
            {
                user.Auctions = new HashSet<Auction>();
            }

            user.Auctions.Add(this);
        }

        public int Copies { get; set; }

        //product keys for the reward. delimiter = /r or /c
        //if Copies > keys, then the remaining ones will assumed to be gifts
        public string AuctionKeys { get; set; }
        public IEnumerable<string> GetKeys()
        {
            if (String.IsNullOrEmpty(AuctionKeys))
            {
                return new List<string>();
            }

            string tempKeys = AuctionKeys.Replace("\r\n", "\r");
            tempKeys = AuctionKeys.Replace("\n", "\r");

            return tempKeys.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        public int GiftCount()
        {
            return Copies - GetKeys().Count();
        }
    }
}
