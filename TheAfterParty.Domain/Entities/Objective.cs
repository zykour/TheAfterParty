using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data about the users, their points balance, and when their points balance was last updated.

    public class Objective
    {
        public Objective() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ObjectiveID { get; set; }

        // the description of the objective
        public string Description { get; set; }
        
        // what type of category this can be fitted into (i.e. Campaign, Missions, Versus)
        public string Category { get; set; }
        
        // a list of products this objective can be completed in 
        public virtual ICollection<ObjectiveGameMapping> ObjectiveGameMappings { get; set; }
        
        // how many points will be rewarded
        public int Reward { get; set; }

        // whether or not the player needs to play with the admin to play (must be self-reported if false)
        public bool RequiresAdmin { get; set; }
    }

    public class ObjectiveGameMapping
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }

        public int ObjectiveID { get; set; }

        public virtual Objective Objective { get; set; }

        public int ProductID { get; set; }

        public virtual Product Product { get; set;}
    }
}