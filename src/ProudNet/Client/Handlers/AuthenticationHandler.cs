using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BlubLib.Security.Cryptography;
using Microsoft.Extensions.Options;
using ProudNet.Configuration;
using ProudNet.Firewall;
using ProudNet.Serialization.Messages.Core;

namespace ProudNet.Client.Handlers
{
    internal class AuthenticationHandler
        : IHandle<NotifyServerConnectionHintMessage>,
          IHandle<NotifyCSSessionKeySuccessMessage>,
          IHandle<NotifyServerConnectSuccessMessage>,
          IHandle<ConnectServerTimedoutMessage>,
          IHandle<NotifyProtocolVersionMismatchMessage>
    //      IHandle<NotifyServerDeniedConnectionMessage>
    {
        private readonly IInternalSessionManager<uint> _sessionManager;
        private readonly NetworkOptions _networkOptions;
        private readonly RSACryptoServiceProvider _rsa;

        public AuthenticationHandler(
            ISessionManagerFactory sessionManagerFactory,
            IOptions<NetworkOptions> networkOptions,
            RSACryptoServiceProvider rsa)
        {
            _sessionManager = sessionManagerFactory.GetSessionManager<uint>(SessionManagerType.HostId);
            _networkOptions = networkOptions.Value;
            _rsa = rsa;
        }

        [Firewall(typeof(MustBeInState), SessionState.Handshake)]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NotifyCSSessionKeySuccessMessage message)
        {
            //throw new System.NotImplementedException();
            var response = new NotifyServerConnectionRequestDataMessage
            {
                Version = _networkOptions.Version,
                InternalNetVersion = Constants.NetVersion
            };
            var session = context.Session;
            session.State = SessionState.HandshakeKeyExchanged;
            session.Send(response);
            return true;
        }

        [Firewall(typeof(MustBeInState), SessionState.Handshake)]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NotifyServerConnectionHintMessage message)
        {
            //throw new System.NotImplementedException();

            var session = context.Session;
            session.Logger.Verbose("Handshake:NotifyServerConnectionHintMessage");
            _rsa.ImportParameters(message.PublicKey);

            var response = new NotifyCSEncryptedSessionKeyMessage();
            try
            {
                session.Crypt = new Crypt(message.Config.EncryptedMessageKeyLength, message.Config.FastEncryptedMessageKeyLength);
                response.SecureKey = _rsa.Encrypt(session.Crypt.AES.Key, true);
                response.FastKey = session.Crypt.AES.Encrypt(session.Crypt.RC4.Key);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            session.Send(response);
            return true;
        }

        [Firewall(typeof(MustBeInState), SessionState.HandshakeKeyExchanged)]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NotifyServerConnectSuccessMessage message)
        {
            var session = context.Session;
            _sessionManager.AddSession(session.HostId, session);
            session.State = SessionState.Connected;
            session.HandhsakeEvent.Set();
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ConnectServerTimedoutMessage message)
        {
            var session = context.Session;

            await session.CloseAsync();
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NotifyProtocolVersionMismatchMessage message)
        {
            var session = context.Session;

            await session.CloseAsync();
            return true;
        }

        /*[Inline]
        public async Task<bool> OnHandle(MessageContext context, NotifyServerDeniedConnectionMessage message)
        {
            var session = context.Session;

            await session.CloseAsync();
            return true;
        }*/
    }
}
