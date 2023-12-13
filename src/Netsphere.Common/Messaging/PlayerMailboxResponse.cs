
namespace Netsphere.Common.Messaging
{
    public class PlayerMailboxResponse : MessageWithGuid
    {
        public long MailId { get; set; }

        public PlayerMailboxResponse()
        {
        }

        public PlayerMailboxResponse(long mailId)
        {
            MailId = mailId;
        }
    }
}
