using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;

namespace PersonalNewsSiteSupportTool.Behaviors
{
    class WindowClosingAction : TriggerAction<Window>
    {
        protected override void Invoke(object parameter)
        {
            var cancelEventArgs = parameter as CancelEventArgs;
            cancelEventArgs.Cancel = true;
            var window = Window.GetWindow(AssociatedObject);
            window.Hide();
        }
    }
}
