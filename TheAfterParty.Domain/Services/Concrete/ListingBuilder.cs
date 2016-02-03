using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;
using SteamKit2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TheAfterParty.Domain.Services
{
    public class ListingBuilder
    {
        private IStoreService _storeService;
        private IUnitOfWork _unitOfWork;

        public ListingBuilder(IStoreService storeService, IUnitOfWork unitOfWork)
        {
            _storeService = storeService;
        }

        public async Task BuildListingWithPackageID(Listing listing, int packageId, string name = "")
        {
            SteamClient client = new SteamClient();
            SteamApps steamApps = client.GetHandler<SteamApps>();
            AsyncJobMultiple<SteamApps.PICSProductInfoCallback>.ResultSet resultSet = await steamApps.PICSGetProductInfo(app: null, package: (uint)packageId);

            List<int> appIds = new List<int>();

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

            foreach (int id in appIds)
            {
                Listing subListing = _storeService.GetListingByAppID(id, "Steam");

                if (subListing == null)
                {
                    BuildListingWithAppID(subListing, id);
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

        //platform
        //productkey
        //listing
        //app movie / app screenshot
        // product
        // productdetail
        // productcategory

        public void BuildListingWithAppID(Listing listing, int appId)
        {
            Product product = listing.Product ?? new Product();

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
                return;
            }

            JToken appData = jsonResult[appID]["data"];
            int baseAppID = 0;

            //get all the product details from the jtoken
            productDetail.ProductType = (string)appData["type"];
            productDetail.ProductName = (string)appData["name"];
            productDetail.AgeRequirement = (int)appData["required_age"];
            productDetail.DetailedDescription = (string)appData["detailed_description"];
            productDetail.DLCAppIDs = appData["dlc"].Select(d => (int)d).ToArray();
            productDetail.AboutTheGame = (string)appData["about_the_game"];
            productDetail.BaseProductName = (string)appData["fullgame "]["name"];
            baseAppID = (int)appData["fullgame"]["appid"];
            productDetail.SupportedLanguages = (string)appData["supported_languages"];
            productDetail.HeaderImageURL = (string)appData["header_image"];
            productDetail.ProductWebsite = (string)appData["website"];
            productDetail.PCMinimumRequirements = (string)appData["pc_requirements"]["minimum"];
            productDetail.PCRecommendedRequirements = (string)appData["pc_requirements"]["recommended"];
            productDetail.MacMinimumRequirements = (string)appData["mac_requirements"]["minimum"];
            productDetail.MacRecommendedRequirements = (string)appData["mac_requirements"]["recommended"];
            productDetail.LinuxMinimumRequirements = (string)appData["linux_requirements"]["minimum"];
            productDetail.LinuxRecommendedRequirements = (string)appData["linux_requirements"]["recommended"];
            productDetail.Developers = appData["developers"].Select(d => (string)d).ToArray();
            productDetail.Publishers = appData["publishers"].Select(d => (string)d).ToArray();
            productDetail.DemoAppID = (int)appData["demos"]["appid"];
            productDetail.DemoRestrictions = (string)appData["demos"]["description"];
            productDetail.FinalPrice = (int)appData["price_overview"]["final"];
            productDetail.InitialPrice = (int)appData["price_overview"]["initial"];
            productDetail.CurrencyType = (string)appData["price_overview"]["currency"];
            productDetail.PackageIDs = appData["packages"].Select(d => (int)d).ToArray();
            productDetail.AvailableOnPC = (bool)appData["platforms"]["windows"];
            productDetail.AvailableOnMac = (bool)appData["platforms"]["mac"];
            productDetail.AvailableOnLinux = (bool)appData["platforms"]["linux"];
            productDetail.MetacriticScore = (int)appData["metacritic"]["score"];
            productDetail.MetacriticURL = (string)appData["metacritic"]["url"];
            productDetail.Genres = appData["genres"].Select(d => (string)d["description"]).ToArray();
            productDetail.TotalRecommendations = (int)appData["recommendations"]["total"];
            productDetail.NumAchievements = (int)appData["achievements"]["total"];
            productDetail.ReleaseDate = (string)appData["release_date"]["date"];

            JArray movies = (JArray)appData["movies"];

            for (int i = 0; i < movies.Count; i++)
            {
                AppMovie temp = new AppMovie();

                temp.Highlight = (bool)movies[i]["highlight"];
                temp.LargeMovieURL = (string)movies[i]["webm"]["max"];
                temp.Name = (string)movies[i]["name"];
                temp.SmallMovieURL = (string)movies[i]["webm"]["480"];
                temp.ThumbnailURL = (string)movies[i]["thumbnail"];

                productDetail.AddAppMovie(temp);
            }

            JArray screenshots = (JArray)appData["screenshots"];

            for (int i = 0; i < screenshots.Count; i++)
            {
                AppScreenshot temp = new AppScreenshot();

                temp.FullSizeURL = (string)screenshots[i]["path_full"];
                temp.ThumbnailURL = (string)screenshots[i]["path_thumbnail"];

                productDetail.AddAppScreenshot(temp);
            }

            string[] categories = appData["categories"].Select(c => (string)c).ToArray();

            if (categories != null)
            {
                for (int i = 0; i < categories.Count(); i++)
                {
                    if (_storeService.GetProductCategoryByName(categories[i]) == null)
                    {
                        ProductCategory category = new ProductCategory(categories[i]);
                        //_storeService.AddProductCategory(category);
                        product.AddProductCategory(category);
                    }
                }
            }

            product.AddProductDetail(productDetail);
            if (String.IsNullOrEmpty(product.ProductName))
            {
                product.ProductName = productDetail.ProductName;
            }

            //run subroutine to add DLC
            // or if DLC, the base game
        }

        public void UpdateListing(int listingID)
        {

        }
    }
}
