using System;

namespace PutridParrot.Presentation.Attributes
{
    /// <summary>
    /// Base class for view model property attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {       
    }
}