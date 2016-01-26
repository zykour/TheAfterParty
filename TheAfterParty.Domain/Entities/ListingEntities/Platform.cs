using System.ComponentModel.DataAnnotations;

namespace TheAfterParty.Domain.Entities
{
    public class Platform
    {
        public Platform() { }

        [Key]
        public int PlatformID { get; set; }

        [Required]
        public string PlatformName { get; set; }

        //redemption URL
        public string PlatformURL { get; set; }
    }
}
