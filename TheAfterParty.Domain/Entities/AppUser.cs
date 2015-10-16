using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace TheAfterParty.Domain.Entities
{ 
    public class AppUser : IdentityUser
    {
        [Key, Required, RegularExpression(@"^[0-9]{17}$", ErrorMessage = "SteamIDs are numeric only.")]
        public string SteamID { get; set; }

        [Required]
        public int Balance { get; set; }
    }
}