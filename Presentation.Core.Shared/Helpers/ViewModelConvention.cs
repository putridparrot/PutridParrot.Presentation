using System;
using System.Linq;

namespace PutridParrot.Presentation.Core.Helpers
{
    /// <summary>
    /// The class includes methods for view model for convention based
    /// view/view model methods
    /// </summary>
    public static class ViewModelConvention
    {
        /// <summary>
        /// Simple helper method for use with convention based
        /// code. Takes a view model name of convention
        /// XViewModel and returns the XView string.
        /// </summary>
        /// <param name="viewModelName"></param>
        /// <returns></returns>
        public static string GetViewName(string viewModelName)
        {
            const string viewmodel = "ViewModel";
            const string model = "Model";

            return viewModelName.EndsWith(viewmodel)
                ? viewModelName.Substring(0, viewModelName.Length - model.Length)
                : null;
        }

        /// <summary>
        /// Gets a view type based upon the supplied view model and the naming convention
        /// </summary>
        /// <param name="viewModelType">The view model type to be matched to a view</param>
        /// <returns>The view associated (via naming convention) with the supplied view model type</returns>
        public static Type GetViewType(Type viewModelType)
        {
            var viewName = GetViewName(viewModelType.Name);
            return viewModelType.Assembly.GetTypes().FirstOrDefault(t => t.Name == viewName);
        }
    }
}
