using System;

namespace Presentation.Patterns.Attributes
{
    /// <summary>
    /// Allows the view model to create an instance
    /// of an object automatically using a factory
    /// class, supplied via it's type. This is useful
    /// for situations where you want to use customised
    /// creation logic, type must implement IFactory
    /// </summary>
    public sealed class CreateInstanceUsingAttribute : PropertyAttribute
    {
        public CreateInstanceUsingAttribute(Type factoryType)
        {
            FactoryType = factoryType;
        }

        /// <summary>
        /// The type for the factory code
        /// </summary>
        public Type FactoryType { get; private set; }
    }
}