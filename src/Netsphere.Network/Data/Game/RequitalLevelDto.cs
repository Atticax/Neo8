using BlubLib.Serialization;

namespace Netsphere.Network.Data.Game
{
    [BlubContract]
    public class RequitalLevelDto
    {
        [BlubMember(0)]
        public ulong ItemNumber { get; set; }

        [BlubMember(1)]
        public ItemNumber ItemNumber2 { get; set; }

        [BlubMember(2)]
        public ItemPriceType PriceType { get; set; }

        [BlubMember(3)]
        public ItemPeriodType PeriodType { get; set; }

        [BlubMember(4)]
        public ushort Period { get; set; }

        [BlubMember(5)]
        public byte Color { get; set; }

        [BlubMember(6)]
        public uint EffectId { get; set; }

        //[BlubMember(7)]
        //public int Unk8 { get; set; }
    }
}
