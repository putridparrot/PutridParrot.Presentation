using System;
using Presentation.Patterns.Helpers;

namespace Presentation.Patterns
{
    /// <summary>
    /// Implementation of an ICommand where the 
    /// actions/functions are supplied to the 
    /// command
    /// </summary>
    public class ActionCommand : CommandCommon
    {
        public ActionCommand()
        {
        }

        public ActionCommand(Action execute)
        {
            ExecuteCommand = execute;
        }

        public ActionCommand(Action execute, Func<bool> canExecute) :
            this(execute)
        {
            CanExecuteCommand = canExecute;
        }

        public Action ExecuteCommand { get; set; }
        public Func<bool> CanExecuteCommand { get; set; }

        public override bool CanExecute(object parameter)
        {
#if !NET4
            return CanExecuteCommand?.Invoke() ?? true;
#else
            return CanExecuteCommand == null || CanExecuteCommand();
#endif
        }

        public override void Execute(object parameter)
        {
#if !NET4
            ExecuteCommand?.Invoke();
#else
            if (ExecuteCommand != null)
            {
                ExecuteCommand();
            }
#endif
        }
    }

    /// <summary>
    /// Implementation of an ICommand where the 
    /// actions/functions are supplied to the 
    /// command
    /// </summary>
    public class ActionCommand<T> : CommandCommon
    {
        public ActionCommand()
        {
        }

        public ActionCommand(Action<T> execute)
        {
            ExecuteCommand = execute;
        }

        public ActionCommand(Action<T> execute, Func<T, bool> canExecute) :
            this(execute)
        {
            CanExecuteCommand = canExecute;
        }

        public Action<T> ExecuteCommand { get; set; }
        public Func<T, bool> CanExecuteCommand { get; set; }

        public override bool CanExecute(object parameter)
        {
#if !NET4
            return CanExecuteCommand?.Invoke(SafeConvert.ChangeType<T>(parameter)) ?? true;
#else
            return CanExecuteCommand == null || CanExecuteCommand(SafeConvert.ChangeType<T>(parameter));
#endif
        }

        public override void Execute(object parameter)
        {
#if !NET4
            ExecuteCommand?.Invoke(SafeConvert.ChangeType<T>(parameter));
#else
            if (ExecuteCommand != null)
            {
                ExecuteCommand(SafeConvert.ChangeType<T>(parameter));
            }
#endif
        }
    }

}
