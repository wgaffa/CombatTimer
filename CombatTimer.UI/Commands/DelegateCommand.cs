using System;
using System.Windows.Input;

namespace CombatTimer.UI.Commands
{
    class DelegateCommand : ICommand
    {
        private readonly Action<object> _delegateAction;
        private readonly Func<object, bool> _delegateCanExecute;
        
        #region Constructors
        public DelegateCommand(Action<object> action, Func<object, bool> canExecutePredicate = null)
        {
            _delegateAction = action;
            _delegateCanExecute = canExecutePredicate;
        }
        #endregion

        #region ICommand
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _delegateCanExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _delegateAction.Invoke(parameter);

        internal void InvokeCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
