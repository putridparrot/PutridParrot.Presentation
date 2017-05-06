using System;

namespace Presentation.Patterns.Attributes
{
    /// <summary>
    /// Base class for view model property attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {       
    }
}