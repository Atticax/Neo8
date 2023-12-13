using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Logging;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Netsphere.Server.Chat.Rules;
using ProudNet;

namespace Netsphere.Server.Chat.Handlers
{
    internal class PrivateMessageHandler
        : IHandle<NoteListReqMessage>, IHandle<NoteReadReqMessage>, IHandle<NoteDeleteReqMessage>, IHandle<NoteSendReqMessage>
    {
        private readonly ILogger _logger;

        public PrivateMessageHandler(ILogger<PrivateMessageHandler> logger)
        {
            _logger = logger;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NoteListReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var logger = plr.AddContextToLogger(_logger);

            logger.Debug("Note list Page={Page} MessageType={MessageType}", message.Page, message.MessageType);

            var mailbox = session.Player.Mailbox;
            var maxPages = mailbox.Count / Mailbox.ItemsPerPage + 1;

            if (message.Page > maxPages)
            {
                logger.Warning("Page={Page} does not exist", message.Page);
                return true;
            }

            var mails = session.Player.Mailbox.GetMailsByPage(message.Page);
            session.Send(new NoteListAckMessage(maxPages, message.Page,
                mails.Select(mail => mail.Map<Mail, NoteDto>()).ToArray()));

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NoteReadReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var logger = plr.AddContextToLogger(_logger);

            logger.Debug("Read note {Id}", message.Id);

            var mail = session.Player.Mailbox[(long)message.Id];
            if (mail == null)
            {
                logger.Error("Mail={Id} not found", message.Id);

                session.Send(new NoteReadAckMessage(0, new NoteContentDto(), 1));
                return true;
            }

            mail.IsNew = false;
            plr.Mailbox.UpdateReminder();
            session.Send(new NoteReadAckMessage((ulong)mail.Id, mail.Map<Mail, NoteContentDto>(), 0));

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NoteDeleteReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var logger = plr.AddContextToLogger(_logger);

            logger.Debug("Delete note Ids={Ids}", string.Join(",", message.Notes));
            plr.Mailbox.Remove(message.Notes.Select(x => (long)x));
            session.Send(new NoteDeleteAckMessage());

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NoteSendReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var logger = plr.AddContextToLogger(_logger);

            logger.Debug("Send note {Message}", message);

            // ToDo use config file
            if (message.Title.Length > 100)
            {
                logger.Warning("Title is too big({Length})", message.Title.Length);
                return true;
            }

            if (message.Message.Length > 112)
            {
                logger.Warning("Message is too big({Length})", message.Message.Length);
                return true;
            }

            var result = await plr.Mailbox.SendAsync(message.Receiver, message.Title, message.Message);
            session.Send(new NoteSendAckMessage(result ? 0 : 1));

            return true;
        }
    }
}
