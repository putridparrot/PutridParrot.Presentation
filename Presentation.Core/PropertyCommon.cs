using System;
using System.Collections.Generic;
using Presentation.Patterns.Attributes;
using Presentation.Patterns.Helpers;
using Presentation.Patterns.Interfaces;

namespace Presentation.Patterns
{
    public class PropertyCommon<T> : IProperty
    {
        private IEqualityComparer<T> _comparer;
        private DisposableAction _disposable;

        public PropertyCommon()
        {
            _comparer = EqualityComparer<T>.Default;
        }

        public bool Equals(T x, T y)
        {
            return _comparer.Equals(x, y);
        }

        public object Comparer
        {
            get { return _comparer; }
            set
            {
                if (_comparer != value)
                {
                    var comparer = value as IEqualityComparer<T>;
                    _comparer = comparer ?? EqualityComparer<T>.Default;
                }
            }
        }

        public bool SupportsNotifications { get; set; }
        public List<RuleAttribute> Rules { get; set; }

        public void Attach(Action onCreate, Action onDetach)
        {
            if (SupportsNotifications)
            {
                Detach();
                _disposable = new DisposableAction(onCreate, onDetach);
            }
        }

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