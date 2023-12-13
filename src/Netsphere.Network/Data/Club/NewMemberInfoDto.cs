using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.Club
{
    [BlubContract]
    public class NewMemberInfoDto
    {
        [BlubMember(0)]
        public ulong AccountId { get; set; }

        [BlubMember(1)]
        public string Name { get; set; }

        [BlubMember(2)]
        public int Unk2 { get; set; }

        [BlubMember(3)]
        public ClubRole Role { get; set; }

        [BlubMember(4)]
        public int Unk4 { get; set; }

        [BlubMember(5)]
        [BlubSerializer(typeof(ClubCreationDateSerializer))]
        public DateTimeOffset JoinDate { get; set; }

        [BlubMember(6)]
        public string Question1 { get; set; }

        [BlubMember(7)]
        public string Question2 { get; set; }

        [BlubMember(8)]
        public string Question3 { get; set; }

        [BlubMember(9)]
        public string Question4 { get; set; }

        [BlubMember(10)]
        public string Question5 { get; set; }

        [BlubMember(11)]
        public string Answer1 { get; set; }

        [BlubMember(12)]
        public string Answer2 { get; set; }

        [BlubMember(13)]
        public string Answer3 { get; set; }

        [BlubMember(14)]
        public string Answer4 { get; set; }

        [BlubMember(15)]
        public string Answer5 { get; set; }
    }
}
