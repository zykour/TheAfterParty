using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class Gift
    {
        public Gift() { }

        [Key, ForeignKey("ClaimedProductKey")]
        public int GiftID { get; set; }

        // the ID of the user who sent the gift
        public string SenderID { get; set; }

        // the associated AppUser of the sender
        public virtual AppUser AppUserSender { get; set; }

        // the ID of the user who is targeted to receive the gift
        public string ReceiverID { get; set; }

        // the associated AppUser of the recipient
        public virtual AppUser AppUserReceiver { get; set; }

        // the date the gift was sent
        public DateTime DateSent { get; set; }

        // the date the gift was received (if received)
        public DateTime? DateReceived { get; set; }
        
        // has the gift been accepted yet?
        public bool IsPending { get; set; }

        // the product key of the gifted product
        public virtual ClaimedProductKey ClaimedProductKey { get; set; }
    }
}
