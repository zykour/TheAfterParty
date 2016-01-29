using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SteamKit2;
using System;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;

namespace TheAfterParty.Domain.Services
{
    public class StoreService : IStoreService
    {
        private IListingRepository listingRepository;
        private IUserRepository userRepository;
        private IUnitOfWork unitOfWork;
        public AppUserManager UserManager { get; private set; }
        public string userName { get; set; }

        public StoreService(IListingRepository listingRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.listingRepository = listingRepository;
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }
        protected StoreService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }


        public IEnumerable<Listing> GetStockedStoreListings()
        {
            return listingRepository.GetListings().Where(l => l.Quantity > 0).ToList();
        }

        public IEnumerable<AppUser> GetAppUsers()
        {
            return  UserManager.Users;
        }

        public IEnumerable<Listing> GetListingsWithDeals()
        {
            return listingRepository.GetListings().Where(l => l.HasSale()).ToList();
        }

        public IEnumerable<Platform> GetPlatforms()
        {
            return listingRepository.GetPlatforms();
        }
        
        public Platform GetPlatformByID(int id)
        {
            return listingRepository.GetPlatformByID(id);
        }

        public void EditPlatform(Platform platform)
        {
            if (platform.PlatformID == 0)
            {
                listingRepository.InsertPlatform(platform);
            }
            else
            {
                listingRepository.UpdatePlatform(platform);
            }
        }
        
        public IEnumerable<Listing> AddProductKeys(Platform platform, string input)
        {
            //platform
            //productkey
            //listing
            //app movie / app screenshot
            // product
            // productdetail
            // productcategory

            ICollection<Listing> updatedListings = new HashSet<Listing>();

            input = input.Replace("\r\n", "\r");

            List<String> lines = input.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            
            Regex AppIDPriceKey = new Regex(@"([0-9]+)\t+([0-9]+)\t+([^\t]+)");
            Regex NamePriceKey = new Regex(@"([^\t]+)\t+([0-9]+)\t+([^\t]+)");
            Regex AppIDPrice = new Regex(@"([0-9]+)\t+([0-9]+)");
            Regex NamePrice = new Regex(@"([^\t]+)\t+([0-9]+)");

            bool isGift = false;
            string key = "";
            string gameName = "";
            int appId = 0;
            int price = 0;

            Match match;

            foreach (String line in lines)
            {
                if (AppIDPriceKey.Match(line).Success)
                {
                    match = AppIDPriceKey.Match(line);

                    appId = Int32.Parse(match.Groups[1].ToString());
                    price = Int32.Parse(match.Groups[2].ToString());
                    key = match.Groups[3].ToString();
                }
                else if (NamePriceKey.Match(line).Success)
                {
                    match = NamePriceKey.Match(line);

                    gameName = match.Groups[1].ToString();
                    price = Int32.Parse(match.Groups[2].ToString());
                    key = match.Groups[3].ToString();
                }
                else if (AppIDPrice.Match(line).Success)
                {
                    match = AppIDPrice.Match(line);

                    appId = Int32.Parse(match.Groups[1].ToString());
                    price = Int32.Parse(match.Groups[2].ToString());
                    isGift = true;
                }
                else if (NamePrice.Match(line).Success)
                {
                    match = NamePrice.Match(line);

                    gameName = match.Groups[1].ToString();
                    price = Int32.Parse(match.Groups[2].ToString());
                    isGift = true;
                }

                Listing listing;

                if (appId != 0)
                {
                    listing = listingRepository.GetListings().Where(l => l.ContainsPlatform(platform) && l.Product.AppID == appId).SingleOrDefault();
                }
                else
                {
                    listing = listingRepository.GetListings().Where(l => l.ContainsPlatform(platform) && object.Equals(l.Product.ProductName, gameName)).SingleOrDefault();
                }

                if (listing != null)
                {
                    listing.ListingPrice = price;
                    listing.AddProductKey(new ProductKey(isGift, key));
                    listingRepository.UpdateListing(listing);
                }
                else
                {
                    listing = new Listing(gameName, price);
                    listing.AddPlatform(platform);
                    listing.AddProductKey(new ProductKey(isGift, key));

                    Product product = new Product(appId, gameName);
                    //Add logic to get data from api on product info & product details

                    listing.AddProduct(product);
                    listingRepository.InsertListing(listing);
                }

                // test regex
                // manage what has already been added ?
                updatedListings.Add(listing);
                unitOfWork.Save();
            }
            
            return updatedListings.ToList();
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
            return await UserManager.FindByNameAsync(userName);
        }
    }
}
