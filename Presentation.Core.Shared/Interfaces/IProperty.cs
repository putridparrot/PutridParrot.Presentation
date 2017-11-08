using System;
using System.Collections.Generic;
using Presentation.Patterns.Attributes;

namespace Presentation.Patterns.Interfaces
{
    public interface IProperty
    {
        /// <summary>
        /// When creating a property the view model will
        /// call this method to attach event handlers
        /// </summary>
        /// <param name="onCreate"></param>
        /// <param name="onDetach"></param>
        void Attach(Action onCreate, Action onDetach);
        /// <summary>
        /// When disposing of a property the view model will
        /// call this method to allow the property to carry
        /// out cleanup of event handlers
        /// </summary>
        void Detach();
        /// <summary>
        /// Defines whether a property has interfaces which support
        /// change notifications, for example INotifyPropertyChanged, 
        /// INotifyCollectionChange and IItemChanged
        /// </summary>
        bool SupportsNotifications { get; set; }
        /// <summary>
        /// A list of rule attributes to be executed upon a property change
        /// </summary>
        List<RuleAttribute> Rules { get; set; }
    }
}