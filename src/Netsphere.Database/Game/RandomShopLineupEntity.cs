using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("randomshop_lineup")]
    public class RandomShopLineupEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        [ForeignKey("RandomShopPackage")]
        public long RandomShopPackageId { get; set; }

        public RandomShopPackageEntity RandomShopPackage { get; set; }

        [Column]
        public byte ItemCategoryId { get; set; }

        [Column]
        public byte RewardValue { get; set; }

        [Column]
        [ForeignKey("ShopItem")]
        public long ShopItemId { get; set; }

        public ShopItemEntity ShopItem { get; set; }

        [Column]
        [ForeignKey("RandomShopColor")]
        public long RandomShopColorId { get; set; }

        public RandomShopColorEntity RandomShopColor { get; set; }

        [Column]
        [ForeignKey("RandomShopEffect")]
        public long RandomShopEffectId { get; set; }

        public RandomShopEffectEntity RandomShopEffect { get; set; }

        [Column]
        [ForeignKey("RandomShopPeriod")]
        public long RandomShopPeriodId { get; set; }

        public RandomShopPeriodEntity RandomShopPeriod { get; set; }

        [Column]
        public byte DefaultColor { get; set; }

        [Column]
        public int Probability { get; set; }

        [Column]
        public byte Grade { get; set; }
    }
}
