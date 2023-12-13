using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using BlubLib.Serialization;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProudNet.Abstraction;
using ProudNet.Client.Handlers;
using ProudNet.Client.Services;
using ProudNet.Configuration;
using ProudNet.DotNetty.Codecs;
using ProudNet.DotNetty.Handlers;
using ProudNet.Hosting.Services;
using ProudNet.Serialization;
using ProudNet.Serialization.Serializers;
using SessionHandler = ProudNet.Client.Handlers.SessionHandler;

namespace ProudNet.Client
{
    internal class ProudNetClientBuilder : IProudNetClientBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly BlubSerializer _serializer;

        public ProudNetClientBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _serializer = new BlubSerializer();

            serviceCollection
                .AddOptions()

                // Logging
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddTransient<ILogger, Logger>()
                .AddTransient(typeof(ILogger<>), typeof(Logger<>))

                // Hosted services
                .AddSingleton<ISchedulerService, SchedulerService>()
                .AddSingleton<IProudNetClientService, ProudNetClientService>()
                .AddSingleton(_ => (INetworkService)_.GetRequiredService<IProudNetClientService>())
                //.AddSingleton(_ => (IHostedService)_.GetRequiredService<ISchedulerService>())
                //.AddSingleton(_ => (IHostedService)_.GetRequiredService<IProudNetServerService>())

                // Session managers
                .AddSingleton<ISessionManagerFactory>(x =>
                {
                    /*
                    * External code can access the default session manager(by host id lookup)
                    * by using the public ISessionManager interface
                    *
                    * Internal code can use the ISessionManagerFactory interface
                    * to request additional internal session managers
                    */
                    var schedulerService = x.GetRequiredService<ISchedulerService>();
                    var sessionManager = new SessionManager(schedulerService);
                    return new SessionManagerFactory(new Dictionary<SessionManagerType, object>
                    {
                        [SessionManagerType.HostId] = sessionManager,
                        //[SessionManagerType.UdpId] = new SessionManager<uint>(schedulerService),
                        //[SessionManagerType.MagicNumber] = new SessionManager<Guid>(schedulerService)
                    });
                })
                .AddSingleton(_ => (ISessionManager)_.GetRequiredService<ISessionManagerFactory>()
                    .GetSessionManager<uint>(SessionManagerType.HostId))
                .AddSingleton(_serializer)
                .AddSingleton(new RSACryptoServiceProvider(1024))
                //.AddSingleton<P2PGroupManager>()
                //.AddSingleton<UdpSocketManager>()
                //.AddTransient<UdpSocket>()

                // Pipeline handlers & en-/decoders
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
                .AddTransient<UdpHandler>()

                // User message factories
                .AddTransient(_ => _.GetServices<MessageFactory>().ToArray());

            ConfigureSerializer(serializer =>
            {
                serializer.AddSerializer(new ArrayWithScalarSerializer());
                serializer.AddSerializer(new IPEndPointSerializer());
                serializer.AddSerializer(new StringSerializer());
            });
        }

        public IProudNetClientBuilder UseHostIdFactory(IHostIdFactory hostIdFactory)
        {
            _serviceCollection.AddSingleton(hostIdFactory);
            return this;
        }

        public IProudNetClientBuilder UseHostIdFactory<THostIdFactory>()
            where THostIdFactory : class, IHostIdFactory
        {
            _serviceCollection.AddSingleton<IHostIdFactory, THostIdFactory>();
            return this;
        }

        public IProudNetClientBuilder UseSessionFactory(ISessionFactory sessionFactory)
        {
            _serviceCollection.AddSingleton(sessionFactory);
            return this;
        }

        public IProudNetClientBuilder UseSessionFactory<TSessionFactory>()
            where TSessionFactory : class, ISessionFactory
        {
            _serviceCollection.AddSingleton<ISessionFactory, TSessionFactory>();
            return this;
        }

        public IProudNetClientBuilder UseMessageHandlerResolver(IMessageHandlerResolver resolver)
        {
            _serviceCollection.AddSingleton(resolver);
            return this;
        }

        public IProudNetClientBuilder ConfigureSerializer(Action<BlubSerializer> configure)
        {
            configure(_serializer);
            return this;
        }

        public IProudNetClientBuilder UseNetworkConfiguration(Action<NetworkOptions> configure)
        {
            _serviceCollection.Configure<NetworkOptions>(option => configure(option));
            return this;
        }

        public IProudNetClientBuilder UseThreadingConfiguration(Action<ThreadingOptions> configure)
        {
            _serviceCollection.Configure<ThreadingOptions>(option => configure(option));
            return this;
        }

        public IProudNetClientBuilder AddMessageFactory<TMessageFactory>()
            where TMessageFactory : MessageFactory
        {
            _serviceCollection.AddSingleton<MessageFactory, TMessageFactory>();
            return this;
        }
    }
}
