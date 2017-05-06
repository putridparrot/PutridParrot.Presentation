using System.ComponentModel;
using System.Runtime.CompilerServices;
using Presentation.Patterns.Interfaces;

namespace Presentation.Patterns
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
            var handler = PropertyChanging;
            handler?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#else    
        protected virtual bool OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangingEventArgs(propertyName));
            }
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
#endif

        /// <summary>
        /// Raises a property changing event using
        /// the OnPropertyChanging method.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool RaisePropertyChanging(string propertyName)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            return OnPropertyChanging(propertyName);
        }

        /// <summary>
        /// Raises a property changed event using the 
        /// OnPropertyChanged method. 
        /// </summary>
        /// <param name="propertyName"></param>
        public void RaisePropertyChanged(string propertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises multiple property changed events 
        /// against the OnPropertyChanged method.
        /// </summary>
        /// <param name="propertyNames"></param>
        public void RaiseMultiplePropertyChanged(params string[] propertyNames)
        {
            if (propertyNames != null)
            {
                foreach (var propertyName in propertyNames)
                {
                    // ReSharper disable once ExplicitCallerInfoArgument
                    OnPropertyChanged(propertyName);
                }
            }
        }
    }
}
