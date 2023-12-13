
namespace Netsphere.Common.Messaging
{
    public class ClanAdminJoinRequest : MessageWithGuid
    {
        public ulong AccountId { get; set; }

        public string NoteTitle { get; set; }

        public ClanAdminJoinRequest()
        {
            NoteTitle = string.Empty;
        }

        public ClanAdminJoinRequest(ulong accountId, string noteTitle)
        {
            AccountId = accountId;
            NoteTitle = noteTitle;
        }
    }
}
