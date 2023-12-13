using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "onetimechargelist", IsNullable = false, Namespace = "")]
    public class OneTimeChargeListDto
    {
        [XmlElement("onetimecharge")]
        public OneTimeChargeDto[] OneTimeChargeDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class OneTimeChargeDto
    {
        [XmlAttribute]
        public uint item_key { get; set; }

        [XmlElement("category")]
        public OneTimeChargeCategoryDto[] OneTimeChargeCategory { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class OneTimeChargeCategoryDto
    {
        [XmlAttribute]
        public int type { get; set; }

        [XmlElement("sub_category")]
        public OneTimeChargeSubCategoryDto[] OneTimeChargeSubCategory { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class OneTimeChargeSubCategoryDto
    {
        [XmlAttribute]
        public bool aleatory { get; set; }

        [XmlAttribute]
        public bool unique_boost { get; set; }

        [XmlElement("item")]
        public OneTimeChargeItemDto[] OneTimeChargeItem { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class OneTimeChargeItemDto
    {
        [XmlAttribute]
        public uint key { get; set; }

        [XmlAttribute]
        public uint previewEffect { get; set; }

        [XmlElement]
        public int priceType { get; set; }

        [XmlElement]
        public int periodType { get; set; }

        [XmlElement]
        public ushort period { get; set; }

        [XmlElement]
        public int color { get; set; }

        [XmlElement]
        public uint amount { get; set; }

        [XmlElement]
        public uint boostKey { get; set; }

        [XmlElement]
        public uint rate { get; set; }
    }
}
