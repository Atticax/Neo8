using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("player_friends")]
    public class PlayerFriendEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Column]
        public int PlayerId { get; set; }
        public PlayerEntity Player { get; set; }

        [Column]
        public int FriendPlayerId { get; set; }
        public PlayerEntity FriendPlayer { get; set; }

        [Column]
        public byte State { get; set; }
    }
}
