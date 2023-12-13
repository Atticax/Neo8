using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("randomshop_period")]
    public class RandomShopPeriodEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        [ForeignKey("ShopPrice")]
        public int ShopPriceId { get; set; }

        public ShopPriceEntity ShopPrice { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public int Probability { get; set; }

        [Column]
        public byte Grade { get; set; }
    }
}
