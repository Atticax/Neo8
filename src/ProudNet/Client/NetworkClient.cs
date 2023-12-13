using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlubLib;
using BlubLib.Serialization;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Flow;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProudNet;
using ProudNet.Client.Handlers;
using ProudNet.Configuration;
using ProudNet.DotNetty.Codecs;
using ProudNet.DotNetty.Handlers;
using ProudNet.Hosting.Services;
using ProudNet.Serialization;
using ProudNet.Serialization.Messages;
using ProudNet.Serialization.Messages.Core;
using ProudNet.Serialization.Serializers;
using SessionHandler = ProudNet.Client.Handlers.SessionHandler;

namespace ProudNet.Client
{
    /*
    public class NetworkClient : INetworkClient
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly NetworkOptions _networkOptions;
        private readonly ISchedulerService _schedulerService;
        private IEventLoopGroup _workerThreads;

        public bool IsRunning { get; private set; }

        public bool IsShuttingDown { get; private set; }

        #region Events
        /*public event EventHandler Started;

        public event EventHandler Stopping;

        public event EventHandler Stopped;*

        public event EventHandler<ErrorEventArgs> Error;

        public event EventHandler<UnhandledRmiEventArgs> UnhandledRmi;

        /*protected virtual void OnStarted()
        {
            Started?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStopping()
        {
            Stopping?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStopped()
        {
            Stopped?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnConnected(ProudSession session)
        {
            Connected?.Invoke(this, session);
        }

        protected virtual void OnDisconnected(ProudSession session)
        {
            Disconnected?.Invoke(this, session);
        }*

        protected virtual void OnError(ErrorEventArgs e)
        {
            var log = _logger;
            log.Error(e.Exception, "Unhandled server exception");
            Error?.Invoke(this, e);
        }

        internal void RaiseError(ErrorEventArgs e)
        {
            OnError(e);
        }

        protected virtual void OnUnhandledRmi(MessageContext context)
        {
            UnhandledRmi?.Invoke(this, new UnhandledRmiEventArgs(context.Session, context.Message));
        }
        #endregion

        public NetworkClient(ILogger<NetworkClient> logger, IServiceProvider serviceProvider,
            IOptions<NetworkOptions> networkOptions, ISchedulerService schedulerService,
            Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
            _serviceProvider = serviceProvider;
            _networkOptions = networkOptions.Value;
            _schedulerService = schedulerService;

            InternalLoggerFactory.DefaultFactory = loggerFactory;
        }

        public async Task RunClientAsync(CancellationToken cancellationToken)
        {
            if (IsRunning)
                throw new InvalidOperationException("Server is already running");

            var tcpListener = _networkOptions.TcpListener;
            //var udpAddress = _networkOptions.UdpAddress;
            //var udpListenerPorts = _networkOptions.UdpListenerPorts;
            _logger.Information("Starting - tcp={TcpEndPoint}", // udp={UdpAddress} udp-port={UdpPorts}",
                tcpListener);//, udpAddress, udpListenerPorts);

            try
            {
                _workerThreads = new MultithreadEventLoopGroup();

                await new Bootstrap()
                    .Group(_workerThreads)
                    .Channel<TcpSocketChannel>()
                    .Handler(new ActionChannelInitializer<ISocketChannel>(ch =>
                    {
                        var coreMessageHandler = new MessageHandler(_serviceProvider,
                            new ClientMessageHandlerResolver(
                                new[] { typeof(AuthenticationHandler).Assembly }, typeof(ICoreMessage)
                            ),
                            _schedulerService
                        );

                        var internalRmiMessageHandler = new MessageHandler(
                            _serviceProvider,
                            new DefaultMessageHandlerResolver(
                                new[] { typeof(ReliablePingMessage).Assembly }, typeof(IMessage)
                            ),
                            _schedulerService
                        );

                        var rmiMessageHandler = new MessageHandler(
                            _serviceProvider,
                            _serviceProvider.GetRequiredService<IMessageHandlerResolver>(),
                            _schedulerService
                        );

                        coreMessageHandler.UnhandledMessage +=
                            ctx => ctx.Session.Logger.Debug("Unhandled core message={@Message}", ctx.Message);

                        internalRmiMessageHandler.UnhandledMessage +=
                            ctx => ctx.Session.Logger.Debug("Unhandled internal rmi message={@Message}", ctx.Message);

                        rmiMessageHandler.UnhandledMessage += OnUnhandledRmi;

                        ch.Pipeline
                            .AddLast(_serviceProvider.GetRequiredService<SessionHandler>())
                            .AddLast(_serviceProvider.GetRequiredService<ProudFrameDecoder>())
                            .AddLast(_serviceProvider.GetRequiredService<ProudFrameEncoder>())
                            .AddLast(_serviceProvider.GetRequiredService<MessageContextDecoder>())
                            .AddLast(_serviceProvider.GetRequiredService<CoreMessageDecoder>())
                            .AddLast(_serviceProvider.GetRequiredService<CoreMessageEncoder>())
                            .AddLast(new FlowControlHandler(false))
                            .AddLast(Constants.Pipeline.CoreMessageHandlerName, coreMessageHandler)
                            .AddLast(_serviceProvider.GetRequiredService<SendContextEncoder>())
                            .AddLast(_serviceProvider.GetRequiredService<MessageDecoder>())
                            .AddLast(_serviceProvider.GetRequiredService<MessageEncoder>())

                            // MessageHandler discards the message after handling
                            // so internal messages wont reach the rmiMessageHandler
                            .AddLast(Constants.Pipeline.InternalMessageHandlerName, internalRmiMessageHandler)
                            .AddLast(_serviceProvider.GetRequiredService<MessageDecoder>())
                            .AddLast(rmiMessageHandler)
                            //.AddLast(_serviceProvider.GetRequiredService<ErrorHandler>())
                            ;
                    }))
                    .Option(ChannelOption.TcpNodelay, !_networkOptions.EnableNagleAlgorithm)
                    .Option(ChannelOption.AutoRead, false)
                    .Attribute(ChannelAttributes.ServiceProvider, _serviceProvider)
                    .Attribute(ChannelAttributes.Session, default)
                    .ConnectAsync(_networkOptions.TcpListener);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to start server - tcp={TcpEndPoint}", tcpListener);
                await ShutdownThreads();
                throw ex;
            }

            IsRunning = true;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (IsShuttingDown || !IsRunning)
                throw new InvalidOperationException("Server is already stopped");

            _logger.Information("Shutting down...");
            IsShuttingDown = true;
            //OnStopping();
            //_udpSocketManager.Dispose();
            await ShutdownThreads();

            IsRunning = false;
            IsShuttingDown = false;
            //OnStopped();
        }

        /*private void SessionManager_OnAdded(object sender, SessionEventArgs e)
        {
            OnConnected(e.Session);
        }

        private void SessionManager_OnRemoved(object sender, SessionEventArgs e)
        {
            _udpSessionManager.RemoveSession(e.Session.UdpSessionId);
            _magicNumberSessionManager.RemoveSession(e.Session.HolepunchMagicNumber);
            OnDisconnected(e.Session);
        }*

        private Task ShutdownThreads()
        {
            return Task.WhenAll(_workerThreads?.ShutdownGracefullyAsync(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5)) ?? Task.CompletedTask);
        }

        public static IServiceCollection Configure(IServiceCollection services)
        {
            var serializer = new BlubSerializer();
            serializer.AddSerializer(new ArrayWithScalarSerializer());
            serializer.AddSerializer(new IPEndPointSerializer());
            serializer.AddSerializer(new StringSerializer());

            return services
                .AddSingleton(serializer)
                .AddSingleton<ISchedulerService, SchedulerService>()

                .AddSingleton(new RSACryptoServiceProvider(1024))

                .AddTransient<SessionHandler>()
                .AddTransient<ProudFrameDecoder>()
                .AddTransient<ProudFrameEncoder>()
                .AddTransient<MessageContextDecoder>()
                .AddTransient<CoreMessageDecoder>()
                .AddTransient<CoreMessageEncoder>()
                .AddTransient<SendContextEncoder>()
                .AddTransient<MessageDecoder>()
                .AddTransient<MessageEncoder>()
                .AddTransient<ErrorHandler>()

                .AddTransient(_ => _.GetServices<MessageFactory>().ToArray());
            ;
        }
    }*/

}
