using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_shopping_basket")]
    public class PlayerShoppingBasketEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public int PlayerId { get; set; }

        public PlayerEntity Player { get; set; }

        [Column]
        public long ShopItemId { get; set; }

        public ShopItemEntity ShopItem { get; set; }

        [Column]
        public int ShopPriceType { get; set; }

        [Column]
        public int ShopPeriodType { get; set; }

        [Column]
        public int ShopPeriod { get; set; }

        [Column]
        public byte ShopColor { get; set; }

        [Column]
        public int ShopEffectId { get; set; }
    }
}
