using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("randomshop_package")]
    public class RandomShopPackageEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public string NameKey { get; set; }

        [Column]
        public string DescKey { get; set; }

        [Column]
        public byte PriceType { get; set; }

        [Column]
        public int Price { get; set; }

        [Column]
        public byte RequiredGender { get; set; }

        [Column]
        public bool IsEnabled { get; set; }
    }
}
