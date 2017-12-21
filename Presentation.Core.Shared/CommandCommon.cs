using System;
using System.Windows.Input;
using Presentation.Core.Shared.Interfaces;

namespace Presentation.Patterns
{
    /// <summary>
    /// Implemented common functionality that can be used
    /// be specific ICommand implementations
    /// </summary>
    public abstract class CommandCommon : NotifyPropertyChanged, 
        ICommand, IRaiseCanExecuteChanged
    {
        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);

        public void RaiseCanExecuteChanged()
        {
            var canExecuteChanged = CanExecuteChanged;
#if !NET4
            canExecuteChanged?.Invoke(this, EventArgs.Empty);
#else
            if (canExecuteChanged != null)
            {
                canExecuteChanged(this, EventArgs.Empty);
            }
#endif
        }
    }
}