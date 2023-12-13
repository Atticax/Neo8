using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "decomposition", IsNullable = false, Namespace = "")]
    public class DecompositionInfoDto
    {
        [XmlAttribute]
        public uint pen_price { get; set; }

        [XmlAttribute]
        public int min_hours { get; set; }

        [XmlAttribute]
        public int min_days { get; set; }

        [XmlElement("method")]
        public DecompositionMethodDto[] Methods { get; set; }

        public DecompositionBonusDataDto bonus_data { get; set; }

        public DecompositionProhibitionDto prohibition { get; set; }

        public DecompositionSubProhibition Subprohibition { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class DecompositionMethodDto
    {
        [XmlAttribute]
        public string period_type { get; set; }

        [XmlAttribute]
        public int effect_min_cnt { get; set; }

        [XmlAttribute]
        public int effect_max_cnt { get; set; }

        [XmlAttribute]
        public string use { get; set; }

        [XmlAttribute]
        public string bonus { get; set; }

        [XmlElement("component")]
        public DecompositionMethodComponentDto[] Components { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class DecompositionMethodComponentDto
    {
        [XmlAttribute]
        public int condition { get; set; }

        [XmlAttribute]
        public uint item_key { get; set; }

        [XmlAttribute]
        public uint shop_id { get; set; }

        [XmlAttribute]
        public string period_type { get; set; }

        [XmlAttribute]
        public ushort period { get; set; }

        [XmlAttribute]
        public byte color { get; set; }

        [XmlAttribute]
        public uint effect_id { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class DecompositionBonusDataDto
    {
        [XmlElement("bonus")]
        public DecompositionBonusDataDataDto[] Datas { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class DecompositionBonusDataDataDto
    {
        [XmlAttribute]
        public int period_multiple_value { get; set; }

        [XmlAttribute]
        public string item_main_type { get; set; }

        [XmlAttribute]
        public string item_sub_type { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class DecompositionProhibitionDto
    {
        [XmlElement("data")]
        public DecompositionProhibitionDataDto[] Datas { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class DecompositionProhibitionDataDto
    {
        [XmlAttribute]
        public uint item_key { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class DecompositionSubProhibition
    {
    }
}
