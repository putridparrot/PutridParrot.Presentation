using System;

namespace Presentation.Patterns.Helpers
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

        public DisposableAction(Action onCreate, Action onDispose)
        {
#if !NET4
            onCreate?.Invoke();
#else
            if (onCreate != null)
            {
                onCreate();
            }
#endif
            _onDispose = onDispose;
        }

        public void Dispose()
        {
#if !NET4
            _onDispose?.Invoke();
#else
            if (_onDispose != null)
            {
                _onDispose();
            }
#endif
        }
    }
}
