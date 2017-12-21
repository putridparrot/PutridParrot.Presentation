namespace Presentation.Core
{
    /// <summary>
    /// Creates a basic property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Property<T> : PropertyCommon<T>
    {
        public T Value { get; set; }
    }
}