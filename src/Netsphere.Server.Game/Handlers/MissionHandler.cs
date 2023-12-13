using Logging;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Rules;
using ProudNet;
using System.Threading.Tasks;

namespace Netsphere.Server.Game.Handlers
{
    internal class MissionHandler :
        IHandle<AchieveMissionReqMessage>,
        IHandle<AchieveMissionRewardReqMessage>,
        IHandle<DailyMissionInitReqMessage>,
        IHandle<DailyMissionNextStepReqMessage>,
        IHandle<DailyMissionRewardReqMessage>
    {
        private readonly ILogger _logger;

        public MissionHandler(ILogger<MissionHandler> logger) => _logger = logger;

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInChannel), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, AchieveMissionReqMessage message)
        {
            _logger.Error("Error: AchieveMissionReqMessage");
            context.GetSession<Session>().Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInChannel), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, AchieveMissionRewardReqMessage message)
        {
            context.GetSession<Session>().Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInChannel), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, DailyMissionInitReqMessage message)
        {
            context.GetSession<Session>().Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInChannel), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, DailyMissionNextStepReqMessage message)
        {
            context.GetSession<Session>().Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInChannel), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, DailyMissionRewardReqMessage message)
        {
            context.GetSession<Session>().Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }
    }
}
