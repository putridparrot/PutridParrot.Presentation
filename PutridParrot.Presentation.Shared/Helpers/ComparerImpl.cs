using System;
using System.Collections;
using System.Collections.Generic;

namespace PutridParrot.Presentation.Helpers
{
    /// <summary>
    /// Implements IComparer&lt;T&gt;, IEqualityComparer&lt;T&gt; 
    /// and IComparer
    /// </summary>
    /// <typeparam name="T">The type of being compared</typeparam>
    public class ComparerImpl<T> : IComparer<T>, IEqualityComparer<T>, IComparer
    {
        private readonly Func<T, T, int> _objectComparer;
        private readonly Func<T, int> _objectHash;

        /// <summary>
        /// Constructor take a comparison function which expects two types of T and
        /// returns an integer where a value less than 0 indicates the first item is 
        /// before the second, a value of 0 indicates they're equivalent 
        /// and a value greater than 0 indicates the first item is after
        /// the second item.
        /// </summary>
        /// <param name="objectComparer"></param>
        public ComparerImpl(Func<T, T, int> objectComparer) :
            this(objectComparer, o => 0)
        {
        }

        /// <summary>
        /// Constructor take a comparison function which expects two types of T and
        /// returns an integer where a value less than 0 indicates the first item is 
        /// before the second, a value of 0 indicates they're equivalent 
        /// and a value greater than 0 indicates the first item is after
        /// the second item. This constructor also takes a function for the object 
        /// hash code method.
        /// </summary>
        /// <param name="objectComparer">A function to compare two items of type T</param>
        /// <param name="objectHash">A function to return the hash code for a given instance of type T</param>
        /// <exception cref="System.NullReferenceException">Thrown when the objectComparer is null</exception>
        public ComparerImpl(Func<T, T, int> objectComparer, Func<T, int> objectHash)
        {
            _objectComparer = objectComparer ?? throw new NullReferenceException("Comparer function cannot be null");
            _objectHash = objectHash ?? throw new NullReferenceException("Hash code function cannot be null");
        }

        /// <summary>
        /// Calls the supplied comparison method
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item to compare with the first</param>
        /// <returns>Returns the value from the comparison method</returns>
        public int Compare(T x, T y)
        {
            return _objectComparer(x, y);
        }

        /// <summary>
        /// Compares whether the first and second item are equivalent
        /// by calling the supplied comparison method.
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item to compare with the first</param>
        /// <returns>True if equivalent or False otherwise</returns>
        public bool Equals(T x, T y)
        {
            return Compare(x, y) == 0;
        }

        /// <summary>
        /// Calls the suppled has code method
        /// </summary>
        /// <param name="obj">The object to get the hash code from</param>
        /// <returns>The result of the hash method</returns>
        public int GetHashCode(T obj)
        {
            return _objectHash(obj);
        }

        /// <summary>
        /// Calls the supplied comparison method
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item to compare with the first</param>
        /// <returns>Returns the value from the comparison method</returns>
        public int Compare(object x, object y)
        {
            return Compare((T)x, (T)y);
        }
    }
}