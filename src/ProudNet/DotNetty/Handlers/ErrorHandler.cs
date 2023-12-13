using System;
using System.Net.Sockets;
using DotNetty.Transport.Channels;
using Logging;
using ProudNet.Abstraction;
using ProudNet.Hosting.Services;

namespace ProudNet.DotNetty.Handlers
{
    internal class ErrorHandler : ChannelHandlerAdapter
    {
        private readonly ILogger _logger;
        private readonly INetworkService _networkService;

        public ErrorHandler(ILogger<ErrorHandler> logger, INetworkService networkService)
        {
            _logger = logger;
            _networkService = networkService;
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            var session = context.Channel.GetAttribute(ChannelAttributes.Session).Get();
            if (exception is SocketException socketException)
            {
                if (socketException.SocketErrorCode == SocketError.ConnectionReset ||
                    socketException.SocketErrorCode == SocketError.TimedOut ||
                    socketException.SocketErrorCode == SocketError.HostUnreachable)
                {
                    session?.CloseAsync();
                    return;
                }
            }

            _logger.Error(exception, "Unhandled exception");
            _networkService.RaiseError(new ErrorEventArgs(session, exception));
            session?.CloseAsync();
        }
    }
}
