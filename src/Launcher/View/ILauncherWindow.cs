using System.Windows.Input;
using System.Windows.Threading;

namespace Launcher.View
{
    public interface ILauncherWindow
    {
        Dispatcher Dispatcher { get; }
        void AttachLoginButtonActionHandler(MouseButtonEventHandler handler);
        void InitializeComponent();
        void Show();
        void Close();
    }
}
