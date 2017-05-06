using System;
using System.Linq;

namespace Presentation.Patterns.Helpers
{
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

        public static Type GetViewType(Type viewModelType)
        {
            var viewName = GetViewName(viewModelType.Name);
            return viewModelType.Assembly.GetTypes().FirstOrDefault(t => t.Name == viewName);
        }
    }
}
