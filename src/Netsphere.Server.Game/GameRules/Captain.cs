using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.GameRule;
using ProudNet.Hosting.Services;

namespace Netsphere.Server.Game.GameRules
{
    public class Captain : GameRuleBase
    {
        private const float BaseCaptainHealth = 500;
        private static readonly TimeSpan s_nextRoundWaitTime = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan s_roundTimeLimit = TimeSpan.FromMinutes(3);

        private readonly CaptainOptions _options;
        private readonly ISchedulerService _schedulerService;
        private CancellationTokenSource _roundEndedCancellationTokenSource;

        public override GameRule GameRule => GameRule.Captain;
        public override bool HasHalfTime => false;
        public override bool HasTimeLimit => false;
        public int CurrentRound => (int)TeamManager.Sum(x => x.Value.Score) + 1;
        public float AlphaHealth { get; private set; }
        public float BetaHealth { get; private set; }

        public event EventHandler RoundEnded;

        protected virtual void OnRoundEnded()
        {
            RoundEnded?.Invoke(this, EventArgs.Empty);
        }

        public Captain(GameRuleStateMachine stateMachine, IOptions<GameOptions> gameOptions,
            IOptions<CaptainOptions> options, ISchedulerService schedulerService)
            : base(stateMachine, gameOptions)
        {
            _options = options.Value;
            _schedulerService = schedulerService;
            StateMachine.GameStateChanged += OnGameStateChanged;
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

        protected override bool CanStartGame()
        {
            if (StateMachine.GameState != GameState.Waiting)
                return false;

            // Is atleast one player per team ready?
            var teams = TeamManager.Values;
            return teams.All(team => team.Players.Any(plr => plr.IsReady || Room.Master == plr));
        }

        protected override bool HasEnoughPlayers()
        {
            return TeamManager.Values.All(team => team.PlayersPlaying.Any());
        }

        protected override PlayerScore CreateScore(Player plr)
        {
            return new CaptainPlayerScore(_options);
        }

        protected override BriefingPlayer CreateBriefingPlayer(Player plr)
        {
            return new BriefingPlayerCaptain(plr);
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

        protected internal override void OnScoreKill(ScoreContext killer, ScoreContext assist, ScoreContext target,
            AttackAttribute attackAttribute)
        {
            if (_roundEndedCancellationTokenSource.IsCancellationRequested)
                return;

            if (target.IsSentry)
            {
                SendScoreKill(killer, assist, target, attackAttribute);
                return;
            }

            if (GetScore(target).IsCaptain)
            {
                GetScore(target).IsCaptain = false;
                GetScore(killer).CaptainKills++;
            }
            else
            {
                killer.Score.Kills++;
            }

            if (assist != null)
                GetScore(assist).KillAssists++;

            target.Score.Deaths++;
            SendScoreKill(killer, assist, target, attackAttribute);
            CheckCaptains();
        }

        protected internal override void OnScoreHeal(Player plr)
        {
            if (_roundEndedCancellationTokenSource.IsCancellationRequested)
                return;

            plr.Score.HealAssists++;
            SendScoreHeal(plr);
        }

        protected internal override void OnScoreSuicide(Player plr)
        {
            if (_roundEndedCancellationTokenSource.IsCancellationRequested)
                return;

            plr.Score.Deaths++;

            // Suicides with no score do not reduce your score
            if (plr.Score.GetTotalScore() > 0)
                plr.Score.Suicides++;

            GetScore(plr).IsCaptain = false;
            SendScoreSuicide(plr);
            CheckCaptains();
        }

        protected internal override void OnPlayerIntrude(Player plr)
        {
            plr.Session.Send(new CaptainCurrentRoundInfoAckMessage(
                CurrentRound,
                StateMachine.RoundTime
            ));
        }

        private void OnGameStateChanged(object sender, EventArgs e)
        {
            switch (StateMachine.GameState)
            {
                case GameState.Playing:
                    NewRound(this, null);
                    break;

                case GameState.Result:
                    EndRound();
                    break;
            }
        }

        protected int GetNumCaptains(TeamId teamId)
        {
            var team = TeamManager[teamId];
            return team?.PlayersPlaying.Count(x => GetScore(x).IsCaptain) ?? 0;
        }

        protected void CheckCaptains()
        {
            if (GetNumCaptains(TeamId.Alpha) == 0 || GetNumCaptains(TeamId.Beta) == 0)
                EndRound();
        }

        protected static CaptainPlayerScore GetScore(ScoreContext plr)
        {
            return (CaptainPlayerScore)plr.Score;
        }

        protected static CaptainPlayerScore GetScore(Player plr)
        {
            return (CaptainPlayerScore)plr.Score;
        }

        private void EndRound()
        {
            if (_roundEndedCancellationTokenSource.IsCancellationRequested)
                return;

            _roundEndedCancellationTokenSource.Cancel();
            var winnerTeam = GetNumCaptains(TeamId.Alpha) > GetNumCaptains(TeamId.Beta)
                ? TeamManager[TeamId.Alpha]
                : TeamManager[TeamId.Beta];

            winnerTeam.Score++;
            foreach (var plr in winnerTeam.PlayersPlaying)
                GetScore(plr).RoundsWon++;

            Room.Broadcast(new CaptainSubRoundWinAckMessage(3, winnerTeam.Id));
            OnRoundEnded();

            if (StateMachine.GameState != GameState.Playing)
                return;

            // Timelimit is round limit
            if (CurrentRound >= Room.Options.TimeLimit.Minutes || winnerTeam.Score >= Room.Options.ScoreLimit)
            {
                StateMachine.StartResult();
                return;
            }

            Room.Broadcast(new GameEventMessageAckMessage(
                GameEventMessage.NextRoundIn,
                (ulong)s_nextRoundWaitTime.TotalMilliseconds,
                0,
                0,
                ""
            ));
            _schedulerService.ScheduleAsync(NewRound, this, null, s_nextRoundWaitTime);
        }

        private static void NewRound(object ctx, object _)
        {
            Debug.Assert(ctx is Captain);
            var This = (Captain)ctx;

            if (This.StateMachine.GameState != GameState.Playing)
                return;

            This.StateMachine.ResetRoundTimer();
            This._roundEndedCancellationTokenSource = new CancellationTokenSource();
            var alpha = This.TeamManager[TeamId.Alpha];
            var beta = This.TeamManager[TeamId.Beta];
            var numAlpha = alpha.PlayersPlaying.Count();
            var numBeta = beta.PlayersPlaying.Count();
            This.AlphaHealth = BaseCaptainHealth;
            This.BetaHealth = BaseCaptainHealth;

            // Scale health based on team size
            // The total amount of health of a team is always equal
            // e.g. num alpha: 4 - num beta: 6 - total health beta: 6 * 500 = 3000 - health for alpha players: 3000 / 4 = 750
            // The scaled health then gets rounded to full hundreds based on if they are winning or losing
            // e.g. alpha is winning: 700 health - alpha is losing: 800 health
            if (numAlpha > numBeta)
            {
                This.BetaHealth = numAlpha * BaseCaptainHealth / numBeta;
                if (beta.Score > alpha.Score)
                    This.BetaHealth = MathF.Floor(This.BetaHealth / 100) * 100;
                else
                    This.BetaHealth = MathF.Ceiling(This.BetaHealth / 100) * 100;
            }
            else if (numBeta > numAlpha)
            {
                This.AlphaHealth = numBeta * BaseCaptainHealth / numAlpha;
                if (alpha.Score > beta.Score)
                    This.AlphaHealth = MathF.Floor(This.AlphaHealth / 100) * 100;
                else
                    This.AlphaHealth = MathF.Ceiling(This.AlphaHealth / 100) * 100;
            }

            foreach (var plr in This.TeamManager.PlayersPlaying)
            {
                GetScore(plr).IsCaptain = true;
                plr.Session.Send(new CaptainRoundCaptainLifeInfoAckMessage(
                    This.TeamManager.PlayersPlaying
                        .Select(x => new CaptainLifeDto(
                            x.Account.Id, x.Team.Id == TeamId.Alpha ? This.AlphaHealth : This.BetaHealth
                        ))
                        .ToArray()
                ));
                plr.Session.Send(new GameEventMessageAckMessage(GameEventMessage.ResetRound, 0, 0, 0, string.Empty));
                plr.Session.Send(new CaptainCurrentRoundInfoAckMessage(
                    This.CurrentRound,
                    This.StateMachine.RoundTime
                ));
            }

            This._schedulerService.ScheduleAsync(
                (x, __) => ((Captain)x).EndRound(),
                This,
                null,
                s_roundTimeLimit,
                This._roundEndedCancellationTokenSource.Token
            );
        }
    }

    public class BriefingPlayerCaptain : BriefingPlayer
    {
        public uint Kills { get; set; }
        public uint KillAssists { get; set; }
        public uint CaptainKills { get; set; }
        public uint HealAssists { get; set; }
        public uint Deaths { get; set; }
        public uint RoundsWon { get; set; }
        public bool IsCaptain { get; set; }

        public BriefingPlayerCaptain(Player plr)
        {
            AccountId = plr.Account.Id;
            Experience = plr.TotalExperience;
            TeamId = plr.Team.Id;
            State = plr.State;
            Mode = plr.Mode;
            IsReady = plr.IsReady;
            TotalScore = plr.Score.GetTotalScore();

            var score = (CaptainPlayerScore)plr.Score;
            Kills = score.Kills;
            KillAssists = score.KillAssists;
            CaptainKills = score.CaptainKills;
            Deaths = score.Deaths;
            HealAssists = score.HealAssists;
            RoundsWon = score.RoundsWon;
            IsCaptain = score.IsCaptain;
        }

        public override void Serialize(BinaryWriter w)
        {
            base.Serialize(w);

            w.Write(Kills);
            w.Write(KillAssists);
            w.Write(HealAssists);
            w.Write(0);
            w.Write(0);
            w.Write(0);
            w.Write(0);
            w.Write(CaptainKills);
            w.Write(RoundsWon);
            w.Write(Deaths);
            w.Write(IsCaptain);
            w.Write(0);
        }
    }

    public class CaptainPlayerScore : PlayerScore
    {
        private readonly CaptainOptions _options;

        public uint CaptainKills { get; set; }
        public bool IsCaptain { get; set; }
        public uint RoundsWon { get; set; }

        public CaptainPlayerScore(CaptainOptions options)
        {
            _options = options;
        }

        public override uint GetTotalScore()
        {
            return (uint)(Kills * _options.PointsPerKill +
                          KillAssists * _options.PointsPerKillAssist +
                          CaptainKills * _options.PointsPerCaptainKill +
                          RoundsWon * _options.PointsPerRoundWin +
                          Suicides * _options.PointsPerSuicide);
        }

        public override void Reset()
        {
            base.Reset();
            CaptainKills = 0;
            RoundsWon = 0;
            IsCaptain = false;
        }
    }
}
