using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netsphere.Database.Game
{
    [Table("players")]
    public class PlayerEntity
    {
        [Key]
        public int Id { get; set; }

        [Column]
        public byte TutorialState { get; set; }

        [Column]
        public int TotalExperience { get; set; }

        [Column]
        public int PEN { get; set; }

        [Column]
        public int AP { get; set; }

        [Column]
        public int Coins1 { get; set; }

        [Column]
        public int Coins2 { get; set; }

        [Column]
        public byte CurrentCharacterSlot { get; set; }

        //[Column]
        //public int TotalLosses { get; set; }

        public ClanMemberEntity ClanMember { get; set; }
        public List<PlayerBoosterEntity> Boosters { get; set; } = new List<PlayerBoosterEntity>();
        public PlayerNameTagEntity Nametag { get; set; }
        public List<PlayerCharacterEntity> Characters { get; set; } = new List<PlayerCharacterEntity>();
        public List<PlayerDenyEntity> Ignores { get; set; } = new List<PlayerDenyEntity>();
        public List<PlayerFriendEntity> Friends { get; set; } = new List<PlayerFriendEntity>();
        public List<PlayerItemEntity> Items { get; set; } = new List<PlayerItemEntity>();
        public List<PlayerMailEntity> Inbox { get; set; } = new List<PlayerMailEntity>();
        public List<PlayerSettingEntity> Settings { get; set; } = new List<PlayerSettingEntity>();
        public List<PlayerShoppingBasketEntity> ShoppingBaskets { get; set; } = new List<PlayerShoppingBasketEntity>();
        public List<PlayerTutorialWeaponEntity> TutorialWeapon { get; set; } = new List<PlayerTutorialWeaponEntity>();
        public List<PlayerTutorialSkillEntity> TutorialSkill { get; set; } = new List<PlayerTutorialSkillEntity>();
        public List<PlayerTutorialRealEntity> TutorialReal { get; set; } = new List<PlayerTutorialRealEntity>();
    }
}
