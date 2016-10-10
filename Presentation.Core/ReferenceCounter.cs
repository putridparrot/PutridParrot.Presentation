namespace Presentation.Core
{
    /// <summary>
    /// Simple reference counter, it doesn't store
    /// anything except the reference count - so is basically
    /// a threadsafe counter
    /// </summary>
    public class ReferenceCounter
    {
        private int refCount;
        private readonly object syncObject = new object();

        /// <summary>
        /// Increments the reference counter
        /// </summary>
        /// <returns>The new reference count</returns>
        public int AddRef()
        {
            lock (syncObject)
                return ++refCount;
        }

        /// <summary>
        /// Decrements the reference counter
        /// </summary>
        /// <returns>The new reference count</returns>
        public int Release()
        {
            lock (syncObject)
                return refCount = refCount > 0 ? refCount - 1 : 0;
        }

        /// <summary>
        /// Resets the reference count to zero
        /// </summary>
        public void Reset()
        {
            lock (syncObject)
                refCount = 0;
        }

        /// <summary>
        /// Gets the current reference count
        /// </summary>
        public int Count
        {
            get
            {
                lock (syncObject)
                    return refCount;
            }
        }
    }

    /// <summary>
    /// Reference counter which stores an object with 
    /// a count around the number of reference to it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReferenceCounter<T> : ReferenceCounter
    {
        private T value;

        public ReferenceCounter()
        {
        }

        public ReferenceCounter(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets/sets an item which is associated with 
        /// a reference counter
        /// </summary>
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!ReferenceEquals(this.value, value))
                {
                    this.value = value;
                    Reset();
                }
            }
        }
    }

}
