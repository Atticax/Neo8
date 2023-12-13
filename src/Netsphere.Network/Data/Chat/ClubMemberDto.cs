using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.Chat
{
    [BlubContract]
    public class ClubMemberDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public string Nickname { get; set; }

        [BlubMember(2)]
        public int Level { get; set; }

        [BlubMember(3)]
        public ClubRole Role { get; set; }

        [BlubMember(4)]
        public int Rank { get; set; }

        [BlubMember(5)]
        public int Unk1 { get; set; }

        [BlubMember(6)]
        [BlubSerializer(typeof(ClubCreationDateSerializer))]
        public DateTimeOffset LastLoginDate { get; set; }

        [BlubMember(7)]
        public string Unk2 { get; set; }

        [BlubMember(8)]
        public ClubMemberPresenceState PresenceState { get; set; }
    }
}
