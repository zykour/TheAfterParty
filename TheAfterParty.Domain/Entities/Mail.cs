using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class Mail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MailID { get; set; }

        // the ID of who sent the message
        public int SenderUserID { get; set; }

        // the senders AppUser object
        public virtual AppUser AppUserSender { get; set; }

        // the ID of who receives the message
        public int ReceiverUserID { get; set; }

        // the receivers AppUser object
        public virtual AppUser AppUserReceiver { get; set; }

        // the date the message was sent
        public DateTime DateSent { get; set; }

        // the message
        public string Message { get; set; }
    }
}
