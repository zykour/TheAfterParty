using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class OwnedGame
    {
        public OwnedGame() { }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }

        public string UserID { get; set; }

        public int AppID { get; set; }

        public virtual AppUser AppUser { get; set; }
    }
}
