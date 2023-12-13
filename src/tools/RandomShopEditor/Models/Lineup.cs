using Netsphere.Database.Game;

namespace RandomShopEditor.Models
{
  public class Lineup
  {
    public int Id { get; set; }
    public int PackageId { get; set; }
    public byte ItemCategoryId { get; set; }
    public byte RewardValue { get; set; }
    public long ShopItemId { get; set; }
    public long ColorId { get; set; }
    public long EffectId { get; set; }
    public long PeriodId { get; set; }
    public byte DefaultColor { get; set; }
    public int Probability { get; set; }
    public byte Grade { get; set; }

    public Lineup() { }
    public Lineup(RandomShopLineupEntity entity)
    {
      Id = (int)entity.Id;
      PackageId = (int)entity.RandomShopPackageId;
      ItemCategoryId = entity.ItemCategoryId;
      RewardValue = entity.RewardValue;
      ShopItemId = entity.ShopItemId;
      ColorId = entity.RandomShopColorId;
      EffectId = entity.RandomShopEffectId;
      PeriodId = entity.RandomShopPeriodId;
      DefaultColor = entity.DefaultColor;
      Probability = entity.Probability;
      Grade = entity.Grade;
    }
  }
}
