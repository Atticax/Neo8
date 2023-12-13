using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProudNet.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseProudNetClient(this IServiceCollection This, Action<IProudNetClientBuilder> builder)
        {
            builder(new ProudNetClientBuilder(This));
            return This;
        }
    }
}
