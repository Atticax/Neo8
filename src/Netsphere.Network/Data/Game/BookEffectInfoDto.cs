using System;
using BlubLib.Serialization;
using Netsphere.Network.Serializers;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class BookEffectInfoDto
    {
        [BlubMember(0)]
        public byte Enable { get; set; }

        [BlubMember(1)]
        public int Unk2 { get; set; }

        [BlubMember(2)]
        public short Unk3 { get; set; }

        [BlubMember(3)]
        public int NameTagId { get; set; }

        [BlubMember(4)]
        public int EffectId { get; set; }

        [BlubMember(5)]
        public int EffectId2 { get; set; }

        [BlubMember(6)]
        public string PeriodType { get; set; }

        [BlubMember(7)]
        public string Value { get; set; }

        [BlubMember(8)]
        [BlubSerializer(typeof(UnixTimeSerializer))]
        public DateTimeOffset NameTagExpireTime { get; set; }

        [BlubMember(9)]
        [BlubSerializer(typeof(UnixTimeSerializer))]
        public DateTimeOffset EffectTagExpireTime { get; set; }

        [BlubMember(10)]
        [BlubSerializer(typeof(UnixTimeSerializer))]
        public DateTimeOffset Effect2TagExpireTime { get; set; }
    }
}
