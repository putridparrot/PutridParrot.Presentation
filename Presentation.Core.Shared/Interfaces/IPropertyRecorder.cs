using System.Collections.Generic;

namespace Presentation.Patterns.Interfaces
{
    /// <summary>
    /// Should be implemented by a type supporting
    /// recording and playback of property changes
    /// </summary>
    public interface IPropertyRecorder
    {
        /// <summary>
        /// Called by the view model when a properyt changes
        /// but is deferred/suppressed
        /// </summary>
        /// <param name="propertyName"></param>
        void Record(string propertyName);
        /// <summary>
        /// Returns an IEnumerable of property changes for
        /// playback by the view model
        /// </summary>
        /// <returns></returns>
        string[] Playback();
    }
}
