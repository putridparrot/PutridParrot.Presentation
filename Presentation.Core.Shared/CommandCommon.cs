using System;
using System.Windows.Input;
using PutridParrot.Presentation.Core.Interfaces;

namespace PutridParrot.Presentation.Core
{
    /// <summary>
    /// Implemented common functionality that can be used
    /// be specific ICommand implementations
    /// </summary>
    public abstract class CommandCommon : NotifyPropertyChanged, 
        ICommand, IRaiseCanExecuteChanged
    {
        /// <summary>
        /// Event indicates when the CanExecuteChanged
        /// is called 
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Override to implement the CanExecute method
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract bool CanExecute(object parameter);
        /// <summary>
        /// Override to implement the Execute method
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void Execute(object parameter);

        /// <summary>
        /// Raises the CanExecuteChanged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var canExecuteChanged = CanExecuteChanged;
            canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}