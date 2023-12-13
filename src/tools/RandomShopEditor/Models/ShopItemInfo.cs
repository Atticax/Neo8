using Netsphere.Database.Game;

namespace RandomShopEditor.Models
{
  public class ShopItemInfo
  {
    public int Id { get; set; }
    public int ShopItemId { get; set; }
    public int PriceGroupId { get; set; }
    public int EffectGroupId { get; set; }
    public bool IsEnabled { get; set; }
    public int Discount { get; set; }

    public ShopItemInfo(ShopItemInfoEntity entity) {
      Id = entity.Id;
      ShopItemId = (int) entity.ShopItemId;
      PriceGroupId = entity.PriceGroupId;
      EffectGroupId = entity.EffectGroupId;
      IsEnabled = entity.IsEnabled;
      Discount = entity.DiscountPercentage;
    }
  }
}
