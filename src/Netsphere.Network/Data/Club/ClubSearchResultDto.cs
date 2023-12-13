using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.Club
{
    [BlubContract]
    public class ClubSearchResultDto
    {
        [BlubMember(0)]
        public int Id { get; set; }

        [BlubMember(1)]
        public string Icon { get; set; }

        [BlubMember(2)]
        public string Name { get; set; }

        [BlubMember(3)]
        public string OwnerName { get; set; }

        [BlubMember(4)]
        public ClubClass Class { get; set; }

        [BlubMember(5)]
        public uint Points { get; set; }

        [BlubMember(6)]
        public uint Unk { get; set; }

        [BlubMember(7)]
        [BlubSerializer(typeof(ClubCreationDateSerializer))]
        public DateTimeOffset CreationDate { get; set; }

        [BlubMember(8)]
        public uint MemberCount { get; set; }

        [BlubMember(9)]
        public ClubArea Area { get; set; }

        [BlubMember(10)]
        public ClubActivity Activity { get; set; }

        [BlubMember(11)]
        public string Description { get; set; }
    }
}
