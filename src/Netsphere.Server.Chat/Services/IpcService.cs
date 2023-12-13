using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlubLib.Collections.Generic;
using ExpressMapper.Extensions;
using Foundatio.Messaging;
using Logging;
using Microsoft.Extensions.Hosting;
using Netsphere.Common.Messaging;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;

namespace Netsphere.Server.Chat.Services
{
    public class IpcService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;
        private readonly PlayerManager _playerManager;
        private readonly ChannelManager _channelManager;
        private readonly CancellationTokenSource _shutdown;

        public IpcService(ILogger<IpcService> logger, IMessageBus messageBus, PlayerManager playerManager,
            ChannelManager channelManager)
        {
            _logger = logger;
            _messageBus = messageBus;
            _playerManager = playerManager;
            _channelManager = channelManager;
            _shutdown = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _messageBus.SubscribeAsync<ChannelPlayerJoinedMessage>(OnPlayerJoinedChannel, _shutdown.Token);
            await _messageBus.SubscribeAsync<ChannelPlayerLeftMessage>(OnPlayerLeftChannel, _shutdown.Token);
            await _messageBus.SubscribeAsync<PlayerDisconnectedMessage>(OnPlayerDisconnected, _shutdown.Token);
            await _messageBus.SubscribeAsync<PlayerUpdateMessage>(OnPlayerUpdate, _shutdown.Token);
            await _messageBus.SubscribeAsync<ClanMemberUpdateMessage>(OnClanMemberUpdate, _shutdown.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _shutdown.Cancel();
            return Task.CompletedTask;
        }

        private Task OnPlayerJoinedChannel(ChannelPlayerJoinedMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr == null)
            {
                _logger.Warning("<OnPlayerJoinedChannel> Cant find player={Id}", message.AccountId);
                return Task.CompletedTask;
            }

            var channel = _channelManager.GetOrCreateChannel(message.ChannelId);
            channel.Join(plr);
            return Task.CompletedTask;
        }

        private Task OnPlayerLeftChannel(ChannelPlayerLeftMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr == null)
            {
                _logger.Warning("<OnPlayerLeftChannel> Cant find player={Id}", message.AccountId);
                return Task.CompletedTask;
            }

            var channel = _channelManager.GetChannel(message.ChannelId);
            if (channel == null)
            {
                _logger.Warning("<OnPlayerLeftChannel> Cant find channel={Id}", message.ChannelId);
                return Task.CompletedTask;
            }

            channel.Leave(plr);
            return Task.CompletedTask;
        }

        private Task OnPlayerDisconnected(PlayerDisconnectedMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr == null)
            {
                _logger.Warning("<OnPlayerDisconnected> Cant find player={Id}", message.AccountId);
                return Task.CompletedTask;
            }

            plr.Disconnect();
            return Task.CompletedTask;
        }

        private Task OnPlayerUpdate(PlayerUpdateMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr == null)
            {
                _logger.Warning("<OnPlayerUpdate> Cant find player={Id}", message.AccountId);
                return Task.CompletedTask;
            }

            plr.TotalExperience = message.TotalExperience;
            plr.Level = message.Level;
            plr.RoomId = message.RoomId;
            plr.TeamId = message.TeamId;

            if (plr.RoomId != 0)
            {
                plr.Channel.Broadcast(new ChannelLeavePlayerAckMessage(plr.Account.Id));
            }
            else if (plr.RoomId == 0)
            {
                plr.Session.Send(new ChannelPlayerListAckMessage(
                    plr.Channel.Players.Values
                        .Where(x => x.RoomId == 0)
                        .Select(x => x.Map<Player, PlayerInfoShortDto>())
                        .ToArray()
                ));
                plr.Channel.Broadcast(new ChannelEnterPlayerAckMessage(plr.Map<Player, PlayerInfoShortDto>()));
            }

            return Task.CompletedTask;
        }

        private async Task OnClanMemberUpdate(ClanMemberUpdateMessage message)
        {
            var plr = _playerManager[message.AccountId];
            if (plr == null)
                return;

            var update = new ClubMemberLoginStateAckMessage(message.PresenceState, message.AccountId);
            _playerManager.Where(x => x.ClanId == message.ClanId).ForEach(x =>
            {
                x.Session.Send(update);
                if (message.PresenceState != ClubMemberPresenceState.Offline)
                    x.Session.Send(new PlayerInfoAckMessage(plr.Map<Player, PlayerInfoDto>()));

                switch (message.PresenceState)
                {
                    case ClubMemberPresenceState.Online when message.LoggedIn:
                        x.Session.Send(new ClubSystemMessageMessage(
                            plr.Account.Id,
                            $"<Chat Key=\"1\" Cnt=\"2\" Param1=\"{plr.Account.Nickname}\" Param2=\"1\"/>")
                        );
                        break;

                    case ClubMemberPresenceState.Offline:
                        x.Session.Send(new ClubSystemMessageMessage(
                            plr.Account.Id,
                            $"<Chat Key=\"1\" Cnt=\"2\" Param1=\"{plr.Account.Nickname}\" Param2=\"2\"/>")
                        );
                        break;
                }
            });
        }
    }
}
