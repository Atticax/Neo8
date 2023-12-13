using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "BtcOptions", IsNullable = false, Namespace = "")]
    public class BtcOptionsDto
    {
        [XmlElement("tutorial")]
        public BtcOptionTutorialDto[] TutorialDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class BtcOptionTutorialDto
    {
        [XmlAttribute]
        public string option { get; set; }

        [XmlElement("component")]
        public BtcOptionTutorialComponentDto[] ComponentDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class BtcOptionTutorialComponentDto
    {
        [XmlAttribute]
        public byte step { get; set; }

        [XmlElement("requitial")]
        public BtcOptionTutorialRequitialDto[] RequitialDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class BtcOptionTutorialRequitialDto
    {
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
}
