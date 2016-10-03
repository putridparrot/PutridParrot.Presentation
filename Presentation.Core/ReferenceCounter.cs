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

        public int AddRef()
        {
            lock (syncObject)
                return ++refCount;
        }

        public int Release()
        {
            lock (syncObject)
                return refCount = refCount > 0 ? refCount - 1 : 0;
        }

        public void Reset()
        {
            lock (syncObject)
                refCount = 0;
        }

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
