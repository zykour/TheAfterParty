using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAfterParty.Domain.Entities
{
    public class UserTag
    {
        public UserTag() { }
        public UserTag(AppUser user, Tag tag)
        {
            this.AppUser = user;
            this.Tag = tag;
        }

        public int UserTagID { get; set; }
        public int TagID { get; set; }
        public virtual Tag Tag { get; set; }
        public string AppUserID { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
