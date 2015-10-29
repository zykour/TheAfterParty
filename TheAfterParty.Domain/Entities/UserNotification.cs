﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class UserNotification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserNotificationID { get; set; }

        // the user this notification is for
        public int UserID { get; set; }

        // when the notification happened
        public DateTime DateTime { get; set; }

        // the notification message
        public string Message { get; set; }

        // has the user marked this notice as being read
        public bool IsRead { get; set; }

        public virtual AppUser AppUser { get; set; }
    }
}
