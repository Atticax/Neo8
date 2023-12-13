using System;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Foundatio.Messaging;
using Netsphere.Common;
using Netsphere.Common.Messaging;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Netsphere.Server.Chat.Rules;
using ProudNet;

namespace Netsphere.Server.Chat.Handlers
{
    internal class ClanHandler : IHandle<ClubMemberListReqMessage>
    {
        private readonly IMessageBus _messageBus;

        public ClanHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, ClubMemberListReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            var response = await _messageBus.PublishRequestAsync<ClanMemberListRequest, ClanMemberListResponse>(
                new ClanMemberListRequest(message.ClanId)
            );

            session.Send(new ClubMemberListAckMessage(
                response.Members.Select(x => x.Map<ClanMemberInfo, ClubMemberDto>()).ToArray()
            ));
            return true;
        }
    }
}
