using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

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
    public class ChangeTrackingObservableCollection<T> : ExtendedObservableCollection<T>
    {
        private ConcurrentDictionary<T, TrackedState> _changes;

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
            IsChanged = false;
        }
    }

}
