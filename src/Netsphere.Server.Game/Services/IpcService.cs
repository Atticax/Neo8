using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Common.Messaging;

namespace Netsphere.Server.Game.Services
{
    public class IpcService : IHostedService
    {
        private readonly IMessageBus _messageBus;
        private readonly PlayerManager _playerManager;
        private readonly ChannelService _channelService;
        private readonly GameDataService _gameDataService;
        private readonly ClanManager _clanManager;
        private readonly ServerListOptions _serverListOptions;
        private readonly CancellationTokenSource _cts;

        public IpcService(IMessageBus messageBus, PlayerManager playerManager, ChannelService channelService,
            IOptions<ServerListOptions> serverListOptions, GameDataService gameDataService, ClanManager clanManager)
        {
            _messageBus = messageBus;
            _playerManager = playerManager;
            _channelService = channelService;
            _gameDataService = gameDataService;
            _clanManager = clanManager;
            _serverListOptions = serverListOptions.Value;
            _cts = new CancellationTokenSource();

            _channelService.PlayerJoined += ChannelOnPlayerJoined;
            _channelService.PlayerLeft += ChannelOnPlayerLeft;
            _playerManager.PlayerConnected += OnPlayerConnected;
            _playerManager.PlayerDisconnected += OnPlayerDisconnected;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _messageBus.SubscribeToRequestAsync<ChatLoginRequest, ChatLoginResponse>(OnChatLogin, _cts.Token);
            await _messageBus.SubscribeToRequestAsync<RelayLoginRequest, RelayLoginResponse>(OnRelayLogin, _cts.Token);
            await _messageBus.SubscribeToRequestAsync<LevelFromExperienceRequest, LevelFromExperienceResponse>(
                OnLevelFromExperience, _cts.Token
            );
            await _messageBus.SubscribeAsync<PlayerPeerIdMessage>(OnPlayerPeerId, _cts.Token);
            await _messageBus.SubscribeToRequestAsync<ClanMemberListRequest, ClanMemberListResponse>(
                OnClanMemberList,
                _cts.Token
            );
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        private Task<ChatLoginResponse> OnChatLogin(ChatLoginRequest request)
        {
            var plr = _playerManager[request.AccountId];
            if (plr == null || plr.Session.SessionId != request.SessionId)
                return Task.FromResult(new ChatLoginResponse(false, null, 0, 0));

            return Task.FromResult(new ChatLoginResponse(true, plr.Account, plr.TotalExperience, plr.Clan?.Id ?? 0));
        }

        private Task<RelayLoginResponse> OnRelayLogin(RelayLoginRequest request)
        {
            var plr = _playerManager[request.AccountId];
            if (plr == null || plr.Account.Nickname != request.Nickname ||
                request.ServerId != _serverListOptions.Id ||
                plr.Channel == null || plr.Room == null ||
                plr.Channel.Id != request.ChannelId || plr.Room.Id != request.RoomId)
            {
                return Task.FromResult(new RelayLoginResponse(false, null));
            }

            return Task.FromResult(new RelayLoginResponse(true, plr.Account));
        }

        private async Task<LevelFromExperienceResponse> OnLevelFromExperience(LevelFromExperienceRequest request)
        {
            return new LevelFromExperienceResponse(_gameDataService.GetLevelFromExperience(request.TotalExperience).Level);
        }

        private Task OnPlayerPeerId(PlayerPeerIdMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr.PeerId == null)
                plr.PeerId = new LongPeerId(message.AccountId, message.PeerId);

            return Task.CompletedTask;
        }

        private async Task<ClanMemberListResponse> OnClanMemberList(ClanMemberListRequest request)
        {
            var clan = _clanManager[request.ClanId];
            return new ClanMemberListResponse(GetClanMembers(clan));
        }

        private void ChannelOnPlayerJoined(object sender, ChannelEventArgs e)
        {
            _messageBus.PublishAsync(new ChannelPlayerJoinedMessage(e.Player.Account.Id, e.Channel.Id));
            if (e.Player.Clan != null)
            {
                _messageBus.PublishAsync(new ClanMemberUpdateMessage(
                    e.Player.Clan.Id,
                    e.Player.Account.Id,
                    ClubMemberPresenceState.Online
                ));
            }
        }

        private void ChannelOnPlayerLeft(object sender, ChannelEventArgs e)
        {
            _messageBus.PublishAsync(new ChannelPlayerLeftMessage(e.Player.Account.Id, e.Channel.Id));
            if (e.Player.Clan != null)
            {
                _messageBus.PublishAsync(new ClanMemberUpdateMessage(
                    e.Player.Clan.Id,
                    e.Player.Account.Id,
                    ClubMemberPresenceState.Online
                ));
            }
        }

        private void OnPlayerConnected(object sender, PlayerEventArgs e)
        {
            e.Player.RoomJoined += OnPlayerRoomJoined;
            e.Player.RoomLeft += OnPlayerRoomLeft;
        }

        private void OnPlayerDisconnected(object sender, PlayerEventArgs e)
        {
            e.Player.RoomJoined -= OnPlayerRoomJoined;
            e.Player.RoomLeft -= OnPlayerRoomLeft;
            _messageBus.PublishAsync(new PlayerDisconnectedMessage(e.Player.Account.Id));
        }

        private void OnPlayerRoomJoined(object sender, RoomPlayerEventArgs e)
        {
            if (e.Player.Clan != null)
            {
                _messageBus.PublishAsync(new ClanMemberUpdateMessage(
                    e.Player.Clan.Id,
                    e.Player.Account.Id,
                    ClubMemberPresenceState.Playing
                ));
            }
        }

        private void OnPlayerRoomLeft(object sender, RoomPlayerEventArgs e)
        {
            if (e.Player.Clan != null)
            {
                _messageBus.PublishAsync(new ClanMemberUpdateMessage(
                    e.Player.Clan.Id,
                    e.Player.Account.Id,
                    ClubMemberPresenceState.Online
                ));
            }
        }

        private static ClanMemberInfo[] GetClanMembers(Clan clan)
        {
            var members = Array.Empty<ClanMemberInfo>();
            if (clan != null)
            {
                members = clan.Where(x => x.State == ClubMemberState.Joined).Select(x => new ClanMemberInfo
                {
                    AccountId = x.AccountId,
                    Nickname = x.Name,
                    Role = x.Role,
                    LastLoginDate = x.LastLogin,
                    PresenceState = x.Player == null
                        ? ClubMemberPresenceState.Offline
                        : x.Player.Room?.GameRule.StateMachine.GameState == GameState.Playing
                            ? ClubMemberPresenceState.Playing
                            : ClubMemberPresenceState.Online
                }).ToArray();
            }

            return members;
        }
    }
}
