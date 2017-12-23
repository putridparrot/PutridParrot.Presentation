using System;
using Presentation.Core.Helpers;

namespace Presentation.Core
{
    /// <summary>
    /// Implementation of an ICommand where the 
    /// actions/functions are supplied to the 
    /// command
    /// </summary>
    public class ActionCommand : CommandCommon
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ActionCommand()
        {
        }

        /// <summary>
        /// Constructor takes a method to be invoked when Execute is called
        /// </summary>
        /// <param name="execute">The execute method to be invoked</param>
        public ActionCommand(Action execute)
        {
            ExecuteCommand = execute;
        }

        /// <summary>
        /// Constructor takes a method to be invoked when Execute is called and
        /// a method that returns a boolean when CanExecute is called
        /// </summary>
        /// <param name="execute">The execute method to be invoked</param>
        /// <param name="canExecute">The canExecute method to be invoked</param>
        public ActionCommand(Action execute, Func<bool> canExecute) :
            this(execute)
        {
            CanExecuteCommand = canExecute;
        }

        /// <summary>
        /// Gets/Sets the execute method
        /// </summary>
        public Action ExecuteCommand { get; set; }
        /// <summary>
        /// Gets/Sets the can execute method
        /// </summary>
        public Func<bool> CanExecuteCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
#if !NET4
            return CanExecuteCommand?.Invoke() ?? true;
#else
            return CanExecuteCommand == null || CanExecuteCommand();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
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
        /// <summary>
        /// Default constructor
        /// </summary>
        public ActionCommand()
        {
        }

        /// <summary>
        /// Constructor takes a method to be invoked when Execute is called
        /// </summary>
        /// <param name="execute">The execute method to be invoked</param>
        public ActionCommand(Action<T> execute)
        {
            ExecuteCommand = execute;
        }

        /// <summary>
        /// Constructor takes a method to be invoked when Execute is called and
        /// a method that returns a boolean when CanExecute is called
        /// </summary>
        /// <param name="execute">The execute method to be invoked</param>
        /// <param name="canExecute">The canExecute method to be invoked</param>
        public ActionCommand(Action<T> execute, Func<T, bool> canExecute) :
            this(execute)
        {
            CanExecuteCommand = canExecute;
        }

        /// <summary>
        /// Gets/Sets the execute method
        /// </summary>
        public Action<T> ExecuteCommand { get; set; }
        /// <summary>
        /// Gets/Sets the can execute method
        /// </summary>
        public Func<T, bool> CanExecuteCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
#if !NET4
            return CanExecuteCommand?.Invoke(SafeConvert.ChangeType<T>(parameter)) ?? true;
#else
            return CanExecuteCommand == null || CanExecuteCommand(SafeConvert.ChangeType<T>(parameter));
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
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
