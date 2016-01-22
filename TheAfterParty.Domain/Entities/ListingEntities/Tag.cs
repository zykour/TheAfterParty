using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class Tag
    {
        public Tag() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }

        public string UserID { get; set; }

        public virtual AppUser AppUser { get; set; }

        public int ProductID { get; set; }

        public virtual Product Product { get; set; }

        public string TagName { get; set; }
    }
}
