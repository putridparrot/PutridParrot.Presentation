using System;
using PutridParrot.Presentation.Helpers;

namespace PutridParrot.Presentation
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
            return CanExecuteCommand?.Invoke() ?? true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            ExecuteCommand?.Invoke();
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
            return CanExecuteCommand?.Invoke(SafeConvert.ChangeType<T>(parameter)) ?? true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            ExecuteCommand?.Invoke(SafeConvert.ChangeType<T>(parameter));
        }
    }
}
