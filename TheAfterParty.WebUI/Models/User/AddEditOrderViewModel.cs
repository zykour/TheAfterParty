using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.User
{
    public class AddEditOrderViewModel : NavModel
    {
        public Order Order { get; set; }
        public ProductOrderEntry ProductOrderEntry { get; set; }
        public ClaimedProductKey ClaimedProductKey { get; set; }
        public bool UseDBKey { get; set; }
        public string UserID { get; set; }
        public bool AlreadyCharged { get; set; }
        public string UserNickName { get; set; }
    }
}