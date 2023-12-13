using System.Xml.Serialization;

namespace Netsphere.Resource.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "enchant_data", IsNullable = false, Namespace = "")]
    public class EnchantDataDto
    {
        public EnchantConfigDto enchant_config { get; set; }

        public MasteryNeedTableDto mastery_need_table { get; set; }

        public EnchantPriceTableDto enchant_price_table { get; set; }

        public EnchantResetProbTableDto enchant_reset_prob_table { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EnchantConfigDto
    {
        [XmlElement("data")]
        public DataDto[] Datas { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class DataDto
    {
        [XmlAttribute]
        public int mastery_per_min { get; set; }

        [XmlAttribute]
        public uint bonus_prob { get; set; }

        [XmlAttribute]
        public uint prob_unit { get; set; }

        [XmlAttribute]
        public uint notice_enchant_cnt { get; }
    }

    [XmlType(AnonymousType = true)]
    public class MasteryNeedTableDto
    {
        [XmlElement("mastery_need")]
        public MasteryNeedDto[] MasteryNeedDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class MasteryNeedDto
    {
        [XmlAttribute]
        public string item_type { get; set; }

        [XmlAttribute]
        public uint enchant_cnt { get; set; }

        [XmlAttribute]
        public uint durability { get; set; }

        [XmlAttribute]
        public uint period { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EnchantPriceTableDto
    {
        [XmlElement("enchant_price")]
        public EnchantPriceDto[] EnchantPriceDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EnchantPriceDto
    {
        [XmlAttribute]
        public string item_type { get; set; }

        [XmlAttribute]
        public uint enchant_cnt { get; set; }

        [XmlAttribute]
        public uint enchant_price { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EnchantResetProbTableDto
    {
        [XmlElement("enchant_reset_prob")]
        public EnchantResetProbDto[] EnchantResetProbDtos { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class EnchantResetProbDto
    {
        [XmlAttribute]
        public uint enchant_cnt { get; set; }

        [XmlAttribute]
        public uint durability { get; set; }

        [XmlAttribute]
        public uint period { get; set; }
    }
}
