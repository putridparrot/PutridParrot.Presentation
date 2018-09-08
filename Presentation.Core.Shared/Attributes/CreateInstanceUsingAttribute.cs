using System;

namespace PutridParrot.Presentation.Core.Attributes
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
        /// <summary>
        /// Constructor which takes a type which should have a default
        /// constructor and returns an object which creates the instance
        /// of the property
        /// </summary>
        /// <param name="factoryType"></param>
        public CreateInstanceUsingAttribute(Type factoryType)
        {
            FactoryType = factoryType;
        }

        /// <summary>
        /// The type for the factory code
        /// </summary>
        public Type FactoryType { get; }
    }
}