using System;
using BlubLib.Serialization;
using Microsoft.Extensions.Hosting;
using ProudNet.Configuration;
using ProudNet.Serialization;

namespace ProudNet.Client
{
    public interface IProudNetClientBuilder
    {
        IProudNetClientBuilder UseHostIdFactory(IHostIdFactory hostIdFactory);

        IProudNetClientBuilder UseHostIdFactory<THostIdFactory>()
            where THostIdFactory : class, IHostIdFactory;

        IProudNetClientBuilder UseSessionFactory(ISessionFactory sessionFactory);

        IProudNetClientBuilder UseSessionFactory<TSessionFactory>()
            where TSessionFactory : class, ISessionFactory;

        IProudNetClientBuilder UseMessageHandlerResolver(IMessageHandlerResolver resolver);

        IProudNetClientBuilder ConfigureSerializer(Action<BlubSerializer> configure);

        IProudNetClientBuilder UseNetworkConfiguration(Action<NetworkOptions> configure);

        IProudNetClientBuilder UseThreadingConfiguration(Action<ThreadingOptions> configure);

        IProudNetClientBuilder AddMessageFactory<TMessageFactory>()
            where TMessageFactory : MessageFactory;
    }
}
