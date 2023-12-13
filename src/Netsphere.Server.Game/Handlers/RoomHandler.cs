using System;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Collections.Generic;
using Logging;
using Microsoft.Extensions.Options;
using Netsphere.Network;
using Netsphere.Network.Data.GameRule;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Server.Game.Rules;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class RoomHandler
        : IHandle<RoomMakeReqMessage>, IHandle<RoomMakeReq2Message>, IHandle<RoomEnterPlayerReqMessage>,
          IHandle<RoomEnterReqMessage>, IHandle<RoomLeaveReqMessage>, IHandle<RoomInfoRequestReqMessage>,
          IHandle<RoomTeamChangeReqMessage>, IHandle<RoomPlayModeChangeReqMessage>, IHandle<RoomBeginRoundReq2Message>,
          IHandle<RoomReadyRoundReq2Message>, IHandle<GameEventMessageReqMessage>, IHandle<RoomItemChangeReqMessage>,
          IHandle<GameAvatarChangeReqMessage>, IHandle<RoomChangeRuleNotifyReq2Message>, IHandle<RoomLeaveRequestReqMessage>,
          IHandle<RoomAutoMixingTeamReqMessage>, IHandle<RoomIntrudeRoundReq2Message>, IHandle<GameLoadingSuccessReqMessage>,
          IHandle<ScoreKillReqMessage>, IHandle<ScoreKillAssistReqMessage>, IHandle<ScoreTeamKillReqMessage>,
          IHandle<ScoreHealAssistReqMessage>, IHandle<ScoreSuicideReqMessage>, IHandle<ScoreGoalReqMessage>,
          IHandle<ScoreReboundReqMessage>, IHandle<ScoreOffenseReqMessage>, IHandle<ScoreOffenseAssistReqMessage>,
          IHandle<ScoreDefenseReqMessage>, IHandle<ScoreDefenseAssistReqMessage>, IHandle<ScoreMissionScoreReqMessage>
    {
        private readonly ILogger<RoomHandler> _logger;
        private readonly EquipValidator _equipValidator;
        private readonly AppOptions _appOptions;

        public RoomHandler(ILogger<RoomHandler> logger, IOptions<AppOptions> appOptions, EquipValidator equipValidator)
        {
            _logger = logger;
            _equipValidator = equipValidator;
            _appOptions = appOptions.Value;
        }

        [Firewall(typeof(MustBeInRoom))]
        public async Task<bool> OnHandle(MessageContext context, RoomEnterPlayerReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (!plr.IsConnectingToRoom)
                return true;
            plr.Session.Send(new RoomEnterPlayerInfoListForNameTagAckMessage(
                room.Players.Values
                    .Select(x => new NameTagDto(x.Account.Id, x.Nametag))
                    .ToArray()
            ));
            room.Broadcast(new RoomEnterPlayerForBookNameTagsAckMessage(
                plr.Account.Id,
                plr.Account.Nickname,
                plr.TotalExperience,
                plr.Mode,
                plr.Team.Id,
                0,
                plr.Nametag
            ));
            session.Send(new RoomChangeMasterAckMessage(room.Master.Account.Id));
            session.Send(new RoomChangeRefereeAckMessage(room.Host.Account.Id));
            plr.SendBriefing();
            plr.IsConnectingToRoom = false;
            room.OnPlayerJoined(plr);
            return true;
        }

        [Firewall(typeof(MustBeInChannel))]
        [Firewall(typeof(MustBeInRoom), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, RoomMakeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var logger = plr.AddContextToLogger(_logger).ForContext("ClientMessage", message.Room, true);

            logger.Warning("TODO Arcade room creation is not implemented yet");
            session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Firewall(typeof(MustBeInChannel))]
        [Firewall(typeof(MustBeInRoom), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, RoomMakeReq2Message message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var channel = plr.Channel;
            var roomMgr = channel.RoomManager;
            var logger = plr.AddContextToLogger(_logger).ForContext("ClientMessage", message.Room, true);

            if (message.Room.GameRule == GameRule.Practice)
                message.Room.PlayerLimit = 1;

            var (room, createError) = roomMgr.Create(new RoomCreationOptions
            {
                Name = message.Room.Name,
                GameRule = message.Room.GameRule,
                Map = message.Room.Map,
                PlayerLimit = message.Room.PlayerLimit,
                SpectatorLimit = message.Room.IsSpectatingEnabled ? message.Room.SpectatorLimit : 0,
                TimeLimit = message.Room.TimeLimit,
                ScoreLimit = message.Room.ScoreLimit,
                Password = message.Room.Password,
                EquipLimit = (int)message.Room.ItemLimit,
                RelayEndPoint = _appOptions.RelayEndPoint,
                IsFriendly = message.Room.Settings.HasFlag(RoomSettings.IsFriendly)
            });

            switch (createError)
            {
                case RoomCreateError.OK:
                    break;

                case RoomCreateError.InvalidGameRule:
                    logger.Warning("Trying to create room with invalid gamerule");
                    session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
                    return true;

                case RoomCreateError.InvalidMap:
                    logger.Warning("Trying to create room with invalid map");
                    session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
                    return true;

                default:
                    logger.Warning("Unknown error={Error} when creating room", createError);
                    session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
                    return true;
            }

            room.Join(plr);

            return true;
        }

        [Firewall(typeof(MustBeInChannel))]
        [Firewall(typeof(MustBeInRoom), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, RoomEnterReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var channel = plr.Channel;
            var roomMgr = channel.RoomManager;
            var room = roomMgr[message.RoomId];

            if (room == null)
            {
                session.Send(new ServerResultAckMessage(ServerResult.ImpossibleToEnterRoom));
                session.Send(new RoomDisposeAckMessage(message.RoomId));
                return true;
            }

            if (!string.IsNullOrWhiteSpace(room.Options.Password) &&
                !room.Options.Password.Equals(message.Password) &&
                !plr.IsInGMMode)
            {
                session.Send(new ServerResultAckMessage(ServerResult.PasswordError));
                return true;
            }

            var error = room.Join(plr);
            switch (error)
            {
                case RoomJoinError.AlreadyInRoom:
                case RoomJoinError.RoomFull:
                case RoomJoinError.KickedPreviously:
                case RoomJoinError.NoIntrusion:
                    session.Send(new ServerResultAckMessage(ServerResult.CantEnterRoom));
                    break;

                case RoomJoinError.ChangingRules:
                    session.Send(new ServerResultAckMessage(ServerResult.RoomChangingRules));
                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, RoomLeaveReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            room.Leave(plr);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInChannel))]
        public async Task<bool> OnHandle(MessageContext context, RoomInfoRequestReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var channel = plr.Channel;
            var room = channel.RoomManager[message.RoomId];

            if (room == null)
                return true;

            session.Send(new RoomInfoRequestAck2Message
            {
                MasterName = room.Master.Account.Nickname,
                MasterLevel = room.Master.Level,
                Unk3 = "",
                ScoreLimit = room.Options.ScoreLimit,
                State = room.GameRule.StateMachine.GameState,
                PlayersInAlpha = room.TeamManager.Players.Count(x => x.Team.Id == TeamId.Alpha),
                PlayersInBeta = room.TeamManager.Players.Count(x => x.Team.Id == TeamId.Beta),
                Spectators = room.TeamManager.Spectators.Count(),
                SpectatorLimit = room.Options.SpectatorLimit
            });

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public async Task<bool> OnHandle(MessageContext context, RoomTeamChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            // No reason for gm mode to change teams
            // so just block it to make sure the clients dont break
            if (plr.IsInGMMode)
                return true;

            var error = room.TeamManager.ChangeTeam(plr, message.Team);
            switch (error)
            {
                case TeamChangeError.Full:
                    session.Send(new RoomChangeTeamFailAckMessage(ChangeTeamResult.Full));
                    break;

                case TeamChangeError.PlayerIsReady:
                    session.Send(new RoomChangeTeamFailAckMessage(ChangeTeamResult.AlreadyReady));
                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        public async Task<bool> OnHandle(MessageContext context, RoomPlayModeChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            // Can only change between modes in lobby
            if (plr.State != PlayerState.Lobby)
                return true;

            // gm mode should always be spectator
            if (plr.IsInGMMode)
                return true;

            var error = room.TeamManager.ChangeMode(plr, message.Mode);
            switch (error)
            {
                case TeamChangeModeError.Full:
                    session.Send(new RoomChangeTeamFailAckMessage(ChangeTeamResult.Full));
                    break;

                case TeamChangeModeError.PlayerIsReady:
                    session.Send(new RoomChangeTeamFailAckMessage(ChangeTeamResult.AlreadyReady));
                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public async Task<bool> OnHandle(MessageContext context, RoomBeginRoundReq2Message message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (!_equipValidator.IsValid(plr.CharacterManager.CurrentCharacter))
            {
                session.Send(new ServerResultAckMessage(ServerResult.WearingUnusableItem));
                return true;
            }

            if (!room.GameRule.StateMachine.StartGame())
                session.Send(new GameEventMessageAckMessage(GameEventMessage.CantStartGame, 0, 0, 0, ""));

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public Task<bool> OnHandle(MessageContext context, RoomReadyRoundReq2Message message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (!_equipValidator.IsValid(plr.CharacterManager.CurrentCharacter))
            {
                session.Send(new ServerResultAckMessage(ServerResult.WearingUnusableItem));
                return Task.FromResult(true);
            }

            plr.IsReady = !plr.IsReady;
            room.Broadcast(new RoomReadyRoundAckMessage(plr.Account.Id, plr.IsReady));
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, GameEventMessageReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            room.Broadcast(new GameEventMessageAckMessage(message.Event, session.Player.Account.Id,
                message.Unk1, message.Value, ""));

            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, RoomItemChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;
            var logger = plr.AddContextToLogger(_logger);

            // Can only change items in lobby and when not ready
            if (plr.State != PlayerState.Lobby || plr.IsReady)
            {
                plr.Disconnect();
                return Task.FromResult(true);
            }

            logger.Debug("Item sync unk1={Unk1}", message.Unk1);

            if (message.Unk2.Length > 0)
                logger.Warning("Item sync unk2={Unk2}", (object)message.Unk2);

            room.Broadcast(new RoomChangeItemAckMessage(message.Unk1, message.Unk2));
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, GameAvatarChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;
            var logger = plr.AddContextToLogger(_logger);

            // Can only change characters in lobby, during half time or when not ready
            if (plr.State != PlayerState.Lobby &&
                room.GameRule.StateMachine.TimeState != GameTimeState.HalfTime ||
                plr.IsReady)
            {
                plr.Disconnect();
                return Task.FromResult(true);
            }

            logger.Debug("Avatar sync unk1={Unk1}", message.Unk1);

            if (message.Unk2.Length > 0)
                logger.Warning("Avatar sync unk2={Unk2}", (object)message.Unk2);

            room.Broadcast(new GameAvatarChangeAckMessage(message.Unk1, message.Unk2));
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public async Task<bool> OnHandle(MessageContext context, RoomChangeRuleNotifyReq2Message message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            var error = room.ChangeRules(message.Settings);
            if (error != RoomChangeRulesError.OK)
                session.Send(new RoomChangeRuleFailAckMessage());

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        public Task<bool> OnHandle(MessageContext context, RoomLeaveRequestReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            var targetPlr = room.Players.GetValueOrDefault(message.AccountId);
            if (targetPlr == null)
                return Task.FromResult(true);

            // Cant kick people in gm mode also disables AFK kick
            if (targetPlr.IsInGMMode)
                return Task.FromResult(true);

            switch (message.Reason)
            {
                case RoomLeaveReason.Kicked:
                    // Only the master can kick people and kick is only allowed in the lobby
                    if (room.Master != plr && room.GameRule.StateMachine.GameState != GameState.Waiting)
                        return Task.FromResult(true);

                    break;

                case RoomLeaveReason.AFK:
                    // The client kicks itself when afk is detected
                    if (message.AccountId != plr.Account.Id)
                        return Task.FromResult(true);

                    break;

                default:
                    // Dont allow any other reasons for now
                    return Task.FromResult(true);
            }

            room.Leave(targetPlr, message.Reason);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeGameState), GameState.Waiting)]
        public Task<bool> OnHandle(MessageContext context, RoomAutoMixingTeamReqMessage message)
        {
            var session = context.GetSession<Session>();
            var room = session.Player.Room;

            var players = room.Players.Values
                .Where(x => x.Mode == PlayerGameMode.Normal)
                .Select(x => (plr: x, team: x.Team))
                .ToList();
            var rng = new Random(Guid.NewGuid().GetHashCode());

            foreach (var (plr, _) in players)
                plr.Team.Leave(plr);

            while (players.Count > 0)
            {
                var i = rng.Next(0, players.Count);
                var (plr, oldTeam) = players[i];
                players.RemoveAt(i);
                room.TeamManager.Join(plr);

                _logger.Debug("Shuffle team PlayerId={PlayerId} OldTeam={OldTeam} NewTeam={NewTeam}",
                    plr.Account.Id, oldTeam?.Id, plr.Team?.Id);
            }

            room.Broadcast(new RoomMixedTeamBriefingInfoAckMessage(
                room.Players.Values.Select(x => new MixedTeamBriefingDto(x.Account.Id, x.Team.Id)).ToArray()
            ));
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        public async Task<bool> OnHandle(MessageContext context, RoomIntrudeRoundReq2Message message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            plr.IsReady = false;
            plr.IsLoading = true;
            plr.State = PlayerState.Waiting;
            plr.Score.Reset();

            for (var i = 0; i < plr.CharacterStartPlayTime.Length; ++i)
                plr.CharacterStartPlayTime[i] = default;

            plr.Session.Send(new RoomGameLoadingAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        public async Task<bool> OnHandle(MessageContext context, GameLoadingSuccessReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            var gameState = room.GameRule.StateMachine.GameState;
            if (gameState != GameState.Loading && gameState != GameState.Playing)
                return true;

            plr.IsLoading = false;

            switch (gameState)
            {
                case GameState.Loading:
                    room.Broadcast(new RoomGameEndLoadingAckMessage(plr.Account.Id));
                    if (room.Players.Values.Where(x => x.State == PlayerState.Waiting).All(x => !x.IsLoading))
                        room.GameRule.StateMachine.StartGame();

                    break;

                case GameState.Playing:
                    plr.CharacterStartPlayTime[plr.CharacterManager.CurrentSlot] = DateTimeOffset.Now;
                    plr.StartPlayTime = DateTimeOffset.Now;
                    plr.State = plr.Mode == PlayerGameMode.Normal
                        ? PlayerState.Alive
                        : PlayerState.Spectating;
                    session.Send(new RoomGameStartAckMessage());
                    session.Send(new GameRefreshGameRuleInfoAckMessage(
                        gameState, room.GameRule.StateMachine.TimeState, room.GameRule.StateMachine.RoundTime
                    ));
                    room.GameRule.OnPlayerIntrude(plr);
                    break;
            }

            return true;
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreKillReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreKill(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                null,
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreKillAssistReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            var assist = room.Players.GetValueOrDefault(message.Score.Assist.AccountId);
            if (assist == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreKill(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                new ScoreContext(assist, message.Score.Assist.PeerId.ObjectType != 1 ? message.Score.Assist : null),
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreTeamKillReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreTeamKill(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreHealAssistReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            room.GameRule.OnScoreHeal(plr);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreSuicideReqMessage message)
        {
            _logger.Debug("PeerId={PeerId}", message.Id.PeerId.ToString());
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            room.GameRule.OnScoreSuicide(plr);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreGoalReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            var td = room.Players.GetValueOrDefault(message.PeerId.AccountId);
            if (td == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreTouchdown(td);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeMaster))]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreReboundReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            var newPlr = room.Players.GetValueOrDefault(message.NewId.AccountId);
            var oldPlr = room.Players.GetValueOrDefault(message.OldId.AccountId);
            room.GameRule.OnScoreFumbi(newPlr, oldPlr);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreOffenseReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreOffense(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                null,
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreOffenseAssistReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            var assist = room.Players.GetValueOrDefault(message.Score.Assist.AccountId);
            if (assist == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreOffense(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                new ScoreContext(assist, message.Score.Assist.PeerId.ObjectType != 1 ? message.Score.Assist : null),
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreDefenseReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreDefense(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                null,
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreDefenseAssistReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            var killer = room.Players.GetValueOrDefault(message.Score.Killer.AccountId);
            if (killer == null)
                return Task.FromResult(true);

            var assist = room.Players.GetValueOrDefault(message.Score.Assist.AccountId);
            if (assist == null)
                return Task.FromResult(true);

            room.GameRule.OnScoreDefense(
                new ScoreContext(killer, message.Score.Killer.PeerId.ObjectType != 1 ? message.Score.Killer : null),
                new ScoreContext(assist, message.Score.Assist.PeerId.ObjectType != 1 ? message.Score.Assist : null),
                new ScoreContext(plr, message.Score.Target.PeerId.ObjectType != 1 ? message.Score.Target : null),
                message.Score.Weapon);
            return Task.FromResult(true);
        }

        [Firewall(typeof(MustBeInRoom))]
        [Firewall(typeof(MustBeGameState), GameState.Playing)]
        [Firewall(typeof(MustBeTimeState), GameTimeState.HalfTime, Invert = true)] // Must not be half time
        public Task<bool> OnHandle(MessageContext context, ScoreMissionScoreReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var room = plr.Room;

            if (plr.State != PlayerState.Alive)
                return Task.FromResult(true);

            room.GameRule.OnScoreMission(plr);
            return Task.FromResult(true);
        }
    }
}
