using System;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Logging;
using Netsphere.Common;
using Netsphere.Common.Messaging;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;

namespace Netsphere.Server.Chat
{
    public class Player : ISaveable
    {
        private readonly IMessageBus _messageBus;

        public Session Session { get; private set; }
        public Account Account { get; private set; }
        public Mailbox Mailbox { get; }
        public DenyManager Ignore { get; }
        public FriendManager Friends { get; }
        public PlayerSettingManager Settings { get; }
        public uint TotalExperience { get; internal set; }
        public int Level { get; internal set; }
        public Channel Channel { get; internal set; }
        public uint RoomId { get; internal set; }
        public uint ClanId { get; internal set; }
        public TeamId TeamId { get; internal set; }
        internal bool SentPlayerList { get; set; }

        public event EventHandler<PlayerEventArgs> Disconnected;

        internal void OnDisconnected()
        {
            Channel?.Leave(this);

            Disconnected?.Invoke(this, new PlayerEventArgs(this));
        }

        public Player(Mailbox mailbox, DenyManager denyManager, FriendManager friendManager,
            PlayerSettingManager settings, IMessageBus messageBus)
        {
            _messageBus = messageBus;
            Mailbox = mailbox;
            Ignore = denyManager;
            Friends = friendManager;
            Settings = settings;
        }

        internal async Task Initialize(Session session, Account account, PlayerEntity entity)
        {
            Session = session;
            Account = account;
            TotalExperience = (uint)entity.TotalExperience;
            var response = await _messageBus.PublishRequestAsync<LevelFromExperienceRequest, LevelFromExperienceResponse>(
                new LevelFromExperienceRequest(TotalExperience)
            );
            Level = response.Level;
            await Mailbox.Initialize(this, entity);
            await Ignore.Initialize(this, entity);
            await Friends.Initialize(this, entity);
            Settings.Initialize(this, entity);
        }

        public void Disconnect()
        {
            var _ = DisconnectAsync();
        }

        public Task DisconnectAsync()
        {
            return Session.CloseAsync();
        }

        public async Task Save(GameContext db)
        {
            await Mailbox.Save(db);
            await Ignore.Save(db);
            await Friends.Save(db);
            await Settings.Save(db);
        }

        public ILogger AddContextToLogger(ILogger logger)
        {
            return logger.ForContext(
                ("AccountId", Account.Id),
                ("HostId", Session.HostId),
                ("EndPoint", Session.RemoteEndPoint)
            );
        }
    }
}
