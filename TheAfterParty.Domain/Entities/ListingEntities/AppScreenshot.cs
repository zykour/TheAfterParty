using System.ComponentModel.DataAnnotations;

namespace TheAfterParty.Domain.Entities
{
    public class AppScreenshot
    {
        [Key]
        public int AppScreenshotID { get; set; }
        public string ThumbnailURL { get; set; }
        public string FullSizeURL { get; set; }
    }
}
