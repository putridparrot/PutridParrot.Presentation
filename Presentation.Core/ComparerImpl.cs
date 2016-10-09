using System;
using System.Collections;
using System.Collections.Generic;

namespace Presentation.Core
{
    /// <summary>
    /// A generalised implementation of three comparison interfaces
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public class ComparerImpl<T> : IComparer<T>, IEqualityComparer<T>, IComparer
    {
        private readonly Func<T, T, int> objectComparer;
        private readonly Func<T, int> objectHash;

        public ComparerImpl(Func<T, T, int> objectComparer) :
            this(objectComparer, o => 0)
        {
        }

        public ComparerImpl(Func<T, T, int> objectComparer, Func<T, int> objectHash)
        {
            if (objectComparer == null)
                throw new NullReferenceException("Comparer cannot be null");

            this.objectComparer = objectComparer;
            this.objectHash = objectHash;
        }

        public int Compare(T x, T y)
        {
            return objectComparer(x, y);
        }

        public bool Equals(T x, T y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(T obj)
        {
            return objectHash(obj);
        }

        #region IComparer Members

        public int Compare(object x, object y)
        {
            return Compare((T)x, (T)y);
        }

        #endregion
    }
}
