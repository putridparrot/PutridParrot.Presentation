using System;
using PutridParrot.Presentation.Interfaces;

namespace PutridParrot.Presentation
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

        public T Value => _valueFunc();
        public string[] DependentUpon { get; set; }
    }
}