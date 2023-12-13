using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "card_system_info", IsNullable = false, Namespace = "")]
    public class CardSystemInfoDto
    {
        [XmlAttribute]
        public bool active { get; set; }

        public CurrentSeasonDto current_season { get; set; }

        public SeasonDto season { get; set; }

        public FormulaDto formula { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class CurrentSeasonDto
    {
        [XmlAttribute]
        public int num { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class SeasonDto
    {
        [XmlAttribute]
        public int num { get; set; }

        [XmlAttribute]
        public uint buy_capsule { get; set; }

        [XmlAttribute]
        public uint shop_id { get; set; }

        [XmlElement("card")]
        public CardDto[] CardDtos { get; set; }

        public SeasonRewardDto reward { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class SeasonRewardDto
    {
        [XmlAttribute]
        public uint item_id { get; set; }

        [XmlAttribute]
        public uint shop_id { get; set; }

        [XmlAttribute]
        public string period_type { get; set; }

        [XmlAttribute]
        public ushort period_value { get; set; }

        [XmlAttribute]
        public byte color { get; set; }

        [XmlAttribute]
        public uint effect_id { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class CardDto
    {
        [XmlAttribute]
        public int num { get; set; }

        [XmlAttribute]
        public uint item_id { get; set; }

        [XmlAttribute]
        public uint shop_id { get; set; }

        [XmlAttribute]
        public string period_type { get; set; }

        [XmlAttribute]
        public ushort period_value { get; set; }

        [XmlAttribute]
        public byte color { get; set; }

        [XmlAttribute]
        public uint effect_id { get; set; }

        [XmlAttribute]
        public int grade { get; set; }

        [XmlAttribute]
        public int play_prob { get; set; }

        [XmlAttribute]
        public int try_prob { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class FormulaDto
    {
        [XmlAttribute]
        public int play_limit_time { get; set; }

        [XmlAttribute]
        public int play_limit_min_count { get; set; }

        [XmlAttribute]
        public int play_default_time { get; set; }

        [XmlAttribute]
        public int play_default_count { get; set; }

        [XmlAttribute]
        public int gamble_pen { get; set; }

        [XmlAttribute]
        public int gamble_limit_min_count { get; set; }

        [XmlAttribute]
        public int complete_card_count { get; set; }
    }
}
