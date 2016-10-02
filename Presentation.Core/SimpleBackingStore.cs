using System;
using System.Collections.Generic;

namespace Presentation.Core
{
    public class SimpleBackingStore : IBackingStore
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

                changed?.Invoke((T)value, newValue, propertyName);

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
    }
}