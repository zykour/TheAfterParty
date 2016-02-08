using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Abstract;
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
        public IEnumerable<Listing> GetListingsWithDeals()
        {
            return listingRepository.GetListings().Where(l => l.HasSale()).ToList();
        }
        public Listing GetListingByAppID(int id, string platformName)
        {
            return listingRepository.GetListings().Where(l => l.Product.AppID == id && l.ContainsPlatform(platformName)).SingleOrDefault();
        }
        public void AddListing(Listing listing)
        {
            listingRepository.InsertListing(listing);
            unitOfWork.Save();
        }

        public IEnumerable<AppUser> GetAppUsers()
        {
            return  UserManager.Users;
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


        
        public async Task<List<String>> AddProductKeys(Platform platform, string input)
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
        
        public ProductCategory GetProductCategoryByName(string categoryName)
        {
            return listingRepository.GetProductCategories().Where(c => object.Equals(c.CategoryString, categoryName)).SingleOrDefault();
        }
        public void AddProductCategory(ProductCategory category)
        {
            listingRepository.InsertProductCategory(category);
        }
        public void UpdateListing(Listing listing)
        {
            if (listing.ListingID != 0)
            {
                listingRepository.UpdateListing(listing);
            }
            else
            {
                listingRepository.InsertListing(listing);
            }

            unitOfWork.Save();
        }




        // --- Listing building logic

        public async Task<List<String>> AddSteamProductKeys(Platform platform, string input)
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
                else
                {
                    addedKeys.Add("Unable to parse: " + line);
                    continue;
                }

                Listing listing = null;

                if (appId != 0)
                {
                    listing = GetListingByAppID(appId, platform.PlatformName);
                }
                else if (subId != 0)
                {
                    listing = GetListingByAppID(subId, platform.PlatformName);
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
                    listing = new Listing(gameName, price, dateAdded);
                    listing.AddPlatform(platform);
                    listing.AddProductKey(new ProductKey(isGift, key));

                    Product product = new Product(appId, gameName);
                    //Add logic to get data from api on product info & product details

                    listing.AddProduct(product);

                    // insert this listing entry for now, as we build the listing with data gathered from Steam's store api
                    // we may need to build more listings recursively, we need this listing to be in the repository so it doesn't get stuck in a loop
                    listingRepository.InsertListing(listing);
                    // I'm okay with saving it to the database in this state, no harm done even if the building process fails, so let's save it to the database so we don't have to check
                    // against the context's local cache
                    unitOfWork.Save();

                    if (appId != 0)
                    {
                        BuildListingWithAppID(listing, appId);
                    }
                    else if (subId != 0)
                    {
                        await BuildListingWithPackageID(listing, subId, gameName);
                    }

                    listingRepository.UpdateListing(listing);
                    unitOfWork.Save();

                    addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...created!");
                }

                // test regex

                unitOfWork.Save();
            }

            return addedKeys;
        }

        // gets the AppIDs contained within the package and then builds a listing for each one (and recursively builds listings for DLCs or the base game for each AppID, if any exists) using BuildListingWithAppID
        private async Task BuildListingWithPackageID(Listing listing, int packageId, string name = "")
        {
            List<int> appIds = await GetPackagesAppIDs(packageId);

            foreach (int id in appIds)
            {
                Listing subListing = GetListingByAppID(id, "Steam");

                if (subListing == null)
                {
                    subListing = new Listing();
                    subListing.AddPlatform(GetPlatforms().Where(p => object.Equals(p.PlatformName, "Steam")).SingleOrDefault());
                    subListing.AddProduct(new Product(id));

                    AddListing(subListing);

                    BuildListingWithAppID(subListing, id);

                    UpdateListing(subListing);
                    
                    listing.ChildListings.Add(subListing);
                }
                else
                {
                    listing.ChildListings.Add(subListing);
                }
            }

            if (String.IsNullOrEmpty(listing.ListingName))
            {
                string newName = "";

                string[] names = listing.ChildListings.OrderBy(l => l.ListingName).Select(l => l.ListingName).ToArray();

                if (names.Count() > 0)
                {
                    newName = names[0];
                }

                for (int i = 1; i < names.Count(); i++)
                {
                    newName = newName + "," + names[i];
                }

                listing.ListingName = newName;
            }
        }

        // using a Steam App ID, queries the storefront API to get information about the product and build the listing object
        // listings sent to this method should be persisted to prevent looping behavior
        private void BuildListingWithAppID(Listing listing, int appId)
        {
            if (listing.ListingID == 0)
            {
                throw new Exception("The listing was not persisted!");
            }
            if (listing.Product == null)
            {
                throw new Exception("A valid product object was not added to the listing object!");
            }

            ProductDetail productDetail = listing.Product.ProductDetail ?? new ProductDetail();

            string url = String.Format("http://store.steampowered.com/api/appdetails?appids={0}", listing.Product.AppID);

            string result = new System.Net.WebClient().DownloadString(url);

            JObject jsonResult = JObject.Parse(result);

            string appID = listing.Product.AppID.ToString();

            if (jsonResult == null || jsonResult[appID] == null || jsonResult[appID]["data"] == null)
            {
                return;
            }

            JToken appData = jsonResult[appID]["data"];
            int baseAppID = 0;

            //get all the product details from the jtoken
            productDetail.ProductType = (string)appData["type"] ?? "";
            productDetail.ProductName = (string)appData["name"] ?? "";
            productDetail.AgeRequirement = (appData["required_age"].IsNullOrEmpty()) ? 0 : (int)appData["required_age"];
            productDetail.DetailedDescription = (string)appData["detailed_description"] ?? "";

            if (!appData["dlc"].IsNullOrEmpty())
            {
                productDetail.DLCAppIDs = appData["dlc"].Select(d => (int)d).ToArray();
            }

            productDetail.AboutTheGame = (string)appData["about_the_game"] ?? "";

            if (!appData["fullgame"].IsNullOrEmpty())
            {
                productDetail.BaseProductName = (appData["fullgame"]["name"].Any()) ? (string)appData["fullgame"]["name"] : "";
                baseAppID = (appData["fullgame"]["appid"].Any()) ? (int)appData["fullgame"]["appid"] : 0;
            }

            productDetail.SupportedLanguages = (string)appData["supported_languages"] ?? "";
            productDetail.HeaderImageURL = (string)appData["header_image"] ?? "";
            productDetail.ProductWebsite = (string)appData["website"] ?? "";

            if (!appData["pc_requirements"].IsNullOrEmpty())
            {
                productDetail.PCMinimumRequirements = (string)appData["pc_requirements"]["minimum"] ?? "";
                productDetail.PCRecommendedRequirements = (string)appData["pc_requirements"]["recommended"] ?? "";
            }

            if (!appData["mac_requirements"].IsNullOrEmpty())
            {
                productDetail.MacMinimumRequirements = (string)appData["mac_requirements"]["minimum"] ?? "";
                productDetail.MacRecommendedRequirements = (string)appData["mac_requirements"]["recommended"] ?? "";
            }

            if (!appData["linux_requirements"].IsNullOrEmpty())
            {
                productDetail.LinuxMinimumRequirements = (string)appData["linux_requirements"]["minimum"] ?? "";
                productDetail.LinuxRecommendedRequirements = (string)appData["linux_requirements"]["recommended"] ?? "";
            }

            if (!appData["developers"].IsNullOrEmpty())
            {
                productDetail.Developers = appData["developers"].Select(d => (string)d).ToArray();
            }

            if (!appData["publishers"].IsNullOrEmpty())
            {
                productDetail.Publishers = appData["publishers"].Select(d => (string)d).ToArray();
            }

            if (!appData["demos"].IsNullOrEmpty())
            {
                productDetail.DemoAppID = (appData["demos"]["appid"].IsNullOrEmpty()) ? 0 : (int)appData["demos"]["appid"];
                productDetail.DemoRestrictions = (string)appData["demos"]["description"] ?? "";
            }

            if (!appData["price_overview"].IsNullOrEmpty())
            {
                productDetail.FinalPrice = (appData["price_overview"]["final"].IsNullOrEmpty()) ? 0 : (int)appData["price_overview"]["final"];
                productDetail.InitialPrice = (appData["price_overview"]["initial"].IsNullOrEmpty()) ? 0 : (int)appData["price_overview"]["initial"];
                productDetail.CurrencyType = (string)appData["price_overview"]["currency"] ?? "";
            }

            if (!appData["packages"].IsNullOrEmpty())
            {
                productDetail.PackageIDs = appData["packages"].Select(d => (int)d).ToArray();
            }

            if (!appData["platforms"].IsNullOrEmpty())
            {
                productDetail.AvailableOnPC = (appData["platforms"]["windows"].IsNullOrEmpty()) ? true : (bool)appData["platforms"]["windows"];
                productDetail.AvailableOnMac = (appData["platforms"]["mac"].IsNullOrEmpty()) ? false : (bool)appData["platforms"]["mac"];
                productDetail.AvailableOnLinux = (appData["platforms"]["linux"].IsNullOrEmpty()) ? false : (bool)appData["platforms"]["linux"];
            }

            if (!appData["metacritic"].IsNullOrEmpty())
            {
                productDetail.MetacriticScore = (appData["metacritic"]["score"].IsNullOrEmpty()) ? 0 : (int)appData["metacritic"]["score"];
                productDetail.MetacriticURL = (string)appData["metacritic"]["url"] ?? "";
            }

            if (!appData["genres"].IsNullOrEmpty())
            {
                JArray jGenres = (JArray)appData["genres"];
                List<string> genresList = new List<string>();

                for (int i = 0; i < jGenres.Count; i++)
                {
                    genresList.Add((string)jGenres[i]["description"]);
                }
                
                productDetail.Genres = genresList.ToArray();
            }

            if (!appData["recommendations"].IsNullOrEmpty())
            {
                productDetail.TotalRecommendations = (appData["recommendations"]["total"].IsNullOrEmpty()) ? 0 : (int)appData["recommendations"]["total"];
            }

            if (!appData["achievements"].IsNullOrEmpty())
            {
                productDetail.NumAchievements = (appData["achievements"]["total"].IsNullOrEmpty()) ? 0 : (int)appData["achievements"]["total"];
            }

            if (!appData["release_date"].IsNullOrEmpty())
            {
                productDetail.ReleaseDate = (string)appData["release_date"]["date"] ?? "";
            }

            // run a sub-routine to build a listing/product for the base game of this demo/DLC
            if (baseAppID != 0)
            {
                Listing baseGameListing = GetListingByAppID(baseAppID, "Steam");

                if (baseGameListing == null)
                { 
                    baseGameListing = new Listing(productDetail.BaseProductName);
                    baseGameListing.AddPlatform(GetPlatforms().Where(p => object.Equals(p.PlatformName, "Steam")).SingleOrDefault());
                    baseGameListing.AddProduct(new Product(baseAppID));

                    AddListing(baseGameListing);

                    BuildListingWithAppID(baseGameListing, baseAppID);

                    UpdateListing(baseGameListing);
                }

                productDetail.BaseProduct = baseGameListing.Product;
            }

            // run a series of sub-routines to build listings/products for each DLC of this game (if applicable)
            if (productDetail.DLCAppIDs != null && productDetail.DLCAppIDs.Count() > 0)
            {
                for (int i = 0; i < productDetail.DLCAppIDs.Count(); i++)
                {
                    Listing DLCListing = GetListingByAppID(productDetail.DLCAppIDs[i], "Steam");

                    if (DLCListing == null)
                    {
                        DLCListing = new Listing();
                        DLCListing.AddPlatform(GetPlatforms().Where(p => object.Equals(p.PlatformName, "Steam")).SingleOrDefault());
                        DLCListing.AddProduct(new Product(productDetail.DLCAppIDs[i]));

                        AddListing(DLCListing);

                        BuildListingWithAppID(DLCListing, productDetail.DLCAppIDs[i]);

                        UpdateListing(DLCListing);
                    }
                    
                    productDetail.AddDLC(DLCListing.Product);
                }
            }

            if (!appData["movies"].IsNullOrEmpty())
            {
                JArray movies = (JArray)appData["movies"];

                for (int i = 0; i < movies.Count; i++)
                {
                    AppMovie temp = new AppMovie();

                    temp.Highlight = (!movies[i]["highlight"].IsNullOrEmpty()) ? false : (bool)movies[i]["highlight"];
                    temp.LargeMovieURL = (string)movies[i]["webm"]["max"] ?? "";
                    temp.Name = (string)movies[i]["name"] ?? "";
                    temp.SmallMovieURL = (string)movies[i]["webm"]["480"] ?? "";
                    temp.ThumbnailURL = (string)movies[i]["thumbnail"] ?? "";

                    productDetail.AddAppMovie(temp);
                }
            }

            if (!appData["screenshots"].IsNullOrEmpty())
            {
                JArray screenshots = (JArray)appData["screenshots"];

                for (int i = 0; i < screenshots.Count; i++)
                {
                    AppScreenshot temp = new AppScreenshot();

                    temp.FullSizeURL = (string)screenshots[i]["path_full"] ?? "";
                    temp.ThumbnailURL = (string)screenshots[i]["path_thumbnail"] ?? "";

                    productDetail.AddAppScreenshot(temp);
                }
            }

            if (!appData["categories"].IsNullOrEmpty())
            {
                JArray jCategories = (JArray)appData["categories"];
                List<string> categoriesList = new List<string>();

                for (int i = 0; i < jCategories.Count; i++)
                {
                    categoriesList.Add((string)jCategories[i]["description"]);
                }

                string[] categories = categoriesList.ToArray();

                if (categories != null)
                {
                    for (int i = 0; i < categories.Count(); i++)
                    {
                        if (GetProductCategoryByName(categories[i]) == null)
                        {
                            ProductCategory category = new ProductCategory(categories[i]);
                            listing.Product.AddProductCategory(category);
                        }
                        else
                        {
                            listing.Product.AddProductCategory(GetProductCategoryByName(categories[i]));
                        }
                    }
                }
            }

            listing.Product.AddProductDetail(productDetail);

            if (String.IsNullOrEmpty(listing.Product.ProductName))
            {
                listing.Product.ProductName = productDetail.ProductName;
            }
            if (String.IsNullOrEmpty(listing.ListingName))
            {
                listing.ListingName = productDetail.ProductName;
            }
        }
                
        // queries SteamClient via SteamKit2 and generates a list of AppIDs that are contained within a Steam package
        private async Task<List<int>> GetPackagesAppIDs(int packageId)
        {
            List<int> appIds = new List<int>();

            SteamClient client = new SteamClient();
            SteamApps steamApps = client.GetHandler<SteamApps>();
            AsyncJobMultiple<SteamApps.PICSProductInfoCallback>.ResultSet resultSet = await steamApps.PICSGetProductInfo(app: null, package: (uint)packageId);


            if (resultSet.Complete)
            {
                SteamApps.PICSProductInfoCallback productInfo = resultSet.Results.First();

                SteamApps.PICSProductInfoCallback.PICSProductInfo packageInfo = productInfo.Packages.FirstOrDefault().Value;
                List<KeyValue> list = packageInfo.KeyValues.Children.FirstOrDefault().Children;

                foreach (KeyValue val in list)
                {
                    if (val.Name.CompareTo("appids") == 0)
                    {
                        foreach (KeyValue appVal in val.Children)
                        {
                            appIds.Add(Int32.Parse(appVal.Value));
                        }
                    }
                }
            }

            return appIds;
        }








        // --- GC and User logic

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

    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }
}
