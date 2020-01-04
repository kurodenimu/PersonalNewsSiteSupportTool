using System;
using System.Windows.Input;

namespace PersonalNewsSiteSupportTool.Behaviors
{
    class CommandBase : ICommand
    {

        Action action;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action();
        }

        public CommandBase(Action action)
        {
            this.action = action;
        }
    }
}
