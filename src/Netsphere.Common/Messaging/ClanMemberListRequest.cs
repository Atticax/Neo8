using System;

namespace Netsphere.Common.Messaging
{
    public class ClanMemberListRequest : MessageWithGuid
    {
        public uint ClanId { get; set; }

        public ClanMemberListRequest()
        {
        }

        public ClanMemberListRequest(uint clanId)
        {
            ClanId = clanId;
        }
    }

    public class ClanMemberListResponse : MessageWithGuid
    {
        public ClanMemberInfo[] Members { get; set; }

        public ClanMemberListResponse()
        {
        }

        public ClanMemberListResponse(ClanMemberInfo[] members)
        {
            Members = members;
        }
    }

    public class ClanMemberInfo
    {
        public ulong AccountId { get; set; }
        public string Nickname { get; set; }
        public ClubRole Role { get; set; }
        public DateTimeOffset LastLoginDate { get; set; }
        public ClubMemberPresenceState PresenceState { get; set; }
    }
}
