using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ProudNet.Client.Handlers;
using ProudNet.Handlers;
using AuthenticationHandler = ProudNet.Client.Handlers.AuthenticationHandler;

namespace ProudNet.Client
{
    internal class ClientMessageHandlerResolver : IMessageHandlerResolver
    {
        public Assembly[] Assemblies { get; }

        public Type[] BaseTypes { get; }

        public ClientMessageHandlerResolver(Assembly[] assemblies, params Type[] baseTypes)
        {
            Assemblies = assemblies ?? Array.Empty<Assembly>();
            BaseTypes = baseTypes ?? Array.Empty<Type>();
        }

        public IEnumerable<Type> GetImplementations()
        {
            return new[] {
                typeof(AuthenticationHandler),
                typeof(MessageHandler)
            };
        }
    }
}
