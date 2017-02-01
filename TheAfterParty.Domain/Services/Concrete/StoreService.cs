using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using TheAfterParty.Domain.Model;

namespace TheAfterParty.Domain.Services
{
    public class StoreService : IStoreService
    {
        private IListingRepository listingRepository;
        private IUserRepository userRepository;
        private ISiteRepository siteRepository;
        private IUnitOfWork unitOfWork;
        public AppUserManager UserManager { get; private set; }
        public string userName { get; set; }

        public StoreService(IListingRepository listingRepository, IUserRepository userRepository, ISiteRepository siteRepository, IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.listingRepository = listingRepository;
            this.userRepository = userRepository;
            this.siteRepository = siteRepository;
            this.unitOfWork = unitOfWork;
            userName = "";
        }
        protected StoreService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }

        public IEnumerable<int> GetAppIDsByID(string id, string apiKey)
        {
            string gamesURL = String.Format("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={0}&steamid={1}&format=json", apiKey, id);

            string result = new System.Net.WebClient().DownloadString(gamesURL);

            JObject gameData = JObject.Parse(result);

            if (gameData["response"] != null && gameData["response"]["games"] != null)
            {
                JArray jGames = (JArray)gameData["response"]["games"];
                int[] array = jGames.Select(j => (int)j["appid"]).ToArray<int>();

                return array.AsEnumerable();
            }

            return new HashSet<int>();
        }

        public List<Listing> FilterListingsByUserSteamID(List<Listing> currentListing, string id, string apiKey)
        {
            string gamesURL = String.Format("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={0}&steamid={1}&format=json", apiKey, id);

            string result = new System.Net.WebClient().DownloadString(gamesURL);

            JObject gameData = JObject.Parse(result);

            if (gameData["response"] != null && gameData["response"]["games"] != null)
            {
                JArray jGames = (JArray)gameData["response"]["games"];
                int[] array = jGames.Select(j => (int)j["appid"]).ToArray<int>();

                return currentListing.Where(l => !array.Contains(l.Product.AppID)).ToList();
            }

            return currentListing;
        }

        #region Admin (CRUD) & Auxiliary methods

        public void AddDiscountedListing(DiscountedListing discountedListing, int daysDealLast)
        {
            discountedListing.ItemSaleExpiry = DateTime.Now.AddDays(daysDealLast);

            Listing saleListing = listingRepository.GetListingByID(discountedListing.ListingID);

            saleListing.AddDiscountedListing(discountedListing);

            if (daysDealLast == 0)
            {
                if (discountedListing.WeeklyDeal)
                {
                    discountedListing.ItemSaleExpiry = DateTime.Now.AddDays(7);
                }
                else if (discountedListing.DailyDeal)
                {
                    discountedListing.ItemSaleExpiry = DateTime.Now.AddDays(1);
                }
            }

            listingRepository.InsertDiscountedListing(discountedListing);
            unitOfWork.Save();
        }
        public void EditDiscountedListing(DiscountedListing discountedListing, int daysToAdd)
        {
            DiscountedListing updatedDiscountedListing = listingRepository.GetDiscountedListingByID(discountedListing.DiscountedListingID);

            updatedDiscountedListing.ItemDiscountPercent = discountedListing.ItemDiscountPercent;
            updatedDiscountedListing.WeeklyDeal = discountedListing.WeeklyDeal;
            updatedDiscountedListing.DailyDeal = discountedListing.DailyDeal;
            updatedDiscountedListing.ItemSaleExpiry = updatedDiscountedListing.ItemSaleExpiry.AddDays(daysToAdd);

            listingRepository.UpdateDiscountedListing(updatedDiscountedListing);
            unitOfWork.Save();
        }
        public void DeleteDiscountedListing(int id)
        {
            listingRepository.DeleteDiscountedListing(id);
            unitOfWork.Save();
        }

        public void AddListing(Listing listing)
        {
            listingRepository.InsertListing(listing);
            unitOfWork.Save();
        }
        public void EditListing(Listing listing)
        {
            Listing updatedListing = listingRepository.GetListingByID(listing.ListingID);

            updatedListing.ListingName = listing.ListingName;
            updatedListing.ListingPrice = listing.ListingPrice;
            updatedListing.DateEdited = DateTime.Now;

            listingRepository.UpdateListing(updatedListing);
            unitOfWork.Save();
        }
        public void DeleteListing(int listingId)
        {
            listingRepository.DeleteListing(listingId);
            unitOfWork.Save();
        }

        public void AddPlatform(Platform platform)
        {
            listingRepository.InsertPlatform(platform);
            unitOfWork.Save();
        }
        public void EditPlatform(Platform platform)
        {
            listingRepository.UpdatePlatform(platform);
            unitOfWork.Save();
        }
        public void DeletePlatform(int platformId)
        {
            Platform platform = listingRepository.GetPlatformByID(platformId);

            // safeguard, can only delete platforms without any active listings
            if (platform?.Listings.Where(l => l.Quantity > 0).Count() > 0)
            {
                return;
            }

            listingRepository.DeletePlatform(platformId);
            unitOfWork.Save();
        }

        public void EditProduct(Product product)
        {
            listingRepository.UpdateProduct(product);
            unitOfWork.Save();
        }
        public void DeleteProduct(int productId)
        {
            listingRepository.DeleteProduct(productId);
            unitOfWork.Save();
        }

        public void EditProductCategory(ProductCategory category)
        {
            listingRepository.UpdateProductCategory(category);
            unitOfWork.Save();
        }
        public void DeleteProductCategory(int productCategoryId)
        {
            listingRepository.DeleteProductCategory(productCategoryId);
            unitOfWork.Save();
        }

        public void EditProductKey(ProductKey productKey)
        {
            listingRepository.UpdateProductKey(productKey);
            unitOfWork.Save();
        }
        public void DeleteProductKey(int productKeyId)
        {
            ProductKey key = listingRepository.GetProductKeyByID(productKeyId);
            Listing listing = key.Listing;
            listingRepository.DeleteProductKey(productKeyId);
            listing.UpdateQuantity();
            listing.UpdateParentQuantities();
            listingRepository.UpdateListing(listing);
            unitOfWork.Save();
        }

        public List<String> BuildListingsWithSteamID(string appIDCsv)
        {
            string[] appIds = appIDCsv.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> outputList = new List<string>();

            if (appIds.Count() > 0)
            {
                foreach (string appId in appIds)
                {
                    int appIdVal = 0;

                    Int32.TryParse(appId, out appIdVal);

                    if (appIdVal != 0)
                    {
                        Product prod = listingRepository.GetProducts().FirstOrDefault(p => p.AppID == appIdVal);

                        if (prod == null)
                        {
                            prod = new Product(appIdVal);
                        }

                        BuildOrUpdateSteamProduct(appIdVal, prod);
                        
                        if (prod != null)
                        {

                            if (prod.Listings == null || prod.Listings.Count == 0 || !prod.Listings.Any(l => l.ContainsPlatform("Steam")))
                            {
                                Listing newListing = new Listing(prod.ProductName);
                                Platform steam = GetPlatforms().Where(p => p.PlatformName.CompareTo("Steam") == 0).Single();
                                newListing.AddProduct(prod);
                                newListing.AddPlatform(steam);
                                listingRepository.InsertListing(newListing);
                                outputList.Add("Added " + appId);
                            }
                            else
                            {
                                listingRepository.UpdateProduct(prod);
                                outputList.Add("Updated " + appId);
                            }
                        }
                    }
                    else
                    {
                        outputList.Add("Failed to add entry for " + appId);
                    }
                }
            }

            unitOfWork.Save();

            return outputList;
        }
        public List<String> BuildOrUpdateProductsWithSteamID(string appIDCsv)
        {
            string[] appIds = appIDCsv.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> outputList = new List<string>();

            if (appIds.Count() > 0)
            {
                foreach (string appId in appIds)
                {
                    int appIdVal = 0;

                    Int32.TryParse(appId, out appIdVal);

                    if (appIdVal != 0)
                    {
                        Product prod = listingRepository.GetProducts().FirstOrDefault(p => p.AppID == appIdVal);

                        if (prod == null)
                        {
                            prod = new Product(appIdVal);
                        }

                        BuildOrUpdateSteamProduct(appIdVal, prod);

                        if (prod != null)
                        {
                            if (prod.ProductID == 0)
                            {
                                listingRepository.InsertProduct(prod);
                                outputList.Add("Added " + appId);
                            }
                            else
                            {
                                listingRepository.UpdateProduct(prod);
                                outputList.Add("Updated " + appId);
                            }
                        }
                    }
                    else
                    {
                        outputList.Add("Failed to add entry for " + appId);
                    }
                }
            }

            unitOfWork.Save();

            return outputList;
        }

        public List<String> AddOrUpdateNonSteamProducts(string input)
        {
            List<String> addedProducts = new List<String>();

            input = input.Replace("\r\n", "\r");

            List<String> lines = input.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            Regex IDNameImage = new Regex(@"^id/([^\t]+)\t+([^\t]+)\t+([^\t]+)$");
            Regex IDName = new Regex(@"^id/([^\t]+)\t+([^\t]+)$");

            Match match;
            
            foreach (String line in lines)
            {
                string gameName = String.Empty;
                string p_id = String.Empty;
                string image = String.Empty;

                if (IDNameImage.Match(line).Success || IDName.Match(line).Success)
                {
                    if (IDNameImage.Match(line).Success)
                    {
                        match = IDNameImage.Match(line);
                        image = match.Groups[3].ToString();
                    }
                    else
                    {
                        match = IDName.Match(line);
                    }

                    p_id = match.Groups[1].ToString();
                    gameName = match.Groups[2].ToString();

                    int idVal = 0;

                    Int32.TryParse(p_id, out idVal);
                    Product product = null;

                    if (idVal != 0)
                    {
                        product = listingRepository.GetProducts().FirstOrDefault(p => p.AppID == idVal && p.IsSteamAppID == false);
                    }
                    else
                    {
                        product = listingRepository.GetProducts().FirstOrDefault(p => String.Equals(p.StringID, p_id) && p.IsSteamAppID == false);
                    }
                    
                    if (product == null)
                    {
                        product = new Product(gameName, false);
                        if (idVal != 0)
                        {
                            product.AppID = idVal;
                        }
                        else
                        {
                            product.StringID = p_id;
                        }
                    }
                    else
                    {
                        product.ProductName = gameName;
                    }

                    product.HeaderImageURL = image;

                    if (product.ProductID == 0)
                    {
                        listingRepository.InsertProduct(product);
                        addedProducts.Add("Added " + product.ProductName);
                    }
                    else
                    {
                        listingRepository.UpdateProduct(product);
                        addedProducts.Add("Updated " + product.ProductName);
                    }
                }
                else
                {
                    addedProducts.Add("Unable to updated or add " + line);
                }
            }
            
            unitOfWork.Save();

            return addedProducts;
        }

        public List<String> AddProductKeys(Platform platform, string input)
        {
            if (platform.PlatformName.ToLower().CompareTo("steam") == 0)
            {
                return AddSteamProductKeys(platform, input);
            }

            List<String> addedKeys = new List<String>();

            input = input.Replace("\r\n", "\r");

            List<String> lines = input.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            Regex IDPriceKey = new Regex(@"^id/([0-9]+)\t+([0-9]+)\t+([^\t]+)$");
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
                int p_id = 0;

                if (IDPriceKey.Match(line).Success)
                {
                    match = IDPriceKey.Match(line);

                    p_id = Int32.Parse(match.Groups[1].ToString());
                    price = Int32.Parse(match.Groups[2].ToString());
                    key = match.Groups[3].ToString();
                }
                else if (NamePriceKey.Match(line).Success)
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

                Listing listing = null;

                if (String.IsNullOrEmpty(gameName) == false)
                {
                    listing = listingRepository.GetListings().Where(l => l.ContainsPlatform(platform) && object.Equals(l.Product.ProductName, gameName)).SingleOrDefault();
                }

                if (listing != null)
                {
                    listing.ListingPrice = price;
                    listing.AddProductKeyAndUpdateQuantity(new ProductKey(isGift, key));
                    listing.DateEdited = dateAdded;
                    listingRepository.UpdateListing(listing);

                    addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...+1!");
                }
                else
                {
                    if (p_id != 0)
                    {
                        listing = listingRepository.GetListings().Where(l => l.Platforms.Any(p => p.PlatformID == platform.PlatformID) && l.Product.AppID == p_id && l.Product.IsSteamAppID == true).FirstOrDefault();
                    }

                    if (listing == null)
                    {
                        Product product = null;

                        if (p_id != 0)
                        {
                            product = listingRepository.GetProducts().FirstOrDefault(p => p.AppID == p_id && p.IsSteamAppID == true);

                            if (product == null)
                            {
                                product = new Product(p_id);

                                bool success = BuildOrUpdateSteamProduct(p_id, product);

                                if (success == false)
                                {
                                    product = null;
                                }
                            }
                        }

                        if (product != null)
                        {
                            listing = new Listing(product.ProductName, price, dateAdded);
                        }
                        else
                        {
                            product = new Product(gameName, false);
                            listing = new Listing(gameName, price, dateAdded);
                        }

                        listing.AddPlatform(platform);
                        listing.AddProduct(product);
                        listing.AddProductKeyAndUpdateQuantity(new ProductKey(isGift, key));

                        listingRepository.InsertListing(listing);

                        addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...created!");
                    }
                    else
                    {

                        listing.ListingPrice = price;
                        listing.AddProductKeyAndUpdateQuantity(new ProductKey(isGift, key));
                        listing.DateEdited = dateAdded;

                        listingRepository.UpdateListing(listing);

                        addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...+1!");
                    }
                }

                unitOfWork.Save();
            }
            
            SiteNotification notification = new SiteNotification();
            var url = String.Format("https://theafterparty.azurewebsites.net/store/date?month={0}&day={1}&year={2}", DateTime.Today.Month, DateTime.Today.Day, DateTime.Today.Year);
            notification.Notification = "[new][/new] [url=" + url + "][gtext]" + addedKeys.Count() + "[/gtext][/url] items added to the [url=https://theafterparty.azurewebsites.net/store/newest]Co-op Shop[/url]!";
            notification.NotificationDate = DateTime.Now;
            siteRepository.InsertSiteNotification(notification);

            unitOfWork.Save();

            return addedKeys;
        }

        // Does not save context, context should be saved from calling method
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
        public List<String> AddSteamProductKeys(Platform platform, string input)
        {
            List<String> addedKeys = new List<String>();

            input = input.Replace("\r\n", "\r");
            input = input.Replace("\n", "\r");

            List<String> lines = input.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            Regex SubIDPriceKey = new Regex(@"^sub/([0-9]+)\t+([0-9,]+)\t+([0-9]+)\t+([^\t]+)(\t+[^\t]+)?$");
            Regex AppIDPriceKey = new Regex(@"^([0-9]+)\t+([0-9]+)(\t+[^\t]+)?$");
            //\t+([^\t]+)

            DateTime dateAdded = DateTime.Now;

            Match match;
            bool isGift = false;
            string key = "";
            string gameName = "";
            int appId = 0;
            int price = 0;
            int subId = 0;
            List<int> appIds = new List<int>();

            foreach (String line in lines)
            {
                price = 0;
                appId = 0;
                gameName = "";
                key = "";
                isGift = false;
                subId = 0;
                appIds = new List<int>();

                if (AppIDPriceKey.Match(line).Success)
                {
                    match = AppIDPriceKey.Match(line);

                    appId = Int32.Parse(match.Groups[1].ToString());
                    price = Int32.Parse(match.Groups[2].ToString());
                    
                    if (String.IsNullOrEmpty(match.Groups[3].Value))
                    {
                        isGift = true;
                    }
                    else
                    {
                        key = match.Groups[3].Value.Trim();
                    }
                }
                else if (SubIDPriceKey.Match(line).Success)
                {
                    match = SubIDPriceKey.Match(line);

                    subId = Int32.Parse(match.Groups[1].Value);

                    string[] separator = new string[] { "," };
                    foreach (string splitId in match.Groups[2].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        appIds.Add(Int32.Parse(splitId));
                    }

                    if (String.IsNullOrEmpty(match.Groups[5].Value))
                    {
                        isGift = true;
                    }
                    else
                    {
                        key = match.Groups[5].Value.Trim();
                    }

                    gameName = match.Groups[4].Value;
                    price = Int32.Parse(match.Groups[3].ToString());
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
                    listing.AddProductKeyAndUpdateQuantity(new ProductKey(isGift, key));
                    listing.DateEdited = dateAdded;
                    listingRepository.UpdateListing(listing);

                    addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...+1!");
                }
                else
                {
                    listing = new Listing(gameName, price, dateAdded);
                    listing.AddPlatform(platform);
                    listing.AddProductKeyAndUpdateQuantity(new ProductKey(isGift, key));

                    Product product = new Product(appId, gameName);
                    //Add logic to get data from api on product info & product details

                    listing.AddProduct(product);

                    // insert this listing entry for now, as we build the listing with data gathered from Steam's store api
                    // we may need to build more listings recursively, we need this listing to be in the repository so it doesn't get stuck in a loop
                    //AddListing(listing);

                    if (appId != 0)
                    {
                        BuildListingWithAppID(listing, appId);
                    }
                    else if (appIds.Count != 0)
                    {
                        listing.Product.AppID = subId;
                        BuildListingWithPackageID(listing, appIds, gameName);
                    }

                    UpdateListing(listing);

                    addedKeys.Add(platform.PlatformName + ": " + listing.ListingName + "...created!");
                }
                
                unitOfWork.Save();
            }

            SiteNotification notification = new SiteNotification();
            var url = String.Format("https://theafterparty.azurewebsites.net/store/date?month={0}&day={1}&year={2}", DateTime.Today.Month, DateTime.Today.Day, DateTime.Today.Year);
            notification.Notification = "[new][/new] [url=" + url + "][gtext]" + addedKeys.Count() + "[/gtext][/url] items added to the [url=https://theafterparty.azurewebsites.net/store/newest]Co-op Shop[/url]!";
            notification.NotificationDate = DateTime.Now;
            siteRepository.InsertSiteNotification(notification);

            unitOfWork.Save();

            return addedKeys;
        }

        // gets the AppIDs contained within the package and then builds a listing for each one using BuildListingWithAppID
        private void BuildListingWithPackageID(Listing listing, List<int> appIds, string name = "")
        {
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

                    listing.AddChildListing(subListing);
                }
                else
                {
                    listing.AddChildListing(subListing);
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

        public bool BuildOrUpdateSteamProduct(int appId, Product product)
        {
            string url = String.Format("http://store.steampowered.com/api/appdetails?appids={0}", appId);

            string result = new System.Net.WebClient().DownloadString(url);

            JObject jsonResult = JObject.Parse(result);

            string appID = appId.ToString();

            if (jsonResult == null || jsonResult[appID] == null || jsonResult[appID]["data"] == null)
            {
                return false;
            }
            
            JToken appData = jsonResult[appID]["data"];
            
            product.ProductName = (string)appData["name"] ?? "";
            product.HeaderImageURL = (string)appData["header_image"] ?? "";

            if (!appData["genres"].IsNullOrEmpty())
            {
                JArray jGenres = (JArray)appData["genres"];
                List<string> genresList = new List<string>();

                for (int i = 0; i < jGenres.Count; i++)
                {
                    genresList.Add((string)jGenres[i]["description"]);
                }

                string[] genres = genresList.ToArray();

                if (genres != null)
                {
                    for (int i = 0; i < genres.Count(); i++)
                    {
                        if (product.HasTag(genres[i]) == false)
                        {
                            if (GetTagByName(genres[i]) != null)
                            {
                                product.AddTag(GetTagByName(genres[i]));
                            }
                            else
                            {
                                Tag tag = new Tag(genres[i]);
                                product.AddTag(tag);
                            }
                        }
                    }
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
                        if (product.HasProductCategory(categories[i]) == false)
                        {
                            if (GetProductCategoryByName(categories[i]) == null)
                            {
                                ProductCategory category = new ProductCategory(categories[i]);
                                product.AddProductCategory(category);
                            }
                            else
                            {
                                product.AddProductCategory(GetProductCategoryByName(categories[i]));
                            }
                        }
                    }
                }
            }

            return true;
        }

        // using a Steam App ID, queries the storefront API to get information about the product and build the listing object
        // listings sent to this method should be persisted to prevent looping behavior
        private void BuildListingWithAppID(Listing listing, int appId)
        {
            if (listing.Product == null)
            {
                throw new Exception("A valid product object was not added to the listing object!");
            }
            
            string url = String.Format("http://store.steampowered.com/api/appdetails?appids={0}", listing.Product.AppID);

            string result = new System.Net.WebClient().DownloadString(url);

            JObject jsonResult = JObject.Parse(result);

            string appID = listing.Product.AppID.ToString();

            if (jsonResult == null || jsonResult[appID] == null || jsonResult[appID]["data"] == null)
            {
                return;
            }

            JToken appData = jsonResult[appID]["data"];


            if (String.IsNullOrWhiteSpace(listing.Product.ProductName))
            {
                listing.Product.ProductName = (string)appData["name"] ?? "";
            }
            if (String.IsNullOrWhiteSpace(listing.ListingName))
            {
                listing.ListingName = (string)appData["name"] ?? "";
            }
                        
            listing.Product.HeaderImageURL = (string)appData["header_image"] ?? "";
            
            if (!appData["genres"].IsNullOrEmpty())
            {
                JArray jGenres = (JArray)appData["genres"];
                List<string> genresList = new List<string>();

                for (int i = 0; i < jGenres.Count; i++)
                {
                    genresList.Add((string)jGenres[i]["description"]);
                }

                string[] genres = genresList.ToArray();

                if (genres != null)
                {
                    for (int i = 0; i < genres.Count(); i++)
                    {
                        if (GetTagByName(genres[i]) != null)
                        {
                            listing.Product.AddTag(GetTagByName(genres[i]));
                        }
                        else
                        {
                            Tag tag = new Tag(genres[i]);
                            listing.Product.AddTag(tag);
                        }
                    }
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
        }
        
        public void DownloadIconURL(Platform platform, string localPath, string fileExtension)
        {
            if (String.IsNullOrEmpty(platform.PlatformIconURL) == false && String.IsNullOrEmpty(fileExtension) == false && platform.PlatformIconURL.ToLower().Contains("." + fileExtension.ToLower()))
            {
                WebClient wc = new WebClient();
                string localFile = localPath + platform.PlatformName + "." + fileExtension;
                wc.DownloadFile(platform.PlatformIconURL, localFile);
                platform.PlatformIconURL = "/Content/PlatformIcons/" + platform.PlatformName + "." + fileExtension;
            }
        }
        
        #endregion

        #region Getters
        #region Collections

        public IEnumerable<Listing> GetListings()
        {
            return listingRepository.GetListings();
        }
        public IEnumerable<Listing> GetStockedStoreListings()
        {
            return listingRepository.GetListings().Where(l => l.Quantity > 0 && l.ListingPrice > 0);
        }
        public IEnumerable<Listing> GetListingsWithDeals()
        {
            return listingRepository.GetListings().Where(l => l.HasSale() && l.Quantity > 0);
        }
        public IEnumerable<Listing> GetListingsWithFilter(ListingFilter filter, out int TotalItems)
        {
            return listingRepository.GetListingsWithFilter(filter, out TotalItems);
        }
        public IEnumerable<AppUser> GetAppUsers()
        {
            return UserManager.Users;
        }
        public IEnumerable<Platform> GetPlatforms()
        {
            return listingRepository.GetPlatforms();
        }
        public IEnumerable<Platform> GetActivePlatforms()
        {
            return listingRepository.GetActivePlatforms();
        }
        public IEnumerable<Product> GetProducts()
        {
            return listingRepository.GetProducts().ToList();
        }
        public IEnumerable<Tag> GetTags()
        {
            return listingRepository.GetTags();
        }
        public IEnumerable<ProductCategory> GetProductCategories()
        {
            return listingRepository.GetProductCategories();
        }
        public IEnumerable<ProductKey> GetProductKeys()
        {
            return listingRepository.GetProductKeys();
        }
        public IEnumerable<ProductOrderEntry> GetStoreHistory()
        {
            return userRepository.GetProductOrderEntries().OrderByDescending(p => p.ProductOrderEntryID).ThenBy(p => p.Listing.ListingName);
        }
        public IEnumerable<AppUser> GetUsersWhoOwn(int appId)
        {
            return userRepository.GetAppUsersWhoOwn(appId).OrderBy(a => a.UserName);
        }
        public IEnumerable<AppUser> GetUsersWhoDoNotOwn(int appId)
        {
            return userRepository.GetAppUsersWhoDoNotOwn(appId).OrderBy(a => a.UserName);
        }
        #endregion
        #region Object

        public Listing GetListingByAppID(int id, string platformName)
        {
            return listingRepository.GetListings().Where(l => l.Product != null && l.Product.AppID == id && l.Product.IsSteamAppID == true && l.ContainsPlatform(platformName)).SingleOrDefault();
        }

        public DiscountedListing GetDiscountedListingByID(int id)
        {
            return listingRepository.GetDiscountedListingByID(id);
        }
        public Product GetProductByID(int id)
        {
            return listingRepository.GetProductByID(id);
        }

        public Listing GetListingByID(int id)
        {
            return listingRepository.GetListingByID(id);
        }

        public Platform GetPlatformByID(int id)
        {
            return listingRepository.GetPlatformByID(id);
        }
        public Tag GetTagByID(int id)
        {
            return listingRepository.GetTagByID(id);
        }
        public ProductCategory GetProductCategoryByID(int id)
        {
            return listingRepository.GetProductCategoryByID(id);
        }
        public ProductCategory GetProductCategoryByName(string categoryName)
        {
            return listingRepository.GetProductCategories().Where(c => object.Equals(c.CategoryString, categoryName)).SingleOrDefault();
        }
        public ProductKey GetProductKeyByID(int id)
        {
            return listingRepository.GetProductKeyByID(id);
        }
        public Tag GetTagByName(string tagName)
        {
            return listingRepository.GetTags().Where(t => object.Equals(t.TagName, tagName)).SingleOrDefault();
        }
        #endregion
        #endregion

        // --- GC and User logic
        #region Standard Methods
        public void Dispose()
        {
            this.unitOfWork.Dispose();
        }
        public AppUser GetCurrentUserSynch()
        {
            return UserManager.FindByName(userName);
        }
        public async Task<AppUser> GetCurrentUserWithStoreFilters()
        {
            return await UserManager.FindByNameAsyncWithStoreFilters(userName);
        }
        public async Task<AppUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsync(userName);
        }
        #endregion
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
