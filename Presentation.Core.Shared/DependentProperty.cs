using System;
using Presentation.Core.Interfaces;

namespace Presentation.Core
{
    /// <summary>
    /// Creates a property which is dependent upon other properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DependentProperty<T> : PropertyCommon<T>,
        IDependentProperty
    {
        private readonly Func<T> _valueFunc;

        public DependentProperty(Func<T> valueFunc)
        {
            _valueFunc = valueFunc;
        }

#if !NET4
        public T Value => _valueFunc();
#else
        public T Value
        {
            get { return _valueFunc(); }
        }
#endif

        public string[] DependentUpon { get; set; }
    }
}