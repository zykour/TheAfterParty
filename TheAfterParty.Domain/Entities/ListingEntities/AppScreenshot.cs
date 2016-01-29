using System.ComponentModel.DataAnnotations;

namespace TheAfterParty.Domain.Entities
{
    public class AppScreenshot
    {
        public AppScreenshot() { }

        [Key]
        public int AppScreenshotID { get; set; }
        public string ThumbnailURL { get; set; }
        public string FullSizeURL { get; set; }

        public byte[] Screenshot { get; set; }

        public string ScreenshotMimeType { get; set; }

        public int ProductDetailID { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
    }
}
