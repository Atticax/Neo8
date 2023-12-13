using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "combination", IsNullable = false, Namespace = "")]
    public class CombinationInfoDto
    {
        [XmlAttribute]
        public uint pen_price { get; set; }

        [XmlAttribute]
        public uint unique_prob_up_item_key { get; set; }

        [XmlAttribute]
        public int unique_prob_up_using_max_count { get; set; }

        public CombinationArgonComponentDto argon_component { get; set; }

        public CombinationKryptonComponentDto krypton_component { get; set; }

        public CombinationEnchantOptionDto enchant_option { get; set; }

        public OvercountWeightDto overcount_weight { get; set; }

        [XmlElement("component")]
        public CombinationComponentDto[] ComponentDtos { get; set; }

        [XmlElement("method")]
        public CombinationMethodDto[] MethodDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class CombinationArgonComponentDto
    {
        [XmlAttribute]
        public uint item_key { get; set; }

        [XmlAttribute]
        public int min_use_cnt { get; set; }

        [XmlAttribute]
        public int max_use_cnt { get; set; }

        [XmlAttribute]
        public int unique_prob { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class CombinationKryptonComponentDto
    {
        [XmlAttribute]
        public uint item_key { get; set; }

        [XmlAttribute]
        public int min_use_cnt { get; set; }

        [XmlAttribute]
        public int max_use_cnt { get; set; }

        [XmlAttribute]
        public int unique_prob { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class CombinationEnchantOptionDto
    {
        [XmlAttribute]
        public string use { get; set; }

        [XmlAttribute]
        public int min_count { get; set; }

        [XmlAttribute]
        public int max_count { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class OvercountWeightDto
    {
        [XmlElement("data")]
        public OvercountWeightDataDto[] OvercountWeightDataDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class OvercountWeightDataDto
    {
        [XmlAttribute]
        public uint item_key { get; set; }

        [XmlAttribute]
        public int weight { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class CombinationComponentDto
    {
        [XmlAttribute]
        public uint key { get; set; }

        [XmlElement("data")]
        public CombinationComponentDataDto[] ComponentDataDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class CombinationComponentDataDto
    {
        [XmlAttribute]
        public uint item_key { get; set; }

        [XmlAttribute]
        public int min_use_cnt { get; set; }

        [XmlAttribute]
        public int max_use_cnt { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class CombinationMethodDto
    {
        [XmlAttribute]
        public uint key { get; set; }

        [XmlAttribute]
        public uint component_key { get; set; }

        [XmlAttribute]
        public string use { get; set; }

        [XmlAttribute]
        public uint represent_item_key { get; set; }

        [XmlElement("requital")]
        public CombinationMethodRequitalDto[] MethodRequitalDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class CombinationMethodRequitalDto
    {
        [XmlAttribute]
        public uint item_key { get; set; }

        [XmlAttribute]
        public uint probability { get; set; }

        [XmlAttribute]
        public uint shop_id { get; set; }

        [XmlAttribute]
        public byte color { get; set; }

        [XmlAttribute]
        public uint effect_id { get; set; }

        [XmlAttribute]
        public string period_type { get; set; }

        [XmlAttribute]
        public ushort period { get; set; }
    }
}
