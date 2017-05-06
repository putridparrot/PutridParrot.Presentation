namespace Presentation.Patterns.Interfaces
{
    /// <summary>
    /// Should be implemented by a property which
    /// supports dependency tracking - this is
    /// internally used for ReadOnly properties
    /// </summary>
    public interface IDependentProperty
    {
        string[] DependentUpon { get; set; }
    }
}