using System;

namespace Netsphere.Server.Game
{
    public abstract class PlayerScore
    {
        public LuckyContext LuckyContextPEN { get; set; }
        public LuckyContext LuckyContextEXP { get; set; }
        public uint Kills { get; set; }
        public uint KillAssists { get; set; }
        public uint HealAssists { get; set; }

        public uint Survived { get; set; }
        public uint Suicides { get; set; }
        public uint Deaths { get; set; }

        public uint Roundswon { get; set; }

        public uint Bonuskills { get; set; }

        public abstract uint GetTotalScore();

        public virtual void Reset()
        {
            LuckyContextPEN = (LuckyContext)null;
            LuckyContextEXP = (LuckyContext)null;
            Kills = 0;
            KillAssists = 0;
            HealAssists = 0;
            Bonuskills = 0;
            Roundswon = 0;
            Survived = 0;
            Suicides = 0;
            Deaths = 0;
        }
    }

    public class LuckyContext
    {
        public Player Player { get; }

        public int LuckyType { get; }

        public DateTimeOffset TimeOffset { get; }

        public int[] LuckyShotRange { get; }

        public LuckyContext(Player plr, int luckyType, DateTimeOffset timeOffset)
        {
            Player = plr;
            LuckyType = luckyType;
            TimeOffset = timeOffset;
            LuckyShotRange = new int[9]
            {
                10,
                15,
                20,
                25,
                30,
                35,
                40,
                45,
                50
            };
        }

        public uint GetLucky(Random ran) => (uint)LuckyShotRange[ran.Next(0, LuckyShotRange.Length)];
    }
}
