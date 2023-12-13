
namespace Netsphere.Common.Messaging
{
    public class ClanAdminJoinResponse : MessageWithGuid
    {
        public bool Result { get; set; }

        public ClanAdminJoinResponse()
        {
            Result = false;
        }

        public ClanAdminJoinResponse(bool result)
        {
            Result = result;
        }
    }
}
