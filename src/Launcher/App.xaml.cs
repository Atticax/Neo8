using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Launcher.Controller;
using Launcher.Model;
using Launcher.View;
using Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Netsphere.Common.Configuration;
using ProudNet.Configuration;
using ProudNet.Client;
using NetworkOptions = ProudNet.Configuration.NetworkOptions;
using ProudNet;
using Netsphere.Network.Message.Auth;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages.Core;
using ProudNet.Hosting.Services;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Hosting;

namespace Launcher
{
	/// <summary>
	/// Interaktionslogik für "App.xaml"
	/// </summary>
	public partial class App : Application
    {
        private CancellationTokenSource _hostedServicesCts;
        private void AppOnStartup(object sender, StartupEventArgs e)
        {
            var baseDirectory = Environment.GetEnvironmentVariable("NETSPHEREPIRATES_BASEDIR_AUTH");
            if (string.IsNullOrWhiteSpace(baseDirectory))
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var configuration = Netsphere.Common.Startup.Initialize(baseDirectory, "config.hjson",
                x => x.GetSection(nameof(AppOptions.Logging)).Get<LoggerOptions>());

            _hostedServicesCts = new CancellationTokenSource();
            var serviceProvider = ConfigureServiceProvider(configuration);
            serviceProvider.GetRequiredService<ILoginController>().Initialize(_hostedServicesCts.Token);
            Exit += (s, e) => AppOnShutdown();
        }

        public void AppOnShutdown()
        {
            _hostedServicesCts.Cancel();
            _hostedServicesCts.Dispose();
        }

        private IServiceProvider ConfigureServiceProvider(IConfiguration configuration)
        {
            AppOptions appOptions = configuration.Get<AppOptions>();
            var services = new ServiceCollection()
                //.AddSingleton<IServiceProvider>(sp => sp)
                .Configure<AppOptions>(configuration)
                .AddSingleton<Application>(this)
                .AddSingleton<ILoginModel, LoginModel>()
                .AddSingleton<ILauncherWindow, LauncherWindow>()
                .AddSingleton<ILoginController, LoginController>()
                //.AddTransient<INetworkClient, NetworkClient>()
                //.AddTransient<CancellationTokenSource, CancellationTokenSource>()
                //.AddSingleton<ILoggerFactory, LoggerFactory>()
                //.AddTransient<ILogger, Logger>()
                //.AddTransient(typeof(ILogger<>), typeof(Logger<>))
                .UseProudNetClient(builder =>
                {
                    var messageHandlerResolver = new FilteringMessageHandlerResolver(
                        AppDomain.CurrentDomain.GetAssemblies(), typeof(IAuthMessage)
                    );

                    builder
                        .UseHostIdFactory<HostIdFactory>()
                        .UseSessionFactory<SessionFactory>()
                        .AddMessageFactory<AuthMessageFactory>()
                        .UseMessageHandlerResolver(messageHandlerResolver)
                        .UseNetworkConfiguration((options) =>
                        {
                            options.Version = new Guid("{9be73c0b-3b10-403e-be7d-9f222702a38c}");
                            options.TcpListener = appOptions.AuthEndpoint;
                        })
                        .UseThreadingConfiguration((options) =>
                        {
                            //options.SocketListenerThreadsFactory = () => new MultithreadEventLoopGroup(1);
                            options.SocketWorkerThreadsFactory = () => appOptions.WorkerThreads < 1
                                ? new MultithreadEventLoopGroup()
                                : new MultithreadEventLoopGroup(appOptions.WorkerThreads);
                            options.WorkerThreadFactory = () => new SingleThreadEventLoop();
                        });
                })
                .AddLogging();
            var serviceProvider = services.BuildServiceProvider();
            (serviceProvider.GetRequiredService<ISchedulerService>() as IHostedService).StartAsync(_hostedServicesCts.Token);

            return serviceProvider;
        }
    }
}
