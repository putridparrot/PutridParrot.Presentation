using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Presentation.Patterns.Helpers;

namespace Presentation.Patterns
{
    /// <summary>
    /// A Task/async aware command object. Automatically handles changes
    /// to the IsBusy flag
    /// </summary>
    public class AsyncCommand : CommandCommon
    {
        private ReferenceCounter _busyCount;

        public AsyncCommand()
        {
        }

        public AsyncCommand(Func<Task> execute)
        {
            ExecuteCommand = execute;
        }

        public AsyncCommand(Func<Task> execute, Func<Task<bool>> canExecute) :
            this(execute)
        {
            CanExecuteCommand = canExecute;
        }

        public Func<Task> ExecuteCommand { get; set; }
        public Func<Task<bool>> CanExecuteCommand { get; set; }

        public override bool CanExecute(object parameter)
        {
            // slightly naff to use the Result like this, but need this method
            // signature to support ICommand
            bool result = true;
            if(CanExecuteCommand != null)
                result = CanExecuteCommand().Result;

            return result && !IsBusy;
        }

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

        private void CompleteTask(Task tsk)
        {
            IsBusy = false;

            if (tsk.IsFaulted)
            {
                Dispatcher.CurrentDispatcher.Throw(tsk.Exception);
            }
        }

        private ReferenceCounter GetOrCreateBusyCount()
        {
            return _busyCount ?? (_busyCount = new ReferenceCounter());
        }

        public bool IsBusy
        {
            get { return GetOrCreateBusyCount().Count > 0; }
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

        public AsyncCommand()
        {
        }

        public AsyncCommand(Func<T, Task> execute)
        {
            ExecuteObjectCommand = execute;
        }

        public AsyncCommand(Func<T, Task> execute, Func<T, Task<bool>> canExecute) :
            this(execute)
        {
            CanExecuteObjectCommand = canExecute;
        }

        public Func<T, Task> ExecuteObjectCommand { get; set; }
        public Func<T, Task<bool>> CanExecuteObjectCommand { get; set; }

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

        private void CompleteTask(Task tsk)
        {
            IsBusy = false;

            if (tsk.IsFaulted)
            {
                Dispatcher.CurrentDispatcher.Throw(tsk.Exception);
            }
        }

        private ReferenceCounter GetOrCreateBusyCount()
        {
            return _busyCount ?? (_busyCount = new ReferenceCounter());
        }

        public bool IsBusy
        {
            get { return GetOrCreateBusyCount().Count > 0; }
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
