using System;
using System.Collections.Generic;

namespace Netsphere.Server.Game.Data
{
    public class DecompositionInfo
    {
        public uint PricePEN { get; set; }

        public int MinHours { get; set; }

        public int MinDays { get; set; }

        public List<DecompositionMethodInfo> MethodInfos { get; set; }

        public List<DecompositionBonusInfo> BonusInfos { get; set; }

        public List<DecompositionProhibitionInfo> ProhibitionInfos { get; set; }

        public DecompositionInfo()
        {
            MethodInfos = new List<DecompositionMethodInfo>();
            BonusInfos = new List<DecompositionBonusInfo>();
            ProhibitionInfos = new List<DecompositionProhibitionInfo>();
        }

        public DecompositionMethodInfo GetMethodInfo(ItemPeriodType periodType) => MethodInfos.Find(x => x.PeriodType.Equals(periodType) && x.Use);

        public DecompositionBonusInfo GetBonusInfo(ItemNumber item) => BonusInfos.Find(x => x.ItemCategory.Equals(item.Category));
    }

    public class DecompositionMethodInfo
    {
        private readonly Random _random;

        public ItemPeriodType PeriodType { get; set; }

        public int EffectMinCount { get; set; }

        public int EffectMaxCount { get; set; }

        public bool Use { get; set; }

        public bool Bonus { get; set; }

        public List<DecompositionComponentInfo> ComponentInfos { get; set; }

        public DecompositionMethodInfo()
        {
            _random = new Random();
            ComponentInfos = new List<DecompositionComponentInfo>();
        }

        public DecompositionComponentInfo GetComponentInfo() => ComponentInfos[_random.Next(0, ComponentInfos.Count)];

        public DecompositionComponentInfo GetComponentInfo(int condition) => PeriodType.Equals(ItemPeriodType.None) ? GetComponentInfo() : ComponentInfos.Find(x => x.Condition.Equals(condition));
    }

    public class DecompositionComponentInfo
    {
        public int Condition { get; set; }

        public ItemNumber ItemNumber { get; set; }

        public ItemPriceType PriceType { get; set; }

        public ItemPeriodType PeriodType { get; set; }

        public ushort Period { get; set; }

        public byte Color { get; set; }

        public uint EffectId { get; set; }
    }

    public class DecompositionBonusInfo
    {
        public ItemCategory ItemCategory { get; set; }

        public int Period { get; set; }
    }

    public class DecompositionProhibitionInfo
    {
        public ItemNumber ItemNumber { get; set; }
    }
}
