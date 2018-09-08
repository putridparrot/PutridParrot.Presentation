using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PutridParrot.Presentation.Core.Helpers
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
        /// <param name="propertyName">The property name to be used in the PropertyChangedEventArgs</param>
        /// <returns>A new instance of the PropertyChangedEventArgs</returns>
        public static PropertyChangedEventArgs Create([CallerMemberName] string propertyName = null)
        {
            return new PropertyChangedEventArgs(propertyName);
        }
    }
}
