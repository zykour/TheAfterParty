using System.ComponentModel.DataAnnotations;

namespace TheAfterParty.Domain.Entities
{
    public class AppMovie
    {
        [Key]
        public int AppMovieID { get; set; }
        public string Name { get; set; }
        public string ThumbnailURL { get; set; }
        public string SmallMovieURL { get; set; }
        public string LargeMovieURL { get; set; }
        public bool Highlight { get; set; }
    }
}
