﻿namespace Netsphere.Common.Configuration
{
    public class GameOptions
    {
        public bool EnableTutorial { get; set; }
        public int MaxLevel { get; set; }
        public int StartLevel { get; set; }
        public int StartPEN { get; set; }
        public int StartAP { get; set; }
        public int StartCoins1 { get; set; }
        public int StartCoins2 { get; set; }
        public NickRestrictionOptions NickRestrictions { get; set; }
        public SystemsOptions Systems { get; set; }
        public ClanOptions ClanOptions { get; set; }
        public int DurabilityLossPerDeath { get; set; }
        public int DurabilityLossPerMinute { get; set; }
        public DeathmatchOptions Deathmatch { get; set; }
        public TouchdownOptions Touchdown { get; set; }
        public BattleRoyalOptions BattleRoyal { get; set; }
        public CaptainOptions Captain { get; set; }
        public ChaserOptions Chaser { get; set; }

    }

    public class DeathmatchOptions
    {
        public ExperienceRateOptions ExperienceRates { get; set; }
        public int PointsPerKill { get; set; }
        public int PointsPerKillAssist { get; set; }
        public int PointsPerHealAssist { get; set; }
        public int PointsPerDeath { get; set; }
    }

    public class TouchdownOptions
    {
        public ExperienceRateOptions ExperienceRates { get; set; }
        public int PointsPerGoal { get; set; }
        public int PointsPerGoalAssist { get; set; }
        public int PointsPerOffense { get; set; }
        public int PointsPerOffenseAssist { get; set; }
        public int PointsPerDefense { get; set; }
        public int PointsPerDefenseAssist { get; set; }
        public int PointsPerFumbi { get; set; }
        public int PointsPerKill { get; set; }
        public int PointsPerKillAssist { get; set; }
        public int PointsPerHealAssist { get; set; }
        public int PointsPerDeath { get; set; }
    }

    public class BattleRoyalOptions
    {
        public ExperienceRateOptions ExperienceRates { get; set; }
        public int PointsPerKill { get; set; }
        public int PointsPerKillAssist { get; set; }
        public int PointsPerBonusKill { get; set; }
        public int PointsPerBonusAssist { get; set; }
        public int PointsPerDeath { get; set; }
    }

    public class ChaserOptions
    {
        public ExperienceRateOptions ExperienceRates { get; set; }
        public int PointsPerKill { get; set; }
        public int PointsPerSurvived { get; set; }
        public int PointsPerRoundWin { get; set; }
        public int PointsPerBonuskills { get; set; }
        
    }

    public class CaptainOptions
    {
        public ExperienceRateOptions ExperienceRates { get; set; }
        public int PointsPerKill { get; set; }
        public int PointsPerKillAssist { get; set; }
        public int PointsPerCaptainKill { get; set; }
        public int PointsPerRoundWin { get; set; }
        public int PointsPerSuicide { get; set; }
        public int PointsPerHeal { get; set; }
    }

    public class ExperienceRateOptions
    {
        public float ScoreFactor { get; set; }
        public float FirstPlaceBonus { get; set; }
        public float SecondPlaceBonus { get; set; }
        public float ThirdPlaceBonus { get; set; }
        public float PlayerCountFactor { get; set; }
        public float ExperiencePerMinute { get; set; }
    }
}
