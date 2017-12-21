using Presentation.Core.Helpers;
using Presentation.Core.Interfaces;

namespace Presentation.Core
{
    public interface ITabManager<T> where T : INotifyViewModel
    {
        T Selected { get; set; }
        ExtendedObservableCollection<T> Items { get; }

        void Load(T item);
        void LoadAll();
    }

    /// <summary>
    /// Classes can implement this to handle tab control view model
    /// mappings. Derived class should supply the Items collection with
    /// the view model types that the Tab control should support and
    /// then if the TabControl uses the ViewTemplateSelector it will
    /// automatically assign the correct view to each tab as it's selected.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TabManager<T> : NotifyPropertyChanged,
        ITabManager<T> where T : INotifyViewModel
    {
        private T _selectedTab;

        protected TabManager()
            : base()
        {
        }

        public virtual T Selected
        {
            get { return _selectedTab; }
            set { this.SetProperty(ref _selectedTab, value); }
        }

        public virtual void Load(T item)
        {
        }

        public void LoadAll()
        {
            foreach (var item in Items)
            {
                Load(item);
            }
        }

        public ExtendedObservableCollection<T> Items { get; protected set; }
    }

}
