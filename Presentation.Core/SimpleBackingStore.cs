using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Presentation.Core
{
    /// <summary>
    /// A simple backing store mechanism to store fields
    /// in a dictionary.
    /// </summary>
    public class SimpleBackingStore : 
        IBackingStore, ISupportInitialize
    {
        private readonly Dictionary<string, object> _backingStore;
        private readonly object _sync = new object();

        public SimpleBackingStore()
        {
            _backingStore = new Dictionary<string, object>();
        }

        bool IBackingStore.Set<T>(string propertyName, T newValue, Func<T, T, string, bool> changing, Action<T, T, string> changed)
        {
            lock (_sync)
            {
                object value;
                if (!_backingStore.TryGetValue(propertyName, out value))
                {
                    value = default(T);
                }

                if (EqualityComparer<T>.Default.Equals((T)value, newValue))
                    return false;

                if (changing != null && !changing((T)value, newValue, propertyName))
                    return false;

                _backingStore[propertyName] = newValue;

#if !NET4
                changed?.Invoke((T)value, newValue, propertyName);
#else
                if (changed != null)
                {
                    changed((T) value, newValue, propertyName);
                }
#endif

                return true;
            }
        }

        T IBackingStore.Get<T>(string propertyName)
        {
            lock (_sync)
            {
                object value;
                if (_backingStore.TryGetValue(propertyName, out value))
                {
                    return (T)value;
                }
            }
            return default(T);
        }

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
        }
    }
}