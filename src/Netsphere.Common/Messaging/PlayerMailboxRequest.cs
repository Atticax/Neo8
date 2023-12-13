
namespace Netsphere.Common.Messaging
{
    public class PlayerMailboxRequest : MessageWithGuid
    {
        public string Sender { get; set; }

        public string Reciever { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public MailType Type { get; set; }

        public PlayerMailboxRequest()
        {
        }

        public PlayerMailboxRequest(string sender, string reciever, string subject, string messsage, MailType type)
        {
            Sender = sender;
            Reciever = reciever;
            Subject = subject;
            Message = messsage;
            Type = type;
        }
    }
}
