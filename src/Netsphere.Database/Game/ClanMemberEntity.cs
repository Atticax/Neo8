using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("clan_members")]
    public class ClanMemberEntity
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
        public long JoinDate { get; set; }

        [Column]
        public byte State { get; set; }

        [Column]
        public byte Role { get; set; }

        [Column]
        public long LastLoginDate { get; set; }

        [Column]
        public string Answer1 { get; set; }

        [Column]
        public string Answer2 { get; set; }

        [Column]
        public string Answer3 { get; set; }

        [Column]
        public string Answer4 { get; set; }

        [Column]
        public string Answer5 { get; set; }
    }
}
