using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Presentation.Core.Shared.Helpers
{
    /// <summary>
    /// Helper class to further simplify creation of
    /// PropertyChangedEventArgs
    /// </summary>
    public static class PropertyChangedEventFactory
    {
        /// <summary>
        /// Creates the PropertyChangedEventArgs from the supplied property name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyChangedEventArgs Create([CallerMemberName] string propertyName = null)
        {
            return new PropertyChangedEventArgs(propertyName);
        }
    }
}
