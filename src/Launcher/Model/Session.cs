using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Transport.Channels;
using Logging;
using ProudNet;

namespace Launcher.Model
{
    internal class Session : ProudSession
    {
        public Session(ILogger logger, uint hostId, IChannel channel)
            : base(logger, hostId, channel)
        {
        }
    }

    internal class SessionFactory : ISessionFactory
    {
        public ProudSession Create(ILogger logger, uint hostId, IChannel channel)
        {
            return new Session(logger, hostId, channel);
        }
    }
}
