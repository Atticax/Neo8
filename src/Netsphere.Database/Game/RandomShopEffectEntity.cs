using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("randomshop_effect")]
    public class RandomShopEffectEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        [ForeignKey("EffectGroup")]
        public int Effect { get; set; }

        public ShopEffectGroupEntity EffectGroup { get; set; }

        [Column]
        public int Probability { get; set; }

        [Column]
        public byte Grade { get; set; }
    }
}
