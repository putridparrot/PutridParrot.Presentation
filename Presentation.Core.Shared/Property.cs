namespace Presentation.Core
{
    /// <summary>
    /// Creates a basic property
    /// </summary>
    /// <typeparam name="T">The type of the property</typeparam>
    public class Property<T> : PropertyCommon<T>
    {
        /// <summary>
        /// Gets/Sets the value on the property
        /// </summary>
        public T Value { get; set; }
    }
}