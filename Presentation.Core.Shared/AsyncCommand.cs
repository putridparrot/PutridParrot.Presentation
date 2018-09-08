using System;
using System.Threading.Tasks;
#if !NETSTANDARD2_0
using System.Windows.Threading;
#endif
using PutridParrot.Presentation.Core.Helpers;

namespace PutridParrot.Presentation.Core
{
    /// <summary>
    /// A Task/async aware command object. 
    /// </summary>
    /// <remarks>
    /// Automatically handles changes to the built-in, IsBusy flag, so when
    /// the command is executing the IsBusy is true and when completed or not 
    /// executing IsBusy is false.
    /// </remarks>
    public class AsyncCommand : CommandCommon
    {
        private ReferenceCounter _busyCount;

        /// <summary>
        /// Default constructor, creates an instance
        /// of AsyncCommand without any Execute or CanExecute
        /// </summary>
        public AsyncCommand()
        {
        }

        /// <summary>
        /// Constructor takes an execute async method which is 
        /// called when the Execute method is called
        /// </summary>
        /// <param name="execute">A function returning a Task</param>
        public AsyncCommand(Func<Task> execute)
        {
            ExecuteCommand = execute;
        }

        /// <summary>
        /// Constructor takes an execute async method which is 
        /// called when the Execute method is called and a canExecute
        /// method called when CanExecute is called.
        /// </summary>
        /// <param name="execute">A function returning a Task</param>
        /// <param name="canExecute">A function returning a Task of type boolean</param>
        public AsyncCommand(Func<Task> execute, Func<Task<bool>> canExecute) :
            this(execute)
        {
            CanExecuteCommand = canExecute;
        }

        /// <summary>
        /// Gets/Sets the async function to be used when the
        /// Execute method is called
        /// </summary>
        public Func<Task> ExecuteCommand { get; set; }
        /// <summary>
        /// Gets/Sets the asyc function to be used when the
        /// CanExecuteCommand is called
        /// </summary>
        public Func<Task<bool>> CanExecuteCommand { get; set; }

        /// <summary>
        /// Implementation of CanExecute which returns a boolean
        /// to allow the calling method to determine whether the
        /// Execute method can be called
        /// </summary>
        /// <param name="parameter">An object is ignored in this implementation</param>
        /// <returns>True if the command can be executed, otherwise False</returns>
        public override bool CanExecute(object parameter)
        {
            // slightly naff to use the Result like this, but need this method
            // signature to support ICommand
            bool result = true;
            if(CanExecuteCommand != null)
                result = CanExecuteCommand().Result;

            return result && !IsBusy;
        }

        /// <summary>
        /// Executes the previously supplied ExecuteCommand
        /// </summary>
        /// <remarks>
        /// The Execute method will execute the supplied ExecuteCommand, setting
        /// the IsBusy flag to True and running on using a Task which, upon 
        /// completion will set the IsFlag back to False.
        /// </remarks>
        /// <param name="parameter">This parameter is igored within this implementation</param>
        public override void Execute(object parameter)
        {
            try
            {
                if (ExecuteCommand != null)
                {
                    IsBusy = true;

                    ExecuteCommand().
                        ContinueWith(CompleteTask,
                            TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Called when the Task completes, this will set the IsBusy flag
        /// to False and where possible, ensure any exceptions are rethrown
        /// on the main thread
        /// </summary>
        /// <param name="tsk">The task representing the executed method</param>
        private void CompleteTask(Task tsk)
        {
            IsBusy = false;

            if (tsk.IsFaulted)
            {
#if !NETSTANDARD2_0
                Dispatcher.CurrentDispatcher.Throw(tsk.Exception);
#endif
            }
        }

        /// <summary>
        /// Lazily creates a ReferenceCounter to allows us to 
        /// handle multiple calls to IsBusy
        /// </summary>
        /// <returns>The stored or a new ReferenceCounter</returns>
        private ReferenceCounter GetOrCreateBusyCount()
        {
            return _busyCount ?? (_busyCount = new ReferenceCounter());
        }

        /// <summary>
        /// Gets/Sets the IsBusy flag
        /// </summary>
        public bool IsBusy
        {
            get => GetOrCreateBusyCount().Count > 0;
            set
            {
                var busyCount = GetOrCreateBusyCount();
                var current = busyCount.Count;
                if ((value ? busyCount.AddRef() : busyCount.Release()) != current)
                {
                    OnPropertyChanged();
                    RaiseCanExecuteChanged();
                }
            }
        }
    }

    /// <summary>
    /// A Task/async aware command object. Automatically handles changes
    /// to the IsBusy flag
    /// </summary>
    public class AsyncCommand<T> : CommandCommon
    {
        private ReferenceCounter _busyCount;

        /// <summary>
        /// Default constructor, creates an instance
        /// of AsyncCommand without any Execute or CanExecute
        /// </summary>
        public AsyncCommand()
        {
        }

        /// <summary>
        /// Constructor takes an execute async method which is 
        /// called when the Execute method is called
        /// </summary>
        /// <param name="execute">A function taken a type T and returning a Task</param>
        public AsyncCommand(Func<T, Task> execute)
        {
            ExecuteObjectCommand = execute;
        }

        /// <summary>
        /// Constructor takes an execute async method which is 
        /// called when the Execute method is called and a canExecute
        /// method called when CanExecute is called.
        /// </summary>
        /// <param name="execute">A function taking a type T and returning a Task</param>
        /// <param name="canExecute">A function taking a type T and returning a Task of type boolean</param>
        public AsyncCommand(Func<T, Task> execute, Func<T, Task<bool>> canExecute) :
            this(execute)
        {
            CanExecuteObjectCommand = canExecute;
        }

        /// <summary>
        /// Gets/Sets the async function to be used when the
        /// Execute method is called
        /// </summary>
        public Func<T, Task> ExecuteObjectCommand { get; set; }
        /// <summary>
        /// Gets/Sets the asyc function to be used when the
        /// CanExecuteCommand is called
        /// </summary>
        public Func<T, Task<bool>> CanExecuteObjectCommand { get; set; }

        /// <summary>
        /// Implementation of CanExecute which returns a boolean
        /// to allow the calling method to determine whether the
        /// Execute method can be called
        /// </summary>
        /// <param name="parameter">An object is ignored in this implementation</param>
        /// <returns>True if the command can be executed, otherwise False</returns>
        public override bool CanExecute(object parameter)
        {
            // slightly naff to use the Result like this, but need this method
            // signature to support ICommand
            var result = true;

            if (CanExecuteObjectCommand != null)
            {
                result = CanExecuteObjectCommand((T) parameter).Result;
            }

            return result && !IsBusy;
        }

        /// <summary>
        /// Executes the previously supplied ExecuteCommand
        /// </summary>
        /// <remarks>
        /// The Execute method will execute the supplied ExecuteCommand, setting
        /// the IsBusy flag to True and running on using a Task which, upon 
        /// completion will set the IsFlag back to False.
        /// </remarks>
        /// <param name="parameter">This parameter is igored within this implementation</param>
        public override void Execute(object parameter)
        {
            try
            {
                if (ExecuteObjectCommand != null)
                {
                    IsBusy = true;

                    ExecuteObjectCommand((T)parameter).
                        ContinueWith(CompleteTask,
                            TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Called when the Task completes, this will set the IsBusy flag
        /// to False and where possible, ensure any exceptions are rethrown
        /// on the main thread
        /// </summary>
        /// <param name="tsk">The task representing the executed method</param>
        private void CompleteTask(Task tsk)
        {
            IsBusy = false;

            if (tsk.IsFaulted)
            {
#if !NETSTANDARD2_0
                Dispatcher.CurrentDispatcher.Throw(tsk.Exception);
#endif
            }
        }

        /// <summary>
        /// Lazily creates a ReferenceCounter to allows us to 
        /// handle multiple calls to IsBusy
        /// </summary>
        /// <returns>The stored or a new ReferenceCounter</returns>
        private ReferenceCounter GetOrCreateBusyCount()
        {
            return _busyCount ?? (_busyCount = new ReferenceCounter());
        }

        /// <summary>
        /// Gets/Sets the IsBusy flag
        /// </summary>
        public bool IsBusy
        {
            get => GetOrCreateBusyCount().Count > 0;
            set
            {
                var busyCount = GetOrCreateBusyCount();
                var current = busyCount.Count;
                if ((value ? busyCount.AddRef() : busyCount.Release()) != current)
                {
                    OnPropertyChanged();
                    RaiseCanExecuteChanged();
                }
            }
        }
    }
}
