using System;
using Presentation.Core.Attributes;

namespace Presentation.Core.Attributes
{
    /// <summary>
    /// The attribute is used on a property to set changes tracking on
    /// or off (by default change tracking will be assumed to be on, so 
    /// you only need to really use this to turn off change tracking, unless
    /// you want to be explicit in usage.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ChangeTrackingAttribute : PropertyAttribute
    {
        public ChangeTrackingAttribute(bool isTracking = true)
        {
            IsTracking = isTracking;
        }

        /// <summary>
        /// Defines whether the property should be participate
        /// in the change tracking , i.e. IsTracking = false
        /// means changes to this property should not be tracked
        /// </summary>
        public bool IsTracking { get; }
    }
}