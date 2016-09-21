using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data about the users, their points balance, and when their points balance was last updated.

    public class Objective
    {
        public Objective()
        {
            RequiresAdmin = false;
            IsActive = true;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ObjectiveID { get; set; }

        // game name or title
        public string Title { get; set; }

        // the description of the objective
        [Required]
        public string Description { get; set; }
        
        // what type of category this can be fitted into (i.e. Campaign, Missions, Versus)
        public string Category { get; set; }

        // a descriptive name for this particular objective (whereas the Title is what game this objective is for, or if it's for various games, the title
        [Required]
        public string ObjectiveName { get; set; }
        
        // the product this objective can be completed in
        public virtual Product Product { get; set; }
        public void AddProduct(Product product)
        {
            Product = product;

            if (string.IsNullOrEmpty(Title))
            {
                if (string.IsNullOrEmpty(product.ProductName) == false)
                {
                    Title = product.ProductName;
                }
            }
        }

        // how many points will be rewarded
        [Required]
        public int Reward { get; set; }

        // whether or not the player needs to play with the admin to play (must be self-reported if false)
        public bool RequiresAdmin { get; set; }

        public virtual BoostedObjective BoostedObjective { get; set; }
        public void AddBoostedObjective(BoostedObjective boostedObjective)
        {
            BoostedObjective = boostedObjective;
            boostedObjective.Objective = this;
        }
        public int FixedReward()
        {
            if (BoostedObjective == null) return Reward;
            return (int)System.Math.Ceiling(Reward * BoostedObjective.BoostAmount);
        }
        public DateTime? GetBoostedExpiryOrNull()
        {
            if (HasBoostedObjective() == false)
            {
                return null;
            }

            return BoostedObjective.EndDate;
        }
        public bool HasBoostedObjective()
        {
            return BoostedObjective != null;
        }

        // a list of balance entries associated with users who have done this objective
        public virtual ICollection<BalanceEntry> BalanceEntries { get; set; }

        // is the objective currently valid to be completed?
        public bool IsActive { get; set; }
    }
}