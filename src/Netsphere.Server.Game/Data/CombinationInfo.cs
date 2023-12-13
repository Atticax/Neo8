using System;
using System.Collections.Generic;

namespace Netsphere.Server.Game.Data
{
    public class CombinationInfo
    {
        private readonly Random _random;

        public uint Key { get; set; }

        public bool Use { get; set; }

        public uint PricePEN { get; set; }

        public ItemNumber UniqueProbItem { get; set; }

        public int UniqueProbCount { get; set; }

        public CombinationComponentInfo ComponentInfo { get; set; }

        public List<CombinationRequitialInfo> Requitials { get; set; }

        public CombinationInfo()
        {
            _random = new Random();
            ComponentInfo = new CombinationComponentInfo();
            Requitials = new List<CombinationRequitialInfo>();
        }

        public CombinationRequitialInfo GetRequitial(uint probability) => Requitials?.Find(x => x.Probability.Equals(probability)) ?? GetAleatoryOrFirst();

        public CombinationRequitialInfo GetRequitial1(ItemNumber itemNumber) => Requitials?.Find(x => x.ItemNumber.Equals(itemNumber)) ?? GetAleatoryOrFirst();

        public CombinationRequitialInfo GetAleatoryOrFirst()
        {
            int index = _random.Next(90, 5000);
            return Requitials?.Find(x => x.PeriodType.Equals(ItemPeriodType.Days) && x.Probability.Equals((uint)index)) ?? Requitials[0];
        }
    }

    public class CombinationComponentInfo
    {
        public List<CombinationComponentDataInfo> DataInfos { get; set; }

        public CombinationComponentInfo() => DataInfos = new List<CombinationComponentDataInfo>();
    }

    public class CombinationComponentDataInfo
    {
        public ItemNumber ItemNumber { get; set; }

        public int Period { get; set; }
    }

    public class CombinationRequitialInfo
    {
        public ItemNumber ItemNumber { get; set; }

        public ItemPriceType PriceType { get; set; }

        public ItemPeriodType PeriodType { get; set; }

        public ushort Period { get; set; }

        public byte Color { get; set; }

        public uint EffectId { get; set; }

        public uint Probability { get; set; }
    }
}
