using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProudNet.Client
{
    public interface INetworkClient
    {
        bool IsRunning { get; }
        bool IsShuttingDown { get; }

        event EventHandler<ErrorEventArgs> Error;
        event EventHandler<UnhandledRmiEventArgs> UnhandledRmi;

        Task RunClientAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
