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

        public void AddRange(IEnumerable<T> e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
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

        public void BeginUpdate()
        {
            updating.AddRef();
        }

        public void EndUpdate()
        {
            if (updating.Release() == 0)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

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
