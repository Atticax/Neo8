using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Launcher.Model;
using Launcher.View;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Netsphere.Network.Data.Auth;
using Netsphere.Network.Message.Auth;
using ProudNet;
using ProudNet.Client;
using ProudNet.Client.Services;
using ProudNet.Configuration;

namespace Launcher.Controller
{
    public class LoginController : ILoginController
    {
        private readonly ILoginModel _loginModel;
        private readonly ILauncherWindow _window;
        private readonly IServiceProvider _serviceProvider;
        private CancellationToken _hostedServicesCancellationToken;

        public LoginController(ILoginModel loginModel, ILauncherWindow window, IServiceProvider serviceProvider)
        {
            _loginModel = loginModel;
            _window = window;
            _window.AttachLoginButtonActionHandler((s, e) => DoLogin());
            _serviceProvider = serviceProvider;
        }

        public void Initialize(CancellationToken hostedServicesCancellationToken)
        {
            _window.Show();
            _hostedServicesCancellationToken = hostedServicesCancellationToken;
            //DoLogin();
            _loginModel.Username = "gian";
            _loginModel.Password = "gian";
            _loginModel.Status = "- / -";
        }

        private void DoLogin()
        {
            var client = _serviceProvider.GetRequiredService<IProudNetClientService>();
            if (_hostedServicesCancellationToken.IsCancellationRequested)
            {
                throw new Exception();
            }
            else if(client.IsRunning)
            {
                throw new Exception();
            }
            else
            {
                _loginModel.Status = "Connecting to server...";
                var cts = new CancellationTokenSource();
                _hostedServicesCancellationToken.Register(cts.Cancel);
                client.Connected += (object sender, ProudSession session) =>
                {
                    _loginModel.Status = "Connection success. Login in...";
                    session.Send(
                        new LoginEUReqMessage()
                        {
                            Username = _loginModel.Username,
                            Password = _loginModel.Password,
                            Token = new AeriaTokenDto()
                        },
                        SendOptions.ReliableSecure
                    );
                };
                _loginModel.LoginTokenProperty.PropertyChanged += (s, loginToken) =>
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        _loginModel.Status = "Login success. Starting game...";
                        var options = _serviceProvider.GetRequiredService<IOptions<AppOptions>>().Value;
                        var processStartInfo = new ProcessStartInfo()
                        {
#if DEBUG
                            FileName = "s4client.exe",
                            //WorkingDirectory = "../../S4PLeague/",
#else
                            FileName = "s4client.exe",
#endif
                            Arguments = string.Format(
                                "-rc:eu -auth_server_ip:{0} -aeria_acc_code:{1} -lac:{2} -key:0|0|0",
                                (object)options.GameEndpoint.Address,
                                (object)_loginModel.LoginToken,
                                "eng"
                            )
                        };
                        processStartInfo.EnvironmentVariables["__COMPAT_LAYER"] = "RUNASINVOKER";
                        Process.Start(processStartInfo);
                        client.StopAsync(cts.Token).ContinueWith(t => _window.Dispatcher.Invoke(() => _window.Close()));
                    }
                };
                var clientTask = client.RunClientAsync(cts.Token);
            }
        }
    }
}
