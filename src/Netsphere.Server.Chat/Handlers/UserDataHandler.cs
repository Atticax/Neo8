using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Netsphere.Server.Chat.Rules;
using ProudNet;

namespace Netsphere.Server.Chat.Handlers
{
    internal class UserDataHandler : IHandle<UserDataOneReqMessage>
    {
        private readonly PlayerManager _playerManager;

        public UserDataHandler(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, UserDataOneReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            if (plr.Account.Id == message.AccountId)
            {
                session.Send(new UserDataFourAckMessage(25, plr.Map<Player, UserDataDto>()));
                return true;
            }

            var target = _playerManager[message.AccountId];
            if (plr.Channel != target.Channel)
                return true;

            session.Send(new UserDataFourAckMessage(25, target.Map<Player, UserDataDto>()));
            return true;
        }
    }
}
