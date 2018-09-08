using System;

namespace PutridParrot.Presentation.Attributes
{
    /// <summary>
    /// Allows the view model to create an instance
    /// of an object automatically (assuming a default ctor).
    /// This is useful for getter only properties or situations
    /// where DefaultValue cannot handle the type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CreateInstanceAttribute : PropertyAttribute
    {
    }
}
