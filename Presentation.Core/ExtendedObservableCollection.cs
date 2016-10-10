using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace Presentation.Core
{
    /// <summary>
    /// A dispatcher aware observable collection. As the default ObservableCollection does
    /// not marshal changes onto the UI thread, this class handled such marshalling as well
    /// as offering the ability to Begin and End updates, so trying to only fire update events
    /// when necessary.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtendedObservableCollection<T> : ObservableCollection<T>, IItemChanged
    {
        public event PropertyChangedEventHandler ItemChanged;

        private readonly ReferenceCounter updating = new ReferenceCounter();

        /// <summary>
        /// Adds multiple items to the collection, without calling
        /// the collection changed event until all items have been 
        /// added
        /// </summary>
        /// <param name="e">The items as an IEnumerable</param>
        public void AddRange(IEnumerable<T> e)
        {
            if (e == null)
            {
#if !NET4
                throw new ArgumentNullException(nameof(e));
#else
                throw new ArgumentNullException("e");
#endif
            }

            try
            {
                BeginUpdate();

                foreach (var item in e)
                {
                    Add(item);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Switch the collection into update mode
        /// </summary>
        public void BeginUpdate()
        {
            updating.AddRef();
        }

        /// <summary>
        /// End update mode on the collection, this will 
        /// cause collection changed events
        /// </summary>
        public void EndUpdate()
        {
            if (updating.Release() == 0)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Sort the collection using the supplied comparison delegate
        /// </summary>
        /// <param name="comparison">Delegate used to compare items</param>
        public void Sort(Comparison<T> comparison)
        {
            try
            {
                BeginUpdate();

                ListExtensions.Sort(this, comparison);
            }
            finally
            {
                EndUpdate();
            }
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (updating.Count <= 0)
            {
                //base.OnCollectionChanged(e);
                // Taken from http://stackoverflow.com/questions/2104614/updating-an-observablecollection-in-a-separate-thread
                // to allow marshalling onto the UI thread, seems a neat solution
                var eventHandler = CollectionChanged;
                if (eventHandler != null)
                {
                    var dispatcher = (from NotifyCollectionChangedEventHandler n in eventHandler.GetInvocationList()
                                             let dpo = n.Target as DispatcherObject
                                             where dpo != null
                                             select dpo.Dispatcher).FirstOrDefault();

                    if (dispatcher != null && !dispatcher.CheckAccess())
                    {
                        dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
                    }
                    else
                    {
                        foreach (var eh in eventHandler.GetInvocationList())
                        {
                            var n = (NotifyCollectionChangedEventHandler) eh;
                            n.Invoke(this, e);
                        }
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var propertyChanged = item as INotifyPropertyChanged;
                    if (propertyChanged != null)
                    {
                        propertyChanged.PropertyChanged += ItemPropertyChanged;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var propertyChanged = item as INotifyPropertyChanged;
                    if (propertyChanged != null)
                    {
                        propertyChanged.PropertyChanged -= ItemPropertyChanged;
                    }
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var itemChanged = ItemChanged;
#if !NET4
            itemChanged?.Invoke(sender, propertyChangedEventArgs);
#else
            if (itemChanged != null)
            {
                itemChanged(sender, propertyChangedEventArgs);
            }
#endif
        }
    }

}
