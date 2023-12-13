using Netsphere.Database.Game;

namespace Netsphere.Server.Game.Data
{
    public class RandomShopItem
    {
        public byte RewardValue { get; set; }

        public ItemNumber Item { get; set; }

        public RandomShopColor Color { get; set; }

        public RandomShopEffect Effect { get; set; }

        public RandomShopPeriod Period { get; set; }

        public byte DefaultColor { get; set; }

        public uint Probability { get; set; }

        public RandomShopGrade Grade { get; set; }

        public RandomShopItem(
          RandomShopLineupEntity entity,
          RandomShopColor color,
          RandomShopEffect effect,
          RandomShopPeriod period)
        {
            RewardValue = entity.RewardValue;
            Item = (ItemNumber)entity.ShopItemId;
            DefaultColor = entity.DefaultColor;
            Probability = (uint)entity.Probability;
            Grade = (RandomShopGrade)entity.Grade;
            Color = color;
            Effect = effect;
            Period = period;
        }
    }
}
