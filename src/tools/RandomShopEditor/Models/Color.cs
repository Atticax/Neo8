using NeoNetsphere.Database.Game;

namespace RandomShopEditor.Models
{
  public class Color
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public byte Color_ { get; set; }

    public Color(Randomshop entity)
    {
      Id = (int)entity.Id;
      Name = entity.Name;
      Color_ = entity.Color;
    }
  }
}
