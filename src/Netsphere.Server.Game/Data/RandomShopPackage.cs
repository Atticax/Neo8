using Netsphere.Database.Game;
using System.Collections.Generic;

namespace Netsphere.Server.Game.Data
{
    public class RandomShopPackage : List<RandomShopLineup>
    {
        private static List<RandomShopColor> colors;
        private static List<RandomShopEffect> effects;
        private static List<RandomShopPeriod> periods;

        public static bool IsInitialized => RandomShopPackage.Colors != null && RandomShopPackage.Effects != null && RandomShopPackage.Periods != null;

        public static List<RandomShopColor> Colors
        {
            get => RandomShopPackage.colors;
            private set => RandomShopPackage.colors = value;
        }

        public static List<RandomShopEffect> Effects
        {
            get => RandomShopPackage.effects;
            private set => RandomShopPackage.effects = value;
        }

        public static List<RandomShopPeriod> Periods
        {
            get => RandomShopPackage.periods;
            private set => RandomShopPackage.periods = value;
        }

        public uint Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Open { get; set; }

        public ItemPriceType PriceType { get; set; }

        public uint Price { get; set; }

        public Gender Gender { get; set; }

        public RandomShopPackage(RandomShopPackageEntity entity, List<RandomShopLineup> lineups)
        {
            Id = (uint)entity.Id;
            Name = entity.NameKey;
            Description = entity.DescKey;
            Open = entity.IsEnabled;
            PriceType = (ItemPriceType)entity.PriceType;
            Price = (uint)entity.Price;
            Gender = (Gender)entity.RequiredGender;
            AddRange(lineups);
        }

        public static void Initialize(
          List<RandomShopColor> colors,
          List<RandomShopEffect> effects,
          List<RandomShopPeriod> periods)
        {
            Colors = colors;
            Effects = effects;
            Periods = periods;
        }
    }
}
