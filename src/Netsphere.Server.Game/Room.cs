using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlubLib.Collections.Concurrent;
using ExpressMapper.Extensions;
using Foundatio.Messaging;
using Logging;
using Netsphere.Common;
using Netsphere.Common.Messaging;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;
using ProudNet.Hosting.Services;

namespace Netsphere.Server.Game
{
    public class Room
    {
        private static readonly EventPipeline<RoomJoinHookEventArgs> s_joinHook =
            new EventPipeline<RoomJoinHookEventArgs>();
        private static readonly EventPipeline<RoomChangeHookEventArgs> s_changeRulesHook =
            new EventPipeline<RoomChangeHookEventArgs>();

        private ILogger _logger;
        private readonly GameRuleResolver _gameRuleResolver;
        private readonly GameDataService _gameDataService;
        private readonly ISchedulerService _schedulerService;
        private readonly IMessageBus _messageBus;
        private readonly ConcurrentDictionary<ulong, Player> _players;
        private readonly ConcurrentDictionary<ulong, object> _kickedPlayers;
        private readonly CounterRecycler _idRecycler;

        public RoomManager RoomManager { get; internal set; }
        public IReadOnlyDictionary<ulong, Player> Players => _players;
        public uint Id { get; internal set; }
        public RoomCreationOptions Options { get; internal set; }
        public DateTime TimeCreated { get; }
        public Player Master { get; private set; }
        public Player Host { get; private set; }
        public bool IsChangingRules { get; private set; }
        public TeamManager TeamManager { get; }
        public MapInfo Map { get; private set; }
        public GameRuleBase GameRule { get; private set; }

        public event EventHandler<RoomPlayerEventArgs> PlayerJoining;
        public event EventHandler<RoomPlayerEventArgs> PlayerJoined;
        public event EventHandler<RoomPlayerEventArgs> PlayerLeft;
        public event EventHandler<RoomEventArgs> OptionsChanged;
        public static event EventPipeline<RoomJoinHookEventArgs>.SubscriberDelegate JoinHook
        {
            add => s_joinHook.Subscribe(value);
            remove => s_joinHook.Unsubscribe(value);
        }
        public static event EventPipeline<RoomChangeHookEventArgs>.SubscriberDelegate ChangeRulesHook
        {
            add => s_changeRulesHook.Subscribe(value);
            remove => s_changeRulesHook.Unsubscribe(value);
        }

        protected virtual void OnPlayerJoining(Player plr)
        {
            PlayerJoining?.Invoke(this, new RoomPlayerEventArgs(this, plr));
            RoomManager.Channel.Broadcast(new RoomChangeRoomInfoAck2Message(this.Map<Room, Room2Dto>()));
            _messageBus.PublishAsync(new PlayerUpdateMessage(
                plr.Account.Id, plr.TotalExperience, plr.Level, Id, plr.Nametag, TeamId.Neutral
            ));
        }

        internal virtual void OnPlayerJoined(Player plr)
        {
            plr.OnRoomJoined(this);
            PlayerJoined?.Invoke(this, new RoomPlayerEventArgs(this, plr));

            var team = TeamId.Neutral;
            if (!plr.IsInGMMode)
                team = plr.Team?.Id ?? TeamId.Neutral;

            _messageBus.PublishAsync(new PlayerUpdateMessage(
                plr.Account.Id, plr.TotalExperience, plr.Level, Id, plr.Nametag, team
            ));
        }

        protected virtual void OnPlayerLeft(Player plr)
        {
            plr.OnRoomLeft(this);
            PlayerLeft?.Invoke(this, new RoomPlayerEventArgs(this, plr));
            RoomManager.Channel.Broadcast(new RoomChangeRoomInfoAck2Message(this.Map<Room, Room2Dto>()));
            _messageBus.PublishAsync(new PlayerUpdateMessage(
                plr.Account.Id, plr.TotalExperience, plr.Level, 0, plr.Nametag, TeamId.Neutral
            ));
        }

        protected virtual void OnOptionsChanged()
        {
            OptionsChanged?.Invoke(this, new RoomEventArgs(this));
            Broadcast(new RoomChangeRuleAckMessage(Options.Map<RoomCreationOptions, ChangeRuleDto>()));
            RoomManager.Channel.Broadcast(new RoomChangeRoomInfoAck2Message(this.Map<Room, Room2Dto>()));
        }

        public Room(ILogger<Room> logger, GameRuleResolver gameRuleResolver, GameDataService gameDataService,
            ISchedulerService schedulerService, IMessageBus messageBus)
        {
            _logger = logger;
            _gameRuleResolver = gameRuleResolver;
            _gameDataService = gameDataService;
            _schedulerService = schedulerService;
            _messageBus = messageBus;
            TimeCreated = DateTime.Now;
            _players = new ConcurrentDictionary<ulong, Player>();
            _kickedPlayers = new ConcurrentDictionary<ulong, object>();
            _idRecycler = new CounterRecycler(3);
            TeamManager = new TeamManager(this);
        }

        internal void Initialize(RoomManager roomManager, uint id, RoomCreationOptions options)
        {
            RoomManager = roomManager;
            Id = id;
            Options = options;
            Map = _gameDataService.Maps.First(x => x.Id == options.Map);
            GameRule = _gameRuleResolver.CreateGameRule(Options);
            GameRule.Initialize(this);
            GameRule.StateMachine.GameStateChanged += OnGameStateChanged;
            TeamManager.PlayerTeamChanged += OnPlayerTeamChanged;

            _logger = _logger.ForContext(
                ("ChannelId", RoomManager.Channel.Id),
                ("RoomId", Id)
            );
        }

        public RoomJoinError Join(Player plr)
        {
            var eventArgs = new RoomJoinHookEventArgs(this, plr);
            s_joinHook.Invoke(eventArgs);
            if (eventArgs.Error != RoomJoinError.OK)
                return eventArgs.Error;

            if (plr.Room != null)
                return RoomJoinError.AlreadyInRoom;

            if (_players.Count(x => !x.Value.IsInGMMode) >= Options.PlayerLimit + Options.SpectatorLimit &&
                !plr.IsInGMMode)
            {
                return RoomJoinError.RoomFull;
            }

            if (_kickedPlayers.ContainsKey(plr.Account.Id) && !plr.IsInGMMode)
                return RoomJoinError.KickedPreviously;

            if (IsChangingRules)
                return RoomJoinError.ChangingRules;

            if (plr.IsInGMMode)
            {
                plr.Mode = PlayerGameMode.Spectate;
            }
            else
            {
                if (TeamManager.Any(team => team.Value.Players.Count(x => !x.IsInGMMode) < team.Value.PlayerLimit))
                {
                    plr.Mode = PlayerGameMode.Normal;
                }
                else
                {
                    if (TeamManager.Any(team => team.Value.Spectators.Count(x => !x.IsInGMMode) < team.Value.SpectatorLimit))
                        plr.Mode = PlayerGameMode.Spectate;
                    else
                        return RoomJoinError.RoomFull;
                }
            }

            plr.Slot = (byte)_idRecycler.GetId();
            plr.State = PlayerState.Lobby;
            plr.IsReady = false;

            if (plr.IsInGMMode)
            {
                TeamManager.Values.First().Join(plr);
            }
            else
            {
                if (TeamManager.Join(plr) != TeamJoinError.OK)
                    return RoomJoinError.RoomFull;
            }

            _players.TryAdd(plr.Account.Id, plr);
            plr.Room = this;
            plr.IsConnectingToRoom = true;
            plr.PeerId = null;

            if (Master == null)
            {
                ChangeMaster(plr);
                ChangeHost(plr);
            }

            Broadcast(new RoomEnterPlayerInfoAckMessage(plr.Map<Player, RoomPlayerDto>()));
            plr.Session.Send(new RoomEnterRoomInfoAck2Message(this.Map<Room, EnterRoomInfo2Dto>()));
            plr.Session.Send(new RoomCurrentCharacterSlotAckMessage(0, plr.Slot));
            plr.Session.Send(new RoomPlayerInfoListForEnterPlayerAckMessage(
                _players.Values.Select(x => x.Map<Player, RoomPlayerDto>()).ToArray())
            );
            OnPlayerJoining(plr);

            return RoomJoinError.OK;
        }

        public void Leave(Player plr, RoomLeaveReason roomLeaveReason = RoomLeaveReason.Left)
        {
            if (plr.Room != this)
                return;

            Broadcast(new RoomLeavePlayerAckMessage(plr.Account.Id, plr.Account.Nickname, roomLeaveReason));

            if (roomLeaveReason == RoomLeaveReason.Kicked ||
                roomLeaveReason == RoomLeaveReason.ModeratorKick ||
                roomLeaveReason == RoomLeaveReason.VoteKick)
                _kickedPlayers.TryAdd(plr.Account.Id, null);

            plr.Team.Leave(plr);
            _players.Remove(plr.Account.Id);
            _idRecycler.Return(plr.Slot);
            plr.Room = null;
            plr.PeerId = null;
            plr.IsReady = false;

            plr.Session.Send(new RoomLeavePlayerInfoAckMessage(plr.Account.Id));

            OnPlayerLeft(plr);

            if (_players.Count > 0)
            {
                if (Master == plr)
                {
                    // Prioritize players that are ready
                    // This makes it possible for players to give master to a specific player
                    var newMaster = GetPlayerWithLowestPing(Players.Values.Where(x => x.IsReady))
                                    ?? GetPlayerWithLowestPing();
                    ChangeMaster(newMaster);
                    ChangeHost(newMaster);
                }
            }
            else
            {
                RoomManager.Remove(this);
            }
        }

        public void ChangeMaster(Player plr)
        {
            if (plr.Room != this || Master == plr)
                return;

            Master = plr;
            Master.IsReady = false;
            Broadcast(new RoomChangeMasterAckMessage(Master.Account.Id));
        }

        public void ChangeHost(Player plr)
        {
            if (plr.Room != this || Host == plr)
                return;

            _logger.Debug("Changing host to {Nickname} - Ping:{Ping} ms", plr.Account.Nickname, plr.Session.UnreliablePing);
            Host = plr;
            Broadcast(new RoomChangeRefereeAckMessage(Host.Account.Id));
        }

        public RoomChangeRulesError ChangeRules(ChangeRule2Dto options)
        {
            if (IsChangingRules)
                return RoomChangeRulesError.AlreadyChangingRules;

            var eventArgs = new RoomChangeHookEventArgs(this, options);
            s_changeRulesHook.Invoke(eventArgs);
            if (eventArgs.Error != RoomChangeRulesError.OK)
                return eventArgs.Error;

            if (!_gameRuleResolver.HasGameRule(new RoomCreationOptions
            {
                Name = options.Name,
                GameRule = options.GameRule,
                Map = options.Map,
                PlayerLimit = options.PlayerLimit,
                SpectatorLimit = options.SpectatorLimit,
                TimeLimit = options.TimeLimit,
                ScoreLimit = options.ScoreLimit,
                Password = options.Password,
                EquipLimit = options.ItemLimit,
                IsFriendly = options.Settings.HasFlag(RoomSettings.IsFriendly)
            }))
            {
                return RoomChangeRulesError.InvalidGameRule;
            }

            var map = _gameDataService.Maps.FirstOrDefault(x => x.Id == options.Map);
            if (map == null)
                return RoomChangeRulesError.InvalidMap;

            if (map.GameRule != options.GameRule)
                return RoomChangeRulesError.InvalidGameRule;

            if (options.PlayerLimit + options.SpectatorLimit < Players.Count)
                return RoomChangeRulesError.PlayerLimitTooLow;

            IsChangingRules = true;

            Options.Name = options.Name;
            Options.GameRule = options.GameRule;
            Options.Map = options.Map;
            Options.PlayerLimit = options.PlayerLimit;
            Options.SpectatorLimit = options.SpectatorLimit;
            Options.TimeLimit = options.TimeLimit;
            Options.ScoreLimit = options.ScoreLimit;
            Options.Password = options.Password;
            Options.Name = options.Name;
            Options.EquipLimit = options.ItemLimit;
            Options.IsFriendly = options.Settings.HasFlag(RoomSettings.IsFriendly);
            Broadcast(new RoomChangeRuleNotifyAck2Message(Options.Map<RoomCreationOptions, ChangeRule2Dto>()));

            GameRule.Cleanup();
            Map = _gameDataService.Maps.First(x => x.Id == Options.Map);
            GameRule = _gameRuleResolver.CreateGameRule(Options);
            GameRule.Initialize(this);
            GameRule.StateMachine.GameStateChanged += OnGameStateChanged;

            foreach (var plr in Players.Values)
            {
                // Move spectators to normal when spectators are disabled
                if (plr.Mode == PlayerGameMode.Spectate && !Options.IsSpectatingEnabled)
                    plr.Mode = PlayerGameMode.Normal;

                // Try to rejoin the old team first then fallback to default join
                var team = TeamManager[plr.Team.Id];
                TeamJoinError error;
                if (team != null)
                {
                    error = team.Join(plr);
                    if (error == TeamJoinError.OK)
                        continue;
                }

                // Original team was full
                // Fallback to default join and try to join another team
                error = TeamManager.Join(plr);
                if (error != TeamJoinError.OK && plr.Mode == PlayerGameMode.Spectate)
                {
                    // Should only happen when the spectator limit got reduced
                    // Move spectators to normal when spectator slots are filled
                    plr.Mode = PlayerGameMode.Normal;
                    TeamManager.Join(plr);
                }
            }

            BroadcastBriefing();
            IsChangingRules = false;
            OnOptionsChanged();
            return RoomChangeRulesError.OK;
        }

        public uint GetAveragePing()
        {
            var players = TeamManager.SelectMany(t => t.Value.Values).ToArray();
            return (uint)(players.Sum(plr => plr.Session.UnreliablePing) / players.Length);
        }

        public void Broadcast(IGameMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        public void Broadcast(IGameRuleMessage message)
        {
            foreach (var plr in _players.Values)
                plr.Session.Send(message);
        }

        public Briefing GetBriefing()
        {
            var briefing = GameRule.CreateBriefing();
            briefing.Teams = GameRule.CreateBriefingTeams();
            briefing.Players = GameRule.CreateBriefingPlayers();
            briefing.Spectators = TeamManager.Spectators.Select(x => x.Account.Id).ToArray();
            return briefing;
        }

        public void BroadcastBriefing()
        {
            var briefing = GetBriefing();
            Broadcast(new GameBriefingInfoAckMessage(false, false, briefing.GetData()));
        }

        private Player GetPlayerWithLowestPing(IEnumerable<Player> players = null)
        {
            players = players ?? Players.Values;
            return players.Aggregate(default(Player),
                (lowestPlayer, player) =>
                    lowestPlayer == null || player.Session.UnreliablePing < lowestPlayer.Session.UnreliablePing
                        ? player
                        : lowestPlayer);
        }

        private void OnGameStateChanged(object sender, EventArgs e)
        {
            RoomManager.Channel.Broadcast(new RoomChangeRoomInfoAck2Message(this.Map<Room, Room2Dto>()));
            if (GameRule.StateMachine.GameState == GameState.Result)
            {
                foreach (var plr in Players.Values)
                {
                    _messageBus.PublishAsync(new PlayerUpdateMessage(
                        plr.Account.Id, plr.TotalExperience, plr.Level, Id, plr.Nametag, plr.Team?.Id ?? TeamId.Neutral
                    ));
                }
            }
        }

        private void OnPlayerTeamChanged(object sender, PlayerTeamChangedEventArgs e)
        {
            var plr = e.Player;
            var team = TeamId.Neutral;
            if (!plr.IsInGMMode)
                team = plr.Team?.Id ?? TeamId.Neutral;

            _messageBus.PublishAsync(new PlayerUpdateMessage(
                plr.Account.Id, plr.TotalExperience, plr.Level, Id, plr.Nametag, team
            ));
        }
    }
}
