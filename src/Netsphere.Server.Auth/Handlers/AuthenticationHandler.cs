using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Foundatio.Caching;
using Logging;
using Microsoft.EntityFrameworkCore;
using Netsphere.Common.Cryptography;
using Netsphere.Database;
using Netsphere.Database.Auth;
using Netsphere.Network;
using Netsphere.Network.Message.Auth;
using Netsphere.Server.Auth.Rules;
using Netsphere.Server.Auth.Services;
using ProudNet;
using Constants = Netsphere.Common.Constants;

namespace Netsphere.Server.Auth.Handlers
{
    internal class AuthenticationHandler : IHandle<LoginEUReqMessage>, IHandle<GameDataXBNReqMessage>
    {
        private readonly ILogger _logger;
        private readonly DatabaseService _databaseService;
        private readonly ICacheClient _cacheClient;
        private readonly XbnService _xbnService;
        private readonly RandomNumberGenerator _randomNumberGenerator;

        public AuthenticationHandler(ILogger<AuthenticationHandler> logger, DatabaseService databaseService,
            ICacheClient cacheClient, XbnService xbnService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _cacheClient = cacheClient;
            _xbnService = xbnService;
            _randomNumberGenerator = RandomNumberGenerator.Create();
        }

        [Firewall(typeof(MustBeLoggedIn), Invert = true)]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, LoginEUReqMessage message)
        {
            var session = context.GetSession<Session>();
            var remoteAddress = session.RemoteEndPoint.Address.ToString();

            var loginTask = string.Empty.Equals(message.Token.Token)
                ? LoginUsingCredentials(session, remoteAddress, message.Username, message.Password)
                : LoginUsingToken(session, remoteAddress, message.Token.Token);

            var accountId = await loginTask;
            if (accountId != null)
            {
                var sessionId = NewSessionId();
                await _cacheClient.SetAsync(Constants.Cache.SessionKey(accountId.Value), sessionId, TimeSpan.FromMinutes(30));
                session.Authenticated = true;
                session.Send(new LoginEUAckMessage(AuthLoginResult.OK, (ulong)accountId.Value, sessionId));
            }

            return true;
        }

        private async Task<int?> LoginUsingCredentials(Session session, string remoteAddress, string username, string password)
        {
            var logger = _logger.ForContext(
                ("RemoteEndPoint", remoteAddress),
                ("Username", username));

            logger.Debug("Login from {RemoteEndPoint} with username {Username}");

            AccountEntity account;
            using (var db = _databaseService.Open<AuthContext>())
            {
                var usernameLower = username.ToLower();
                account = await db.Accounts
                    .Include(x => x.Bans)
                    .FirstOrDefaultAsync(x => x.Username == usernameLower);

                if (account == null)
                {
                    logger.Information("Wrong login");
                    session.Send(new LoginEUAckMessage(AuthLoginResult.WrongLogin));
                    return null;
                }

                if (!PasswordHasher.IsPasswordValid(password, account.Password, account.Salt))
                {
                    logger.Information("Wrong login");
                    session.Send(new LoginEUAckMessage(AuthLoginResult.WrongLogin));
                    return null;
                }

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                var ban = account.Bans.FirstOrDefault(x => x.Duration == null || x.Date + x.Duration > now);

                if (ban != null)
                {
                    var unbanDate = DateTimeOffset.MinValue;
                    if (ban.Duration != null)
                        unbanDate = DateTimeOffset.FromUnixTimeSeconds(ban.Date + (ban.Duration ?? 0));

                    logger.Information("Account is banned until {UnbanDate}", unbanDate);
                    session.Send(new LoginEUAckMessage(unbanDate));
                    return null;
                }

                db.LoginHistory.Add(new LoginHistoryEntity
                {
                    AccountId = account.Id,
                    Date = now,
                    IP = remoteAddress
                });

                await db.SaveChangesAsync();
            }

            logger.Information("Login success");
            return account.Id;
        }

        private async Task<int?> LoginUsingToken(Session session, string remoteAddress, string token)
        {
            var logger = _logger.ForContext(
                ("RemoteEndPoint", remoteAddress),
                ("Token", token.Substring(0, 4) + "..."));

            logger.Debug("Login from {RemoteEndPoint} with token {Token}");

            var tokenParts = token.Split(':');
            int accountId;
            if (tokenParts.Length != 2 || !int.TryParse(tokenParts[0], out accountId))
            {
                logger.Information("Invalid token format");
                session.Send(new LoginEUAckMessage(AuthLoginResult.WrongLogin));
                return null;
            }

            AccountEntity account;
            using (var db = _databaseService.Open<AuthContext>())
            {
                account = await db.Accounts
                    .Include(x => x.Bans)
                    .FirstOrDefaultAsync(x => x.Id == accountId);

                if (account == null)
                {
                    logger.Information("Wrong login");
                    session.Send(new LoginEUAckMessage(AuthLoginResult.WrongLogin));
                    return null;
                }

                var lastLogin = await db.LoginHistory
                    .Where(x => x.IP == remoteAddress && x.AccountId == accountId)
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefaultAsync();

                if (lastLogin == null)
                {
                    logger.Information("Wrong login");
                    session.Send(new LoginEUAckMessage(AuthLoginResult.WrongLogin));
                    return null;
                }

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();

                if (now - 60 > lastLogin.Date)
                {
                    logger.Information("Wrong login");
                    session.Send(new LoginEUAckMessage(AuthLoginResult.WrongLogin));
                    return null;
                }

                var ban = account.Bans.FirstOrDefault(x => x.Duration == null || x.Date + x.Duration > now);

                if (ban != null)
                {
                    var unbanDate = DateTimeOffset.MinValue;
                    if (ban.Duration != null)
                        unbanDate = DateTimeOffset.FromUnixTimeSeconds(ban.Date + (ban.Duration ?? 0));

                    logger.Information("Account is banned until {UnbanDate}", unbanDate);
                    session.Send(new LoginEUAckMessage(unbanDate));
                    return null;
                }

                var sessionId = await _cacheClient.GetAsync<string>(Constants.Cache.SessionKey(account.Id));
                if (!sessionId.HasValue || sessionId.IsNull || !sessionId.Value.Equals(tokenParts[1]))
                {
                    logger.Information("Wrong login");
                    session.Send(new LoginEUAckMessage(AuthLoginResult.WrongLogin));
                    return null;
                }
            }

            logger.Information("Login success");
            return account.Id;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, GameDataXBNReqMessage message)
        {
            var session = context.GetSession<Session>();

            if (session.XbnSent)
                return true;

            session.XbnSent = true;

            const int sizeLimit = 40000;
            var data = _xbnService.GetData();

            foreach (var pair in data)
            {
                var sent = 0;
                while (sent < pair.Value.Length)
                {
                    var remainingBytes = pair.Value.Length - sent;
                    var chunk = remainingBytes > sizeLimit
                        ? new byte[sizeLimit]
                        : new byte[remainingBytes];
                    Array.Copy(pair.Value, sent, chunk, 0, chunk.Length);
                    sent += chunk.Length;
                    session.Send(
                        new GameDataXBNAckMessage(pair.Key, chunk, pair.Value.Length),
                        SendOptions.ReliableSecureCompress
                    );
                }
            }

            return true;
        }

        private string NewSessionId()
        {
            Span<byte> bytes = stackalloc byte[16];
            _randomNumberGenerator.GetBytes(bytes);
            return new Guid(bytes).ToString("N");
        }
    }
}
