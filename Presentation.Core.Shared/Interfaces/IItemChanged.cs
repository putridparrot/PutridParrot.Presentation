using System.ComponentModel;

namespace Presentation.Core.Interfaces
{
    /// <summary>
    /// Defines the interface which collections
    /// should implement to fire item change events
    /// </summary>
    public interface IItemChanged
    {
        /// <summary>
        /// The ItemChanged event
        /// </summary>
        event PropertyChangedEventHandler ItemChanged;
    }
}
