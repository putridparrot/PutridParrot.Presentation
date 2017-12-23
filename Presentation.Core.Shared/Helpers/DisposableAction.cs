using System;

namespace Presentation.Core.Helpers
{
    /// <summary>
    /// The DisposableAction creates a instance
    /// of a class which allows some form of action when 
    /// instantiated (optional) and another to be invoked 
    /// when disposed. This is useful for creating 
    /// event handlers without the need of Observables and Rx
    /// </summary>
    public class DisposableAction : IDisposable
    {
        private readonly Action _onDispose;

        /// <summary>
        /// Constructor which takes the method run upon creation
        /// of this instance and another for when the object
        /// is disposed of.
        /// </summary>
        /// <param name="onCreate">The method called when this is created</param>
        /// <param name="onDispose">The method called when this is disposed of</param>
        public DisposableAction(Action onCreate, Action onDispose)
        {
            onCreate?.Invoke();
            _onDispose = onDispose;
        }

        /// <summary>
        /// Calls the supplied method when this object is disposed of
        /// </summary>
        public void Dispose()
        {
            _onDispose?.Invoke();
        }
    }
}
