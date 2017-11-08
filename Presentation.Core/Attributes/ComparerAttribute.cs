using System;

namespace Presentation.Patterns.Attributes
{
    /// <summary>
    /// Used on a property to define the IEqualityComparer&lt;T&gt;
    /// to be used when deciding if a property has changed, by default
    /// EqualityComparer&lt;T&gt;.Default is used, so you only need
    /// to use this attribute if providing a different comparer
    /// </summary>
    public sealed class ComparerAttribute : PropertyAttribute
    {
        public ComparerAttribute(Type comparerType)
        {
            ComparerType = comparerType;
        }

        /// <summary>
        /// The type of the comparer
        /// </summary>
        public Type ComparerType { get; private set; }
    }
}
