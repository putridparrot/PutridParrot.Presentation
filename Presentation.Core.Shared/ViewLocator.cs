using System;
using Presentation.Patterns.Helpers;

namespace Presentation.Patterns
{
    public interface IViewLocator
    {
        object CreateView(Type viewModelType);
    }

    /// <summary>
    /// Simple convention based view locator.
    /// </summary>
    public class ViewLocator : IViewLocator
    {
        /// <summary>
        /// Locates and then creates a view based upon the
        /// convention of the view model type being named
        /// XViewModel and a corresponding XView exists.
        /// </summary>
        /// <param name="viewModelType">The type of the view model</param>
        /// <returns>An instance of the view or null</returns>
        public object CreateView(Type viewModelType)
        {
            var viewType = ViewModelConvention.GetViewType(viewModelType);
            return viewType != null ? Activator.CreateInstance(viewType) : null;
        }
    }
}
