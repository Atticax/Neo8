using System;

namespace Netsphere
{
    public enum SecurityLevel : byte
    {
        User = 0,
        GameMaster = 1,
        Developer = 2,
        Administrator = 3
    }

    public enum ServerType : uint
    {
        Game = 3,
        Chat = 5,

        Relay = 10
    }

    public enum Gender : byte
    {
        None = 0,
        Male = 1,
        Female = 2
    }

    public enum CharacterGender : byte
    {
        Male = 0,
        Female = 1
    }

    public enum CommunitySetting : byte
    {
        Allow = 0,
        FriendOnly = 1,
        Deny = 2
    }

    public enum UseCoinMessage : int
    {
        Ok = 0,
        InsufficientCoin = 1,
        UnableToUse = 2
    }

    public enum BuffType : int
    {
        PEN = 0,
        EXP = 1,
        Respawn = 2,
        Tracking = 3,
        HP = 4,
        SP = 5
    }

    public enum LuckyShotType : int //todo
    {
        PEN = 1,
        EXP = 2
    }

    public enum RoomLeaveReason : byte
    {
        Left = 0,
        Kicked = 1,
        MasterAFK = 2,
        AFK = 3,
        ModeratorKick = 4,
        VoteKick = 5
    }

    public enum VoteKickReason : byte
    {
        Hacking = 0,
        BadMannger = 1,
        BugUsing = 2,
        AFK = 3,
        Etc = 4
    }

    public enum MissionRewardType : byte
    {
        PEN = 1
    }

    [Flags]
    public enum ShopResourceType : byte
    {
        None = 0,
        Price = 1,
        Effect = 2,
        Item = 4,
        UniqueItem = 8,
        RandomShop = 16
    }

    public enum CostumeSlot : byte
    {
        Hair = 0,
        Face = 1,
        Shirt = 2,
        Pants = 3,
        Gloves = 4,
        Shoes = 5,
        Accessory = 6,
        Pet = 7
    }

    public enum WeaponSlot : byte
    {
        Weapon1 = 0,
        Weapon2 = 1,
        Weapon3 = 2,
        None = 3
    }

    public enum SkillSlot : byte
    {
        Skill = 0
    }

    public enum ItemPriceType : uint
    {
        PEN = 1,
        AP = 2,
        Premium = 3,
        None = 4,
        CP = 5
    }

    public enum ItemPeriodType : uint
    {
        None = 1,
        Hours = 2,
        Days = 3,
        Units = 4 // ?
    }

    public enum UseItemAction : byte
    {
        Equip = 1,
        UnEquip = 2,
        Unk4 = 4
    }

    public enum InventoryAction : uint
    {
        Add = 1,
        Update = 2
    }

    public enum ItemBuyResult : byte
    {
        DBError = 0,
        NotEnoughMoney = 1,
        UnkownItem = 2,
        OK = 3
    }

    public enum ItemRepairResult : byte
    {
        Error0 = 0,
        Error1 = 1,
        Error2 = 2,
        Error3 = 3,
        NotEnoughMoney = 4,
        OK = 5,
        Error4 = 6
    }

    public enum ItemRefundResult : byte
    {
        OK = 0,
        Failed = 1
    }

    public enum CapsuleRewardType : uint
    {
        PEN = 1,
        Item = 2
    }

    public enum GameRule : uint
    {
        Deathmatch = 1,
        Touchdown = 2,
        Survival = 3,
        Practice = 4,
        Tutorial = 5,
        SemiTouchdown = 6, // Dev
        Scenario = 7,
        Chaser = 8,
        BattleRoyal = 9,
        Captain = 10,
        Siege = 11,
        Conquest = 12,
        Training = 13,
        Random = 14,
        Warfare = 15,
        Arena = 20
    }

    public enum GameState : uint
    {
        Waiting = 1,
        Playing = 2,
        Result = 3,
        Loading = 4
    }

    public enum GameTimeState : uint
    {
        None = 0,
        FirstHalf = 1,
        HalfTime = 2,
        SecondHalf = 3
    }

    public enum TeamId : byte
    {
        Neutral = 0,
        Alpha = 1,
        Beta = 2
    }

    public enum PlayerGameMode : byte
    {
        Normal = 1,
        Spectate = 2
    }

    public enum PlayerState : byte
    {
        Alive = 0,
        Dead = 1,
        Waiting = 2,
        Spectating = 3,
        Lobby = 4
    }

    public enum GameEventMessage : byte
    {
        ChangedTeamTo = 1,
        EnteredRoom = 2,
        LeftRoom = 3,
        Kicked = 4,
        MasterAFK = 5,
        AFK = 6,
        KickedByModerator = 7,
        BallReset = 8,
        StartGame = 9,
        TouchdownAlpha = 10,
        TouchdownBeta = 11,
        ChatMessage = 13,
        TeamMessage = 14,
        ResetRound = 15,
        NextRoundIn = 16,
        ResultIn = 17,
        HalfTimeIn = 18,
        RespawnIn = 21,
        GodmodeForXSeconds = 22,
        PlayerRatioMsgBox = 23,
        CantStartGame = 24,
        UserEntering = 25,
        UserNotReady = 26,
        RoomModeIsChanging = 27,
        ChaserIn = 28
    }

    //    public enum VoteKickReason : byte
    //    {
    //        Hacking = 0,
    //        BadMannger = 1,
    //        BugUsing = 2,
    //        AFK = 3,
    //        Etc = 4,
    //    }

    public enum ItemCategory : byte
    {
        Costume = 1,
        Weapon = 2,
        Skill = 3,
        OneTimeUse = 4,
        Boost = 5,
        Coupon = 6,
        EsperChip = 7
    }

    public enum ItemLicense : byte
    {
        None = 0,
        PlasmaSword = 1,
        CounterSword = 2,
        StormBat = 26,
        VitalShock = 29,
        SpyDagger = 33,
        DoubleSword = 34, // What weapon is this?
        SubmachineGun = 3,
        Revolver = 4,
        SemiRifle = 25,
        HandGun = 30,
        SmashRifle = 31,
        BurstShotgun = 32,
        HeavymachineGun = 5,
        GaussRifle = 27,
        RailGun = 6,
        Cannonade = 7,
        Sentrygun = 8,
        SentiForce = 9,
        SentiNel = 28,
        MineGun = 10,
        MindEnergy = 11,
        MindShock = 12,

        // Skills
        Anchoring = 13,
        Flying = 14,
        Invisible = 15,
        Detect = 16,
        Shield = 17,
        Block = 18,
        Bind = 19,
        Metallic = 20,
        HealthMastery = 22,
        SkillMastery = 23,
        SpeedMastery = 24
    }

    public enum ChannelCategory : byte
    {
        Speed = 0,
        Club = 3
    }

    public enum DenyAction : uint
    {
        Add = 0,
        Remove = 1
    }

    public enum FriendAction : uint
    {
        Add = 0,
        Remove = 1,
        AcceptRequest = 2,
        DenyRequest = 3
    }

    public enum FriendActionResult : uint
    {
        Success = 0,
        UserDoesNotExist = 1
    }

    public enum FriendState : uint
    {
        Requested = 1,
        Friends = 2,
        IncomingRequest = 3,
        Removed = 4,
        OnlyInMyList = 5
    }

    public enum ActorState : byte
    {
        Spectate,
        Ghost,
        Respawn,
        Wait,
        Standby,
        Normal,
        Run,
        Sit,
        Jump,
        BoundJump,
        Fall,
        DodgeLeft,
        DodgeRight,
        Stun,
        Down,
        StandUp,
        Blow,
        BoundBlow,
        Damage,
        CounterAttackDamage,
        DownDamage,
        FastRun,
        DodgeLeftAfterStun,
        DodgeRightAfterStun,
        JumpAfterAnchoring,
        Reload,
        SocialAction,
        ResultAction,
        Death,
        Idle,
        Destruction,

        SkillFly,
        SkillAnchoring,
        SkillStealth,
        SkillShield,
        SkillWallCreation,
        SkillBind,
        SkillMetalic,
        SkillBerserk,

        UseWeapon1,
        UseWeapon2,
        UseWeapon2Weak,
        UseWeapon2Strong,
        UseWeapon1Weak,
        UseWeapon1Strong,
        UseWeapon1Jump,
        UseWeapon1Strong1,
        UseWeapon1Attack2,
        UseWeapon1Attack3,
        UseWeapon1Attack4,
        UseWeapon1Attack5,
        UseWeapon1CounterAttack,
        ArcadeResultAction,
        Total
    }

    public enum AttackAttribute : byte
    {
        PlasmaSwordCritical = 1,
        PlasmaSwordStandWeak,
        PlasmaSwordStandStrong,
        PlasmaSwordAttack2Weak,
        PlasmaSwordAttack2,
        PlasmaSwordJumpCritical,
        PlasmaSwordJump,
        SubmachineGun,
        MachineGunLower,
        MachineGunMiddle,
        MachineGunUpper,
        AimedShot,
        AimedShot2,
        MineLauncher,
        MindEnergy,
        SentryGunMachineGun,
        BoundBlow,
        KillOneSelf,
        SentiWallWave,
        SentiNelWave,
        Revolver,
        CannonadeShot,
        CannonadeShot2,
        CounterSwordCounterCritical,
        CounterSwordCounterAttack,
        CounterSwordCritical,
        CounterSwordAttack1,
        CounterSwordAttack2,
        CounterSwordAttack3,
        CounterSwordAttack4,
        CounterSwordJumpDash,
        MindStormAttack1,
        MindStormAttack2,
        Smg2,
        BatSwordStandWeak,
        BatSwordStandStrong,
        BatSwordAttack2Weak,
        BatSwordAttack2,
        BatSwordCritical,
        BatSwordJumpCritical,
        BatSwordJump,
        KatanaSwordCritical,
        KatanaSwordAttack1,
        KatanaSwordAttack2,
        KatanaSwordAttack3,
        KatanaSwordAttack4,
        KatanaSwordJumpCritical,
        KatanaSwordJump,
        CardAttack1,
        CardAttack2,
        CardAttack3,
        Mg2,
        AssassinClaw,
        Smg3,
        Revolver2,
        Smg4,
        Smg3Gun,
        Smg3Sword,
        SpyDaggerCritical,
        SpyDaggerAttack1,
        SpyDaggerAttack2,
        SpydaggerAttack3,
        SpyDaggerJumpCritical,
        SpyDaggerJump,
        DoubleSwordCritical,
        DoubleSwordAttack1,
        DoubleSwordAttack2,
        DoubleSwordAttack3,
        DoubleSwordAttack4,
        DoubleSwordJumpDash,
        Airgun,
        Smg2Homing,
        EarthBomber,
        LightBomber,
        ChainLightGun,
        SparkRifle,
        ChainLightGunExplosion,
        BossVirusKnuckle,
        BossVirusSmoke,
        BossVirusExplosion,
        BossVirusStun,
        BossShotaKnuckle,
        BossShotaAssault,
        BossShotaLaser,
        TRAAttack1,
        TRBAttack1Left,
        TRBAttack1Right,
        VirusAttack1,
        TRAAttack2Left,
        TRAAttack2Right,
        TRBAttack2,
        TrabigExplosion,
        TeamChange
    }

    public enum Condition : uint
    {
        Blow = 2,
        Push = 4,
        Stun = 8,
        Bind = 16
    }

    public enum EffectType
    {
        None = 0,
        HP = 300,
        SP = 301,
        Defense = 302,
        Attack = 303,
        ReloadSpeed = 304,
        ChaserMovement = 306,
        Movespeed = 307,
        StationaryWeaponHP = 308,
        StationaryWeaponDistance = 309,
        StationaryWeaponInstallSpeed = 310,
        DefenseMelee = 311,
        DefenseRifle = 312,
        DefenseHead = 313,
        DefenseGuns = 314,
        DefenseHeadGuns = 315,
        DefenseHeavyGuns = 316,
        DefenseHeadHeavyGuns = 317,
        DefenseSnipe = 318,
        StationaryWeaponDefense = 319,
        MentalWeaponDefense = 320,
        DefenseThrowingWeapon = 321,
        StunTimeReduce = 330,
        RecoveryByHeal = 331,
        SPRegen = 332,
        MetallicReflectDamage = 333,
        BlockHP = 334,
        InvisibleTimeReduce = 335,
        BindTimeReduce = 336,
        CriticalRate = 337,
        CriticalProbability = 338,
        ChaserAttack = 339,
        CaptainAttack = 340,
        DamageReductionByDistance = 341,
        WallJumpSPReduce = 342,
        EvadeSPReduce = 343,
        InvincibleTime = 344,
        Accuracy = 345,
        AnchorSPReduce = 346,
        DetectRange = 347,
        FlyingSPReduce = 348,
        InvisibleSPReduce = 349,
        ShiledSPReduce = 350,
        BlockSPReduce = 351,
        BindSPReduce = 352,
        MetallicSPReduce = 353,
        KnockbackDistance = 354,
        StunTime = 355,
        Blow = 356,
        BlowReduce = 357,
        KnockbackDistanceReduce = 358,
        AttackBelowHealth = 359,
        DefenseBelowHealth = 360,
        MagnumSPReduce = 361,
        TwinbladeChargeTimeReduce = 362,
        IronBootsSPReduce = 363,
        Attack2 = 600,
        Magazine = 601,
        Movespeed2 = 602,
        ObjectAttack = 605,
        ChargeTimeReduce = 607,
        ChargeAttack = 608,
        Zoom = 609,
        Magazine2 = 613,
        Experience = 800,
        DefensePiercing = 606,
        PENGain = 801,
        ChaserChance = 802,
        FastSiege = 803,
        CoinFromKill = 804,
        ExperienceFromKill = 805,
        PENFromKill = 806,
        MPGain = 807,
        EnchantJackpotDoubleChance = 808,
        SuccessRate = 809,
        PreventReset = 810,
        UniqueBooster = 811
    }

    public enum ChatType : uint
    {
        Channel = 0,
        Club = 1
    }

    public enum MoneyType
    {
        PEN = 0,
        AP = 1
    }

    public enum XBNType : uint
    {
        ConstantInfo = 1,
        Actions = 2,
        Weapons = 3,
        Effects = 4,
        EffectMatch = 5,
        EnchantData = 6,
        EquipLimit = 7,
        MonsterStatus = 8,
        MonsterMapMiddle = 9
    }

    [Flags]
    public enum RoomSettings : uint
    {
        None = 0,
        IsFriendly = 1,
        EnableBurningBuff = 2
    }

    public enum RoomRandomSettings : uint
    {
        None = 0,
        Map = 1,
        ModeAndMap = 2
    }

    public enum ClubMemberState : uint
    {
        None = 0,
        JoinRequested = 1,
        Joined = 2
    }

    public enum ClubMemberPresenceState : uint
    {
        Offline = 0,
        Online = 1,
        Playing = 2
    }

    public enum ClubRole : uint
    {
        Master = 1,
        TemporaryMaster = 2,
        Staff = 3,
        Regular = 4,
        Normal = 5,
        BadManner = 6,
        AClass = 7,
        BClass = 8,
        CClass = 9
    }

    public enum ClubArea : uint
    {
        Europe = 1,
        Germany = 2,
        France = 3,
        Spain = 4,
        Italy = 5,
        Russia = 6,
        England = 7,
        NorthAmerica = 8,
        LatinAmerica = 9
    }

    public enum ClubActivity : uint
    {
        Fellowship = 1,
        ClanBattle = 2,
        Meeting = 3,
        KnowHowTransfer = 4
    }

    public enum ClubClass : uint
    {
        A = 0,
        B = 1,
        C = 2
    }

    public enum ClubSearchType : uint
    {
        None = 0,
        Name = 1,
        OwnerName = 3
    }

    public enum ClubSearchSort : uint
    {
        None = 0,
        Members = 1,
        Class = 2,
        Points = 3
    }

    public enum ClubSearchSortType : byte
    {
        Descending = 0,
        Ascending = 1
    }

    public enum ClubCreateResult : uint
    {
        Success = 0,
        Failed = 1,
        AlreadyInClan = 2,
        PendingJoinRequest = 3,
        NameAlreadyExists = 4,
        LevelRequirementNotMet = 6
    }

    public enum ClubNameCheckResult : uint
    {
        Available = 0,
        NotInAClan = 1,
        NotAvailable = 2,
        CannotBeUsed = 3,
        BreaksRules = 4,
        TooLong = 5,
        TooShort = 6,
    }

    public enum ClubCloseResult : uint
    {
        Success = 0,
        NotInClan = 1,
        MasterRequired = 2,
        ClanNotEmpty = 3,
        Four = 4
    }

    public enum ClubJoinResult : uint
    {
        Registered = 0,
        Joined = 1,
        NotInClan = 2,
        Failed = 3,
        AlreadyRegistered = 4,
        CantRegister = 5,
        ClubFull = 6,
        LevelRequirementNotMet = 7,
        WaitingForApproval = 8,
    }

    public enum ClubLeaveResult : uint
    {
        Success = 0,
        NotInClan = 1,
        Failed = 2
    }

    public enum ClubCommand : uint
    {
        Accept = 1,
        Decline = 2,
        Kick = 3,
        Ban = 4,
        Unban = 5
    }

    public enum ClubCommandResult : uint
    {
        Success = 0,
        NotInClan = 1,
        MemberNotFound = 2,
        MemberNotFound2 = 3,
        PermissionDenied = 4,
        NoMemberSelected = 5
    }

    public enum ClubLeaveReason : uint
    {
        Leave = 1,
        Kick = 2,
        Ban = 3
    }

    public enum ClubNoticeChangeResult : uint
    {
        Success = 0,
        NotInClan = 1,
        NoMatchFound = 2
    }

    public enum ClubAdminInfoModifyResult : uint
    {
        Success = 0,
        NotInClan = 1,
        NoMatchFound = 2
    }

    public enum ClubAdminJoinConditionModifyResult : uint
    {
        Success = 0,
        NotInClan = 1,
        NoMatchFound = 2
    }

    public enum ClubAdminChangeRoleResult : uint
    {
        Success = 0,
        NotInClan = 1,
        MemberNotFound = 2,
        MemberNotFound2 = 3,
        PermissionDenied = 4,
        CantChangeRank = 5,
        NothingChanged = 6
    }

    public enum OptionBtcClear
    {
        Tutorial = 1,
        Weapons = 2,
        Skills = 3,
        Battle = 4
    }

    public enum RandomShopGrade : byte
    {
        Common = 0, // 0x0E <- weird!
        Rare = 10, // 0x0A
        Legendary = 30, // 0x1E
    }

    public enum RandomShopRollingResult
    {
        Failed,
        Ok,
    }

    public enum NoteGiftResult : uint
    {
        Error,
        NotEnogthAP,
        Error2,
        Success,
    }

    public enum CardGambleResult : uint
    {
        None,
        NotEnoughCard,
        NotEnoughPEN,
        Success,
        Failed,
    }

    public enum EnchantResult
    {
        Error,
        NotEnoughMoney,
        ItemEnchantError,
        None,
        NotEnoughEffect,
        Reset,
        Success,
        SuccessJackpot,
        SuccessChipOrAnother,
    }

    public enum ShopEnabledType : byte
    {
        Off,
        On,
        Hot,
        New,
        Event,
    }

    public enum BonusEtc : uint
    {
        None = 0,
        PCRoom = 1,
        PENPlus = 2,
        EXPPlus = 4,
        Twenty = 8,
        TwentyFive = 16, // 0x00000010
        Thirty = 32, // 0x00000020
    }

    public enum ClubAdminInviteResult : uint
    {
        Success,
        NotInClan,
        NoMatchFound,
    }

    public enum MailType
    {
        Private = 0,
        Club = 1,
        Group = 2,
        System = 4,
        Gift = 5,
        OpenGift = 8,
        Public = 9,
    }

    public enum DecompositionResult
    {
        Error,
        CannotDecomposition,
        NotEnoughDays,
        OnlyTenDays,
        NotEnoughPen,
        NotDecomposition,
        NotDecomposition2,
        ErrorDB,
        ErrorDB2,
        Success,
    }

    public enum CombinationResult
    {
        CantCombination = 0,
        NotEnoughPEN = 1,
        Success = 10,
    }
}
