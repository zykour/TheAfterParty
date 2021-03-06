﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class OwnedGame
    {
        public OwnedGame() { }
        public OwnedGame(int appId, int minutesPlayed = 0)
        {
            AppID = appId;
            this.MinutesPlayed = minutesPlayed;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int OwnedGameID { get; set; }

        public string UserID { get; set; }

        public int AppID { get; set; }

        public virtual AppUser AppUser { get; set; }

        public int MinutesPlayed { get; set; }
    }
}
