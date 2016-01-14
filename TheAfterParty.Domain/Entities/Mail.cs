using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAfterParty.Domain.Entities
{
    public class Mail
    {
        public Mail(AppUser receiver, AppUser sender, string heading, string body, DateTime dateSent)
        {
            AppUserReceiver = receiver;
            ReceiverUserID = receiver.Id;
            receiver.ReceivedMail.Add(this);

            AppUserSender = sender;
            SenderUserID = sender.Id;
            sender.SentMail.Add(this);

            Heading = heading;
            Message = body;
            DateSent = dateSent;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MailID { get; set; }

        // the ID of who sent the message
        public string SenderUserID { get; set; }

        // the senders AppUser object
        public virtual AppUser AppUserSender { get; set; }

        // the ID of who receives the message
        public string ReceiverUserID { get; set; }

        // the receivers AppUser object
        public virtual AppUser AppUserReceiver { get; set; }

        // the date the message was sent
        public DateTime DateSent { get; set; }

        // the message
        public string Message { get; set; }

        // the heading
        public string Heading { get; set; }
    }
}
