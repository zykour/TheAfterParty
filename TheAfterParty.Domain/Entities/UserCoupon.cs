using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class UserCoupon
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }

        public int CouponID { get; set; }

        public virtual Coupon Coupon { get; set; }

        public int Quantity { get; set; }

        public int UserID { get; set; }

        public virtual AppUser AppUser { get; set; }
    }
}
