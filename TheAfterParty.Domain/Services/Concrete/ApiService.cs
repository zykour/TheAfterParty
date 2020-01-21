using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;

namespace TheAfterParty.Domain.Services
{
    public class ApiService : IApiService
    {
        private IUnitOfWork unitOfWork;
        private IUserRepository userRepository;
        private IListingRepository listingRepository;
        private IAuctionRepository auctionRepository;
        private IObjectiveRepository objectiveRepository;
        private ISiteRepository siteRepository;
        public AppUserManager UserManager { get; private set; }

        public ApiService(IAuctionRepository auctionRepository, IUserRepository userRepository, IListingRepository listingRepository, IObjectiveRepository objectiveRepository, ISiteRepository siteRepository, IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.auctionRepository = auctionRepository;
            this.userRepository = userRepository;
            this.listingRepository = listingRepository;
            this.objectiveRepository = objectiveRepository;
            this.siteRepository = siteRepository;
            this.unitOfWork = unitOfWork;
        }
        protected ApiService(AppUserManager userManager)
        {
            UserManager = userManager;
        }
        
        public IEnumerable<String> GetUsersWhoOwn(int appId)
        {
            return userRepository.GetUsersWhoOwn(appId).OrderBy(s => s);
        }

        public IEnumerable<Listing> SearchListings(string searchText, int resultLimit)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                return new HashSet<Listing>();
            }

            return listingRepository.SearchListings(searchText, resultLimit);
        }

        public Listing GetDailyDeal()
        {
            return listingRepository.GetDiscountedListings().FirstOrDefault(l => l.DailyDeal == true).Listing;
        }

        public IEnumerable<Listing> GetWeeklyDeals()
        {
            return listingRepository.GetListingsQuery()
                            .Where(l => l.DiscountedListings.Any(d => d.WeeklyDeal) && l.ListingPrice > 0 && l.Quantity > 0)
                            .OrderBy(l => l.ListingName)
                            .AsEnumerable();
        }

        public IEnumerable<Listing> GetOtherDeals()
        {
            return listingRepository.GetListingsQuery()
                            .Where(l => l.DiscountedListings.Any(d => d.WeeklyDeal == false && d.DailyDeal == false) && l.ListingPrice > 0 && l.Quantity > 0)
                            .OrderBy(l => l.ListingName)
                            .AsEnumerable();
        }

        public IEnumerable<Listing> GetWeeklyDeals(int resultsLimit)
        {
            return listingRepository.GetListingsQuery()
                            .Where(l => l.DiscountedListings.Any(d => d.WeeklyDeal) && l.ListingPrice > 0 && l.Quantity > 0)
                            .OrderByDescending(l => l.ListingPrice)
                            .Take(resultsLimit)
                            .AsEnumerable();
        }

        public IEnumerable<Listing> GetOtherDeals(int resultsLimit)
        {
            return listingRepository.GetListingsQuery()
                            .Where(l => l.DiscountedListings.Any(d => d.WeeklyDeal == false && d.DailyDeal == false) && l.ListingPrice > 0 && l.Quantity > 0)
                            .OrderByDescending(l => l.ListingPrice)
                            .Take(resultsLimit)
                            .AsEnumerable();
        }
        public Listing GetListingByID(int id)
        {
            return listingRepository.GetListingByID(id);
        }

        public Listing GetListingByAppID(int appId)
        {
            return listingRepository.GetListings().Where(l => l.Product.AppID == appId).FirstOrDefault();
        }

        public Order BuyAndRevealListing(AppUser user, int listingId, int price)
        {
            Order order = null;

            if (user.Balance < price)
            {
                return order;
            }

            Listing targetListing = listingRepository.GetListingByID(listingId);

            if (targetListing == null)
            {
                return order;
            }

            DateTime orderDate = new DateTime();
            orderDate = DateTime.Now;
            
            List<ProductKey> keys;

            ProductKey key = listingRepository.GetProductKeys().Where(k => k.ListingID == listingId).FirstOrDefault();

            if (key == null && targetListing.ChildListings != null)
            {
                keys = new List<ProductKey>();

                foreach (Listing childListing in targetListing.ChildListings)
                {
                    ProductKey temp = listingRepository.GetProductKeys().Where(k => k.ListingID == childListing.ListingID).SingleOrDefault();

                    if (temp != null)
                    {
                        keys.Add(temp);
                    }
                    else
                    {
                        return order;
                    }
                }
            }
            else
            {
                keys = new List<ProductKey>() { key };
            }
            
            order = new Order(user, orderDate);
            userRepository.InsertOrder(order);
            unitOfWork.Save();

            ProductOrderEntry orderEntry = new ProductOrderEntry(order, targetListing);
            order.AddProductOrderEntry(orderEntry);

            //if (targetListing.ChildListings == null || targetListing.ChildListings.Count == 0)
            //{
            foreach (ProductKey productKey in keys)
            {
                listingRepository.DeleteProductKey(productKey.ProductKeyID);

                productKey.Listing.Quantity--;
                //productKey.Listing.UpdateParentQuantities();

                listingRepository.UpdateListing(productKey.Listing);

                ClaimedProductKey claimedKey = new ClaimedProductKey(productKey, user, orderDate, "Purchase - Order #" + order.OrderID);
                user.AddClaimedProductKey(claimedKey);
                claimedKey.IsRevealed = true;
                userRepository.InsertClaimedProductKey(claimedKey);
                unitOfWork.Save();

                orderEntry.AddClaimedProductKey(claimedKey);
            }

            userRepository.InsertProductOrderEntry(orderEntry);

            unitOfWork.Save();

            return order;
        }

        public int GetNumDeals()
        {
            return listingRepository.GetDiscountedListings().Count();
        }

        public IEnumerable<Objective> SearchObjectives(string searchText, int resultsLimit)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                return new HashSet<Objective>();
            }

            return objectiveRepository.SearchObjectives(searchText, resultsLimit);
        }

        public bool ToggleActiveObjective(Objective objective)
        {
            objective.IsActive = !objective.IsActive;

            objectiveRepository.UpdateObjective(objective);

            unitOfWork.Save();

            return objective.IsActive;
        }

        public AppUser GetUserByNickName(string nickname)
        {
            nickname = nickname.ToUpper();

            return UserManager.Users.Where(u => u.Nickname.ToUpper().CompareTo(nickname) == 0).FirstOrDefault();
        }
        public AppUser GetUserByUserName(string username)
        {
            username = username.Trim().ToLower();

            return UserManager.Users.Where(u => u.UserName.ToLower().CompareTo(username) == 0).FirstOrDefault();
        }
        public AppUser GetUserBySteamID(long id)
        {
            return UserManager.Users.Where(u => u.UserSteamID == id).FirstOrDefault();
        }
        public AppUser GetUserBySteamID(UInt64 id)
        {
            return GetUserBySteamID(Convert.ToInt64(id));
        }
        public AppUser GetUserWithHighestBalance()
        {
            return UserManager.Users.OrderByDescending(u => u.Balance).FirstOrDefault();
        }

        public AppUser GetUser(string identifier)
        {
            AppUser user;

            user = GetUserByUserName(identifier.Trim());

            if (user != null)
            {
                return user;
            }

            user = GetUserByNickName(identifier.Trim());

            if (user != null)
            {
                return user;
            }

            long id = 0;
            Int64.TryParse(identifier, out id);

            if (id != 0)
            {
                user = GetUserBySteamID(id);

                if (user != null)
                {
                    return user;
                }
            }

            return null;
        }

        public AppUser GetPOTW()
        {
            return siteRepository.GetPOTWs().OrderByDescending(p => p.StartDate).FirstOrDefault().AppUser;
        }
        public void SetPOTW(AppUser user)
        {
            POTW potw = new POTW();
            potw.AppUser = user;
            potw.StartDate = DateTime.Today;
            potw.StartDate.AddHours(15);

            siteRepository.InsertPOTW(potw);

            unitOfWork.Save();
        }

        public Objective GetObjectiveByID(int objectiveId)
        {
            return objectiveRepository.GetObjectiveByID(objectiveId);
        }

        public List<String> AddBalance(string[] userNickNames)
        {
            int points = 0;

            List<String> updatedUsers = new List<String>();

            DateTime time = DateTime.Now;

            foreach (string userNickname in userNickNames)
            {
                string nickname = userNickname;
                if (char.IsNumber(nickname[0]) == true || nickname[0] == '-')
                {
                    string numSubString = String.Empty;
                    int k = 0;

                    // easy way to check the first part of the string to see if it's a negative number
                    if (nickname[0] == '-')
                    {
                        k = 1;
                        numSubString = "-";
                    }

                    for (int i = k; i < nickname.Length - 1; i++)
                    {
                        if (char.IsNumber(nickname[i]))
                        {
                            numSubString += nickname[i];
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    nickname = nickname.Substring(numSubString.Length);
                    Int32.TryParse(numSubString, out points);
                }
                AppUser user = GetUserByNickName(nickname);

                if (user != null)
                {
                    user.CreateBalanceEntry("Admin adjusted balance update", points , time);
                    UserManager.Update(user);

                    updatedUsers.Add(user.UserName + ": " + points + "\n");
                }
            }
            unitOfWork.Save();

            return updatedUsers;
        }

        public List<String> AddBalanceForObjective(Objective objective, string[] userNickNames, int objectiveReward)
        {
            List<String> userRewards = new List<String>();
            int numCompletions = 1; //default 1

            DateTime time = DateTime.Now;

            foreach (string userNickname in userNickNames)
            {
                string nickname = userNickname;
                if (char.IsNumber(nickname[0]) == true)
                {
                    string numSubString = String.Empty;

                    for (int i = 0; i < nickname.Length - 1; i++)
                    {
                        if (char.IsNumber(nickname[i]))
                        {
                            numSubString = numSubString += nickname[i];
                        }
                        else
                        {
                            break;
                        }
                    }
                    nickname = nickname.Substring(numSubString.Length);
                    Int32.TryParse(numSubString, out numCompletions);
                }
                AppUser user = GetUserByNickName(nickname);

                if (user != null)
                {
                    for (int i = 0; i < numCompletions; i++)
                    {
                        user.CreateBalanceEntry(objective, time);
                    }
                    UserManager.Update(user);

                    userRewards.Add(user.UserName + ": " + objectiveReward * numCompletions + "\n");
                }
            }

            unitOfWork.Save();

            return userRewards;
        }

        public IEnumerable<Auction> GetOpenAuctions()
        {
            return auctionRepository.GetAuctions().Where(a => a.IsOpen());
        }
        public IEnumerable<BoostedObjective> GetLiveBoostedObjectives()
        {
            return objectiveRepository.GetBoostedObjectives().Where(o => o.IsLive());
        }
        public IEnumerable<DiscountedListing> GetLiveDiscountedListings()
        {
            return listingRepository.GetDiscountedListings().Where(l => l.IsLive());
        }

        public void RolloverDailyDeal()
        {
            DiscountedListing discountedListing = listingRepository.GetDiscountedListings().Where(d => d.DailyDeal).FirstOrDefault();

            if (discountedListing != null)
            {
                listingRepository.DeleteDiscountedListing(discountedListing.DiscountedListingID);
            }

            List<DiscountedListing> expiredListings = listingRepository.GetDiscountedListings().Where(d => d.IsLive() == false).ToList();

            if (expiredListings != null)
            {
                foreach (DiscountedListing listing in expiredListings)
                {
                    listingRepository.DeleteDiscountedListing(listing.DiscountedListingID);
                }
            }

            DiscountedListing newDiscountedListing = new DiscountedListing();

            Listing randomListing = listingRepository.GetListings().Where(l => l.Quantity > 0).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            if (randomListing == null)
            {
                unitOfWork.Save();
                return;
            }

            SelectDealPercent(newDiscountedListing);

            DateTime expiry = DateTime.Now.AddDays(1).AddMinutes(-5);

            newDiscountedListing.DailyDeal = true;
            newDiscountedListing.ItemSaleExpiry = expiry;

            randomListing.AddDiscountedListing(newDiscountedListing);

            listingRepository.UpdateListing(randomListing);
            
            SiteNotification notification = new SiteNotification() { Notification = "Today's daily deal is " + randomListing.ListingName + ", now on sale for " + randomListing.SaleOrDefaultPrice() + " points!", NotificationDate = DateTime.Now };
            siteRepository.InsertSiteNotification(notification);

            unitOfWork.Save();
        }

        private void SelectDealPercent(DiscountedListing discountedListing)
        {
            Random rand = new Random();
            
            int ran = rand.Next(100);

            if (ran < 40)
            {
                discountedListing.ItemDiscountPercent = 33;
            }
            else if (ran < 65)
            {
                discountedListing.ItemDiscountPercent = 50;
            }
            else if (ran < 85)
            {
                discountedListing.ItemDiscountPercent = 66;
            }
            else if (ran < 99)
            {
                discountedListing.ItemDiscountPercent = 75;
            }
            else
            {
                discountedListing.ItemDiscountPercent = 85;
            }
        }

        public void CreateNewWeeklyDeals(int numDeals)
        {
            List<Listing> newDeals = listingRepository.GetListings().Where(l => l.Quantity > 0).OrderBy(x => Guid.NewGuid()).Take(numDeals).ToList();

            DateTime dealExpiry = DateTime.Now.AddDays(7);

            foreach (Listing listing in newDeals)
            {
                DiscountedListing newDiscountedListing = new DiscountedListing();

                SelectDealPercent(newDiscountedListing);

                newDiscountedListing.WeeklyDeal = true;
                newDiscountedListing.ItemSaleExpiry = dealExpiry;

                listing.AddDiscountedListing(newDiscountedListing);

                listingRepository.UpdateListing(listing);
            }

            unitOfWork.Save();
        }

        public bool TransferPoints(AppUser sender, AppUser recipient, int points)
        {
            if (sender.Balance - sender.ReservedBalance() >= points)
            {
                sender.CreateBalanceEntry("Transfer of points to " + recipient.UserName, 0 - points, DateTime.Now);
                recipient.CreateBalanceEntry("Gift of points from " + sender.UserName, points, DateTime.Now);

                UserManager.Update(sender);
                UserManager.Update(recipient);

                unitOfWork.Save();

                return true;
            }

            return false;
        }

        public void AddSiteNotification(string notification)
        {
            SiteNotification siteNotification = new SiteNotification();
            siteNotification.Notification = notification;
            siteNotification.NotificationDate = DateTime.Now;

            siteRepository.InsertSiteNotification(siteNotification);

            unitOfWork.Save();
        }

        public Objective GetObjectiveWithBoostedDaily()
        {
            BoostedObjective obj = objectiveRepository.GetBoostedObjectives().Where(b => b.IsDaily && b.IsLive()).FirstOrDefault();

            if (obj != null)
            {
                return obj.Objective;
            }

            return null;
        }
        
        public void FixImages()
        {

            foreach (Product product in listingRepository.GetProducts().ToList())
            {
                if (product.HeaderImageURL != null && product.HeaderImageURL.Contains("?t="))
                {
                    product.HeaderImageURL = product.HeaderImageURL.Substring(0, product.HeaderImageURL.IndexOf("?t="));
                    listingRepository.UpdateProduct(product);
                }
            }

            unitOfWork.Save();
        }


        // change return to string list
        public String FixNames(string webApiKey)
        {
            String nullNames = String.Empty;
            //foreach (Listing listing in listingRepository.GetListings().ToList())
            //{
            //    /// temp
            //    if (listing.ListingName.StartsWith("ValveTest") == false)
            //    {
            //        continue;
            //    }
            //    else if (listing.Product.ProductName != null)
            //    {
            //        listing.ListingName = listing.Product.ProductName;
            //        listingRepository.UpdateListing(listing);
            //    }
            //}
            //unitOfWork.Save();
            //return nullNames;

            foreach (Listing listing in listingRepository.GetListings().ToList())
            {
                //temp
                if (listing.ListingName.StartsWith("UntitledAp") == false)
                {
                    continue;
                }
                if (listing.Product.AppID <= 0)
                {
                    nullNames += listing.ListingID + "(NoAppID) ";
                    continue;
                }

                string appID = listing.Product.AppID.ToString();
                string url = String.Format("https://store.steampowered.com/api/appdetails?appids={0}&l=english", appID); //String.Format("http://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key={0}&appid={1}&l=english", webApiKey, appID);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string result = String.Empty;
                try
                {
                    result = new System.Net.WebClient().DownloadString(url);
                }
                catch (WebException webEx)
                {
                    nullNames += listing.ListingID + "(WebEx) ";
                    continue;
                }
                JObject jsonResult = JObject.Parse(result);                

                if (jsonResult == null || jsonResult["game"] == null || jsonResult["game"]["gameName"] == null || ((JToken)jsonResult["game"]["gameName"]).IsNullOrEmpty())
                {
                    //url = String.Format("https://store.steampowered.com/api/appdetails?appids={0}&l=english", appID);
                    //try
                    //{
                    //    result = new System.Net.WebClient().DownloadString(url);
                    //}
                    //catch (WebException webEx)
                    //{
                    //    nullNames += listing.ListingID + "(WebEx) ";
                    //    continue;
                    //}
                    //jsonResult = JObject.Parse(result);

                    if (jsonResult == null || jsonResult[appID] == null || jsonResult[appID]["data"] == null)
                    {
                        nullNames += listing.ListingID + " ";
                        continue;
                    }           
                    else
                    {
                        JToken appData = jsonResult[appID]["data"];

                        if (appData["name"].IsNullOrEmpty())
                        {
                            nullNames += listing.ListingID + " ";
                            continue;
                        }
                        else
                        {
                            listing.Product.ProductName = (string)appData["name"];
                            listing.ListingName = (string)appData["name"];
                        }
                    }
                }
                else
                {
                    JToken appData = jsonResult["game"];

                    if (appData["gameName"].IsNullOrEmpty())
                    {
                        nullNames += listing.ListingID + " ";
                        continue;
                    }
                    else
                    {
                        listing.Product.ProductName = (string)appData["gameName"];
                        listing.ListingName = (string)appData["gameName"];
                    }
                }

                listingRepository.UpdateListing(listing);
            }

            unitOfWork.Save();

            return nullNames;
        }

        public void FixQuantities()
        {
            foreach (Listing listing in listingRepository.GetListingsPlain().ToList())
            {
                // count on ProductKeys for listingID
                // listing.Quantity = actualQuantity
                listingRepository.UpdateListing(listing);
            }

            unitOfWork.Save();
        }

        public int RollDeals(int deals, bool days, int duration, bool unique)
        {
            List<Listing> listings = new List<Listing>();
            DiscountedListing discount = new DiscountedListing();
            DateTime expiry = DateTime.UtcNow;


            if (unique)
            {
                listings = listingRepository.GetListingsQuery().Where(x => x.Quantity > 0 && x.ListingPrice > 1 && x.DiscountedListings.Count == 0).OrderBy(x => Guid.NewGuid()).ToList();
            }
            else
            {
                listings = listingRepository.GetListingsQuery().Where(x => x.Quantity > 0 && x.ListingPrice > 1).OrderBy(x => Guid.NewGuid()).ToList();
            }

            if (days)
            {
                expiry = expiry.AddDays(duration);
            }
            else
            {
                expiry = expiry.AddHours(duration);
            }

            int count = 0;
            int discountPct = 25;
            Random rand = new Random();

            int ran = 1;

            foreach (Listing listing in listings)
            {
                ran = rand.Next(100);

                if (ran < 40)
                {
                    discountPct = 33;
                }
                else if (ran < 65)
                {
                    discountPct = 50;
                }
                else if (ran < 85)
                {
                    discountPct = 66;
                }
                else if (ran < 99)
                {
                    discountPct = 75;
                }
                else
                {
                    discountPct = 85;
                }

                discount = new DiscountedListing(discountPct, expiry);
                discount.Listing = listing;
                discount.DailyDeal = false;
                discount.WeeklyDeal = false;
                
               if (++count >= deals)
                {
                    break;
                }

                listingRepository.InsertDiscountedListing(discount);
            }

            unitOfWork.Save();

            return count;
        }

        // If percent is 0, this signifies that percentages should be randomized
        public int RollDeals(int percent, bool days, int duration)
        {
            List<Listing> listings = listingRepository.GetListingsQuery().Where(x => x.Quantity > 0 && x.ListingPrice > 1).OrderBy(x => Guid.NewGuid()).ToList();
            DiscountedListing discount = new DiscountedListing();
            DateTime expiry = DateTime.UtcNow;

            if (days)
            {
                expiry = expiry.AddDays(duration);
            }
            else
            {
                expiry = expiry.AddHours(duration);
            }

            int discountPct = 25;

            if (percent > 0)
            {
                discountPct = percent;
            }
            Random rand = new Random();
            int count = 0;

            int ran = 1;

            foreach (Listing listing in listings)
            {
                if (percent == 0)
                {
                    ran = rand.Next(100);

                    if (ran < 40)
                    {
                        discountPct = 33;
                    }
                    else if (ran < 65)
                    {
                        discountPct = 50;
                    }
                    else if (ran < 85)
                    {
                        discountPct = 66;
                    }
                    else if (ran < 99)
                    {
                        discountPct = 75;
                    }
                    else
                    {
                        discountPct = 85;
                    }
                }

                discount = new DiscountedListing(discountPct, expiry);
                count++;
                listingRepository.InsertDiscountedListing(discount);
            }

            unitOfWork.Save();

            return count;
        }

        public void Dispose()
        {
            this.unitOfWork.Dispose();
        }

    }
}
