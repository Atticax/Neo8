using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BlubLib;
using Logging;
using Microsoft.EntityFrameworkCore;
using Netsphere.Database;
using Netsphere.Database.Auth;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Rules;
using Netsphere.Server.Game.Services;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class MiscHandler
        : IHandle<TimeSyncReqMessage>,
        IHandle<AdminShowWindowReqMessage>,
        IHandle<AdminActionReqMessage>,
        IHandle<NickCheckReqMessage>,
        IHandle<ItemUseChangeNickReqMessage>,
        IHandle<ItemUseChangeNickCancelReqMessage>
    {
        private readonly ILogger _logger;
        private readonly NicknameValidator _nicknameValidator;
        private readonly DatabaseService _databaseService;
        private readonly CommandService _commandService;

        public MiscHandler(
            ILogger<MiscHandler> logger,
            NicknameValidator nicknameValidator,
            CommandService commandService,
            DatabaseService databaseService)
        {
            _logger = logger;
            _nicknameValidator = nicknameValidator;
            _commandService = commandService;
            _databaseService = databaseService;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, TimeSyncReqMessage message)
        {
            context.Session.Send(new TimeSyncAckMessage
            {
                ClientTime = message.Time,
                ServerTime = (uint)Environment.TickCount
            });

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, AdminShowWindowReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            session.Send(new AdminShowWindowAckMessage(plr.Account.SecurityLevel <= SecurityLevel.User));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, AdminActionReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            await _commandService.Execute(plr, message.Command.GetArgs());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NickCheckReqMessage message)
        {
            var session = context.GetSession<Session>();
            if (!await _nicknameValidator.IsAvailableAsync(message.Nickname))
                session.Send(new NickCheckAckMessage(true));
            else
                session.Send(new NickCheckAckMessage(false));
            var flag = true;
            session = null;
            return flag;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemUseChangeNickReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            plr.AddContextToLogger(_logger);
            var item = plr.Inventory[message.ItemId];
            if (item == null)
            {
                session.Send(new ItemUseChangeNickAckMessage(1));
                return true;
            }
            if (!await _nicknameValidator.IsAvailableAsync(message.Nickname))
            {
                session.Send(new NickCheckAckMessage(true));
                return true;
            }
            using (var db = _databaseService.Open<AuthContext>())
            {
                var nicknameHistoryEntity1 = await EntityFrameworkQueryableExtensions.LastOrDefaultAsync(db.Nicknames, (x => x.AccountId.Equals((int)plr.Account.Id)), new CancellationToken());
                var nicknameHistoryEntity2 = new NicknameHistoryEntity()
                {
                    AccountId = (int)plr.Account.Id,
                    Nickname = message.Nickname
                };
                switch (item.ItemNumber.Number)
                {
                    case 1:
                        nicknameHistoryEntity2.ExpireDate = new long?(-1);
                        break;
                    case 3:
                        nicknameHistoryEntity2.ExpireDate = new long?(DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds());
                        break;
                    case 4:
                        nicknameHistoryEntity2.ExpireDate = new long?(DateTimeOffset.Now.AddDays(7).ToUnixTimeSeconds());
                        break;
                    case 5:
                        nicknameHistoryEntity2.ExpireDate = new long?(DateTimeOffset.Now.AddDays(30).ToUnixTimeSeconds());
                        break;
                    case 6:
                        nicknameHistoryEntity2.ExpireDate = new long?(DateTimeOffset.Now.AddDays(3).ToUnixTimeSeconds());
                        break;
                    default:
                        session.Send(new ItemUseChangeNickAckMessage(3));
                        return true;
                }
                plr.Inventory.Remove(item);
                var entityEntry = await db.Nicknames.AddAsync(nicknameHistoryEntity2, new CancellationToken());
                int num = await db.SaveChangesAsync(new CancellationToken());
                session.Send(new ItemUseChangeNickAckMessage(0, plr.Account.Id, message.Nickname));
            }
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemUseChangeNickCancelReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var item = plr.Inventory.FirstOrDefault(x => x.ItemNumber.Id.Equals(4000002));
            if (item == null)
            {
                session.Send(new ItemUseChangeNickAckMessage(3));
                return true;
            }
            using (var db = _databaseService.Open<AuthContext>())
            {
                var nicknames = db.Nicknames;
                var expression = (Expression<Func<NicknameHistoryEntity, bool>>)(x => x.AccountId.Equals((int)plr.Account.Id) && x.Nickname.Equals(plr.Account.Nickname));
                var cancellationToken = new CancellationToken();
                db.Nicknames.Remove(await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(nicknames, expression, cancellationToken));
                int num = await db.SaveChangesAsync(new CancellationToken());
                plr.Inventory.Remove(item);
                session.Send(new ItemUseChangeNickCancelAckMessage(0));
            }
            return true;
        }

    }
}
