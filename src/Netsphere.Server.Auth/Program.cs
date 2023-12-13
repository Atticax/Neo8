﻿using System;
using System.Collections.Generic;
using System.IO;
using DotNetty.Transport.Channels;
using Foundatio.Caching;
using Foundatio.Messaging;
using Foundatio.Serializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Common.Plugins;
using Netsphere.Database;
using Netsphere.Network.Message.Auth;
using Netsphere.Server.Auth.Services;
using Newtonsoft.Json;
using ProudNet;
using ProudNet.Hosting;
using ProudNet.Hosting.Services;
using Serilog;
using StackExchange.Redis;

namespace Netsphere.Server.Auth
{
    internal static class Program
    {
        private static void Main()
        {
            var baseDirectory = Environment.GetEnvironmentVariable("NETSPHEREPIRATES_BASEDIR_AUTH");
            if (string.IsNullOrWhiteSpace(baseDirectory))
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var configuration = Startup.Initialize(baseDirectory, "config.hjson",
                x => x.GetSection(nameof(AppOptions.Logging)).Get<LoggerOptions>());

            Log.Information("Starting...");

            var appOptions = configuration.Get<AppOptions>();
            var hostBuilder = new HostBuilder();
            var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(appOptions.Database.ConnectionStrings.Redis);

            IPluginHost pluginHost = new MefPluginHost();
            pluginHost.Initialize(configuration, Path.Combine(baseDirectory, "plugins"));

            hostBuilder
                .ConfigureServices((context, services) =>
                {
                    services
                        .Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true)
                        .Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(1))
                        .Configure<AppOptions>(context.Configuration)
                        .Configure<DatabaseOptions>(context.Configuration.GetSection(nameof(AppOptions.Database)))
                        .AddSingleton<DatabaseService>()
                        .AddDbContext<AuthContext>(x => x.UseMySql(appOptions.Database.ConnectionStrings.Auth))
                        .AddSingleton(redisConnectionMultiplexer)
                        .AddTransient<ISerializer>(x => new JsonNetSerializer(JsonConvert.DefaultSettings()))
                        .AddSingleton<ICacheClient, RedisCacheClient>()
                        .AddSingleton<IMessageBus, RedisMessageBus>()
                        .AddSingleton(x => new RedisCacheClientOptions
                        {
                            ConnectionMultiplexer = x.GetRequiredService<ConnectionMultiplexer>(),
                            Serializer = x.GetRequiredService<ISerializer>()
                        })
                        .AddSingleton(x => new RedisMessageBusOptions
                        {
                            Subscriber = x.GetRequiredService<ConnectionMultiplexer>().GetSubscriber(),
                            Serializer = x.GetRequiredService<ISerializer>()
                        })
                        .AddHostedServiceEx<ServerlistService>()
                        .AddHostedServiceEx<XbnService>();

                    pluginHost.OnConfigure(services);
                })
                .ConfigureHostConfiguration(builder => builder.AddConfiguration(configuration))
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
                .UseConsoleLifetime()
                .UseProudNetServer(builder =>
                {
                    var messageHandlerResolver = new DefaultMessageHandlerResolver(
                        AppDomain.CurrentDomain.GetAssemblies(), typeof(IAuthMessage));

                    builder
                        .UseHostIdFactory<HostIdFactory>()
                        .UseSessionFactory<SessionFactory>()
                        .AddMessageFactory<AuthMessageFactory>()
                        .UseMessageHandlerResolver(messageHandlerResolver)
                        .UseNetworkConfiguration((context, options) =>
                        {
                            options.Version = new Guid("{9be73c0b-3b10-403e-be7d-9f222702a38c}");
                            options.TcpListener = appOptions.Listener;
                        })
                        .UseThreadingConfiguration((context, options) =>
                        {
                            options.SocketListenerThreadsFactory = () => new MultithreadEventLoopGroup(1);
                            options.SocketWorkerThreadsFactory = () => appOptions.WorkerThreads < 1
                                ? new MultithreadEventLoopGroup()
                                : new MultithreadEventLoopGroup(appOptions.WorkerThreads);
                            options.WorkerThreadFactory = () => new SingleThreadEventLoop();
                        });
                });

            var host = hostBuilder.Build();

            var contexts = host.Services.GetRequiredService<IEnumerable<DbContext>>();
            foreach (var db in contexts)
            {
                Log.Information("Checking database={Context}...", db.GetType().Name);

                using (db)
                {
                    if (db.Database.GetPendingMigrations().Any())
                    {
                        if (appOptions.Database.RunMigration)
                        {
                            Log.Information("Applying database={Context} migrations...", db.GetType().Name);
                            db.Database.Migrate();
                        }
                        else
                        {
                            Log.Error("Database={Context} does not have all migrations applied", db.GetType().Name);
                            return;
                        }
                    }
                }
            }

            host.Services
                .GetRequiredService<IProudNetServerService>()
                .UnhandledRmi += (s, e) => Log.Debug("Unhandled Message={@Message} HostId={HostId}", e.Message, e.Session.HostId);

            host.Services.GetRequiredService<IApplicationLifetime>().ApplicationStarted.Register(() =>
                Log.Information("Press Ctrl + C to shutdown"));
            host.Run();
            host.Dispose();
            pluginHost.Dispose();
        }
    }
}
