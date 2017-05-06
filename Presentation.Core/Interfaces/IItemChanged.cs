using System.ComponentModel;

namespace Presentation.Patterns.Interfaces
{
    /// <summary>
    /// Defines the interface which collections
    /// should implement to fire item change events
    /// </summary>
    public interface IItemChanged
    {
        event PropertyChangedEventHandler ItemChanged;
    }
}
