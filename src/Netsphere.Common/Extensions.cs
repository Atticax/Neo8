﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlubLib.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netsphere.Common.Messaging;
using Newtonsoft.Json;

namespace Netsphere.Common
{
    public static class MessageBusExtensions
    {
        public static Task SubscribeToRequestAsync<TRequest, TResponse>(this IMessageBus This,
            Func<TRequest, Task<TResponse>> handler, CancellationToken cancellationToken)
            where TRequest : MessageWithGuid
            where TResponse : MessageWithGuid
        {
            return This.SubscribeAsync((Func<TRequest, Task>)InternalHandler, cancellationToken);

            async Task InternalHandler(TRequest request)
            {
                var response = await handler(request);
                response.Guid = request.Guid;
                await This.PublishAsync(response);
            }
        }

        public static async Task<TResponse> PublishRequestAsync<TRequest, TResponse>(this IMessageBus This, TRequest request)
            where TRequest : MessageWithGuid
            where TResponse : MessageWithGuid
        {
            var tcs = new TaskCompletionSource<TResponse>();
            var cts = new CancellationTokenSource();
            request.Guid = Guid.NewGuid();
            await This.SubscribeAsync<TResponse>(x =>
            {
                if (x.Guid == request.Guid)
                {
                    tcs.TrySetResult(x);
                    cts.Cancel();
                }
            }, cts.Token);
            await This.PublishAsync(request);
            var timeout = Task.Delay(TimeSpan.FromSeconds(30));
            if (await Task.WhenAny(timeout, tcs.Task) != timeout)
                return tcs.Task.Result;
            cts.Cancel();
            return Activator.CreateInstance<TResponse>();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddService<TService, TImplementation>(this IServiceCollection This)
            where TImplementation : class, IService, TService
        {
            return This.AddSingleton(typeof(IService), x => x.GetService(typeof(TService)))
                .AddSingleton(typeof(TService), typeof(TImplementation));
        }

        public static IServiceCollection AddService<TImplementation>(this IServiceCollection This)
            where TImplementation : class, IService
        {
            return This.AddSingleton<IService>(x => x.GetRequiredService<TImplementation>())
                .AddSingleton<TImplementation>();
        }

        public static IServiceCollection AddHostedServiceEx<TService, TImplementation>(this IServiceCollection This)
            where TImplementation : class, IHostedService, TService
        {
            return This.AddSingleton(typeof(IHostedService), x => x.GetService(typeof(TService)))
                .AddSingleton(typeof(TService), typeof(TImplementation));
        }

        public static IServiceCollection AddHostedServiceEx<TImplementation>(this IServiceCollection This)
            where TImplementation : class, IHostedService
        {
            return This.AddSingleton<IHostedService>(x => x.GetRequiredService<TImplementation>())
                .AddSingleton<TImplementation>();
        }
    }

    public static class ObjectExtensions
    {
        public static string ToJson(this object This)
        {
            return JsonConvert.SerializeObject(This);
        }

        public static string ToJson(this object This, bool formatted)
        {
            return JsonConvert.SerializeObject(This, formatted ? Formatting.Indented : Formatting.None);
        }
    }

    public static class DnsEndPointExtensions
    {
        public static IPEndPoint ToIPEndPoint(this DnsEndPoint This)
        {
            var addresses = Dns.GetHostAddresses(This.Host);
            var address = addresses.FirstOrDefault(x => This.AddressFamily == AddressFamily.Unspecified ||
                                                        x.AddressFamily == This.AddressFamily);
            return new IPEndPoint(address, This.Port);
        }

        public static async Task<IPEndPoint> ToIPEndPointAsync(this DnsEndPoint This)
        {
            var addresses = await Dns.GetHostAddressesAsync(This.Host).AnyContext();
            var address = addresses.FirstOrDefault(x => This.AddressFamily == AddressFamily.Unspecified ||
                                                        x.AddressFamily == This.AddressFamily);
            return new IPEndPoint(address, This.Port);
        }
    }

    public static class TimeSpanExtensions
    {
        public static string ToHumanReadable(this TimeSpan value)
        {
            var uptime = new StringBuilder();
            if (value.Days > 0)
                uptime.AppendFormat(value.Days > 1 ? "{0} days " : "{0} day ", value.Days);

            if (value.Days > 0 || value.Hours > 0)
                uptime.AppendFormat(value.Hours > 1 ? "{0} hours " : "{0} hour ", value.Hours);

            if (value.Hours > 0 || value.Minutes > 0)
                uptime.AppendFormat(value.Minutes > 1 ? "{0} minutes " : "{0} minute ", value.Minutes);

            if (value.Seconds > 0)
                uptime.AppendFormat(value.Seconds > 1 ? "{0} seconds " : "{0} second ", value.Seconds);

            return uptime.ToString();
        }
    }
}
