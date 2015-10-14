using System.ComponentModel.DataAnnotations;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data about the users, their points balance, and when their points balance was last updated.

    public class Objective
    {
        [Key, Required]
        public int ObjectiveID { get; set; }

        [Required, StringLength(200)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Games { get; set; }

        public int Reward { get; set; }

        [Required]
        public bool RequiresAdmin { get; set; }
    }
}