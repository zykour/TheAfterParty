using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{
    /// <summary>
    /// An entity that describes a platform, of which games and apps can be redeemed on
    /// </summary>
    public class Platform
    {
        #region Constructors

        /// <summary> Creates a new platform entity with the specified name and redemption url. </summary>
        /// <param name="platformName"> The specified platform name. </param>
        /// <param name="platformUrl"> The sepcified platform redemption url. </param>
        public Platform(string platformName, string platformUrl) : this()
        {
            PlatformName = platformName;
            PlatformURL = platformUrl;
        }
        /// <summary> Creates a new platform entity with the specified name and icon url. </summary>
        /// <param name="platformName"> The specified platform name. </param>
        /// <param name="platformUrl"> The sepcified platform redemption url. </param>
        /// <param name="overlord"> Unused overload parameter. </param>
        public Platform(string platformName, string platformUrl, bool overload) : this()
        {
            PlatformName = platformName;
            PlatformURL = platformUrl;
        }
        /// <summary> Creates a new blank platform entity. </summary>
        public Platform()
        {
            Listings = new HashSet<Listing>();
        }

        #endregion

        /// <summary> The entity framework identity for this entity. </summary>
        [Key]
        public int PlatformID { get; set; }
        
        /// <summary> Signifies whether or not this platform has a public-facing identity for it's products. </summary>
        /// <returns> Returns true if this platform has a public-facing identity, or false otherwise. </returns>
        public bool HasAppID { get; set; }

        /// <summary> A collection of listings associated with this platform. </summary>
        public virtual ICollection<Listing> Listings { get; set; }

        /// <summary> A url to an image for this platform, used to represent which platform a listing is associated with. </summary>
        public string PlatformIconURL { get; set; }

        /// <summary> The required name of the platform (e.g. Google Play). </summary>
        [Required]
        public string PlatformName { get; set; }

        /// <summary> A url that points to the page you can redeem product keys on (if it exists), or to the platforms main page. </summary>
        public string PlatformURL { get; set; }

        /// <summary> A url that points to the page where you can view information about an arbitrary product on the platforms site. </summary>
        public string StorePageURL { get; set; }
    }
}
