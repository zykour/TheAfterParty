using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class Giveaway
    {
        public Giveaway()
        {
            CreatedTime = DateTime.Now;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GiveawayID { get; set; }

        // what is being giveaway, if null or empty, product.name should be used
        public string Prize { get; set; }

        // if the prize is not a game, one of the options is to giveaway a number of points
        public int? PointsPrize { get; set; }

        // how much it cost to enter the giveaway
        public int EntryFee { get; set; }

        // if the prize is a game, set the ListingID
        public int ListingID { get; set; }

        // the associated product if the prize is a game
        public virtual Listing Listing { get; set; }

        // the list of entries associated with this giveaway
        public virtual ICollection<GiveawayEntry> GiveawayEntries { get; set; }

        // the datetime this giveaway is supposed to end
        public DateTime EndDate { get; set; }

        // the datetime the giveaway starts
        public DateTime? StartDate { get; set; }

        public DateTime CreatedTime { get; set; }

        // the steam ID of the giveaway creator
        public string CreatorID { get; set; }

        // the appuser object associated with the creator
        public virtual AppUser Creator { get; set; }
        public bool IsCreator(AppUser user)
        {
            return (Creator == null) ? false : Object.Equals(user.Id, Creator.Id);
        }

        public string WinnerID { get; set; }

        public virtual AppUser Winner { get; set; }
        public bool IsWinner(AppUser user)
        {
            return (Winner == null) ? false : Object.Equals(user.Id, Winner.Id);
        }
    }
}
