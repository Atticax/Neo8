using System.Xml.Serialization;

namespace RandomShopEditor.XML
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "string_table")]
  public class RandomShopInfoStringTableDto
  {
    [XmlElement("string")]
    public RSInfo[] RSInfo { get; set; }
  }

  public class RSInfo
  {
    [XmlAttribute("key")]
    public string Key { get; set; }
    [XmlAttribute("kor")]
    public string Kor { get; set; }
    [XmlAttribute("ger")]
    public string Ger { get; set; }
    [XmlAttribute("eng")]
    public string Eng { get; set; }
    [XmlAttribute("tur")]
    public string Tur { get; set; }
    [XmlAttribute("fre")]
    public string Fre { get; set; }
    [XmlAttribute("spa")]
    public string Spa { get; set; }
    [XmlAttribute("ita")]
    public string Ita { get; set; }
    [XmlAttribute("rus")]
    public string Rus { get; set; }
    [XmlAttribute("ame")]
    public string Ame { get; set; }
    [XmlAttribute("cns")]
    public string Cns { get; set; }
    [XmlAttribute("tha")]
    public string Tha { get; set; }
    [XmlAttribute("twn")]
    public string Twn { get; set; }
    [XmlAttribute("jap")]
    public string Jap { get; set; }
    [XmlAttribute("idn")]
    public string Idn { get; set; }
  }
}
