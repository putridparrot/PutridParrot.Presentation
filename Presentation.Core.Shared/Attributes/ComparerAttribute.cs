using System;

namespace Presentation.Core.Attributes
{
    /// <summary>
    /// Used on a property to define the IEqualityComparer&lt;T&gt;
    /// to be used when deciding if a property has changed, by default
    /// EqualityComparer&lt;T&gt;.Default is used, so you only need
    /// to use this attribute if providing a different comparer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ComparerAttribute : PropertyAttribute
    {
        /// <summary>
        /// Constructor takes a type which should implement 
        /// IEqualityComparer&lt;T&gt; that acts as the 
        /// comparison method 
        /// </summary>
        /// <param name="comparerType"></param>
        public ComparerAttribute(Type comparerType)
        {
            ComparerType = comparerType;
        }

        /// <summary>
        /// The type of the comparer
        /// </summary>
        public Type ComparerType { get; }
    }
}
