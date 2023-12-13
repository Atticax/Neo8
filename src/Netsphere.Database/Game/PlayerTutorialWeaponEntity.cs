using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_tutorials_weapon")]
    public class PlayerTutorialWeaponEntity
    {
        [Key]
        public long Id { get; set; }

        [Column]
        public int PlayerId { get; set; }

        public PlayerEntity Player { get; set; }

        [Column]
        public byte OptionBtc { get; set; }

        [Column]
        public byte Option { get; set; }
    }
}
