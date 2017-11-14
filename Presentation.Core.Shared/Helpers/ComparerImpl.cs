using System;
using System.Collections;
using System.Collections.Generic;

namespace Presentation.Patterns.Helpers
{
    public class ComparerImpl<T> : IComparer<T>, IEqualityComparer<T>, IComparer
    {
        private readonly Func<T, T, int> _objectComparer;
        private readonly Func<T, int> _objectHash;

        public ComparerImpl(Func<T, T, int> objectComparer) :
            this(objectComparer, o => 0)
        {
        }

        public ComparerImpl(Func<T, T, int> objectComparer, Func<T, int> objectHash)
        {
            _objectComparer = objectComparer ?? throw new NullReferenceException("Comparer cannot be null");
            _objectHash = objectHash;
        }

        public int Compare(T x, T y)
        {
            return _objectComparer(x, y);
        }

        public bool Equals(T x, T y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(T obj)
        {
            return _objectHash(obj);
        }

        public int Compare(object x, object y)
        {
            return Compare((T)x, (T)y);
        }
    }
}