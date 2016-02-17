using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{
    public class Tag
    {
        public Tag() { }
        public Tag(string name)
        {
            TagName = name;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int TagID { get; set; }

        public string UserID { get; set; }

        public virtual AppUser AppUser { get; set; }

        public int ProductID { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public string TagName { get; set; }

        public virtual ICollection<UserTag> UserTags { get; set; }
    }
}
