namespace Netsphere.Resource.Xml
{
  using System.Collections.Generic;
  using System.Xml.Serialization;

  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "ItemReward")]
  public class ItemRewardDto
  {
    [XmlElement("item")]
    public List<Item> Items { get; set; }
  }

  public class Item
  {
    [XmlAttribute("Number")]
    public int Number { get; set; }
    [XmlElement("group")]
    public List<Group> Groups { get; set; }
  }

  public class Group
  { 
    [XmlElement("reward")]
    public List<Reward> Rewards { get; set; }
  }

  public class Reward
  {
    [XmlAttribute("Type")]
    public byte Type { get; set; }
    [XmlAttribute("Data")]
    public int Data { get; set; }
    [XmlAttribute("PriceType")]
    public byte PriceType { get; set; }
    [XmlAttribute("PeriodType")]
    public byte PeriodType { get; set; }
    [XmlAttribute("Color")]
    public byte Color { get; set; }
    [XmlAttribute("Value")]
    public uint Value { get; set; }
    [XmlAttribute("Effects")]
    public string Effects { get; set; }
    [XmlAttribute("Rate")]
    public uint Rate { get; set; }
  }
}
