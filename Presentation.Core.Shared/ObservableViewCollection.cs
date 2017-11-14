using System;
using System.Collections.Generic;
using Presentation.Core.Shared.Helpers;

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
        private int _selectedIndex = -1;
        private T _defaultValue;
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

        public T DefaultValue
        {
            get => _defaultValue;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_defaultValue, value))
                {
                    _defaultValue = value;
                    OnPropertyChanged(PropertyChangedEventFactory.Create());
                }
            }
        }

        public T SelectedItem
        {
            get => SelectedIndex < 0 ? DefaultValue : this[SelectedIndex];
            set
            {
                if (SelectedIndex < 0 || !EqualityComparer<T>.Default.Equals(this[SelectedIndex], value))
                {
                    SelectedIndex = IndexOf(value);
                    OnPropertyChanged(PropertyChangedEventFactory.Create());
                }
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged(PropertyChangedEventFactory.Create());
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

        protected override void ClearItems()
        {
            SelectedIndex = -1;
            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            if (index == SelectedIndex)
            {
                SelectedIndex = -1;
            }

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
            if (index == SelectedIndex && !EqualityComparer<T>.Default.Equals(this[SelectedIndex], item))
            {
                SelectedIndex = -1;
            }

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
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    ApplyFilter();
                    OnPropertyChanged(PropertyChangedEventFactory.Create());
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
                SelectedIndex = -1;
            }
            else
            {
                // filter has been removed so let's free memory etc.
                _filtered = null;
            }
            OnPropertyChanged(PropertyChangedEventFactory.Create(nameof(Filtered)));
        }
    }
}
