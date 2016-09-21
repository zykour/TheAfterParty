using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAfterParty.Domain.Entities
{
    public class SiteNotification
    {
        public int SiteNotificationID { get; set; }
        public string Notification { get; set; }
        public DateTime NotificationDate { get; set; }

        /// <summary>
        /// Encodes any URL tags
        /// </summary>
        public void EncodeUrlTag()
        {
            Notification = Notification.Replace("url=", "_urlequal_");
        }

        /// <summary>
        /// Decodes any URL tags
        /// </summary>
        public void DecodeUrlTag()
        {
            Notification = Notification.Replace( "_urlequal_", "url=");
        }
    }
}
