using System.Xml.Serialization;

namespace RandomShopEditor.XML
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "packagelist")]
  public class RandomShopPackageDTO
  {
    [XmlElement("package")]
    public RSPackage[] RSPackage { get; set; }
  }

  public class RSPackage
  {
    [XmlAttribute("id")]
    public int Id { get; set; }
    [XmlAttribute("name_key")]
    public string NameKey { get; set; }
    [XmlAttribute("desc_key")]
    public string DescKey { get; set; }
  }
}
