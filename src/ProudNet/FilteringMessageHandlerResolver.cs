using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProudNet
{
    public class FilteringMessageHandlerResolver : IMessageHandlerResolver
    {
        public Assembly[] Assemblies { get; }
        public Type[] BaseTypes { get; }

        public FilteringMessageHandlerResolver(Assembly[] assemblies, params Type[] baseTypes)
        {
            Assemblies = assemblies ?? Array.Empty<Assembly>();
            BaseTypes = baseTypes ?? Array.Empty<Type>();
        }

        public IEnumerable<Type> GetImplementations()
        {
            return Assemblies.SelectMany(x => SelectTypes(x, BaseTypes));
        }

        private static IEnumerable<Type> SelectTypes(Assembly assembly, Type[] baseTypes)
        {
            return assembly.DefinedTypes
                .Where(x =>
                    !x.IsInterface
                    && x.GetCustomAttribute<GeneratedCodeAttribute>() == null
                    && x.ImplementsMessageHandler(baseTypes)
                );
        }
    }
}
