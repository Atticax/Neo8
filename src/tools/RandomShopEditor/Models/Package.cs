using System.Linq;
using System.Text.RegularExpressions;

using Netsphere.Database.Game;
using Netsphere.Tools.RandomShopEditor.Services;

namespace RandomShopEditor.Models
{
  public class Package
  {
    public int Id { get; }
    public string NameKey { get; }
    public string DescKey { get; }
    public byte PriceType { get; }
    public int Price { get; }
    public byte RequiredGender { get; }
    public bool Enabled { get; }

    public Package(RandomShopPackageEntity entity) 
    {
      var rsInfo = ResourceService.Instance.RandomShopInfoStringTable.RSInfo;
      var name = rsInfo.FirstOrDefault(x => x.Key == entity.NameKey);
      var desc = rsInfo.FirstOrDefault(x => x.Key == entity.NameKey);
      var pattern = @"\{((\s*?.*?)*?)\}";
      Id = (int)entity.Id;
      NameKey = (name != null && name.Eng != null) ? Regex.Replace(name.Eng, pattern, "") : "Not available";
      DescKey = (desc != null && desc.Eng != null) ? Regex.Replace(desc.Eng, pattern, "") : "Not available";
      PriceType = entity.PriceType;
      Price = entity.Price;
      RequiredGender = entity.RequiredGender;
      Enabled = entity.IsEnabled;
    }
  }
}
