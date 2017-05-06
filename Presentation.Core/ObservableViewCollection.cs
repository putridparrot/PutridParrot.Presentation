using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Presentation.Patterns
{
    /// <summary>
    /// Used to implement a collection with more
    /// "view" type capabilities, such as a selected
    /// options and filtering. If you're after grouping
    /// etc. then you should use an ICollectionView
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableViewCollection<T> : ExtendedObservableCollection<T>
    {
        private T _selected;
        private Predicate<T> _filter;
        private ExtendedObservableCollection<T> _filtered;

        public ObservableViewCollection() :
            base()
        {
        }

        public ObservableViewCollection(List<T> list) :
            base(list)
        {
        }

        public ObservableViewCollection(IEnumerable<T> collection) :
            base(collection)
        {
        }

        public T Selected
        {
            get { return _selected; }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_selected, value))
                {
                    _selected = Contains(value) ? value : default(T);
                    OnPropertyChanged(new PropertyChangedEventArgs("Selected"));
                }
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            if (_filter != null && _filter(item))
            {
                // this item is filtered so need to refresh
                ApplyFilter();
            }
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];

            base.RemoveItem(index);

            if (_filter != null && _filter(item))
            {
                // this item is filtered so need to refresh
                ApplyFilter();
            }
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];

            base.SetItem(index, item);

            if (_filter != null && (_filter(oldItem) || _filter(item)))
            {
                // this item is filtered so need to refresh
                ApplyFilter();
            }
        }

        public Predicate<T> Filter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    ApplyFilter();
                    OnPropertyChanged(new PropertyChangedEventArgs("Filter"));
                }
            }
        }

        public ExtendedObservableCollection<T> Filtered
        {
            get
            {
                if (_filtered == null)
                {
                    _filtered = new ExtendedObservableCollection<T>();
                    ApplyFilter();
                }
                return _filtered;
            }
        }

        private void ApplyFilter()
        {
            if (_filtered != null)
            {
                _filtered.BeginUpdate();
                _filtered.Clear();
                foreach (var item in this)
                {
                    if (_filter == null || _filter(item))
                    {
                        _filtered.Add(item);
                    }
                }
                _filtered.EndUpdate();
                Selected = default(T);
            }
            else
            {
                // filter has been removed so let's free memory etc.
                _filtered = null;
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Filtered"));
        }

    }
}
