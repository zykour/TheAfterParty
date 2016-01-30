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
using System.Web;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

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
        
        public IList<String> AddProductKeys(Platform platform, string input)
        {
            //platform
            //productkey
            //listing
            //app movie / app screenshot
            // product
            // productdetail
            // productcategory

            List<String> addedKeys = new List<String>();

            input = input.Replace("\r\n", "\r");

            List<String> lines = input.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            
            Regex AppIDPriceKey = new Regex(@"^([0-9]+)\t+([0-9]+)\t+([^\t]+)$");
            Regex NamePriceKey = new Regex(@"^([^\t]+)\t+([0-9]+)\t+([^\t]+)$");
            Regex AppIDPrice = new Regex(@"^([0-9]+)\t+([0-9]+)$");
            Regex NamePrice = new Regex(@"^([^\t]+)\t+([0-9]+)$");

            Match match;
            bool isGift = false;
            string key = "";
            string gameName = "";
            int appId = 0;
            int price = 0;

            foreach (String line in lines)
            {
                price = 0;
                appId = 0;
                gameName = "";
                key = "";
                isGift = false;                

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

                    if (listing == null)
                    {
                        // if we still haven't found a listing with the supplied appID or gameName, make sure this is not some edge case where the game name is purely numeric
                        // and thus the name would have been parsed as the appId with the regex.
                        listing = listingRepository.GetListings().Where(l => l.ContainsPlatform(platform) && object.Equals(l.Product.ProductName, appId)).SingleOrDefault();
                    }
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

                    product = BuildProduct(product, platform.PlatformName);

                    listing.AddProduct(product);
                    listingRepository.InsertListing(listing);
                }

                // test regex

                if (listing.ListingID == 0)
                {
                    addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...created!");
                }
                else
                {
                    addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...+1!");
                }

                unitOfWork.Save();
            }
            
            return addedKeys;
        }

        private Product BuildProduct(Product product, string platformName)
        {
            if (platformName.ToLower().CompareTo("steam") != 0)
            {
                return product;
            }

            ProductDetail productDetail = new ProductDetail();
            ICollection<AppMovie> AppMovies = new HashSet<AppMovie>();
            ICollection<AppScreenshot> AppScreenshots = new HashSet<AppScreenshot>();
            ICollection<ProductCategory> ProductCategories = new HashSet<ProductCategory>();

            string url = String.Format("http://store.steampowered.com/api/appdetails?appids={0}", product.AppID);

            string result = new System.Net.WebClient().DownloadString(url);

            JObject appData = JObject.Parse(result);

            string appID = product.AppID.ToString();

            if (!appData[appID].Any())
            {
                return product;
            }

            //productDetail.ProductType = appData[appID]["data"]

            return product;
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
