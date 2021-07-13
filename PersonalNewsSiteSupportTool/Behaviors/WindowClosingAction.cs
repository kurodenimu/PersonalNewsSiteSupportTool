using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Windows;

namespace PersonalNewsSiteSupportTool.Behaviors
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812")]
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
