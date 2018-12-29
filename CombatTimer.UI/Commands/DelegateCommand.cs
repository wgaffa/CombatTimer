using System;
using System.Windows.Input;

namespace CombatTimer.UI.Commands
{
    class DelegateCommand : ICommand
    {
        private readonly Action<object> _delegateAction;
        
        #region Constructors
        public DelegateCommand(Action<object> action)
        {
            _delegateAction = action;
        }
        #endregion

        #region ICommand
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _delegateAction.Invoke(parameter);
        #endregion
    }
}
