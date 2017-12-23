using System;
using Presentation.Core.Interfaces;

namespace Presentation.Core.Attributes
{
    /// <summary>
    /// When a property, with this attribute, changes (i.e.
    /// OnPropertyChanged is called) it can cause another
    /// property change event to occur on another property.
    /// </summary>
    /// <example>
    /// We might have a Details property which is a list of
    /// items, by default listing all items. When a FilterByUser 
    /// property is set to True the Details should change to 
    /// represent just the user's Details. But as the Details 
    /// property might only have a getter (for example if 
    /// returning an ICollectionView) then we need a way to 
    /// tell the UI to refresh the Details on FilterByUser
    /// changing. We can therefore chain these propetries 
    /// to achieve this.
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyChainAttribute : RuleAttribute
    {
        /// <summary>
        /// Constructor takes a string array of property names
        /// which are "chained" to the property this is applied
        /// to. Thus when the property changes the chained
        /// properties are also raise in the OnPropertyChanged 
        /// method.
        /// </summary>
        /// <param name="properties">The property names to be chained</param>
        public PropertyChainAttribute(params string[] properties)
        {
            Properties = properties;
        }

        /// <summary>
        /// The array of properties to fire property change events
        /// on when the property associated with this attribute 
        /// changes
        /// </summary>
        public string[] Properties { get; }

        /// <summary>
        /// Called after a property change event occurs and
        /// by default, raises property change events for the
        /// chained properties.
        /// </summary>
        /// <param name="o">A view model which should implement INotifyViewModel</param>
        /// <returns>True upon success</returns>
        public override bool PostInvoke(object o)
        {
            if (Properties != null)
            {
                var vm = o as INotifyViewModel;
                vm?.RaiseMultiplePropertyChanged(Properties);
            }
            return true;
        }
    }
}
