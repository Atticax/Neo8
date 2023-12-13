using Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Database;
using Netsphere.Network;
using Netsphere.Network.Data.Club;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Rules;
using Netsphere.Server.Game.Services;
using ProudNet;
using System.Linq;
using System.Threading.Tasks;

namespace Netsphere.Server.Game.Handlers
{
    internal class GameClubHandler :
        IHandle<ClubAddressReqMessage>,
        IHandle<ClubHistoryReqMessage>,
        IHandle<ClubInfoByIdReqMessage>,
        IHandle<ClubInfoByNameReqMessage>,
        IHandle<ClubInfoReqMessage>,
        IHandle<ClubJoinReqMessage>,
        IHandle<ClubNoticeChangeReqMessage>,
        IHandle<ClubNoticePointRefreshReqMessage>,
        IHandle<ClubNoticeRecordRefreshReqMessage>,
        IHandle<ClubOtherClubinfoReqMessage>,
        IHandle<ClubSearchRoomReqMessage>,
        IHandle<ClubStadiumEditBlastinfoEditReqMessage>,
        IHandle<ClubStadiumEditMapDataReqMessage>,
        IHandle<ClubStadiumInfoReqMessage>,
        IHandle<ClubStadiumSelectReqMessage>,
        IHandle<ClubUnJoinReqMessage>,
        IHandle<MatchClubMarkReqMessage>
    {
        private readonly ILogger _logger;
        private readonly SystemsOptions _systemOptions;
        private readonly DatabaseService _databaseService;
        private readonly PlayerManager _playerManager;
        private readonly GameDataService _gameDataService;
        private readonly ClanManager _clanManager;

        public GameClubHandler(
          ILogger<GameClubHandler> logger,
          IOptions<SystemsOptions> systemOptions,
          DatabaseService databaseService,
          PlayerManager playerManager,
          GameDataService gameDataService,
          ClanManager clanManager)
        {
            _logger = (ILogger)logger;
            _systemOptions = systemOptions.Value;
            _databaseService = databaseService;
            _playerManager = playerManager;
            _gameDataService = gameDataService;
            _clanManager = clanManager;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubAddressReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubAddressAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubHistoryReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubHistoryAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubInfoByIdReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubFindInfoAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubInfoByNameReqMessage message)
        {
            var session = context.GetSession<Session>();
            _logger.Debug("OnHandle() => {@MessageName}...", message);
            var findInfoAckMessage = new ClubFindInfoAckMessage();
            session.Send(findInfoAckMessage);
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ClubInfoReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            session.Send(new ClubInfoAckMessage(new PlayerClubInfoDto()
            {
                ClubId = player.Clan.Id,
                ClubName = player.Clan.Name,
                ClubType = player.Clan.Icon
            }));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { }, Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, ClubJoinReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubJoinAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubNoticeChangeReqMessage message)
        {
            context.GetSession<Session>().Send(new ClubNoticeRecordRefreshAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubNoticePointRefreshReqMessage message)
        {
            context.GetSession<Session>().Send(new ClubNoticePointRefreshAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubNoticeRecordRefreshReqMessage message)
        {
            context.GetSession<Session>().Send(new ClubNoticeRecordRefreshAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubOtherClubinfoReqMessage message)
        {
            var session = context.GetSession<Session>();
            var clan = _clanManager.GetClan(message.ClanName);
            if (clan == null)
            {
                _logger.Warning("Clan {Name} not found", message.ClanName);
                session.Send(new ClubOtherClubinfoAckMessage(1));
                return true;
            }
            session.Send(new ClubOtherClubinfoAckMessage(0, new ClubInfo2Dto()
            {
                ClanId = (int)clan.Id,
                ClanName = clan.Name,
                ClanIcon = clan.Icon,
                CreationDate = clan.CreationDate,
                OwnerName = clan.Owner.Name,
                PlayersCount = clan.Count(x => x.State == ClubMemberState.Joined)
            }));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubSearchRoomReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ServerResultAckMessage(ServerResult.FailedToCreateRoom));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubStadiumEditBlastinfoEditReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubStadiumEditBlastinfoEditAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubStadiumEditMapDataReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubStadiumEditMapDataAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubStadiumInfoReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubStadiumInfoAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubStadiumSelectReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubStadiumSelectAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubUnJoinReqMessage message)
        {
            var session = context.GetSession<Session>();
            _logger.Debug("OnHandle() => {@MessageName}...", message);
            var unJoinAckMessage = new ClubUnJoinAckMessage();
            session.Send(unJoinAckMessage);
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, MatchClubMarkReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new MatchClubMarkAckMessage());
            return true;
        }
    }
}
