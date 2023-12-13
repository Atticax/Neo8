namespace Netsphere.Server.Game.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using Netsphere.Resource.Xml;

    public class CapsuleReward
    {
        public ItemNumber Item { get; set; }
        public List<Bag> Bags { get; set; }

        public CapsuleReward(Item item)
        {
            Item = item.Number;
            Bags = item.Groups.Select(x => new Bag(x)).ToList();
        }
    }

    public class Bag
    {
        public List<ItemReward> ItemRewards { get; set; }

        public Bag(Group group)
        {
            ItemRewards = group.Rewards.Select(x => new ItemReward(x)).ToList();
        }
    }

    public class ItemReward
    {
        public CapsuleRewardType Type { get; set; }

        public ItemNumber Item { get; set; }

        public ItemPriceType PriceType { get; set; }

        public ItemPeriodType PeriodType { get; set; }

        public byte Color { get; set; }

        public uint PEN { get; set; }

        public uint[] Effects { get; set; }

        public uint Rate { get; set; }

        public uint Value { get; set; }

        public ItemReward(Reward reward)
        {
            Type = (CapsuleRewardType)reward.Type;
            Item = reward.Data;
            PriceType = (ItemPriceType)reward.PriceType;
            PeriodType = (ItemPeriodType)reward.PeriodType;
            Color = reward.Color;
            Effects = reward.Effects.Split(",").ToList().Select(x => uint.Parse(x)).ToArray();
            Rate = reward.Rate;
            Value = reward.Value;
        }
    }
}
