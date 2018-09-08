using System;
using System.Collections.Generic;
using PutridParrot.Presentation.Attributes;
using PutridParrot.Presentation.Helpers;
using PutridParrot.Presentation.Interfaces;

namespace PutridParrot.Presentation
{
    /// <summary>
    /// Implements common property functionality
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyCommon<T> : IProperty
    {
        private IEqualityComparer<T> _comparer;
        private DisposableAction _disposable;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PropertyCommon()
        {
            _comparer = EqualityComparer<T>.Default;
        }

        /// <summary>
        /// The Equals method compares to values of type T for
        /// equivalence
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second items to compare to the first</param>
        /// <returns>True if they items are equivalent, otherwise False</returns>
        public bool Equals(T x, T y)
        {
            return _comparer.Equals(x, y);
        }

        /// <summary>
        /// Gets/Sets the comparer object
        /// </summary>
        public object Comparer
        {
            get => _comparer;
            set
            {
                if (_comparer != value)
                {
                    var comparer = value as IEqualityComparer<T>;
                    _comparer = comparer ?? EqualityComparer<T>.Default;
                }
            }
        }

        /// <summary>
        /// Gets/Sets whether a property supports notifications
        /// </summary>
        public bool SupportsNotifications { get; set; }
        /// <summary>
        /// Gets/Sets any rules for a property
        /// </summary>
        public List<RuleAttribute> Rules { get; set; }

        /// <summary>
        /// Attaches to a property's notification events, if supported
        /// </summary>
        /// <param name="onCreate"></param>
        /// <param name="onDetach"></param>
        public void Attach(Action onCreate, Action onDetach)
        {
            if (SupportsNotifications)
            {
                Detach();
                _disposable = new DisposableAction(onCreate, onDetach);
            }
        }

        /// <summary>
        /// Detaches from a property's notification events, if supported
        /// </summary>
        public void Detach()
        {
            if (SupportsNotifications && _disposable != null)
            {
                _disposable.Dispose();
                _disposable = null;
            }
        }
    }
}