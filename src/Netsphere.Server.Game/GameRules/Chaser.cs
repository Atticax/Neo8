using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;

namespace Netsphere.Server.Game.GameRules
{
    public class Chaser : GameRuleBase
    {
        

        private readonly ChaserOptions _options;

        public override GameRule GameRule => GameRule.Chaser;

        public override bool HasHalfTime => false;

        public override bool HasTimeLimit => true;

        public Chaser(GameRuleStateMachine stateMachine, IOptions<GameOptions> gameOptions,
        IOptions<ChaserOptions> options)
        : base(stateMachine, gameOptions)
        {
            _options = options.Value;
        }

        public override void Initialize(Room room)
        {
            base.Initialize(room);

            var playersPerTeam = Room.Options.PlayerLimit / 2;
            var spectatorsPerTeam = Room.Options.SpectatorLimit / 2;
            Room.TeamManager.Add(TeamId.Alpha, playersPerTeam, spectatorsPerTeam);
            Room.TeamManager.Add(TeamId.Beta, playersPerTeam, spectatorsPerTeam);
        }

        public override void Cleanup()
        {
            base.Cleanup();

            Room.TeamManager.Remove(TeamId.Alpha);
            Room.TeamManager.Remove(TeamId.Beta);
        }



        protected override (uint baseGain, uint bonusGain) CalculateExperienceGained(Player plr)
        {
            var experienceRates = _options.ExperienceRates;
            var place = 1;

            var plrs = TeamManager.Players
                .Where(x => x.State == PlayerState.Waiting && x.Mode == PlayerGameMode.Normal)
                .ToArray();

            foreach (var x in plrs.OrderByDescending(x => x.Score.GetTotalScore()))
            {
                if (x == plr)
                    break;

                place++;
                if (place > 3)
                    break;
            }

            var rankingBonus = 0f;
            switch (place)
            {
                case 1:
                    rankingBonus = experienceRates.FirstPlaceBonus;
                    break;

                case 2:
                    rankingBonus = experienceRates.SecondPlaceBonus;
                    break;

                case 3:
                    rankingBonus = experienceRates.ThirdPlaceBonus;
                    break;
            }

            var experienceGained = (uint)(plr.Score.GetTotalScore() * experienceRates.ScoreFactor +
                                          rankingBonus +
                                          plrs.Length * experienceRates.PlayerCountFactor +
                                          plr.GetCurrentPlayTime().TotalMinutes * experienceRates.ExperiencePerMinute);

            return (experienceGained, 0);
        }

        protected override (uint baseGain, uint bonusGain) CalculatePENGained(Player plr)
        {
            return (0, 0);
        }



        protected override bool CanStartGame() //future 4 players
        {
            if (StateMachine.GameState != GameState.Waiting)
                return false;

            // Is atleast one player per team ready?
            var teams = TeamManager.Values;
            return teams.All(team => team.Players.Any(plr => plr.IsReady || Room.Master == plr));
        }

        protected override BriefingPlayer CreateBriefingPlayer(Player plr)
        {
            return new BriefingPlayerChaser(plr);
        }

        protected override PlayerScore CreateScore(Player plr)
        {
            return new ChaserPlayerScore(_options);
        }

        protected override bool HasEnoughPlayers()
        {
            return TeamManager.Values.All(team => team.PlayersPlaying.Any());
        }

        
        public class BriefingPlayerChaser : BriefingPlayer
        {
            public uint Kills { get; set; }
            public uint KillAssists { get; set; }
            public uint Bonuskills { get; set; }
            public uint Deaths { get; set; }
            public uint Survived { get; set; }
            public uint Roundswon { get; set; }

            public BriefingPlayerChaser(Player plr)
            {
                AccountId = plr.Account.Id;
                Experience = plr.TotalExperience;
                TeamId = plr.Team.Id;
                State = plr.State;
                Mode = plr.Mode;
                IsReady = plr.IsReady;
                TotalScore = plr.Score.GetTotalScore();

                Kills = plr.Score.Kills;
                KillAssists = plr.Score.KillAssists;
                Bonuskills = plr.Score.Bonuskills;
                Roundswon = plr.Score.Roundswon;
                Deaths = plr.Score.Deaths;
            }

            public override void Serialize(BinaryWriter w)
            {
                base.Serialize(w);

                w.Write(Kills);
                w.Write(KillAssists);
                w.Write(Bonuskills);
                w.Write(Deaths);
                w.Write(Roundswon);
                w.Write(Survived);
                w.Write(0);
            }
        }

        public class ChaserPlayerScore : PlayerScore
        {
            private readonly ChaserOptions _options;

            public ChaserPlayerScore(ChaserOptions options)
            {
                _options = options;
            }

            public override uint GetTotalScore()
            {
                return (uint)Kills * 2 + Bonuskills * 4 + Roundswon * 5 + Survived * 10; //future reverse unk 17 & 18
            }
        }
    }
}
