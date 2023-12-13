using Netsphere.Database.Game;

namespace RandomShopEditor.Models
{
  public class Period
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int ShopPriceId { get; set; }

    public Period(RandomShopPeriodEntity entity)
    {
      Id = (int)entity.Id;
      Name = entity.Name;
      ShopPriceId = entity.ShopPriceId;
    }
  }
}
