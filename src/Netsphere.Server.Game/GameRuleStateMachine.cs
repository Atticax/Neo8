using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Netsphere.Network.Message.GameRule;
using ProudNet.Hosting.Services;
using Stateless;

namespace Netsphere.Server.Game
{
    public class GameRuleStateMachine
    {
        private static readonly TimeSpan s_preHalfTimeWaitTime = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan s_preResultWaitTime = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan s_halfTimeWaitTime = TimeSpan.FromSeconds(25);
        private static readonly TimeSpan s_resultWaitTime = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan s_startingWaitTime = TimeSpan.FromSeconds(5);
        private static readonly EventPipeline<ScheduleTriggerHookEventArgs> s_scheduleTriggerHook =
            new EventPipeline<ScheduleTriggerHookEventArgs>();

        private readonly ISchedulerService _schedulerService;
        private readonly StateMachine<GameRuleState, GameRuleStateTrigger> _stateMachine;
        private GameRuleBase _gameRule;
        private Func<bool> _canStartGame;
        private bool _hasHalfTime;
        private bool _hasTimeLimit;
        private DateTimeOffset _gameStartTime;
        private DateTimeOffset _roundStartTime;
        private CancellationTokenSource _gameEnded;

        public event EventHandler GameStateChanged;
        public event EventHandler TimeStateChanged;
        public static event EventPipeline<ScheduleTriggerHookEventArgs>.SubscriberDelegate ScheduleTriggerHook
        {
            add => s_scheduleTriggerHook.Subscribe(value);
            remove => s_scheduleTriggerHook.Unsubscribe(value);
        }

        protected virtual void OnGameStateChanged()
        {
            GameStateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnTimeStateChanged()
        {
            TimeStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public GameState GameState => GetGameState();
        public GameTimeState TimeState => GetTimeState();
        public TimeSpan GameTime => _gameStartTime == default ? TimeSpan.Zero : DateTimeOffset.Now - _gameStartTime;
        public TimeSpan RoundTime => _roundStartTime == default ? TimeSpan.Zero : DateTimeOffset.Now - _roundStartTime;

        public GameRuleStateMachine(ISchedulerService schedulerService)
        {
            _schedulerService = schedulerService;
            _stateMachine = new StateMachine<GameRuleState, GameRuleStateTrigger>(GameRuleState.Waiting);
            _stateMachine.OnTransitioned(OnTransition);
        }

        public void Initialize(GameRuleBase gameRule, Func<bool> canStartGame, bool hasHalfTime, bool hasTimeLimit)
        {
            _gameRule = gameRule;
            _canStartGame = canStartGame;
            _hasHalfTime = hasHalfTime;
            _hasTimeLimit = hasTimeLimit;

            _stateMachine.Configure(GameRuleState.Waiting)
                .PermitIf(GameRuleStateTrigger.StartGame, GameRuleState.Loading, _canStartGame);

            _stateMachine.Configure(GameRuleState.Loading)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.StartGame, GameRuleState.Starting);

            _stateMachine.Configure(GameRuleState.Starting)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.StartGame, GameRuleState.FirstHalf);

            var firstHalfStateMachine = _stateMachine.Configure(GameRuleState.FirstHalf)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.StartResult, GameRuleState.EnteringResult);

            if (hasHalfTime)
            {
                firstHalfStateMachine.Permit(GameRuleStateTrigger.StartHalfTime, GameRuleState.EnteringHalfTime);

                _stateMachine.Configure(GameRuleState.EnteringHalfTime)
                    .SubstateOf(GameRuleState.Playing)
                    .Permit(GameRuleStateTrigger.StartHalfTime, GameRuleState.HalfTime)
                    .Permit(GameRuleStateTrigger.StartResult, GameRuleState.EnteringResult);

                _stateMachine.Configure(GameRuleState.HalfTime)
                    .SubstateOf(GameRuleState.Playing)
                    .Permit(GameRuleStateTrigger.StartSecondHalf, GameRuleState.SecondHalf)
                    .Permit(GameRuleStateTrigger.StartResult, GameRuleState.EnteringResult);

                _stateMachine.Configure(GameRuleState.SecondHalf)
                    .SubstateOf(GameRuleState.Playing)
                    .Permit(GameRuleStateTrigger.StartResult, GameRuleState.EnteringResult);
            }

            _stateMachine.Configure(GameRuleState.EnteringResult)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.StartResult, GameRuleState.Result);

            _stateMachine.Configure(GameRuleState.Result)
                .SubstateOf(GameRuleState.Playing)
                .Permit(GameRuleStateTrigger.EndGame, GameRuleState.Waiting);
        }

        public bool StartGame()
        {
            if (!_stateMachine.CanFire(GameRuleStateTrigger.StartGame))
                return false;

            _stateMachine.Fire(GameRuleStateTrigger.StartGame);
            return true;
        }

        public bool StartHalfTime()
        {
            if (!_stateMachine.CanFire(GameRuleStateTrigger.StartHalfTime))
                return false;

            _stateMachine.Fire(GameRuleStateTrigger.StartHalfTime);
            return true;
        }

        public bool StartResult()
        {
            if (!_stateMachine.CanFire(GameRuleStateTrigger.StartResult))
                return false;

            _stateMachine.Fire(GameRuleStateTrigger.StartResult);
            return true;
        }

        public void ResetRoundTimer()
        {
            _roundStartTime = DateTimeOffset.Now;
        }

        private GameState GetGameState()
        {
            if (_stateMachine.IsInState(GameRuleState.Waiting))
                return GameState.Waiting;

            if (_stateMachine.IsInState(GameRuleState.Loading) || _stateMachine.IsInState(GameRuleState.Starting))
                return GameState.Loading;

            if (_stateMachine.IsInState(GameRuleState.Result) ||
                _stateMachine.IsInState(GameRuleState.EnteringResult))
                return GameState.Result;

            if (_stateMachine.IsInState(GameRuleState.Playing))
                return GameState.Playing;

            Debug.Assert(false, "Invalid state machine - THIS SHOULD NEVER HAPPEN");
            return default;
        }

        private GameTimeState GetTimeState()
        {
            if (_stateMachine.IsInState(GameRuleState.HalfTime) ||
                _stateMachine.IsInState(GameRuleState.EnteringHalfTime))
                return GameTimeState.HalfTime;

            if (_stateMachine.IsInState(GameRuleState.SecondHalf))
                return GameTimeState.SecondHalf;

            return GameTimeState.FirstHalf;
        }

        private void OnTransition(StateMachine<GameRuleState, GameRuleStateTrigger>.Transition transition)
        {
            var room = _gameRule.Room;
            _roundStartTime = DateTimeOffset.Now;

            switch (transition.Destination)
            {
                case GameRuleState.Loading:
                    _gameEnded = new CancellationTokenSource();
                    foreach (var team in room.TeamManager.Values)
                        team.Score = 0;

                    foreach (var plr in room.Players.Values)
                    {
                        if (!plr.IsReady && plr != room.Master)
                            continue;

                        for (var i = 0; i < plr.CharacterStartPlayTime.Length; ++i)
                            plr.CharacterStartPlayTime[i] = default;

                        plr.IsLoading = true;
                        plr.IsReady = false;
                        plr.Score.Reset();
                        plr.State = PlayerState.Waiting;

                        plr.Session.Send(new RoomGameLoadingAckMessage());
                        plr.Session.Send(new RoomBeginRoundAckMessage());
                    }

                    room.Broadcast(new GameChangeStateAckMessage(GameState.Loading));
                    OnGameStateChanged();
                    break;

                case GameRuleState.Starting:
                    foreach (var plr in room.Players.Values.Where(x => x.State == PlayerState.Waiting))
                        plr.Session.Send(new RoomGamePlayCountDownAckMessage(s_startingWaitTime));

                    // The client starts the countdown after a second
                    ScheduleTrigger(GameRuleStateTrigger.StartGame, s_startingWaitTime.Add(TimeSpan.FromSeconds(1)));
                    break;

                case GameRuleState.EnteringHalfTime:
                    ScheduleTrigger(GameRuleStateTrigger.StartHalfTime, s_preHalfTimeWaitTime);
                    AnnounceHalfTime();
                    OnTimeStateChanged();
                    break;

                case GameRuleState.EnteringResult:
                    ScheduleTrigger(GameRuleStateTrigger.StartResult, s_preResultWaitTime);
                    AnnounceResult();
                    OnGameStateChanged();
                    break;

                case GameRuleState.FirstHalf:
                    _gameStartTime = DateTimeOffset.Now;

                    foreach (var plr in room.Players.Values)
                    {
                        if (plr.State != PlayerState.Waiting)
                            continue;

                        plr.CharacterStartPlayTime[plr.CharacterManager.CurrentSlot] = DateTimeOffset.Now;
                        plr.StartPlayTime = DateTimeOffset.Now;
                        plr.State = plr.Mode == PlayerGameMode.Normal
                            ? PlayerState.Alive
                            : PlayerState.Spectating;

                        plr.Session.Send(new RoomGameStartAckMessage());
                    }

                    room.Broadcast(new GameChangeStateAckMessage(GameState.Playing));

                    if (_gameRule.HasHalfTime)
                        room.Broadcast(new GameChangeSubStateAckMessage(GameTimeState.FirstHalf));

                    room.BroadcastBriefing();

                    if (_hasTimeLimit)
                    {
                        var delay = _hasHalfTime
                            ? TimeSpan.FromSeconds(room.Options.TimeLimit.TotalSeconds / 2)
                            : room.Options.TimeLimit;
                        ScheduleTrigger(_hasHalfTime ? GameRuleStateTrigger.StartHalfTime : GameRuleStateTrigger.StartResult,
                            delay);
                    }

                    OnTimeStateChanged();
                    OnGameStateChanged();
                    break;

                case GameRuleState.HalfTime:
                    ScheduleTrigger(GameRuleStateTrigger.StartSecondHalf, s_halfTimeWaitTime);
                    room.Broadcast(new GameChangeSubStateAckMessage(GameTimeState.HalfTime));
                    break;

                case GameRuleState.SecondHalf:
                    if (_hasTimeLimit)
                    {
                        ScheduleTrigger(
                            GameRuleStateTrigger.StartResult,
                            TimeSpan.FromMinutes(room.Options.TimeLimit.TotalMinutes / 2)
                        );
                    }

                    room.Broadcast(new GameChangeSubStateAckMessage(GameTimeState.SecondHalf));
                    OnTimeStateChanged();
                    break;

                case GameRuleState.Result:
                    ScheduleTrigger(GameRuleStateTrigger.EndGame, s_resultWaitTime);

                    foreach (var plr in room.Players.Values.Where(plr => plr.State != PlayerState.Lobby))
                        plr.State = PlayerState.Waiting;

                    room.Broadcast(new GameChangeStateAckMessage(GameState.Result));
                    _gameRule.OnResult();
                    OnGameStateChanged();
                    break;

                case GameRuleState.Waiting:
                    _gameEnded.Cancel();
                    foreach (var plr in room.Players.Values.Where(plr => plr.State != PlayerState.Lobby))
                        plr.State = PlayerState.Lobby;

                    room.Broadcast(new GameChangeStateAckMessage(GameState.Waiting));
                    room.BroadcastBriefing();
                    OnGameStateChanged();
                    break;
            }
        }

        private void ScheduleTrigger(GameRuleStateTrigger trigger, TimeSpan delay)
        {
            var eventArgs = new ScheduleTriggerHookEventArgs(this, trigger, delay);
            s_scheduleTriggerHook.Invoke(eventArgs);
            if (eventArgs.Cancel)
                return;

            trigger = eventArgs.Trigger;
            delay = eventArgs.Delay;

            _schedulerService.ScheduleAsync((ctx, state) =>
            {
                var This = (GameRuleStateMachine)ctx;
                var parameter = (GameRuleStateTrigger)state;

                if (This._stateMachine.CanFire(parameter))
                    This._stateMachine.Fire(parameter);
            }, this, trigger, delay, _gameEnded.Token);
        }

        private void AnnounceHalfTime(bool isFirst = true)
        {
            if (isFirst)
            {
                _gameRule.Room.Broadcast(new GameEventMessageAckMessage(
                    GameEventMessage.HalfTimeIn, 2, 0, 0,
                    Math.Round((s_preHalfTimeWaitTime - RoundTime).TotalSeconds, 0).ToString("0")
                ));
            }

            _schedulerService.ScheduleAsync((ctx, _) =>
            {
                var This = (GameRuleStateMachine)ctx;
                if (!This._stateMachine.IsInState(GameRuleState.EnteringHalfTime))
                    return;

                This._gameRule.Room.Broadcast(new GameEventMessageAckMessage(
                    GameEventMessage.HalfTimeIn, 2, 0, 0,
                    Math.Round((s_preHalfTimeWaitTime - This.RoundTime).TotalSeconds, 0).ToString("0")));

                This.AnnounceHalfTime(false);
            }, this, null, TimeSpan.FromSeconds(1));
        }

        private void AnnounceResult(bool isFirst = true)
        {
            if (isFirst)
            {
                _gameRule.Room.Broadcast(new GameEventMessageAckMessage(
                    GameEventMessage.ResultIn, 3, 0, 0,
                    (int)Math.Round((s_preResultWaitTime - RoundTime).TotalSeconds, 0) + " second(s)"
                ));
            }

            _schedulerService.ScheduleAsync((ctx, _) =>
            {
                var This = (GameRuleStateMachine)ctx;
                if (!This._stateMachine.IsInState(GameRuleState.EnteringResult))
                    return;

                This._gameRule.Room.Broadcast(new GameEventMessageAckMessage(
                    GameEventMessage.ResultIn, 3, 0, 0,
                    (int)Math.Round((s_preResultWaitTime - This.RoundTime).TotalSeconds, 0) + " second(s)"
                ));

                This.AnnounceResult(false);
            }, this, null, TimeSpan.FromSeconds(1));
        }
    }
}
