using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_boosters")]
    public class PlayerBoosterEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public int PlayerId { get; set; }

        public PlayerEntity Player { get; set; }

        [Column]
        public byte Slot { get; set; }

        [Column]
        public long? BoostId { get; set; }
        public PlayerItemEntity Boost { get; set; }
    }
}
