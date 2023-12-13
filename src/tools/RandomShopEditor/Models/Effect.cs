using Netsphere.Database.Game;

namespace RandomShopEditor.Models
{
  public class Effect
  {
    public int Id { get; set; }
    public string Name { get; set; }

    public Effect(RandomShopEffectEntity entity)
    {
      Id = (int)entity.Id;
      Name = entity.Name;
    }
  }
}
