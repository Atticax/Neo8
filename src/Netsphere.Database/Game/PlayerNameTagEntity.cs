using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_nametag")]
    public class PlayerNameTagEntity
    {
        [Key]
        [ForeignKey("Player")]
        public int PlayerId { get; set; }

        public PlayerEntity Player { get; set; }

        [Column]
        [Required]
        public short TagId { get; set; }
    }
}
