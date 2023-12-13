using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.Club
{
    [BlubContract]
    public class ClubInfo2Dto
    {
        [BlubMember(0)]
        public int ClanId { get; set; }

        [BlubMember(1)]
        public string ClanName { get; set; }

        [BlubMember(2)]
        public string ClanIcon { get; set; }

        [BlubMember(3)]
        public string Unk1 { get; set; }

        [BlubMember(4)]
        public ulong Unk2 { get; set; }

        [BlubMember(5)]
        public int Unk3 { get; set; }

        [BlubMember(6)]
        public string OwnerName { get; set; }

        [BlubMember(7)]
        public int Unk4 { get; set; }

        [BlubMember(8)]
        public int PlayersCount { get; set; }

        [BlubMember(9)]
        public string Unk5 { get; set; }

        [BlubMember(10)]
        [BlubSerializer(typeof(ClubCreationDateSerializer))]
        public DateTimeOffset CreationDate { get; set; }

        [BlubMember(11)]
        public string Unk7 { get; set; }

        [BlubMember(12)]
        public string Unk8 { get; set; }

        [BlubMember(13)]
        public int Unk9 { get; set; }

        [BlubMember(14)]
        public int Unk10 { get; set; }

        [BlubMember(15)]
        public int Unk11 { get; set; }

        [BlubMember(16)]
        public int Unk12 { get; set; }

        [BlubMember(17)]
        public int ClubPoints { get; set; }

        [BlubMember(18)]
        public int Unk13 { get; set; }

        [BlubMember(19)]
        public int ClubRankWins { get; set; }

        [BlubMember(20)]
        public int ClubRankDefeats { get; set; }

        [BlubMember(21)]
        public int LadderPoints { get; set; }

        [BlubMember(22)]
        public short Unk14 { get; set; }

        [BlubMember(23)]
        public int Unk15 { get; set; }
    }
}
