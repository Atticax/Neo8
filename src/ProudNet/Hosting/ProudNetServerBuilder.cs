﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using BlubLib.Serialization;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProudNet.Abstraction;
using ProudNet.Configuration;
using ProudNet.DotNetty.Codecs;
using ProudNet.DotNetty.Handlers;
using ProudNet.Hosting.Services;
using ProudNet.Serialization;
using ProudNet.Serialization.Serializers;

namespace ProudNet.Hosting
{
    internal class ProudNetServerBuilder : IProudNetServerBuilder
    {
        private readonly IHostBuilder _hostBuilder;
        private readonly BlubSerializer _serializer;

        public ProudNetServerBuilder(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
            _serializer = new BlubSerializer();

            _hostBuilder.ConfigureServices((context, collection) =>
            {
                collection
                    .AddOptions()

                    // Logging
                    .AddSingleton<ILoggerFactory, LoggerFactory>()
                    .AddTransient<ILogger, Logger>()
                    .AddTransient(typeof(ILogger<>), typeof(Logger<>))

                    // Hosted services
                    .AddSingleton<ISchedulerService, SchedulerService>()
                    .AddSingleton<IProudNetServerService, ProudNetServerService>()
                    .AddSingleton(_ => (IHostedService)_.GetRequiredService<ISchedulerService>())
                    .AddSingleton(_ => (IHostedService)_.GetRequiredService<IProudNetServerService>())
                    .AddSingleton(_ => (INetworkService)_.GetRequiredService<IProudNetServerService>())

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
                            [SessionManagerType.UdpId] = new SessionManager<uint>(schedulerService),
                            [SessionManagerType.MagicNumber] = new SessionManager<Guid>(schedulerService)
                        });
                    })
                    .AddSingleton(_ => (ISessionManager)_.GetRequiredService<ISessionManagerFactory>()
                        .GetSessionManager<uint>(SessionManagerType.HostId))
                    .AddSingleton(_serializer)
                    .AddSingleton(new RSACryptoServiceProvider(1024))
                    .AddSingleton<P2PGroupManager>()
                    .AddSingleton<UdpSocketManager>()
                    .AddTransient<UdpSocket>()

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
            });

            ConfigureSerializer(serializer =>
            {
                serializer.AddSerializer(new ArrayWithScalarSerializer());
                serializer.AddSerializer(new IPEndPointSerializer());
                serializer.AddSerializer(new StringSerializer());
            });
        }

        public IProudNetServerBuilder UseHostIdFactory(IHostIdFactory hostIdFactory)
        {
            _hostBuilder.ConfigureServices((context, collection) => collection.AddSingleton(hostIdFactory));
            return this;
        }

        public IProudNetServerBuilder UseHostIdFactory<THostIdFactory>()
            where THostIdFactory : class, IHostIdFactory
        {
            _hostBuilder.ConfigureServices((context, collection) => collection.AddSingleton<IHostIdFactory, THostIdFactory>());
            return this;
        }

        public IProudNetServerBuilder UseSessionFactory(ISessionFactory sessionFactory)
        {
            _hostBuilder.ConfigureServices((context, collection) => collection.AddSingleton(sessionFactory));
            return this;
        }

        public IProudNetServerBuilder UseSessionFactory<TSessionFactory>()
            where TSessionFactory : class, ISessionFactory
        {
            _hostBuilder.ConfigureServices((context, collection) => collection.AddSingleton<ISessionFactory, TSessionFactory>());
            return this;
        }

        public IProudNetServerBuilder UseMessageHandlerResolver(IMessageHandlerResolver resolver)
        {
            _hostBuilder.ConfigureServices((context, collection) => collection.AddSingleton(resolver));
            return this;
        }

        public IProudNetServerBuilder ConfigureSerializer(Action<BlubSerializer> configure)
        {
            configure(_serializer);
            return this;
        }

        public IProudNetServerBuilder UseNetworkConfiguration(Action<HostBuilderContext, NetworkOptions> configure)
        {
            _hostBuilder.ConfigureServices((context, collection) =>
                collection.Configure<NetworkOptions>(option => configure(context, option)));
            return this;
        }

        public IProudNetServerBuilder UseThreadingConfiguration(Action<HostBuilderContext, ThreadingOptions> configure)
        {
            _hostBuilder.ConfigureServices((context, collection) =>
                collection.Configure<ThreadingOptions>(option => configure(context, option)));
            return this;
        }

        public IProudNetServerBuilder AddMessageFactory<TMessageFactory>()
            where TMessageFactory : MessageFactory
        {
            _hostBuilder.ConfigureServices((context, collection) => collection.AddSingleton<MessageFactory, TMessageFactory>());
            return this;
        }
    }
}
