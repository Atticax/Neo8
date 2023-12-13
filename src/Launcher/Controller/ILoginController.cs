using System.Threading;

namespace Launcher.Controller
{
    public interface ILoginController
    {
        void Initialize(CancellationToken hostedServicesCancellationToken);
    }
}
