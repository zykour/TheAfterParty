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

        public ListingBuilder(IStoreService storeService)
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
        }
    }
}
