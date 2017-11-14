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
        public static PropertyChangedEventArgs Create([CallerMemberName] string propertyName = null)
        {
            return new PropertyChangedEventArgs(propertyName);
        }
    }
}
