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

        public Listing GetListingByAppID(int id, string platformName)
        {
            return listingRepository.GetListings().Where(l => l.Product.AppID == id && l.Platforms.Where(p => object.Equals(platformName,p.PlatformName)).Count() > 0).SingleOrDefault();
        }
        
        public async Task<IList<String>> AddProductKeys(Platform platform, string input)
        {
            if (platform.PlatformName.ToLower().CompareTo("steam") == 0)
            {
                return await AddSteamProductKeys(platform, input);
            }

            List<String> addedKeys = new List<String>();

            input = input.Replace("\r\n", "\r");

            List<String> lines = input.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            Regex NamePriceKey = new Regex("^([^\t]+)\t+([0-9]+)\t+([^\t]+)$");
            Regex NamePrice = new Regex("^([^\t]+)\t+([0-9]+)$");

            DateTime dateAdded = DateTime.Now;

            Match match;
            
            bool isGift = false;
            string key = "";
            string gameName = "";
            int price = 0;

            foreach (String line in lines)
            {
                price = 0;
                gameName = "";
                key = "";
                isGift = false;

                if (NamePriceKey.Match(line).Success)
                {
                    match = NamePriceKey.Match(line);

                    gameName = match.Groups[1].Value;
                    price = Int32.Parse(match.Groups[2].ToString());
                    key = match.Groups[3].ToString();
                }
                else if (NamePrice.Match(line).Success)
                {
                    match = NamePrice.Match(line);

                    gameName = match.Groups[1].Value;
                    price = Int32.Parse(match.Groups[2].ToString());
                    isGift = true;
                }

                Listing listing;

                listing = listingRepository.GetListings().Where(l => l.ContainsPlatform(platform) && object.Equals(l.Product.ProductName, gameName)).SingleOrDefault();
                
                if (listing != null)
                {
                    listing.ListingPrice = price;
                    listing.AddProductKey(new ProductKey(isGift, key));
                    listing.DateEdited = dateAdded;
                    listingRepository.UpdateListing(listing);

                    addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...+1!");
                }
                else
                {
                    listing = new Listing(gameName, price, dateAdded);
                    listing.AddPlatform(platform);
                    listing.AddProductKey(new ProductKey(isGift, key));

                    Product product = new Product(gameName);
                    //Add logic to get data from api on product info & product details

                    listing.AddProduct(product);

                    listingRepository.InsertListing(listing);

                    addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...created!");
                }
            }

            return addedKeys;
        }

        public async Task<IList<String>> AddSteamProductKeys(Platform platform, string input)
        {
            List<String> addedKeys = new List<String>();

            input = input.Replace("\r\n", "\r");

            List<String> lines = input.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            Regex SubIDPriceKey = new Regex(@"^sub/([0-9]+)(\t+[^\t]+)?\t+([0-9]+)\t+([^\t]+)$");
            Regex SubIDPrice = new Regex(@"^sub/([0-9]+)(\t+[^\t]+)?\t+([0-9]+)$");
            Regex AppIDPriceKey = new Regex(@"^([0-9]+)\t+([0-9]+)\t+([^\t]+)$");
            Regex AppIDPrice = new Regex(@"^([0-9]+)\t+([0-9]+)$");

            DateTime dateAdded = DateTime.Now;

            Match match;
            bool isGift = false;
            string key = "";
            string gameName = "";
            int appId = 0;
            int price = 0;
            int subId = 0;

            foreach (String line in lines)
            {
                price = 0;
                appId = 0;
                gameName = "";
                key = "";
                isGift = false;
                subId = 0;           

                if (AppIDPriceKey.Match(line).Success)
                {
                    match = AppIDPriceKey.Match(line);

                    appId = Int32.Parse(match.Groups[1].ToString());
                    price = Int32.Parse(match.Groups[2].ToString());
                    key = match.Groups[3].ToString();
                }
                else if (SubIDPriceKey.Match(line).Success)
                {
                    match = SubIDPriceKey.Match(line);

                    subId = Int32.Parse(match.Groups[1].ToString());
                    gameName = match.Groups[2].Value ?? gameName;
                    price = Int32.Parse(match.Groups[3].ToString());
                    key = match.Groups[4].ToString();
                }
                else if (AppIDPrice.Match(line).Success)
                {
                    match = AppIDPrice.Match(line);

                    appId = Int32.Parse(match.Groups[1].ToString());
                    price = Int32.Parse(match.Groups[2].ToString());
                    isGift = true;
                }
                else if (SubIDPrice.Match(line).Success)
                {
                    match = SubIDPrice.Match(line);

                    subId = Int32.Parse(match.Groups[1].ToString());
                    gameName = match.Groups[2].Value ?? gameName;
                    price = Int32.Parse(match.Groups[3].ToString());
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
                    listing.DateEdited = dateAdded;
                    listingRepository.UpdateListing(listing);

                    addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...+1!");
                }
                else
                {
                    ListingBuilder builder = new ListingBuilder(this);

                    listing = new Listing(gameName, price, dateAdded);
                    listing.AddPlatform(platform);
                    listing.AddProductKey(new ProductKey(isGift, key));

                    Product product = new Product(appId, gameName);
                    //Add logic to get data from api on product info & product details

                    listing.AddProduct(product);

                    if (appId != 0)
                    {
                        builder.BuildListingWithAppID(listing, appId);

                        if (String.IsNullOrEmpty(listing.ListingName))
                        {
                            listing.ListingName = product.ProductName;
                        }
                    }
                    else if (subId != 0)
                    {
                        await builder.BuildListingWithPackageID(listing, subId, gameName);
                    }

                    listingRepository.InsertListing(listing);

                    addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...created!");
                }

                // test regex

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

            JObject jsonResult = JObject.Parse(result);

            string appID = product.AppID.ToString();

            if (!jsonResult[appID].Any() || !jsonResult[appID]["data"].Any())
            {
                return product;
            }

            JToken appData = jsonResult[appID]["data"];

            productDetail.ProductType = (string)appData["type"];
            productDetail.ProductName = (string)appData["name"];
            productDetail.AgeRequirement = (int)appData["required_age"];
            productDetail.DetailedDescription = (string)appData["detailed_description"];
            productDetail.DLCAppIDs = appData["dlc"].Select(d => (int)d).ToArray();
            productDetail.AboutTheGame = (string)appData["about_the_game"];

            product.AddProductDetail(productDetail);
            if (String.IsNullOrEmpty(product.ProductName))
            {
                product.ProductName = productDetail.ProductName;
            }

            return product;
        }

        public ProductCategory GetProductCategoryByName(string categoryName)
        {
            return listingRepository.GetProductCategories().Where(c => object.Equals(c.CategoryString, categoryName)).SingleOrDefault();
        }

        public void AddProductCategory(ProductCategory category)
        {
            listingRepository.InsertProductCategory(category);
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
