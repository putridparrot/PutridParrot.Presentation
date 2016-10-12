using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Core
{
    /// <summary>
    /// A backing store mechanism to store fields
    /// </summary>
    public class BackingStore : 
        IBackingStore, ISupportInitialize,
        ISupportRevertibleChangeTracking
    {
        private class CurrentValue
        {
            public object original;
            public object current;
        }

        private readonly Dictionary<string, CurrentValue> _backingStore;
        private readonly object _sync = new object();

        private bool _initializing;

        public BackingStore()
        {
            _backingStore = new Dictionary<string, CurrentValue>();
        }

        bool IBackingStore.Set<T>(string propertyName, T newValue, Func<T, T, string, bool> changing, Action<T, T, string> changed)
        {
            lock (_sync)
            {
                object value = default(T);
                CurrentValue currentValue;
                if (_backingStore.TryGetValue(propertyName, out currentValue))
                {
                    value = currentValue.current;
                }

                if (EqualityComparer<T>.Default.Equals((T)value, newValue))
                    return false;

                if (!_initializing && changing != null && !changing((T)value, newValue, propertyName))
                    return false;

                if (currentValue == null)
                {
                    _backingStore[propertyName] = new CurrentValue
                    {
                        original = !_initializing ? default(T) : newValue,
                        current = newValue
                    };
                }
                else
                {
                    currentValue.current = newValue;
                }

                if (!_initializing)
                {
#if !NET4
                changed?.Invoke((T)value, newValue, propertyName);
#else
                    if (changed != null)
                    {
                        changed((T) value, newValue, propertyName);
                    }
#endif
                }
                return true;
            }
        }

        T IBackingStore.Get<T>(string propertyName)
        {
            lock (_sync)
            {
                CurrentValue currentValue;
                if (_backingStore.TryGetValue(propertyName, out currentValue))
                {
                    return (T)currentValue.current;
                }
            }
            return default(T);
        }

        void ISupportRevertibleChangeTracking.AcceptChanges(Action<string> changing, Action<string> changed)
        {
            lock (_sync)
            {
                // change all original values to current values
                foreach (var kv in _backingStore)
                {
                    if (changing != null)
                    {
                        changing(kv.Key);
                    }
                    kv.Value.original = kv.Value.current;
                    if (changed != null)
                    {
                        changed(kv.Key);
                    }
                }
            }
        }

        void ISupportRevertibleChangeTracking.RejectChanges(Action<string> changing, Action<string> changed)
        {
            lock (_sync)
            {
                // change all current values back to original values
                foreach (var kv in _backingStore)
                {
                    if (changing != null)
                    {
                        changing(kv.Key);
                    }
                    kv.Value.current = kv.Value.original;
                    if (changed != null)
                    {
                        changed(kv.Key);
                    }
                }
            }
        }

        void ISupportInitialize.BeginInit()
        {
            _initializing = true;
        }

        void ISupportInitialize.EndInit()
        {
            _initializing = false;
        }
    }
}
