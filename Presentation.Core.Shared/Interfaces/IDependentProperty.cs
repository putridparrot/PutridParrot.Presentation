namespace PutridParrot.Presentation.Core.Interfaces
{
    /// <summary>
    /// Should be implemented by a property which
    /// supports dependency tracking - this is
    /// internally used for ReadOnly properties
    /// </summary>
    public interface IDependentProperty
    {
        /// <summary>
        /// Gets/Sets a collection of property names that are dependent
        /// upon this property
        /// </summary>
        string[] DependentUpon { get; set; }
    }
}