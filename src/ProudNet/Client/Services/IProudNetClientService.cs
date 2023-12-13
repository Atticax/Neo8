using System;
using System.Threading;
using System.Threading.Tasks;
using ProudNet.Abstraction;

namespace ProudNet.Client.Services
{
    public interface IProudNetClientService
    {
        bool IsRunning { get; }

        bool IsShuttingDown { get; }

        event EventHandler Started;

        event EventHandler Stopping;

        event EventHandler Stopped;

        event EventHandler<ProudSession> Connected;

        event EventHandler<ProudSession> Disconnected;

        event EventHandler<ErrorEventArgs> Error;

        event EventHandler<UnhandledRmiEventArgs> UnhandledRmi;
        Task RunClientAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
