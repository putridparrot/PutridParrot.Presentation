using System;

namespace Presentation.Core
{ 
    /// <summary>
    /// Used as a backing storage mechanism within view models.
    /// </summary>
    public interface IBackingStore
    {
        /// <summary>
        /// The equivalent of a property setter. 
        /// </summary>
        /// <typeparam name="T">The type of the property to be stored</typeparam>
        /// <param name="propertyName">The property name as a string</param>
        /// <param name="newValue">The new value to be set</param>
        /// <param name="changing">An action to be called when a value is changing, but before it;'s changed</param>
        /// <param name="changed">An action to be called after a value has been changed</param>
        /// <returns>True if the value has changed, otherwise false</returns>
        bool Set<T>(string propertyName, T newValue, Func<T, T, string, bool> changing = null, Action<T, T, string> changed = null);
        /// <summary>
        /// The equivalent of a property getter.
        /// </summary>
        /// <typeparam name="T">The type of the property to be stored</typeparam>
        /// <param name="propertyName">The property name as a string</param>
        /// <returns>The value stored against the property name, otherwise default(T)</returns>
        T Get<T>(string propertyName);
    }
}