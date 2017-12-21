namespace Presentation.Core.Interfaces
{
    /// <summary>
    /// Used by a view model implementation to allow external code, such 
    /// as extensions classes to be written to work seemlessly with
    /// a view model.
    /// </summary>
    public interface INotifyViewModel
    {
        /// <summary>
        /// Raises a property changing event against the supplied property name
        /// </summary>
        /// <param name="propertyName">The property name to be used in the OnPropertyChanging event</param>
        /// <returns>True if successfully raise, else false</returns>
        bool RaisePropertyChanging(string propertyName);
        /// <summary>
        /// Raises a property changed event against the supplied property name
        /// </summary>
        /// <param name="propertyName">The property name to be used in the OnPropertyChanged event</param>
        /// <returns>True if successfully raise, else false</returns>
        void RaisePropertyChanged(string propertyName);
        /// <summary>
        /// Raises multiple property changed events against the supplied property names
        /// </summary>
        /// <param name="propertyNames">An array of property names as params</param>
        void RaiseMultiplePropertyChanged(params string[] propertyNames);
    }
}