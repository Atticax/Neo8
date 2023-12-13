using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Launcher.Model;
using Netsphere.Network.Message.Auth;
using ProudNet;
using ProudNet.Firewall;
using ProudNet.Serialization.Messages.Core;

namespace Launcher.Handler
{
    class LoginHandler : IHandle<LoginEUAckMessage>
    {
        private ILoginModel _loginModel;

        public LoginHandler(ILoginModel loginModel)
        {
            _loginModel = loginModel;
        }

        public async Task<bool> OnHandle(MessageContext context, LoginEUAckMessage message)
        {
            if (message.Result == Netsphere.Network.AuthLoginResult.OK)
            {
                _loginModel.LoginToken = message.AccountId.ToString() + ":" + message.SessionId;
            }
            else
            {
                throw new NotImplementedException();
            }
            return true;
        }
    }
}
