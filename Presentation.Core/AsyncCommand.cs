using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Presentation.Core
{
    /// <summary>
    /// A Task/async aware command
    /// </summary>
    public class AsyncCommand : NotifyPropertyChanged, ICommand
    {
        private readonly Func<object, Task> _executeObjectCommand;
        private readonly Func<object, Task<bool>> _canExecuteObjectCommand;
        private readonly Func<Task> _executeCommand;
        private readonly Func<Task<bool>> _canExecuteCommand;

        private readonly ReferenceCounter _busyCount = new ReferenceCounter();

        public event EventHandler CanExecuteChanged;

        public AsyncCommand()
        {
        }

        public AsyncCommand(Func<object, Task> execute)
        {
            _executeObjectCommand = execute;
        }

        public AsyncCommand(Func<Task> execute)
        {
            _executeCommand = execute;
        }

        public AsyncCommand(Func<object, Task> execute, Func<object, Task<bool>> canExecute)
        {
            _executeObjectCommand = execute;
            _canExecuteObjectCommand = canExecute;
        }

        public AsyncCommand(Func<Task> execute, Func<Task<bool>> canExecute)
        {
            _executeCommand = execute;
            _canExecuteCommand = canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            // slightly naff to use the Result like this, but need this method
            // signature to support ICommand
            bool result = true;

            if (_canExecuteObjectCommand != null)
                result = _canExecuteObjectCommand(parameter).Result;
            else if (_canExecuteCommand != null)
                result = _canExecuteCommand().Result;

            return result && !IsBusy;
        }

        public void Execute(object parameter)
        {
            try
            {
                if (_executeObjectCommand != null)
                {
                    IsBusy = true;

                    _executeObjectCommand(parameter).
                        ContinueWith(CompleteTask,
                            TaskScheduler.FromCurrentSynchronizationContext());
                }
                else if (_executeCommand != null)
                {
                    IsBusy = true;

                    _executeCommand().
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

        public bool IsBusy
        {
            get { return _busyCount.Count > 0; }
            set
            {
                var current = _busyCount.Count;
                if ((value ? _busyCount.AddRef() : _busyCount.Release()) != current)
                {
                    OnPropertyChanged("IsBusy");
                    RaiseCanExecuteChanged();
                }
            }
        }
    }

}
