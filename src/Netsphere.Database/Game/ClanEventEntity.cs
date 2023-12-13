using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("clan_events")]
    public class ClanEventEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public int ClanId { get; set; }
        public ClanEntity Clan { get; set; }

        [Column]
        public int PlayerId { get; set; }
        public PlayerEntity Player { get; set; }

        [Column]
        public long Date { get; set; }

        [Column]
        public byte Type { get; set; }

        [Column]
        public long Value1 { get; set; }
    }
}
