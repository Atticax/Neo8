using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.Club
{
    public class MemberLeftDto
    {
        [BlubMember(0)]
        public uint AccountId { get; set; }

        [BlubMember(1)]
        public string Name { get; set; }

        [BlubMember(2)]
        public ClubLeaveReason Reason { get; set; }

        [BlubMember(3)]
        public int Unk { get; set; }

        [BlubMember(4)]
        [BlubSerializer(typeof(ClubCreationDateSerializer))]
        public DateTimeOffset Date { get; set; }
    }
}
