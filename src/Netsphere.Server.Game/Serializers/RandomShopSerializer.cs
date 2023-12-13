using BlubLib.Serialization;
using Netsphere.Server.Game.Data;
using ProudNet;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Netsphere.Server.Game.Serializers
{
    public class RandomShopSerializer :
    ISerializer<ImmutableDictionary<int, RandomShopPackage>>,
    ISerializer
    {
        public bool CanHandle(Type type) => type == typeof(ImmutableDictionary<int, RandomShopPackage>);

        public ImmutableDictionary<int, RandomShopPackage> Deserialize(
      BlubSerializer blubSerializer,
      BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public void Serialize(
          BlubSerializer blubSerializer,
          BinaryWriter writer,
          ImmutableDictionary<int, RandomShopPackage> value)
        {
            List<RandomShopPackage> randomShopPackageList = new List<RandomShopPackage>();
            List<RandomShopLineup> lineups = new List<RandomShopLineup>();
            List<RandomShopPackage> list = value.Values.ToList();
            list.ForEach(x => x.ForEach(y => lineups.Add(y)));
            writer.Write(list.Count);
            foreach (RandomShopPackage randomShopPackage in list)
            {
                writer.Write(randomShopPackage.Id);
                writer.WriteProudString("");
                writer.WriteProudString(randomShopPackage.Name);
                writer.WriteProudString(randomShopPackage.Description);
                writer.WriteProudString(randomShopPackage.Open ? "on" : "off");
                switch (randomShopPackage.PriceType)
                {
                    case ItemPriceType.PEN:
                        writer.WriteProudString("pen");
                        break;
                    case ItemPriceType.CP:
                        writer.WriteProudString("coupon");
                        break;
                    default:
                        writer.WriteProudString("item");
                        break;
                }
                writer.Write(randomShopPackage.Price);
                switch (randomShopPackage.Gender)
                {
                    case Gender.None:
                        writer.WriteProudString("all");
                        break;
                    case Gender.Male:
                        writer.WriteProudString("man");
                        break;
                    case Gender.Female:
                        writer.WriteProudString("woman");
                        break;
                }
            }
            writer.Write(RandomShopPackage.Effects.Count);
            foreach (RandomShopEffect effect in RandomShopPackage.Effects)
            {
                writer.Write(effect.Probability);
                writer.Write((byte)effect.Grade);
                writer.WriteProudString(effect.Group);
                writer.Write((uint)effect.EffectId);
            }
            writer.Write(RandomShopPackage.Colors.Count);
            foreach (RandomShopColor color in RandomShopPackage.Colors)
            {
                writer.Write(color.Probability);
                writer.Write((byte)color.Grade);
                writer.WriteProudString(color.Group);
                writer.Write(color.Color);
            }
            writer.Write(lineups.Count);
            foreach (RandomShopLineup randomShopLineup in lineups)
            {
                writer.Write(randomShopLineup.Item.Probability);
                writer.Write((byte)randomShopLineup.Item.Grade);
                writer.Write(randomShopLineup.PackageId);
                writer.WriteProudString("ITEM");
                writer.Write((uint)randomShopLineup.Item.RewardValue);
                writer.Write(randomShopLineup.Item.Item.Id);
                writer.WriteProudString(randomShopLineup.Item.Color.Group);
                writer.WriteProudString(randomShopLineup.Item.Effect.Group);
                writer.WriteProudString(randomShopLineup.Item.Period.Group);
                writer.Write((uint)randomShopLineup.Item.DefaultColor);
            }
            writer.Write(RandomShopPackage.Periods.Count);
            foreach (RandomShopPeriod period in RandomShopPackage.Periods)
            {
                writer.Write(period.Probability);
                writer.Write((byte)period.Grade);
                writer.WriteProudString(period.Group);
                writer.Write((uint)period.Type);
                writer.Write(period.Value);
            }
        }
    }
}
