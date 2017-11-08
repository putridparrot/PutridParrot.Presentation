namespace Presentation.Patterns.Interfaces
{
    /// <summary>
    /// Should be implemented by types which act as factory class.
    /// Specifically used by the view model and the 
    /// CreateInstanceUsingAttribute.
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Gets a new instance of a supported type
        /// </summary>
        /// <returns>A new instance of the type it supports</returns>
        object Create(params object[] args);
    }
}
