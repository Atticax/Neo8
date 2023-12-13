using System;
using System.Collections.Generic;

namespace Netsphere.Server.Game.Data
{
    public class CardSystem
    {
        private readonly Random _random;

        public bool Active { get; set; }

        public List<CardSystemData> Cards { get; }

        public CardSystemDataItem Reward { get; set; }

        public CardSystemFormula Formula { get; set; }

        public CardSystem()
        {
            _random = new Random();
            Cards = new List<CardSystemData>();
        }

        public CardSystemData GetAleatoryCard() => Cards[_random.Next(0, Cards.Count)];
    }

    public class CardSystemData : CardSystemDataItem
    {
        public string Name { get; set; }

        public int PlayProb { get; set; }
    }

    public class CardSystemDataItem
    {
        public ItemNumber ItemNumber { get; set; }

        public ItemPriceType PriceType { get; set; }

        public ItemPeriodType PeriodType { get; set; }

        public ushort Period { get; set; }

        public byte Color { get; set; }

        public uint EffectId { get; set; }
    }

    public class CardSystemFormula
    {
        public TimeSpan PlayLimitTime { get; set; }

        public int PlayLimitCount { get; set; }
    }

}
