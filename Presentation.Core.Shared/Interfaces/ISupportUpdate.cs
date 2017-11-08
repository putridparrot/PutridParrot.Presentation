namespace Presentation.Patterns.Interfaces
{
    /// <summary>
    /// Implement by types which support 
    /// BeginUpdate/EndUpdate functionality
    /// </summary>
    public interface ISupportUpdate
    {
        /// <summary>
        /// Gets whether the object is in updating mode or not
        /// </summary>
        bool IsUpdating { get; }
        /// <summary>
        /// Puts the object into update mode
        /// </summary>
        void BeginUpdate();
        /// <summary>
        /// Ends update mode on the object
        /// </summary>
        void EndUpdate();
    }
}