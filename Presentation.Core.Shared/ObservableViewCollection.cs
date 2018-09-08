using System;
using System.Collections.Generic;
using System.Windows.Input;
using PutridParrot.Presentation.Helpers;
using PutridParrot.Presentation.Interfaces;

namespace PutridParrot.Presentation
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

        private ICommand _addCommand;
        private ICommand _deleteCommand;
        private ICommand _clearCommand;

        /// <summary>
        /// Default constructor
        /// </summary>
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
                    OnPropertyChanged(PropertyChangedEventFactory.Create(nameof(IsSelectedValid)));

                    if (DeleteCommand is IRaiseCanExecuteChanged deleteCommand)
                    {
                        deleteCommand.RaiseCanExecuteChanged();
                    }
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

            if (ClearCommand is IRaiseCanExecuteChanged clearCommand)
            {
                clearCommand.RaiseCanExecuteChanged();
            }
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

        public bool IsSelectedValid => SelectedIndex >= 0 && SelectedIndex < Count;

        /// <summary>
        /// Get/set an add command to allow this class
        /// to particpate in commands. The default 
        /// implementation is lazy created and will add a
        /// default(T) item or a value of type T passed via 
        /// the command parameters
        /// </summary>
        public ICommand AddCommand
        {
            get => _addCommand ?? 
                   (_addCommand = new ActionCommand<T>(Add));
            set
            {
                if (_addCommand != value)
                {
                    _addCommand = value;
                    OnPropertyChanged(PropertyChangedEventFactory.Create());
                }
            }
        }

        /// <summary>
        /// Get/set a delete command to allow this class
        /// to particpate in commands. The default 
        /// implementation is lazy created and will add a
        /// remove the item which at the SelectedIndex. It
        /// does not do anything with command parsm
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? 
                    (_deleteCommand = new ActionCommand(
                           () =>
                           {
                               if (IsSelectedValid)
                               {
                                   RemoveItem(SelectedIndex);
                               }
                           },
                           () => IsSelectedValid));
            }
            set
            {
                if (_deleteCommand != value)
                {
                    _deleteCommand = value;
                    OnPropertyChanged(PropertyChangedEventFactory.Create());
                }
            }
        }

        /// <summary>
        /// Get/set an clear command to allow this class
        /// to particpate in commands. The default 
        /// implementation is lazy created and will clear 
        /// all items from the collection, command parameters
        /// are ignored.
        /// </summary>
        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? 
                    (_clearCommand = new ActionCommand(Clear,
                           () => Count > 0));
            }
            set
            {
                if (_clearCommand != value)
                {
                    _clearCommand = value;
                    OnPropertyChanged(PropertyChangedEventFactory.Create());
                }
            }
        }
    }
}
