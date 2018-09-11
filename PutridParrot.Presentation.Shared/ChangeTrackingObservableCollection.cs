using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace PutridParrot.Presentation.Shared
{
    /// <summary>
    /// ChangeTrackingObservableCollection is an ExtendedObservableCollection which 
    /// tracks changes to it, i.e. added, remove items and also where the supplied
    /// type supports INotifyPropertyChanged events, item edits. The changes are kept
    /// in an internal structure so that as some point the developer might use the 
    /// changes.
    /// 
    /// For example if we need to send changes to a REST service, we can send just the
    /// changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChangeTrackingObservableCollection<T> : ExtendedObservableCollection<T>,
        ISupportInitializeNotification
    {
        private ConcurrentDictionary<T, TrackedState> _changes;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _initializeCounter;

        // ReSharper disable once InconsistentNaming
        private event EventHandler _initialized;

        protected override void ItemPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.ItemPropertyChanged(sender, propertyChangedEventArgs);
            // edited items
            AddToTracking((T)sender, TrackedState.Edited);
        }

        private void AddToTracking(T item, TrackedState state)
        {
            if (!IsInitializing)
            {
                var tracking = GetOrCreateTracking();

                TrackedState current;
                if (tracking.TryGetValue(item, out current))
                {
                    // 1. if the states are the same, do nothing
                    if (current != state)
                    {
                        if (current == TrackedState.Added)
                        {
                            // 2. if the current state is add, ignore if now edited, but remove if deleted
                            if (state == TrackedState.Deleted)
                            {
                                // remove don't care about the state
                                tracking.TryRemove(item, out current);
                                // this might be an issue with concurrency
                                if (tracking.IsEmpty)
                                {
                                    ResetChanges();
                                }
                            }
                        }
                        else
                        {
                            tracking.TryUpdate(item, state, current);
                        }
                    }
                }
                else
                {
                    tracking.TryAdd(item, state);
                }

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            // added items
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (var item in e.NewItems)
                {
                    AddToTracking((T)item, TrackedState.Added);
                }
            }

            // removed items
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    AddToTracking((T)item, TrackedState.Deleted);
                }
            }
        }

        private ConcurrentDictionary<T, TrackedState> GetOrCreateTracking()
        {
            return _changes ?? (_changes = new ConcurrentDictionary<T, TrackedState>());
        }

        /// <summary>
        /// Gets an enumerable of all the tracked changes, i.e.
        /// when saving we'll want to get at the changes
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<T, TrackedState>[] GetTrackedChanges()
        {
            return _changes?.ToArray();
        }

        /// <summary>
        /// Clear/reset the tracked changes, i.e. after a save of changes
        /// we would want to reset our changes list
        /// </summary>
        public void ResetChanges()
        {
            _changes = null;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        /// <summary>
        /// Gets whether the collection or items within it (which 
        /// support INotifyPropertyChanged events) have changed
        /// </summary>
        public bool IsChanged => _changes != null && _changes.Count > 0;

        /// <summary>
        /// Gers whether the collection is initializing, 
        /// Denoted be BeginInit being called and EndInit
        /// not yet called.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected bool IsInitializing => _initializeCounter > 0;

        /// <summary>
        /// Gets wther the collection is not in 
        /// an initialization state.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected bool IsInitialized => _initializeCounter <= 0;

        /// <summary>
        /// Puts the collection into an initialization
        /// state, EndInit must be called to stop
        /// initialization
        /// </summary>
        public void BeginInit()
        {
            _initializeCounter++;
        }

        /// <summary>
        /// End the initialization state, updates will
        /// not be detected and events thrown
        /// </summary>
        public void EndInit()
        {
            if (_initializeCounter > 0 && --_initializeCounter <= 0)
            {
                var initialized = _initialized;
#if !NET4
                initialized?.Invoke(this, EventArgs.Empty);
#else
                if (initialized != null)
                {
                    initialized.Invoke(this, EventArgs.Empty);
                }
#endif
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        event EventHandler ISupportInitializeNotification.Initialized
        {
            add { _initialized += value; }
            remove { _initialized -= value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool ISupportInitializeNotification.IsInitialized => IsInitialized;
    }
}
