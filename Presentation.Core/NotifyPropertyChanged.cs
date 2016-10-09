using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Presentation.Core
{
    /// <summary>
    /// Implementation of an INotifyPropertyChanged interface. This is
    /// the base for more complex implementations but can be used in 
    /// situations where a lightweight implementation is preferred.
    /// </summary>
    public class NotifyPropertyChanged : INotifyViewModel,
        INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

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
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangingEventArgs(propertyName));
            }
            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
#endif

        public bool RaisePropertyChanging(string propertyName)
        {
            return OnPropertyChanging(propertyName);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        public void RaiseMultiplePropertyChanged(params string[] propertyNames)
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