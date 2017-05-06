namespace Presentation.Patterns.Helpers
{
    /// <summary>
    /// Simple reference counter, it doesn't store
    /// anything except the reference count - so is basically
    /// a threadsafe counter
    /// </summary>
    public class ReferenceCounter
    {
        private int _refCount;
        private readonly object _syncObject = new object();

        public int AddRef()
        {
            lock (_syncObject)
                return ++_refCount;
        }

        public int Release()
        {
            lock (_syncObject)
                return _refCount != 0 ? --_refCount : 0;
        }

        public void Reset()
        {
            lock (_syncObject)
                _refCount = 0;
        }

        public int Count
        {
            get
            {
                lock (_syncObject)
                    return _refCount;
            }
        }
    }

}
