using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("clans")]
    public class ClanEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public int OwnerId { get; set; }
        public PlayerEntity Owner { get; set; }

        [Column]
        public long CreationDate { get; set; }

        [Column]
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [Column]
        [Required]
        [MaxLength(8)]
        public string Icon { get; set; }

        [Column]
        [MaxLength(40)]
        public string Description { get; set; }

        [Column]
        public byte Area { get; set; }

        [Column]
        public byte Activity { get; set; }

        [Column]
        [MaxLength(40)]
        public string Question1 { get; set; }

        [Column]
        [MaxLength(40)]
        public string Question2 { get; set; }

        [Column]
        [MaxLength(40)]
        public string Question3 { get; set; }

        [Column]
        [MaxLength(40)]
        public string Question4 { get; set; }

        [Column]
        [MaxLength(40)]
        public string Question5 { get; set; }

        [Column]
        public byte Class { get; set; }

        [Column]
        [MaxLength(40)]
        public string Announcement { get; set; }

        [Column]
        public bool IsPublic { get; set; }

        [Column]
        public byte RequiredLevel { get; set; }

        public List<ClanMemberEntity> Members { get; set; } = new List<ClanMemberEntity>();
        public List<ClanBanEntity> Bans { get; set; } = new List<ClanBanEntity>();
        public List<ClanEventEntity> Events { get; set; } = new List<ClanEventEntity>();
    }
}
