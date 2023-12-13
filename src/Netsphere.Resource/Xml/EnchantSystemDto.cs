using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "enchant_system", IsNullable = false, Namespace = "")]
    public class EnchantSystemDto
    {
        [XmlElement("enchant")]
        public EnchantDtoDto[] EnchantDtoDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EnchantDtoDto
    {
        [XmlAttribute]
        public string level { get; set; }

        [XmlElement("enchant_data_item")]
        public EnchantDataItemDto[] EnchantDataItemDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EnchantDataItemDto
    {
        [XmlAttribute]
        public byte item_category { get; set; }

        [XmlAttribute]
        public byte item_subcategory { get; set; }

        [XmlElement("effect")]
        public EnchantDataItemEffectDto[] EnchantDataItemEffectDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EnchantDataItemEffectDto
    {
        [XmlAttribute]
        public uint id { get; set; }

        [XmlAttribute]
        public int prob { get; set; }  //probability?
    }
}
