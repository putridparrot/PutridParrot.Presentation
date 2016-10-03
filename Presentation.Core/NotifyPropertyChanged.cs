using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Presentation.Core
{
    /// <summary>
    /// Implementation of an INotifyPropertyChanged interface. This is
    /// the base for more complex implementations but can be used in 
    /// situations where a lightweight implementation is preferred.
    /// </summary>
    public class NotifyPropertyChanged : IViewModel,
        INotifyPropertyChanged, INotifyPropertyChanging
    {
        private event PropertyChangedEventHandler propertyChanged;
        private event PropertyChangingEventHandler propertyChanging;

        [ExcludeFromCodeCoverage]
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        [ExcludeFromCodeCoverage]
        event PropertyChangingEventHandler INotifyPropertyChanging.PropertyChanging
        {
            add { propertyChanging += value; }
            remove { propertyChanging -= value; }
        }

#if !NET4
        protected virtual bool OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            var handler = propertyChanging;
            handler?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = propertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#else    
        protected virtual bool OnPropertyChanging(string propertyName = null)
        {
            var handler = propertyChanging;
            handler?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = propertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#endif

        bool IViewModel.RaisePropertyChanging(string propertyName)
        {
            return OnPropertyChanging(propertyName);
        }

        void IViewModel.RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        void IViewModel.RaisePropertyChanged(params string[] propertyNames)
        {
            if (propertyNames != null)
            {
                foreach (var propertyName in propertyNames)
                {
                    OnPropertyChanged(propertyName);
                }
            }
        }
    }
}