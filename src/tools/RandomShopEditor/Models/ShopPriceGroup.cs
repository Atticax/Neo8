using Netsphere.Database.Game;

namespace RandomShopEditor.Models
{
  public class ShopPriceGroup
  {
    public int Id { get; set; }
    public byte PriceType { get; set; }
    public string Name { get; set; }

    public ShopPriceGroup(ShopPriceGroupEntity entity)
    {
      Id = entity.Id;
      PriceType = entity.PriceType;
      Name = entity.Name;
    }
  }
}
